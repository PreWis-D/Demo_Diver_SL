using UnityEngine;

public class PlayerJumpState : EntityState
{
    private PlayerCharacter Player => (PlayerCharacter)Entity;
    private PlayerData _playerData;
    private InputProcessingService _input;
    private bool _isJump;
    private float _cooldown = 0.15f;
    private float _currentTime;
    private float _waterLevel;
    private Vector2 _moveInput;
    private bool _canAirControl = true;
    private float _targetRotation;

    public PlayerJumpState(EntityStateMachine stateMachine, Entity entity) : base(stateMachine, entity)
    {
        _playerData = SL.Get<CharactersService>().GetPlayerData();
        _input = SL.Get<InputProcessingService>();
        _waterLevel = SL.Get<GeneralComponentsService>().GetGeneralSettingsConfig().WaterLevelY;
    }

    public override void Enter()
    {
        base.Enter();

        _currentTime = _cooldown;
        _moveInput = Vector2.zero;
        _canAirControl = true;

        _targetRotation = Player.transform.eulerAngles.y;

        Player.Animator.ChangeJumpState(true);
    }

    public override void Exit()
    {
        base.Exit();

        Player.Animator.ChangeGroundedState(true);
        Player.Animator.ChangeJumpState(false);
        _isJump = false;
        _moveInput = Vector2.zero;
    }

    public override void Update()
    {
        _input.Update();

        _moveInput = _input.Direction;

        if (_canAirControl && _moveInput.magnitude > 0.1f)
        {
            if (_moveInput.x > 0.1f)
            {
                _targetRotation = 90f;
            }
            else if (_moveInput.x < -0.1f)
            {
                _targetRotation = -90f;
            }
        }

        if (Player.transform.position.y < _waterLevel)
            Player.StateMachine.SetState<PlayerWaterState>();

        if (_isJump)
            _currentTime -= Time.deltaTime;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        ApplyCustomGravity();

        if (_canAirControl)
        {
            ApplyAirMovement();
        }

        if (_input.IsJump && _isJump == false)
            PerformJump();
        else if (_isJump == false)
            SetJumpState();

        if (CheckGround() && _currentTime < 0)
        {
            Player.Animator.ChangeJumpState(false);
            Player.Animator.ChangeGroundedState(true);
            Player.StateMachine.SetState<PlayerGroundState>();
        }
    }

    private void PerformJump()
    {
        float calculatedJumpForce = Mathf.Sqrt(_playerData.MovementData.JumpForce * -2f * (Physics.gravity.y * _playerData.MovementData.GravityScale));

        Vector3 velocity = Player.Rigidbody.linearVelocity;
        velocity.y = calculatedJumpForce;
        Player.Rigidbody.linearVelocity = velocity;

        SetJumpState();
    }

    private void SetJumpState()
    {
        _isJump = true;
        Player.Animator.ChangeGroundedState(false);
    }

    private void ApplyCustomGravity()
    {
        if (Player.Rigidbody.linearVelocity.y < 0)
            Player.Rigidbody.linearVelocity += Vector3.up * (Physics.gravity.y * (_playerData.MovementData.GravityScale - 1) * Time.fixedDeltaTime);
        else if (Player.Rigidbody.linearVelocity.y > 0 && !_isJump)
            Player.Rigidbody.linearVelocity += Vector3.up * (Physics.gravity.y * (_playerData.MovementData.GravityScale - 1) * Time.fixedDeltaTime);
    }

    private void ApplyAirMovement()
    {
        float horizontalInput = _moveInput.x;

        ApplySmoothRotation();

        if (Mathf.Abs(horizontalInput) < 0.1f)
            return;

        float targetVelocityX = horizontalInput * _playerData.MovementData.AirControlSpeed;

        float currentYVelocity = Player.Rigidbody.linearVelocity.y;

        float currentXVelocity = Player.Rigidbody.linearVelocity.x;
        float velocityChangeX = targetVelocityX - currentXVelocity;

        float maxVelocityChange = _playerData.MovementData.AirControlAcceleration * Time.fixedDeltaTime;
        velocityChangeX = Mathf.Clamp(velocityChangeX, -maxVelocityChange, maxVelocityChange);
        
        Player.Rigidbody.linearVelocity = new Vector3(
            currentXVelocity + velocityChangeX,
            currentYVelocity,
            0
        );
    }

    private void ApplySmoothRotation()
    {
        Quaternion currentRotation = Player.View.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, _targetRotation, 0);

        Player.View.transform.rotation = Quaternion.Slerp(
            currentRotation,
            targetRotation,
            _playerData.MovementData.AirRotationSmoothTime
        );
    }

    private bool CheckGround()
    {
        return Physics.CheckSphere(
            Player.transform.position,
            0.1f,
            1 << 3,
            QueryTriggerInteraction.Ignore
        );
    }
}
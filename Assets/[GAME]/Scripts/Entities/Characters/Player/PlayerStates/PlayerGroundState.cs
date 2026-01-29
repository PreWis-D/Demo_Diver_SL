using UnityEngine;

public class PlayerGroundState : EntityState
{
    private PlayerCharacter Player => (PlayerCharacter)Entity;
    private PlayerData _playerData;
    private InputProcessingService _input;

    private Vector3 _currentRotateDirectionVector = new Vector3(90,0,0);
    private float _directionMoveHorizontal;
    private float _magnitude;
    private float _stopSpeed = 10f;
    private float _rotateSpeed = 10f;
    private float _waterLevel;

    public PlayerGroundState(EntityStateMachine stateMachine, Entity entity) : base(stateMachine, entity)
    {
        _playerData = SL.Get<CharactersService>().GetPlayerData();
        _input = SL.Get<InputProcessingService>();
        _waterLevel = SL.Get<GeneralComponentsService>().GetGeneralSettingsConfig().WaterLevelY;
    }

    public override void Enter()
    {
        base.Enter();
        _currentRotateDirectionVector = Player.View.transform.eulerAngles.y < 180 ? Vector3.right : Vector3.left;
        Player.Rigidbody.useGravity = true;
        _directionMoveHorizontal = 0f;
        _playerData.ChangeLocationSpace(LocationSpace.Ground);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        _input.Update();
        _directionMoveHorizontal = _input.Direction.x;

        if (Player.transform.position.y < _waterLevel)
            Player.StateMachine.SetState<PlayerWaterState>();
    }

    public override void FixedUpdate()
    {
        if (Player.Rigidbody.isKinematic) return;

        if (CheckGround() == false || _input.IsJump)
        {
            Player.StateMachine.SetState<PlayerJumpState>();
            return;
        }

        var deltaTime = Time.fixedDeltaTime;

        HandleSprint();

        var velocity = Player.Rigidbody.linearVelocity;
        velocity.x = _directionMoveHorizontal * (deltaTime * _playerData.CurrentSpeed);
        Player.Rigidbody.linearVelocity = velocity;

        _magnitude = Mathf.Abs(_directionMoveHorizontal) > 0f
            ? Mathf.Clamp(Mathf.Abs(_directionMoveHorizontal), 0f, 1f)
            : Mathf.Lerp(_magnitude, 0f, deltaTime * _stopSpeed);

        if (_directionMoveHorizontal != 0f)
        {
            _currentRotateDirectionVector = _directionMoveHorizontal switch
            {
                > 0f => RotateDirectionToVector3(RotateDirection.Right),
                < 0f => RotateDirectionToVector3(RotateDirection.Left),
                _ => _currentRotateDirectionVector
            };
        }

        Rotate(deltaTime);

        var blendSpeed = _playerData.CurrentSpeed == _playerData.SprintSpeed ? 1.5f : _magnitude;
        _playerData.ChangeAnimationBlandSpeed(blendSpeed);
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

    private void Rotate(float deltaTime)
    {
        var targetRotate = Quaternion.LookRotation(_currentRotateDirectionVector);
        Player.View.transform.rotation = Quaternion.RotateTowards(Player.View.transform.rotation,
        targetRotate,
            360f * deltaTime * _rotateSpeed);
    }

    private void HandleSprint()
    {
        _playerData.HandleSprint(_input.IsRun && _input.Direction.x != 0);
    }

    private Vector3 RotateDirectionToVector3(RotateDirection rotateDirection)
    {
        return rotateDirection == RotateDirection.Left ? Vector3.left : Vector3.right;
    }
}
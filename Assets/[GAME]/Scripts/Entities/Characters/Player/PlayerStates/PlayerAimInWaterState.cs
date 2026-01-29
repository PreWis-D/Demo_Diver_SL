using UnityEngine;

public class PlayerAimInWaterState : EntityState
{
    private PlayerCharacter Player => (PlayerCharacter)Entity;
    private PlayerData _playerData;
    private InputProcessingService _input;

    private Vector3 _currentRotateDirectionVector = Vector3.right;
    private Vector3 _directionMove;
    private Vector3 _lastMovePosition;
    private float _magnitude;
    private float _moveSpeedInWater;
    private float _lastBlendSpeed;

    public PlayerAimInWaterState(EntityStateMachine stateMachine, Entity entity) : base(stateMachine, entity)
    {
        _playerData = SL.Get<CharactersService>().GetPlayerData();
        _input = SL.Get<InputProcessingService>();
    }

    public override void Enter()
    {
        base.Enter();
        Vector3 directionVector = Player.transform.eulerAngles.y < 180 ? Vector3.right : Vector3.left;
        _currentRotateDirectionVector = directionVector;
        Player.Rigidbody.useGravity = false;
        _lastBlendSpeed = 0;
        _moveSpeedInWater = _playerData.MoveSpeedInWater;
        _playerData.ChangeLocationSpace(LocationSpace.Water);
        _playerData.ChangeAimState(true);

        Player.AimTarget.transform.localPosition = new Vector3(0, 1.5f, 3);
        Vector2 startAimPosition =
            SL.Get<CamerasService>().GetMainCamera().WorldToScreenPoint(Player.AimTarget.position);
        _input.SetStartAimPosition(startAimPosition);
    }

    public override void Exit()
    {
        base.Exit();
        _playerData.ChangeAimState(false);
    }

    public override void Update()
    {
        _input.Update();
        _directionMove = new Vector3(_input.Direction.x, _input.Direction.y, 0f);

        if (_input.IsAim == false)
            stateMachine.SetState<PlayerWaterState>();
    }

    public override void FixedUpdate()
    {
        var deltaTime = Time.fixedDeltaTime;

        Player.Rigidbody.linearVelocity = DirectionMoveWater() * (deltaTime * _moveSpeedInWater);
        _magnitude = _directionMove.magnitude > 0f
            ? Mathf.Clamp(_directionMove.magnitude, 0f, 1f)
            : Mathf.Lerp(_magnitude, 0f, deltaTime * _playerData.MovementData.WaterMoveStopSpeed);

        var direction = DirectionRotate();

        Rotate(direction);
        HandleAnimationBlend(direction);
    }

    private Vector3 DirectionMoveWater()
    {
        var direction = _directionMove;
        if (Player.transform.position.y >= -1.15f)
        {
            direction.y = Mathf.Clamp(direction.y, -1f, 0f);
        }
        return direction;
    }

    private void Rotate(Vector3 direction)
    {
        if (direction == Vector3.zero)
            direction = _currentRotateDirectionVector;

        var targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        Player.View.transform.rotation = Quaternion.Slerp(
            Player.View.transform.rotation,
            targetRotation,
            _playerData.MovementData.WaterRotateSpeedSmooth * Time.deltaTime * _playerData.MovementData.WaterRotateSpeed);

        _currentRotateDirectionVector = direction.x switch
        {
            > 0f => RotateDirectionToVector3(RotateDirection.Right),
            < 0f => RotateDirectionToVector3(RotateDirection.Left),
            _ => _currentRotateDirectionVector
        };
    }

    private Vector3 DirectionRotate()
    {
        var direction = (Player.AimTarget.transform.position - Player.transform.position).normalized;
        direction.y = 0;
        return direction;
    }

    private Vector3 RotateDirectionToVector3(RotateDirection rotateDirection)
    {
        return rotateDirection == RotateDirection.Left ? Vector3.left : Vector3.right;
    }

    private void HandleAnimationBlend(Vector3 direction)
    {
        float targetAnimationBlendSpeed = _magnitude * 0.5f;

        if (direction.x < 0 && _lastMovePosition.x < Player.transform.position.x)
            targetAnimationBlendSpeed *= -1;
        if (direction.x > 0 && _lastMovePosition.x > Player.transform.position.x)
            targetAnimationBlendSpeed *= -1;

        _lastBlendSpeed = Mathf.Lerp(_lastBlendSpeed, targetAnimationBlendSpeed, 0.1f);
        _playerData.ChangeAnimationBlandSpeed(_lastBlendSpeed);
        _lastMovePosition = Player.transform.position;
    }
}
using UnityEngine;

public class PlayerWaterState : EntityState
{
    private PlayerCharacter Player => (PlayerCharacter)Entity;
    private PlayerData _playerData;
    private InputProcessingService _input;
    private CapsuleCollider _collider;

    private Vector3 _currentRotateDirectionVector = Vector3.right;
    private Vector3 _directionMove;
    private float _magnitude;
    private float _moveSpeedInWater;

    private float _transitionThreshold = 0.2f;
    private float _waterLevel;

    public PlayerWaterState(EntityStateMachine stateMachine, Entity entity) : base(stateMachine, entity)
    {
        _playerData = SL.Get<CharactersService>().GetPlayerData();
        _input = SL.Get<InputProcessingService>();
        _collider = Player.Collider.GetComponent<CapsuleCollider>();
        _waterLevel = SL.Get<GeneralComponentsService>().GetGeneralSettingsConfig().WaterLevelY;
    }

    public override void Enter()
    {
        base.Enter();
        _currentRotateDirectionVector = Player.View.transform.eulerAngles.y < 180 ? Vector3.right : Vector3.left;
        _directionMove = Vector3.zero;
        Player.Rigidbody.useGravity = false;
        _moveSpeedInWater = _playerData.MoveSpeedInWater;
        _playerData.ChangeLocationSpace(LocationSpace.Water);
    }

    public override void Exit()
    {
        base.Exit();
    }

    private Vector3 DirectionMoveWater()
    {
        var direction = _directionMove;
        if (Player.transform.position.y >= _waterLevel)
        {
            direction.y = Mathf.Clamp(direction.y, -1f, 0f);
        }
        return direction;
    }

    public override void Update()
    {
        _input.Update();
        _directionMove = new Vector3(_input.Direction.x, _input.Direction.y, 0f);

        if (_input.IsAim)
            stateMachine.SetState<PlayerAimInWaterState>();
    }

    public override void FixedUpdate()
    {
        var deltaTime = Time.fixedDeltaTime;

        Player.Rigidbody.linearVelocity = DirectionMoveWater() * (deltaTime * _moveSpeedInWater);
        _magnitude = _directionMove.magnitude > 0f
            ? Mathf.Clamp(_directionMove.magnitude, 0f, 1f)
            : Mathf.Lerp(_magnitude, 0f, deltaTime * _playerData.MovementData.WaterMoveStopSpeed);
        Rotate(deltaTime);

        UpdateColliderBasedOnMovement();

        _playerData.ChangeAnimationBlandSpeed(_magnitude);
    }

    private Vector3 DirectionRotate()
    {
        return _directionMove;
    }

    private void Rotate(float deltaTime)
    {
        var direction = DirectionRotate();
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

    private Vector3 RotateDirectionToVector3(RotateDirection rotateDirection)
    {
        return rotateDirection == RotateDirection.Left ? Vector3.left : Vector3.right;
    }

    private void UpdateColliderBasedOnMovement()
    {
        var direction = DirectionRotate();

        if (_collider == null || direction.magnitude < 0.01f) return;

        Vector3 normalizedDirection = direction.normalized;

        float absX = Mathf.Abs(normalizedDirection.x);
        float absY = Mathf.Abs(normalizedDirection.y);

        bool isHorizontalDominant = absX > absY + _transitionThreshold;
        bool isVerticalDominant = absY > absX + _transitionThreshold;

        int newColliderDirection;

        if (isHorizontalDominant)
        {
            newColliderDirection = Constants.X_AXIS;
        }
        else if (isVerticalDominant)
        {
            newColliderDirection = Constants.Y_AXIS;
        }
        else
        {
            float angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;
            float normalizedAngle = Mathf.Abs(angle % 180);

            if (normalizedAngle < 45f || normalizedAngle > 135f)
                newColliderDirection = Constants.X_AXIS;
            else
                newColliderDirection = Constants.Y_AXIS;
        }

        if (_collider.direction != newColliderDirection)
        {
            _collider.direction = newColliderDirection;

            if (newColliderDirection == 2)
            {
                _collider.height = _playerData.MovementData.HorizontalHeight;
                _collider.radius = _playerData.MovementData.HorizontalRadius;
                _collider.center = _playerData.MovementData.HorizontalCenter;
            }
            else
            {
                _collider.height = _playerData.MovementData.VerticalHeight;
                _collider.radius = _playerData.MovementData.VerticalRadius;
                _collider.center = _playerData.MovementData.VerticalCenter;
            }
        }
    }
}
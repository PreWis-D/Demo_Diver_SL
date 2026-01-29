using UnityEngine;

public class FishPanicState : EntityState
{
    private AbstractFish _fish;
    private FishData _data;
    private Rigidbody _rigidbody;
    private PlayerCharacter _playerCharacter;

    private Vector3 _panicDirection;
    private Vector3 _panicCenter;
    private Vector3 _currentRotateDirectionVector;
    private float _panicTimer;
    private float _waterEdgeY;

    public FishPanicState(EntityStateMachine stateMachine, IEntity entity) : base(stateMachine, entity)
    {
        _fish = entity as AbstractFish;
        _data = _fish.Data;
        _rigidbody = _fish.GetComponent<Rigidbody>();
        _waterEdgeY = SL.Get<GeneralComponentsService>().GetGeneralSettingsConfig().WaterLevelY;
        _playerCharacter = SL.Get<CharactersService>().GetPlayerCharacter();
    }

    public override void Enter()
    {
        base.Enter();

        _panicCenter = _fish.IdleCenter;

        Vector3 awayFromPlayer = (_fish.transform.position - GetPlayerPosition()).normalized;
        awayFromPlayer.z = _fish.ZVector;

        float randomAngle = Random.Range(-_data.MaxPanicAngle, _data.MaxPanicAngle);

        _panicDirection = Quaternion.Euler(0, randomAngle, 0) * awayFromPlayer;
        _panicDirection.Normalize();

        _panicTimer = _data.PanicDuration;
    }

    private Vector3 GetPlayerPosition()
    {
        if (_playerCharacter != null)
        {
            Vector3 playerPos = _playerCharacter.transform.position;
            playerPos.z = _fish.ZVector;
            return playerPos;
        }
        return _fish.transform.position;
    }

    public override void Update()
    {
        _fish.Animator.SetFloat(Constants.SPEED, _rigidbody.linearVelocity.magnitude > 0.1f ? 2 : 0);

        _panicTimer -= Time.deltaTime;

        if (CheckForObstacles() || CheckForOtherFish())
        {
            RecalculateDirection();
        }

        Vector2 panicCenter2D = new Vector2(_panicCenter.x, _panicCenter.y);
        Vector2 currentPos2D = new Vector2(_fish.transform.position.x, _fish.transform.position.y);

        if (Vector2.Distance(panicCenter2D, currentPos2D) > _data.PanicRadius)
        {
            RecalculateDirection();
        }

        if (_panicTimer <= 0)
        {
            _fish.StateMachine.SetState<FishIdleState>();
        }
    }

    public override void FixedUpdate()
    {
        Vector3 moveForce = _panicDirection * _data.PanicSpeed * 0.05f;
        moveForce.z = _fish.ZVector;

        _rigidbody.AddForce(moveForce, ForceMode.Force);

        Rotate(_panicDirection);
    }

    private void Rotate(Vector3 direction)
    {
        if (direction == Vector3.zero)
            direction = _currentRotateDirectionVector;

        var targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        _fish.transform.rotation = Quaternion.Slerp(
            _fish.transform.rotation,
            targetRotation,
            0.1f * Time.deltaTime * _data.RotationSpeed);

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

    private bool CheckForObstacles()
    {
        Collider[] obstacles = Physics.OverlapSphere(
            _fish.transform.position,
            _data.ObstacleDetectionRadius,
            _data.obstacleMask);

        foreach (var obstacle in obstacles)
        {
            if (Mathf.Abs(obstacle.transform.position.z - _fish.transform.position.z) < _data.MovementPlaneHeight)
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckForOtherFish()
    {
        Collider[] otherFish = Physics.OverlapSphere(
            _fish.transform.position,
            _data.FishAvoidanceRadius);

        foreach (var collider in otherFish)
        {
            if (collider.gameObject != _fish.gameObject && collider.GetComponent<AbstractFish>() != null)
            {
                AbstractFish other = collider.GetComponent<AbstractFish>();
                if (Mathf.Abs(other.transform.position.z - _fish.transform.position.z) < _data.MovementPlaneHeight)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void RecalculateDirection()
    {
        Vector3[] testDirections = {
            _panicDirection,
            Quaternion.Euler(0, 90, 0) * _panicDirection,
            Quaternion.Euler(0, -90, 0) * _panicDirection
        };

        foreach (var direction in testDirections)
        {
            Vector3 rayDirection = direction;
            rayDirection.z = _fish.ZVector;

            if (!Physics.Raycast(_fish.transform.position, rayDirection, _data.ObstacleDetectionRadius, _data.obstacleMask))
            {
                _panicDirection = rayDirection.normalized;

                Vector3 toCenter = (_panicCenter - _fish.transform.position).normalized;
                toCenter.y = Mathf.Clamp(toCenter.y, toCenter.y, _waterEdgeY);
                toCenter.z = _fish.ZVector;
                _panicDirection = Vector3.Lerp(_panicDirection, toCenter, 0.3f).normalized;

                return;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        Vector3 velocity = _rigidbody.linearVelocity;
        velocity.x *= 0.5f;
        velocity.z *= 0.5f;
        _rigidbody.linearVelocity = velocity;
    }
}
using UnityEngine;

public class FishIdleState : EntityState
{
    private AbstractFish _fish;
    private FishData _data;
    private Rigidbody _rigidbody;

    private Vector3 _targetPosition;
    private Vector3 _currentRotateDirectionVector;
    private Vector3 _directionMove;
    private float _waitTimer;
    private float _waterEdgeY;
    private float _blendSpeed;
    private float _magnitude;
    private bool _isWaiting;
    private bool _targetPositionFounded;

    public FishIdleState(EntityStateMachine stateMachine, IEntity entity) : base(stateMachine, entity)
    {
        _fish = entity as AbstractFish;
        _data = _fish.Data;
        _rigidbody = _fish.GetComponent<Rigidbody>();
        _waterEdgeY = SL.Get<GeneralComponentsService>().GetGeneralSettingsConfig().WaterLevelY;
    }

    public override void Enter()
    {
        base.Enter();
        _isWaiting = false;

        _targetPosition = GetRandomPointInRadius();
    }

    public override void Update()
    {
        _fish.Animator.SetFloat(Constants.SPEED, _blendSpeed);

        if (_isWaiting)
        {
            _waitTimer -= Time.deltaTime;
            if (_waitTimer <= 0)
            {
                _isWaiting = false;
                _targetPosition = GetRandomPointInRadius();
            }
            return;
        }

        Vector2 currentPos2D = new Vector2(_fish.transform.position.x, _fish.transform.position.z);
        Vector2 targetPos2D = new Vector2(_targetPosition.x, _targetPosition.z);

        if (Vector2.Distance(currentPos2D, targetPos2D) < 0.5f)
        {
            float random = Random.Range(0f, 1f);
            if (random < _data.IdleWaitProbability)
            {
                StartWaiting();
            }
            else
            {
                _targetPosition = GetRandomPointInRadius();
            }
        }

        if (CheckForObstacles(_targetPosition))
        {
            _targetPositionFounded = false;
            return;
        }

        if (CheckForOtherFish(_targetPosition))
        {
            _targetPositionFounded = false;
            return;
        }

        _targetPositionFounded = true;
    }

    public override void FixedUpdate()
    {
        if (_isWaiting) return;
        if (_targetPositionFounded == false) return;

        Vector3 direction = (_targetPosition - _fish.transform.position).normalized;
        direction.z = _fish.ZVector;

        var deltaTime = Time.fixedDeltaTime;

        _rigidbody.linearVelocity = direction * (deltaTime * _data.SwimSpeed);
        _magnitude = _directionMove.magnitude > 0f
            ? Mathf.Clamp(_directionMove.magnitude, 0f, 1f)
            : Mathf.Lerp(_magnitude, 0f, deltaTime * 50f);

        Rotate(direction);

        _blendSpeed = _rigidbody.linearVelocity.magnitude;
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

    #region Find random move point
    private Vector3 GetRandomPointInRadius()
    {
        Vector3 randomCircle = Random.insideUnitCircle * _fish.IdleRadius;
        Vector3 point = _fish.IdleCenter + new Vector3(randomCircle.x, randomCircle.y, _fish.ZVector);

        point.y = Mathf.Clamp(point.y, point.y, _waterEdgeY);

        return point;
    }

    private bool CheckForObstacles(Vector3 point)
    {
        Collider[] obstacles = Physics.OverlapSphere(
            point,
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

    private bool CheckForOtherFish(Vector3 point)
    {
        Collider[] otherFish = Physics.OverlapSphere(
            point,
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

    #endregion

    private void StartWaiting()
    {
        _isWaiting = true;
        _waitTimer = Random.Range(_data.MinWaitTime, _data.MaxWaitTime);
        _rigidbody.linearVelocity = Vector3.zero;
    }
}
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceMagnetState : EntityState
{
    private Rigidbody _rigidbody;
    private Transform _target;
    private ResourceData _data;

    private float _originalDrag;
    private float _originalAngularDrag;

    public ResourceMagnetState(EntityStateMachine stateMachine, IEntity entity, ResourceData data) : base(stateMachine, entity)
    {
        _rigidbody = Entity.Transform.GetComponent<Rigidbody>();
        _data = data;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public override void Enter()
    {
        base.Enter();

        _originalDrag = _rigidbody.linearDamping;
        _originalAngularDrag = _rigidbody.angularDamping;

        _rigidbody.linearDamping = 0.5f;
        _rigidbody.angularDamping = 0.5f;
        _rigidbody.useGravity = false;
    }

    public override void Exit()
    {
        _rigidbody.linearDamping = _originalDrag;
        _rigidbody.angularDamping = _originalAngularDrag;
        _rigidbody.useGravity = true;

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        _target = null;

        base.Exit();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        UpdateAttraction();
    }

    private void UpdateAttraction()
    {
        if (_target == null)
        {
            stateMachine.SetState<ResourceWaterState>();
            return;
        }

        Vector3 direction = (_target.position - Entity.Transform.position);
        float distance = direction.magnitude;

        if (distance > _data.MaxAttractionDistance)
        {
            stateMachine.SetState<ResourceWaterState>();
            return;
        }

        if (distance <= _data.PickupDistance)
        {
            ResourceCollectState state = stateMachine.GetState<ResourceCollectState>();
            state.SetTarget(_target);
            stateMachine.SetState<ResourceCollectState>();
            return;
        }

        direction.Normalize();

        float normalizedDistance = 1f - Mathf.Clamp01(distance / _data.MaxAttractionDistance);
        float curveValue = _data.SpeedCurve.Evaluate(normalizedDistance);
        float currentSpeed = _data.AttractionSpeed * curveValue;

        Vector3 attractionForce = direction * currentSpeed;

        Vector3 targetVelocity = attractionForce;
        Vector3 velocityChange = targetVelocity - _rigidbody.linearVelocity;

        _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

        if (_data.RotationSpeed > 0)
        {
            Vector3 randomRotation = Random.onUnitSphere * _data.RotationSpeed * Time.fixedDeltaTime;
            _rigidbody.AddTorque(randomRotation, ForceMode.VelocityChange);
        }
    }
}
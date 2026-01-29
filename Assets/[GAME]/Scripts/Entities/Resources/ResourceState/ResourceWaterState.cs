using UnityEngine;

public class ResourceWaterState : EntityState
{
    private Rigidbody _rigidbody;
    private Collider _collider;
    private BuoyancySettings _buoyancy;

    private float _waterLevel;
    private float _waterDensity;
    private float _volume;

    public ResourceWaterState(EntityStateMachine stateMachine, IEntity entity, ResourceData data) : base(stateMachine, entity)
    {
        _rigidbody = Entity.Transform.GetComponent<Rigidbody>();
        _collider = Entity.Transform.GetComponent<Collider>();

        _buoyancy = data.BuoyancySettings;

        _waterLevel = SL.Get<GeneralComponentsService>().GetGeneralSettingsConfig().WaterLevelY;
        _waterDensity = SL.Get<GeneralComponentsService>().GetGeneralSettingsConfig().WaterDensity;
    }

    public override void Enter()
    {
        base.Enter();

        _rigidbody.isKinematic = false;
        _collider.enabled = true;

        CalculateVolume();
        CalculateBuoyancyForce();
        SetupPhysics();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FixedUpdate()
    {
        if (_rigidbody.isKinematic)
            return;

        ApplyBuoyancyForce();
        LimitTerminalVelocity();
        CheckWaterLevel();
    }

    private void CalculateVolume()
    {
        var bounds = _collider.bounds;
        _volume = bounds.size.x * bounds.size.y * bounds.size.z;
        if (_volume < 0.001f) _volume = 0.001f;
    }

    private void CalculateBuoyancyForce()
    {
        float densityDifference = _waterDensity - _buoyancy.Density;
        _buoyancy.BuoyancyForce = densityDifference * _volume * 9.81f * 0.001f;
    }

    private void SetupPhysics()
    {
        _rigidbody.useGravity = false;

        _rigidbody.linearDamping = _buoyancy.WaterDrag;
        _rigidbody.angularDamping = _buoyancy.WaterAngularDrag;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
        _rigidbody.constraints |= RigidbodyConstraints.FreezePositionZ;
    }

    private void ApplyBuoyancyForce()
    {
        Vector3 buoyancyVector = Vector3.up * _buoyancy.BuoyancyForce;

        buoyancyVector += new Vector3(
            Mathf.Sin(Time.time + Entity.Transform.position.x) * 0.1f,
            0,
            0
        );

        _rigidbody.AddForce(buoyancyVector, ForceMode.Force);

        if (_rigidbody.angularVelocity.magnitude > 2f)
            _rigidbody.angularVelocity *= 0.95f;
    }

    private void LimitTerminalVelocity()
    {
        Vector3 velocity = _rigidbody.linearVelocity;

        float verticalVelocity = Mathf.Clamp(velocity.y,
            -_buoyancy.TerminalVelocity,
            _buoyancy.TerminalVelocity);

        float horizontalSpeed = new Vector2(velocity.x, velocity.z).magnitude;
        if (horizontalSpeed > _buoyancy.TerminalVelocity * 0.5f)
        {
            velocity.x *= 0.98f;
            velocity.z *= 0.98f;
        }

        velocity.y = verticalVelocity;
        _rigidbody.linearVelocity = velocity;
    }

    private void CheckWaterLevel()
    {
        if (Entity.Transform.position.y > _waterLevel)
        {
            _rigidbody.linearDamping = _buoyancy.WaterDrag * 0.5f;

            if (_buoyancy.BuoyancyForce > 0)
            {
                float depth = Entity.Transform.position.y - _waterLevel;
                if (depth > 0.1f)
                {
                    _rigidbody.AddForce(Vector3.down * depth * 5f, ForceMode.Force);
                }
            }
        }
        else
        {
            _rigidbody.linearDamping = _buoyancy.WaterDrag;
        }
    }
}
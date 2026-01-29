using DG.Tweening;
using UnityEngine;

public class ResourceSpawnState : EntityState
{
    private Vector3 _targetPosition;
    private Rigidbody _rigidbody;

    public ResourceSpawnState(EntityStateMachine stateMachine, IEntity entity) : base(stateMachine, entity)
    {
        _rigidbody = Entity.Transform.GetComponent<Rigidbody>();
    }

    public void SetTargetPosition(Vector3 targetPos)
    {
        _targetPosition = targetPos;
    }
    
    public override void Enter()
    {
        base.Enter();

        _rigidbody.isKinematic = true;

        Sequence flySequence = DOTween.Sequence();
        
        flySequence.Append(Entity.Transform.DOMove(_targetPosition, 1f)
            .SetEase(Ease.OutExpo));

        flySequence.Join(Entity.Transform.DORotate(
            new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutCubic));

        flySequence.OnComplete(() => stateMachine.SetState<ResourceWaterState>());
    }

    public override void Exit()
    {
        _rigidbody.isKinematic = false;

        base.Exit();
    }
}
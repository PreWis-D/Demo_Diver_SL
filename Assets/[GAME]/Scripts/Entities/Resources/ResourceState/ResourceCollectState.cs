using DG.Tweening;
using UnityEngine;

public class ResourceCollectState : EntityState
{
    private ResourceData _data;
    private Rigidbody _rigidbody;
    private Transform _target;
    private Sequence _sequence;

    private float _animateDuration = 0.25f;

    public ResourceCollectState(EntityStateMachine stateMachine, IEntity entity, ResourceData data) : base(stateMachine, entity)
    {
        _data = data;
        _rigidbody = Entity.Transform.GetComponent<Rigidbody>();
    }

    public override void Enter()
    {
        base.Enter();

        _rigidbody.isKinematic = true;
        ResourcesService resourcesService = SL.Get<ResourcesService>();

        if (resourcesService.CheckAddResource(_data))
        {
            resourcesService.AddResource(_data);
            Animate();
        }
        else
        {
            stateMachine.SetState<ResourceWaterState>();
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    private void Animate()
    {
        Entity.Transform.SetParent(_target);
        _sequence.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Append(Entity.Transform.DOLocalMove(Vector3.zero, _animateDuration).SetEase(Ease.InBack));
        _sequence.Join(Entity.Transform.DOScale(Vector3.zero, _animateDuration).SetEase(Ease.InBack));
        _sequence.OnComplete(() =>
            {
                Entity.Transform.SetParent(null);
                Entity.Transform.localScale = Vector3.one;
                PoolManager.SetPool(Entity.Transform);
            });
    }

    public override void Exit()
    {
        _sequence.Kill();
        Entity.Transform.SetParent(null);

        base.Exit();
    }
}
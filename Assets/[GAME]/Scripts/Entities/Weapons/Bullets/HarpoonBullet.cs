using UnityEngine;

public class HarpoonBullet : AbstractBullet
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Transform _tip;
    [SerializeField] private TrailRenderer _trail;

    private float _radius = 0.1f;
    private float _damage;
    private float _speed;

    protected override void OnEnable()
    {
        base.OnEnable();

        _trail.enabled = true;
    }

    public void Init(
        float damage,
        float speed)
    {
        _damage = damage;
        _speed = speed;
    }

    protected override void Update()
    {
        base.Update();

        Move();
        HandleTargets();
    }

    private void Move()
    {
        var localDirection = transform.rotation * Vector3.forward;

        transform.position = Vector3.MoveTowards(transform.localPosition, transform.localPosition + localDirection,
            Time.deltaTime * _speed);
    }

    private void HandleTargets()
    {
        Collider[] overLappedColliders = Physics.OverlapSphere(_tip.transform.position, _radius, _layerMask);

        if (overLappedColliders.Length > 0)
        {
            if (overLappedColliders[0].TryGetComponent(out IDamagable damageTarget))
                damageTarget.TakeDamage(_damage);

            PoolManager.SetPool(this);
        }
    }

    private void OnDisable()
    {
        _trail.Clear();
        _trail.enabled = false;
    }

    public void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
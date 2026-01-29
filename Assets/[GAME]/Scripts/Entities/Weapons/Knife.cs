using System.Collections.Generic;
using UnityEngine;

public class Knife : AbstractMeleeWeapon
{
    [SerializeField] private TrailRenderer _trail;
    [SerializeField] private LayerMask _layerMask;

    private float _startDamage;
    private float _radiusDetectTargets = 0.5f;
    private bool _isActive;

    private List<IDamagable> _damagables = new List<IDamagable>();

    public override HandItemType Type => HandItemType.Knife;

    public override void Init(HandItemData itemData)
    {
        base.Init(itemData);

        KnifeData data = itemData as KnifeData;

        _startDamage = data.StartDamage;
        _trail.enabled = false;
    }

    public override bool CanInteract()
    {
        return Enabled && Input.IsFiring && _isActive == false;
    }

    public void Interact()
    {
        _isActive = true;
        _trail.enabled = true;
    }

    public void Interrupt()
    {
        _trail.enabled = false;
        _trail.Clear();
        _damagables.Clear();
        _isActive = false;
    }

    protected override void Update()
    {
        base.Update();

        HandleTargets();
    }

    protected override void HandleVisible()
    {
        View.gameObject.SetActive(_isActive);
    }

    private void HandleTargets()
    {
        if (_isActive == false)
            return;

        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            _radiusDetectTargets,
            _layerMask
        );

        foreach (var collider in colliders)
        {
            IDamagable damagable = collider.GetComponent<IDamagable>();
            if (_damagables.Contains(damagable) == false && damagable.IsDied == false)
            {
                _damagables.Add(damagable);
                damagable.TakeDamage(_startDamage);
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _radiusDetectTargets);
        }
    }
}
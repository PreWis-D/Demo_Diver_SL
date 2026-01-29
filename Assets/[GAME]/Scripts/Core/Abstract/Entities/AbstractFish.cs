using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public abstract class AbstractFish : Entity, IDamagable
{
    [SerializeField] private DamageViewer _damageViewer;
    [SerializeField] private ResourcesGenerator _rewardGenerator;
    [SerializeField] private float _resourceSpawnRadius;
    [SerializeField] private float _delaySpawnResource;

    [field: SerializeField] public Animator Animator { get; protected set; }
    [field: SerializeField] protected LayerMask PlayerMask;

    private ResourceSpawner _resourceSpawner;
    private List<ResourceData> _cacheResourceItems = new List<ResourceData>();

    public FishData Data { get; protected set; }
    public float IdleRadius { get; protected set; }
    public float ZVector { get; protected set; }
    public Vector3 IdleCenter { get; protected set; }
    public bool IsDied { get; protected set; }
    public FloatParameter FloatParameter { get; protected set; }

    public event Action<AbstractFish> FishDied;

    public virtual void Init(FishData data, Vector3 idleCenter, float idleRadius)
    {
        IsDied = false;
        Data = data;
        IdleCenter = idleCenter;
        IdleRadius = idleRadius;
        ZVector = transform.position.z;

        _resourceSpawner = new ResourceSpawner(_rewardGenerator, Transform, _resourceSpawnRadius, _delaySpawnResource);
        FloatParameter = new FloatParameter(Data.StartHealth, Data.StartHealth);

        InitStateMachine();
    }

    public void TakeDamage(float damage)
    {
        FloatParameter.DecreaseValue(damage);

        if (FloatParameter.IsAtMinValue())
            Died();
        else
            _damageViewer.Blink();
    }

    public void Died()
    {
        IsDied = true;
        FishDied?.Invoke(this);
        _resourceSpawner.SpawnResources();
        PoolManager.SetPool(this);
    }

    protected abstract void InitStateMachine();
    protected abstract void ReactionForPlayer();
    protected abstract bool CheckState();

    protected override void Update()
    {
        base.Update();

        CheckForPlayer();
    }

    private void CheckForPlayer()
    {
        if (CheckState() == false)
            return;

        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            Data.PlayerDetectionRadius,
            PlayerMask);

        foreach (var collider in colliders)
        {
            PlayerCharacter player = collider.GetComponent<PlayerCharacter>();
            if (player)
            {
                if (Vector2.Distance(transform.position, player.transform.position) <= Data.PlayerDetectionRadius)
                {
                    ReactionForPlayer();
                    return;
                }
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _resourceSpawner.OnDestroy();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (Data != null)
        {
            // Визуализация радиусов в 2D плоскости
            Gizmos.color = Color.yellow;
            DrawCircle(IdleCenter, IdleRadius);

            Gizmos.color = Color.red;
            DrawCircle(transform.position, Data.PlayerDetectionRadius);

            Gizmos.color = Color.blue;
            DrawCircle(transform.position, Data.ObstacleDetectionRadius);

            // Плоскость движения
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Vector3 planeCenter = new Vector3(IdleCenter.x, transform.position.y, IdleCenter.z);
            Gizmos.DrawCube(planeCenter, new Vector3(IdleRadius * 2, 0.1f, IdleRadius * 2));
        }
    }

    private void DrawCircle(Vector3 center, float radius)
    {
        // Рисуем круг в плоскости XZ
        float theta = 0;
        float x = radius * Mathf.Cos(theta);
        float z = radius * Mathf.Sin(theta);
        Vector3 pos = center + new Vector3(x, transform.position.y, z);
        Vector3 newPos;
        Vector3 lastPos = pos;

        for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
        {
            x = radius * Mathf.Cos(theta);
            z = radius * Mathf.Sin(theta);
            newPos = center + new Vector3(x, transform.position.y, z);
            Gizmos.DrawLine(lastPos, newPos);
            lastPos = newPos;
        }

        Gizmos.DrawLine(lastPos, pos);
    }
#endif
}
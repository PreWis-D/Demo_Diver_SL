using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FishesConfig", menuName = "Configs/Characters/FishesConfig")]
public class FishesConfig : ScriptableObject
{
    [field: SerializeField] public FishData[] FishDatas { get; private set; }

    private void OnValidate()
    {
        foreach (var fishData in FishDatas)
            fishData.Id = $"{fishData.EntityType}";
    }
}

[Serializable]
public class FishData
{
    [field: Header("General")]
    [ReadOnlyField] public string Id;
    [field: SerializeField] public EntityType EntityType { get; private set; }
    [field: SerializeField] public AbstractFish FishPrefab { get; private set; }
    [field: SerializeField] public float StartHealth { get; private set; }

    [field: Header("Movement")]
    [field: SerializeField] public float SwimSpeed { get; private set; } = 2f;
    [field: SerializeField] public float PanicSpeed { get; private set; } = 5f;
    [field: SerializeField] public float RotationSpeed { get; private set; } = 5f;
    [field: SerializeField] public float MovementPlaneHeight { get; private set; } = 0.5f;

    [field: Header("Idle State")]
    [field: SerializeField][field: Range(0, 1)] public float IdleWaitProbability { get; private set; } = 30f;
    [field: SerializeField] public float MinWaitTime { get; private set; } = 1f;
    [field: SerializeField] public float MaxWaitTime { get; private set; } = 3f;
    [field: SerializeField] public float ObstacleDetectionRadius { get; private set; } = 2f;
    [field: SerializeField] public float FishAvoidanceRadius { get; private set; } = 1.5f;
    public LayerMask obstacleMask;

    [field: Header("Panic State")]
    [field: SerializeField] public float PanicRadius { get; private set; } = 15f;
    [field: SerializeField] public float PanicDuration { get; private set; } = 3f;
    [field: SerializeField] public float PlayerDetectionRadius { get; private set; } = 5f;
    [field: SerializeField] public float MaxPanicAngle { get; private set; } = 45f;
}
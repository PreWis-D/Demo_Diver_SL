using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/Characters/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [field: SerializeField] public EntityType EntityType { get; private set; }
    [field: SerializeField] public MovementData MovementData { get; private set; }
    [field: SerializeField] public VacuumData VacuumData { get; private set; }
    [field: SerializeField] public ParamData[] Params { get; private set; }

    private void OnValidate()
    {
        if (Params.Length > 0)
            foreach (var data in Params)
                data.Id = $"{data.Type}";
    }
}

[Serializable]
public class ParamData
{
    [ReadOnlyField] public string Id;
    [field: SerializeField] public ParamType Type { get; private set; }
    [field: SerializeField] public float Value { get; private set; }
}

[Serializable]
public class MovementData
{
    [field: SerializeField] public float WaterRotateSpeed { get; private set; } = 10f;
    [field: SerializeField] public float WaterRotateSpeedSmooth { get; private set; } = 0.2f;
    [field: SerializeField] public float WaterMoveStopSpeed { get; private set; } = 4f;

    [field: Header("Ladder to ground settings")]
    [field: SerializeField] public AnimationCurve CurveJumpRaft { get; private set; }
    [field: SerializeField] public float HeightLadderToGround { get; private set; }
    [field: SerializeField] public float SpeedLadderToGround { get; private set; }

    [field: Header("Jump settings")]
    [field: SerializeField] public float JumpForce { get; private set; } = 8f;
    [field: SerializeField] public float GravityScale { get; private set; } = 1.5f;
    [field: Tooltip("Maximum air speed")]
    [field: SerializeField] public float AirControlSpeed { get; private set; } = 5f;
    [field: Tooltip("Acceleration in the air")]
    [field: SerializeField] public float AirControlAcceleration { get; private set; } = 20f;
    [field: Tooltip("Smooth turn time (higher is faster)")]
    [field: SerializeField][field: Range(0, 1)] public float AirRotationSmoothTime { get; private set; } = 0.1f;

    [field: Header("Water collider settings")]
    [field: SerializeField] public float HorizontalHeight { get; private set; } = 2f;
    [field: SerializeField] public float HorizontalRadius { get; private set; } = 0.5f;
    [field: SerializeField] public Vector3 HorizontalCenter { get; private set; } = new Vector3(0, 1f, 0);
    [field: SerializeField] public float VerticalHeight { get; private set; } = 1.8f;
    [field: SerializeField] public float VerticalRadius { get; private set; } = 0.4f;
    [field: SerializeField] public Vector3 VerticalCenter { get; private set; } = new Vector3(0, 0.9f, 0);
}

[Serializable]
public class VacuumData
{
    [Header("Quick Pickup Settings")]
    [field: SerializeField] public float QuickPickupRadius { get; private set; } = 3f;
    [field: SerializeField] public float QuickPickupDuration { get; private set; } = 0.3f;

    [Header("Vacuum Settings")]
    [field: SerializeField] public float MaxVacuumRadius { get; private set; } = 10f;
    [field: SerializeField] public float VacuumGrowthSpeed { get; private set; } = 5f;
    [field: SerializeField] public float VacuumForce { get; private set; } = 15f;
    [field: SerializeField] public LayerMask itemLayer { get; private set; }
}
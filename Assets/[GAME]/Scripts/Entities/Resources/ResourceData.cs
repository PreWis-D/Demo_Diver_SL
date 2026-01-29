using System;
using UnityEngine;

[Serializable]
public class ResourceData
{
    [ReadOnlyField] public string id;
    [field: SerializeField] public ResourceType Type { get; private set; }
    [field: SerializeField] public ResourceItem Prefab { get; private set; }
    [field: SerializeField] public int Value { get; private set; }
    [field: SerializeField] public int LoadValue { get; private set; }
    [field: SerializeField] public BuoyancySettings BuoyancySettings { get; private set; }

    [HideInInspector] public float AttractionSpeed;
    [HideInInspector] public float RotationSpeed;
    [HideInInspector] public float MaxAttractionDistance;
    [HideInInspector] public float PickupDistance;
    [HideInInspector] public AnimationCurve SpeedCurve;
}

[Serializable]
public class BuoyancySettings
{
    [field: Tooltip("Plastic: 300-800(pops up)\nWood: 400-700(pops up)\nGlass: 1800-2500(slowly sinking)\nMetal: 2000-8000(drowning)")]
    [field: SerializeField] public float Density { get; private set; }

    [field: Tooltip("Maximum lift/lower speed")]
    [field: SerializeField] public float TerminalVelocity { get; private set; } = 5f;

    [field: Tooltip("Water resistance")]
    [field: SerializeField] public float WaterDrag { get; private set; } = 2f;

    [field: Tooltip("Angular resistance")]
    [field: SerializeField] public float WaterAngularDrag { get; private set; } = 1f;

    // Ascent/dive force (calculated automatically)
    [HideInInspector] public float BuoyancyForce;
}
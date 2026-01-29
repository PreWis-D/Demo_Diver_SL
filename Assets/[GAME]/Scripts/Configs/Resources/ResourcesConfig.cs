using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourcesConfig", menuName = "Configs/Resources/ResourcesConfig")]
public class ResourcesConfig : ScriptableObject
{
    [field: SerializeField] public ResourceVacuumSettings ResourceVacuumSettings { get; private set; }
    [field: SerializeField] public ResourceData[] Datas { get; private set; }

    private void OnValidate()
    {
        foreach (var data in Datas)
        {
            data.id = $"{data.Type}_{data.Value}";
            data.AttractionSpeed = ResourceVacuumSettings.AttractionSpeed;
            data.RotationSpeed = ResourceVacuumSettings.RotationSpeed;
            data.MaxAttractionDistance = ResourceVacuumSettings.MaxAttractionDistance;
            data.PickupDistance = ResourceVacuumSettings.PickupDistance;
            data.SpeedCurve = ResourceVacuumSettings.SpeedCurve;
        }
    }
}

[Serializable]
public class ResourceVacuumSettings
{
    [field: SerializeField] public float AttractionSpeed { get; private set; } = 8f;
    [field: SerializeField] public float RotationSpeed { get; private set; } = 180f;
    [field: SerializeField] public float MaxAttractionDistance { get; private set; } = 20f;
    [field: SerializeField] public float PickupDistance { get; private set; } = 1.5f;
    [field: SerializeField] public AnimationCurve SpeedCurve { get; private set; } = AnimationCurve.Linear(0, 1, 1, 1);
}
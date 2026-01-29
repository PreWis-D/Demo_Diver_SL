using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "GeneralSettingsConfig", menuName = "Configs/GeneralSettingsConfig")]
public class GeneralSettingsConfig : ScriptableObject
{
    [field: Header("Water")]
    [field: SerializeField] public float WaterLevelY { get; private set; } = 0;
    [field: SerializeField] public float WaterDensity { get; private set; } = 1000;
}
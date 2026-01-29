using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "InputConfig", menuName = "Configs/Input/InputConfig")]
public class InputConfig : ScriptableObject
{
    [field: Header("Gamepad aim settings")]
    [field: SerializeField] public float GamepadDeadzone { get; private set; } = 0.1f;
    [field: SerializeField] public float SnapSpeed { get; private set; } = 10f;
    [field: SerializeField] public float AimEdgeRadius { get; private set; } = 300f;
    [field: SerializeField] public float CenterScreenOffsetY { get; private set; } = 125f;
}
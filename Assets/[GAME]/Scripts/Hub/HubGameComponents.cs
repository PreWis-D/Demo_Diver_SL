using Cinemachine;
using UnityEngine;

public class HubGameComponents : MonoBehaviour, IService
{
    [field: Header("Configs")]
    [field: SerializeField] public PlayerConfig[] CharacterConfigs { get; private set; }
    [field: SerializeField] public PrefabsConfig PrefabsConfig { get; private set; }
    [field: SerializeField] public InputConfig InputConfig { get; private set; }
    [field: SerializeField] public GeneralSettingsConfig GeneralSettingsConfig { get; private set; }
    [field: SerializeField] public SpritesConfig SpritesConfig { get; private set; }
    [field: SerializeField] public HandItemsConfig HandItemsConfig { get; private set; }

    [field: Space(10), Header("References")]
    [field: SerializeField] public Camera MainCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera PlayerCamera { get; private set; }
    [field: SerializeField] public SwitchLevelDoor SwitchLevelDoor { get; private set; }

    public void Initialize()
    {
        SwitchLevelDoor.Init();
    }

    public void Cleanup()
    {
    }
}
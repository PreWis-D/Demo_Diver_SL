using Cinemachine;
using UnityEngine;

public class FirstSceneGameComponents : MonoBehaviour, IService
{
    [field: Header("Configs")]
    [field: SerializeField] public PlayerConfig[] CharacterConfigs { get; private set; }
    [field: SerializeField] public PrefabsConfig PrefabsConfig { get; private set; }
    [field: SerializeField] public InputConfig InputConfig { get; private set; }
    [field: SerializeField] public ResourcesConfig ResourcesConfig { get; private set; }
    [field: SerializeField] public GeneralSettingsConfig GeneralSettingsConfig { get; private set; }
    [field: SerializeField] public SpritesConfig SpritesConfig { get; private set; }
    [field: SerializeField] public HandItemsConfig HandItemsConfig { get; private set; }

    [field: Space(10), Header("References")]
    [field: SerializeField] public Camera MainCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera PlayerCamera { get; private set; }
    [field: SerializeField] public Transform EnvironmentTransform { get; private set; }
    [field: SerializeField] public Transform PlayerSpawnPoint { get; private set; }

    private IInteractable[] _interactables;
    private AbstractResourceProducer[] _abstractResourceProducers;
    private FishesSquad[] _fishesSquads;

    public void Initialize()
    {
        _interactables = EnvironmentTransform.GetComponentsInChildren<IInteractable>();
        _abstractResourceProducers = EnvironmentTransform.GetComponentsInChildren<AbstractResourceProducer>();
        _fishesSquads = EnvironmentTransform.GetComponentsInChildren<FishesSquad>();

        for (int i = 0; i < _interactables.Length; i++)
            _interactables[i].Init();

        for (int i = 0; i < _abstractResourceProducers.Length; i++)
            _abstractResourceProducers[i].Init();

        for (int i = 0; i < _fishesSquads.Length; i++)
            _fishesSquads[i].Init();
    }

    public void Cleanup()
    {
    }
}
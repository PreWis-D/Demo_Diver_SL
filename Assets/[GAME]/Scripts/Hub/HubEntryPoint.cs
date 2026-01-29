using UnityEngine;

[RequireComponent(typeof(HubGameComponents))]
public class HubEntryPoint : MonoBehaviour
{
    private HubUIManager _uiManager;
    private HubGameComponents _gameComponents;

    private void Awake()
    {
        _gameComponents = GetComponent<HubGameComponents>();
    }

    private void Start()
    {
        SL.Init();
        new HubServicesInstaller(_gameComponents);

        CreatePlayer();
        CreateUIManager();

        SL.Get<EventProcessingService>().ChangeGameStateInvoke(GameState.Gameloop);
    }

    private void CreatePlayer()
    {
        var playerGO = Instantiate(SL.Get<GeneralComponentsService>().GetPrefab(PrefabType.Player), new Vector3(0, 1, 0), Quaternion.identity);
        var playerCharacter = playerGO.GetComponent<PlayerCharacter>();
        playerCharacter.Init();
        SL.Get<CharactersService>().SetPlayerCharacter(playerCharacter);
        SL.Get<CamerasService>().SetPlayer(playerCharacter, _gameComponents.PlayerCamera);
    }

    private void CreateUIManager()
    {
        var uiManagerGO = Instantiate(SL.Get<GeneralComponentsService>().GetPrefab(PrefabType.HubUIManager));
        _uiManager = uiManagerGO.GetComponent<HubUIManager>();
        _uiManager.Init();
    }

    private void OnDestroy()
    {
        PoolManager.ClearPool();
        _uiManager.Unsubscribe();
    }
}
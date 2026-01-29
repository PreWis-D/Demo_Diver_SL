using UnityEngine;

[RequireComponent(typeof(FirstSceneGameComponents))]
public class FirstSceneEntryPoint : MonoBehaviour
{
    private FirstSceneGameComponents _gameComponents;
    private FirstSceneUIManager _uiManager;

    private void Awake()
    {
        _gameComponents = GetComponent<FirstSceneGameComponents>();
    }

    private void Start()
    {
        SL.Init();
        new FirstSceneServicesInstaller(_gameComponents);

        CreatePlayer();
        CreateUIManager();

        SL.Get<EventProcessingService>().ChangeGameStateInvoke(GameState.Gameloop);
    }

    private void CreatePlayer()
    {
        var playerGO = Instantiate(
            SL.Get<GeneralComponentsService>().GetPrefab(PrefabType.Player),
            _gameComponents.PlayerSpawnPoint.position,
            Quaternion.identity);
        var playerCharacter = playerGO.GetComponent<PlayerCharacter>();
        playerCharacter.Init();
        SL.Get<CharactersService>().SetPlayerCharacter(playerCharacter);
        SL.Get<CamerasService>().SetPlayer(playerCharacter, _gameComponents.PlayerCamera);
    }

    private void CreateUIManager()
    {
        var uiManagerGO = Instantiate(SL.Get<GeneralComponentsService>().GetPrefab(PrefabType.GameUIManager));
        _uiManager = uiManagerGO.GetComponent<FirstSceneUIManager>();
        _uiManager.Init();
    }

    private void OnDestroy()
    {
        PoolManager.ClearPool();
        _uiManager.Unsubscribe();
    }
}
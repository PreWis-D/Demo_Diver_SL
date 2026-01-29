using Cysharp.Threading.Tasks;

public class GameStateService : IService
{
    private InputProcessingService _inputProcessingService;

    public void Initialize()
    {
        _inputProcessingService = SL.Get<InputProcessingService>();
        SL.Get<EventProcessingService>().GameStateChanged += OnGameStateChanged;
    }

    public void Cleanup()
    {
        SL.Get<EventProcessingService>().GameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Gameloop:
                _inputProcessingService.EnableInput();
                break;
            case GameState.Pause:
                _inputProcessingService.DisableInput();
                break;
            case GameState.SwitchScene:
                SwitchScene();
                break;
        }
    }

    private void SwitchScene()
    {
        _inputProcessingService.DisableInput();
        SL.Get<SceneLoadService>().LoadScene(SceneType.FirstGameplayScene).Forget();
        SL.Get<EventProcessingService>().OpenPanelInvoke(PanelType.Load);
    }
}
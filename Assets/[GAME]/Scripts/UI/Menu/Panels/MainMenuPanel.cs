using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class MainMenuPanel : AbstractPanel
{
    [SerializeField] private ButtonExtension _startButton;
    [SerializeField] private ButtonExtension _settingButton;
    [SerializeField] private ButtonExtension _exitButton;

    public override PanelType Type => PanelType.MainMenu;

    public override void Init()
    {
        base.Init();

        _startButton.Init(StartGame);
        _settingButton.Init(OpenSettingPanel);
        _exitButton.Init(ExitGame);
        SL.Get<InputProcessingService>().StartButtonClicked += StartGame;
    }

    private void StartGame()
    {
        SL.Get<EventProcessingService>().OpenPanelInvoke(PanelType.Load);
        SL.Get<SceneLoadService>().LoadScene(SceneType.Hub).Forget();
    }

    private void OpenSettingPanel()
    {
        SL.Get<EventProcessingService>().OpenPanelInvoke(PanelType.Settings);
    }

    private void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    private void OnDisable()
    {
        SL.Get<InputProcessingService>().StartButtonClicked -= StartGame;
    }
}
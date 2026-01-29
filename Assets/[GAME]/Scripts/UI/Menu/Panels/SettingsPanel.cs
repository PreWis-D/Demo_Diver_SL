using UnityEngine;

public class SettingsPanel : AbstractPanel
{
    [SerializeField] private ButtonExtension _exitButton;

    public override PanelType Type => PanelType.Settings;

    public override void Init()
    {
        base.Init();

        _exitButton.Init(Exit);
    }

    private void Exit()
    {
        SL.Get<EventProcessingService>().OpenPanelInvoke(PanelType.MainMenu);
    }
}
using System;

public class MenuUIManager : AbstractUIManager
{
    public override void Init()
    {
        base.Init();

        OnPanelOpened(PanelType.MainMenu);

        SL.Get<EventProcessingService>().PanelOpened += OnPanelOpened;
    }

    private void EnterMainPanel()
    {
        GetPanel(PanelType.MainMenu).Show();
    }

    private void EnterSettingPanel()
    {
        GetPanel(PanelType.Settings).Show();
    }

    private void EnterLoadPanel()
    {
        GetPanel(PanelType.Load).Show();
    }

    private void OnPanelOpened(PanelType panelType)
    {
        HideAllPanels();
        Action action = GetAction(panelType);
        action();
    }

    private Action GetAction(PanelType panelType)
    {
        return panelType switch
        {
            PanelType.MainMenu => EnterMainPanel,
            PanelType.Settings => EnterSettingPanel,
            PanelType.Load => EnterLoadPanel,
            _ => throw new ArgumentException($"Not found action! Panel type {panelType}")
        };
    }

    public void Unsubscribe()
    {
        SL.Get<EventProcessingService>().PanelOpened -= OnPanelOpened;
    }
}
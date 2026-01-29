using System;

public class FirstSceneUIManager : AbstractUIManager
{
    public override void Init()
    {
        base.Init();

        OnPanelOpened(PanelType.MainHub);

        SL.Get<EventProcessingService>().PanelOpened += OnPanelOpened;
    }

    private void EnterMainPanel()
    {
        GetPanel(PanelType.MainFirstScene).Show();
        GetPanel(PanelType.Indicators).Show();
        GetPanel(PanelType.DebugFirstScene).Show();
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
            PanelType.MainHub => EnterMainPanel,
            PanelType.Load => EnterLoadPanel,
            _ => throw new ArgumentException($"Not found action! Panel type {panelType}")
        };
    }

    public void Unsubscribe()
    {
        SL.Get<EventProcessingService>().PanelOpened -= OnPanelOpened;
    }
}
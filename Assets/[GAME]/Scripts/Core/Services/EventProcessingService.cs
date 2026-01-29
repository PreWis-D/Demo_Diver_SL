using System;

public class EventProcessingService : IService
{
    public event Action<PanelType> PanelOpened;
    public event Action<PopupType> PopupOpened;
    public event Action<GameState> GameStateChanged;
    public event Action<IInteractable, bool> InteractableFounded;
    public event Action<ResourceData> ResourceChanged;
    public event Action<ResourceData> CantAddResource;
    public event Action<HandItemData> HandItemChanged;

    public void Initialize()
    {
        
    }

    public void Cleanup()
    { 
    
    }

    public void OpenPanelInvoke(PanelType panelType)
    {
        PanelOpened?.Invoke(panelType);
    }

    public void OpenPopupInvoke(PopupType popupType)
    {
        PopupOpened?.Invoke(popupType);
    }

    public void ChangeGameStateInvoke(GameState gameState)
    {
        GameStateChanged?.Invoke(gameState);
    }

    public void InteractableFoundedInvoke(IInteractable interactable, bool isFounded)
    {
        InteractableFounded?.Invoke(interactable, isFounded);
    }

    public void RecourceChangedInvoke(ResourceData resourceData)
    {
        ResourceChanged?.Invoke(resourceData);
    }

    public void CantAddResourceInvoke(ResourceData resourceData)
    {
        CantAddResource?.Invoke(resourceData);
    }
    
    public void HandItemChangedInvoke(HandItemData handItemData)
    {
        HandItemChanged?.Invoke(handItemData);
    }
}
public abstract class AbstractInteractionHandler
{
    protected IInteractable Interactable;
    protected InputProcessingService Input;
    protected PlayerCharacter PlayerCharacter;

    public virtual void Init(IInteractable interactable)
    {
        Interactable = interactable;
        Input = SL.Get<InputProcessingService>();
        PlayerCharacter = SL.Get<CharactersService>().GetPlayerCharacter();
    }

    public abstract void HandleInteraction();
    public virtual void Reset() { }
}
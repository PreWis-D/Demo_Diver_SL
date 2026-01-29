public class InstantInteractionHandler : AbstractInteractionHandler
{
    public override void HandleInteraction()
    {
        if (Input.InputType == Interactable.InputType)
            Interactable.Execute();
    }
}
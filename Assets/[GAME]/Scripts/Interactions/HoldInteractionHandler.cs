public class HoldInteractionHandler : AbstractInteractionHandler
{
    public override void HandleInteraction()
    {
        if (Input.InputType == Interactable.InputType)
            Interactable.Execute();
        else
            Interactable.Interrupt();
    }

    public override void Reset()
    {
        base.Reset();

        Interactable.Interrupt();
    }
}
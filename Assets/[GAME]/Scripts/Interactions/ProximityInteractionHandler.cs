public class ProximityInteractionHandler : AbstractInteractionHandler
{
    public override void HandleInteraction()
    {
        PlayerCharacter.ChangeVacuumActiveState(true);
    }
}
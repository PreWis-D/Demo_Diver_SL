using System.Collections;
using UnityEngine;

public class AutomaticInteractionHandler : AbstractInteractionHandler
{
    public override void HandleInteraction()
    {
        Debug.Log($"Enter");
        if (Interactable.CanInteract(PlayerCharacter))
            Interactable.Execute();
    }
}
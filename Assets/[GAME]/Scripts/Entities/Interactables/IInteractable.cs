using UnityEngine;

public interface IInteractable
{
    InteractionType InteractionType {  get; }
    InputType InputType {  get; }
    Transform Transform {  get; }
    FloatParameter FloatParameter {  get; }
    float HoldDuration { get; }

    void Init();
    void Execute();
    void Interrupt();
    bool CanInteract(PlayerCharacter player);
}
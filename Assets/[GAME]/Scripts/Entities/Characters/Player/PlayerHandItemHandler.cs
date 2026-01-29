using System;

public class PlayerHandItemHandler
{
    private PlayerCharacter _playerCharacter;
    private PlayerInventory _inventory;
    private PlayerData _data;
    private InputProcessingService _input;

    public PlayerHandItemHandler(PlayerCharacter playerCharacter, PlayerData data)
    {
        _playerCharacter = playerCharacter;
        _inventory = playerCharacter.PlayerInventory;
        _data = data;
        _input = SL.Get<InputProcessingService>();
    }

    public void Update()
    {
        if (_inventory.CurrentHandItem.CanInteract())
            HandleItem();
    }

    private void HandleItem()
    {
        switch (_inventory.CurrentHandItem)
        {
            case Knife knife:
                InteractKnife(knife);
                break;
            case Harpoon harpoon:
                InteractHarpoon(harpoon);
                break;
            case Jackhammer jackhammer:
                InteractJackhammer(jackhammer);
                break;
            case GasSuction gasSuction:
                InteractGasSuction(gasSuction);
                break;
        }
    }

    private void InteractKnife(Knife knife)
    {
        _playerCharacter.Animator.AttackKnife((bool attack) =>
        {
            if (attack)
                knife.Interact();
            else
                knife.Interrupt();
        });
    }

    private void InteractHarpoon(Harpoon harpoon)
    {
        harpoon.Interact();
    }

    private void InteractJackhammer(Jackhammer jackhammer)
    {
        jackhammer.Interact();
    }

    private void InteractGasSuction(GasSuction gasSuction)
    {
        gasSuction.Interact();
    }
}
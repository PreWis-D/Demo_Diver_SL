using UnityEngine;

public class LocationStateTransit : AbstractInteractable
{
    [SerializeField] private Transform _groundPositionPoint;
    [SerializeField] private Transform _waterPositionPoint;

    public bool IndicatorShowed { get; private set; } = true;

    public override bool CanInteract(PlayerCharacter player)
    {
        bool isAvailable = base.CanInteract(player);

        PlayerData playerData = SL.Get<CharactersService>().GetPlayerData();
        var direction = _groundPositionPoint.position - player.transform.position;

        if (isAvailable == false)
            return false;
        else if (playerData.LocationSpace == LocationSpace.Water &&
            (direction.x > 0 && player.View.transform.localEulerAngles.y < 180 ||
            direction.x < 0 && player.View.transform.localEulerAngles.y > 180))
            return true;
        else 
            return false;
    }

    public override void Interrupt()
    {
        base.Interrupt();

        Collider.enabled = true;
    }

    protected override void ExecuteDone()
    {
        PlayerCharacter playerCharacter = SL.Get<CharactersService>().GetPlayerCharacter();
        PlayerData playerData = SL.Get<CharactersService>().GetPlayerData();

        if (playerData.LocationSpace == LocationSpace.Water)
        {
            PlayerLadderClimbState state = playerCharacter.StateMachine.GetState<PlayerLadderClimbState>();
            state.SetEndPosition(_waterPositionPoint.position, _groundPositionPoint.position);
            playerCharacter.StateMachine.SetState<PlayerLadderClimbState>();
        }
    }
}
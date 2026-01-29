using UnityEngine;

public class Ledge : AbstractInteractable
{
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _finishPoint;

    public override bool CanInteract(PlayerCharacter player)
    {
        bool isAvailable = base.CanInteract(player);

        var direction = _finishPoint.position - player.transform.position;

        if (isAvailable == false)
            return false;
        else if (player.StateMachine.CurrentEntityState is PlayerJumpState &&
            (direction.x > 0 && player.View.transform.localEulerAngles.y < 180 ||
            direction.x < 0 && player.View.transform.localEulerAngles.y > 180))
            return true;
        else
            return false;
    }

    protected override void ExecuteDone()
    {
        PlayerCharacter playerCharacter = SL.Get<CharactersService>().GetPlayerCharacter();
        PlayerData playerData = SL.Get<CharactersService>().GetPlayerData();

        PlayerLadderClimbState state = playerCharacter.StateMachine.GetState<PlayerLadderClimbState>();
        state.SetEndPosition(_startPoint.position, _finishPoint.position);
        playerCharacter.StateMachine.SetState<PlayerLadderClimbState>();
    }
}

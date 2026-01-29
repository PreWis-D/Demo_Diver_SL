using DG.Tweening;
using UnityEngine;

public class PlayerSwitchVectorZState : EntityState
{
    private PlayerCharacter Player => (PlayerCharacter)Entity;
    private PlayerData _playerData;
    private InputProcessingService _input;

    private Tween _tween;

    public PlayerSwitchVectorZState(EntityStateMachine stateMachine, Entity entity) : base(stateMachine, entity)
    {
        _playerData = SL.Get<CharactersService>().GetPlayerData();
        _input = SL.Get<InputProcessingService>();
    }

    public override void Enter()
    {
        base.Enter();

        _input.DisableInput();

        float zVector = _playerData.SwitchLayerZState();
        Vector3 target = Player.transform.position;
        target.z = zVector;
        _tween.Kill();
        _tween = Player.transform.DOMove(target, 1f);
        _tween.OnComplete(() =>
        {
            stateMachine.SetState<PlayerWaterState>();
        });
    }

    public override void Exit()
    {
        base.Exit();

        _tween.Kill();
        _input.EnableInput();
    }
}
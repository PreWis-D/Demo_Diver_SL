using System;
using System.Collections;
using UnityEngine;

public class PlayerLadderClimbState : EntityState
{
    private PlayerCharacter Player => (PlayerCharacter)Entity;
    private PlayerData _playerData;
    private InputProcessingService _input;
    private CapsuleCollider _collider;

    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private float _progress;

    public PlayerLadderClimbState(EntityStateMachine stateMachine, Entity entity) : base(stateMachine, entity)
    {
        _playerData = SL.Get<CharactersService>().GetPlayerData();
        _input = SL.Get<InputProcessingService>();
        _collider = Player.Collider.GetComponent<CapsuleCollider>();
    }

    public override void FixedUpdate()
    {
        var deltaTime = Time.fixedDeltaTime;
        var position = Vector3.Lerp(_startPosition, _endPosition, _progress);
        position.y += _playerData.MovementData.CurveJumpRaft.Evaluate(_progress) * _playerData.MovementData.HeightLadderToGround;
        _progress += deltaTime * _playerData.MovementData.SpeedLadderToGround;
        Player.Rigidbody.MovePosition(position);

        if (_progress >= 1f)
        {
            Player.View.localEulerAngles = Vector3.zero;
            stateMachine.SetState<PlayerGroundState>();
        }
    }

    public override void Enter()
    {
        base.Enter();

        _input.DisableInput();

        _collider.direction = Constants.Y_AXIS;
        _collider.enabled = false;
        Player.Rigidbody.interpolation = RigidbodyInterpolation.None;
        Player.transform.position = _startPosition;
        Player.transform.localEulerAngles = new Vector3(0, Player.transform.localEulerAngles.y, 0);
        _progress = 0f;
        Player.Animator.LadderClimb();
        Player.Animator.ChangeGroundedState(false);
    }

    public override void Exit()
    {
        Player.Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        Player.Rigidbody.useGravity = true;
        Player.Animator.ChangeGroundedState(true);
        _collider.enabled = true;
        _input.EnableInput();

        base.Exit();
    }

    public void SetEndPosition(Vector3 startPosition, Vector3 endPosition)
    {
        _startPosition = startPosition;
        _endPosition = endPosition;
    }
}
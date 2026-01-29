using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Rig _rig;
    [SerializeField, Range(0f, 1f)] private float _handWeight;

    private Animator _animator;
    private PlayerCharacter _playerCharacter;
    private PlayerData _playerData;
    private LocationSpace _currentSpace;
    private bool _isAim;

    private Action<bool> _attackAction;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Init(PlayerCharacter playerCharacter, PlayerData playerData)
    {
        _playerCharacter = playerCharacter;
        _playerData = playerData;
    }

    private void Update()
    {
        _animator.SetFloat(Constants.SPEED, _playerData.AnimationBlendSpeed);

        HandleLocationSpace();
        HandleAimState();
    }

    private void HandleLocationSpace()
    {
        if (_currentSpace != _playerData.LocationSpace)
        {
            _currentSpace = _playerData.LocationSpace;
            switch (_currentSpace)
            {
                case LocationSpace.Water:
                    _animator.SetBool(Constants.SWIM, true);
                    break;
            }
        }
    }

    private void HandleAimState()
    {
        if (_isAim != _playerData.IsAim)
        {
            _isAim = _playerData.IsAim;
            _animator.SetBool(Constants.AIM, _isAim);

            _rig.weight = _isAim ? 1 : 0;
        }
    }

    public void LadderClimb()
    {
        _animator.SetBool(Constants.SWIM, false);
        _animator.SetTrigger(Constants.CLIMB);
    }

    public void ChangeJumpState(bool isJump)
    {
        _animator.SetBool(Constants.JUMP, isJump);
    }

    public void ChangeGroundedState(bool isGrounded)
    {
        _animator.SetBool(Constants.GROUNDED, isGrounded);
    }

    public void AttackKnife(Action<bool> action)
    {
        _attackAction = action;
        _animator.SetBool(Constants.ATTACK, true);
    }

    #region Events
    public void AttackStarted()
    {
        _attackAction?.Invoke(true);
    }

    public void AttackFinished()
    {
        _attackAction?.Invoke(false);
        _animator.SetBool(Constants.ATTACK, false);
    }
    #endregion

    private void OnAnimatorIK(int layerIndex)
    {
        if (_animator == null)
            throw new NullReferenceException($"Animator is null");

        Transform leftHandPoint = _playerCharacter.PlayerInventory.GetItemLeftHandPoint();
        Transform rightHandPoint = _playerCharacter.PlayerInventory.GetItemRightHandPoint();
        bool isItemVisible = _playerCharacter.PlayerInventory.HasItemVisible();

        if (leftHandPoint && rightHandPoint && isItemVisible)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _handWeight);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _handWeight);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPoint.position);
            _animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandPoint.rotation);

            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _handWeight);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _handWeight);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPoint.position);
            _animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandPoint.rotation);
        }
    }
}
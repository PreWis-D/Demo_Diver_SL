using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputProcessingService : IService
{
    private InputActions _inputActions;
    private GamepadInputHandler _gamepadInput;
    private InputConfig _inputConfig;

    private bool _isUsingGamepad;
    private SceneType _currentSceneType;

    public Vector2 Direction { get; private set; }
    public Vector2 AimPosition { get; private set; }
    public bool IsRun { get; private set; }
    public bool IsFiring { get; private set; }
    public bool IsInteract { get; private set; }
    public bool IsAim { get; private set; }
    public bool IsStart { get; private set; }
    public bool IsJump { get; private set; }
    public InputControl InputControl { get; private set; }
    public InputType InputType { get; private set; }

    public event Action StartButtonClicked;
    public event Action InteractStarted;
    public event Action InteractCanceled;

    public event Action<int> SwitchTargetItem;
    public event Action SwitchPreviousItem;
    public event Action SwitchNextItem;

    public InputProcessingService(InputConfig inputConfig)
    {
        _inputConfig = inputConfig;
    }

    public void Initialize()
    {
        _inputActions = new InputActions();
        _gamepadInput = new GamepadInputHandler(_inputConfig);
        _currentSceneType = SL.Get<SceneLoadService>().GetSceneType();

        Subscribe();
    }

    public void EnableInput()
    {
        _inputActions.Enable();
    }

    public void DisableInput()
    {
        _inputActions.Disable();
    }

    public void Update()
    {
        Direction = _inputActions.Player.Move.ReadValue<Vector2>();

        if (_isUsingGamepad)
            HandleGamepadAim();
    }

    public void SetStartAimPosition(Vector2 position)
    {
        AimPosition = position;
    }

    private void Subscribe()
    {
        InputSystem.onActionChange += OnActionChange;

        _inputActions.Player.Sprint.started += OnSprinting;
        _inputActions.Player.Sprint.performed += OnSprinting;
        _inputActions.Player.Sprint.canceled += OnSprinting;

        _inputActions.Player.Attack.started += OnAttack;
        _inputActions.Player.Attack.canceled += OnAttack;

        _inputActions.Player.Interact.started += OnInteract;
        _inputActions.Player.Interact.performed += OnInteract;
        _inputActions.Player.Interact.canceled += OnInteract;

        if (_currentSceneType == SceneType.FirstGameplayScene)
        {
            _inputActions.Player.Aim.started += OnAim;
            _inputActions.Player.Aim.performed += OnAim;
            _inputActions.Player.Aim.canceled += OnAim;

            _inputActions.Player.AimPosition.performed += OnAimMouseHandle;

            _inputActions.Player.Jump.started += OnJump;
            _inputActions.Player.Jump.canceled += OnJump;

            _inputActions.Player.PreviousHandItemGamepad.started += OnPreviousHandItemGamepad;
            _inputActions.Player.NextHandItemGamepad.started += OnNextHandItemGamepad;
            _inputActions.Player.FirstHandItem.started += OnFirstHandItem;
            _inputActions.Player.SecondHandItem.started += OnSecondHandItem;
            _inputActions.Player.ThirdHandItem.started += OnThirdHandItem;
            _inputActions.Player.FourthHandItem.started += OnFourthHandItem;
            _inputActions.Player.SwitchHandItemMouse.started += OnSwitchHandItemMouse;
        }

        _inputActions.UI.Start.started += OnStart;
    }

    private void OnStart(InputAction.CallbackContext context)
    {
        StartButtonClicked?.Invoke();
    }

    private void Unsubscribe()
    {
        InputSystem.onActionChange -= OnActionChange;

        _inputActions.Player.Sprint.started -= OnSprinting;
        _inputActions.Player.Sprint.performed -= OnSprinting;
        _inputActions.Player.Sprint.canceled -= OnSprinting;

        _inputActions.Player.Attack.started -= OnAttack;
        _inputActions.Player.Attack.canceled -= OnAttack;

        _inputActions.Player.Interact.started -= OnInteract;
        _inputActions.Player.Interact.performed -= OnInteract;
        _inputActions.Player.Interact.canceled -= OnInteract;

        if (_currentSceneType == SceneType.FirstGameplayScene)
        {
            _inputActions.Player.Aim.started -= OnAim;
            _inputActions.Player.Aim.performed -= OnAim;
            _inputActions.Player.Aim.canceled -= OnAim;

            _inputActions.Player.AimPosition.performed -= OnAimMouseHandle;

            _inputActions.Player.Jump.started -= OnJump;
            _inputActions.Player.Jump.canceled -= OnJump;

            _inputActions.Player.PreviousHandItemGamepad.started -= OnPreviousHandItemGamepad;
            _inputActions.Player.NextHandItemGamepad.started -= OnNextHandItemGamepad;
            _inputActions.Player.FirstHandItem.started -= OnFirstHandItem;
            _inputActions.Player.SecondHandItem.started -= OnSecondHandItem;
            _inputActions.Player.ThirdHandItem.started -= OnThirdHandItem;
            _inputActions.Player.FourthHandItem.started -= OnFourthHandItem;
            _inputActions.Player.SwitchHandItemMouse.started -= OnSwitchHandItemMouse;
        }

        _inputActions.UI.Start.started -= OnStart;
    }

    private void OnSprinting(InputAction.CallbackContext context)
    {
        IsRun = context.ReadValue<float>() > 0;
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        IsFiring = context.ReadValue<float>() > 0;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        IsInteract = context.ReadValue<float>() > 0;

        InputType = IsInteract ? InputType.Interact : InputType.None;
        if (IsInteract)
            InteractStarted?.Invoke();
        else
            InteractCanceled?.Invoke();
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        IsAim = context.ReadValue<float>() > 0;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        IsJump = context.ReadValue<float>() > 0;

        InputType = IsJump ? InputType.Jump : InputType.None;
    }

    private void OnAimMouseHandle(InputAction.CallbackContext context)
    {
        if (_isUsingGamepad)
            return;

        AimPosition = context.ReadValue<Vector2>();
    }

    private void HandleGamepadAim()
    {
        var gamepadAim = _inputActions.Player.AimPositionGamepad.ReadValue<Vector2>();

        gamepadAim.Normalize();
        IsAim = gamepadAim != Vector2.zero;

        AimPosition = _gamepadInput.CalculateGamepadPosition(AimPosition, gamepadAim);

        if (IsAim == false)
            _gamepadInput.Reset();
    }

    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed)
        {
            var action = (InputAction)obj;
            InputControl = action.activeControl;

            if (InputControl != null)
            {
                bool wasUsingGamepad = _isUsingGamepad;
                _isUsingGamepad = InputControl.device is Gamepad;

                if (wasUsingGamepad != _isUsingGamepad)
                {
                    if (_isUsingGamepad)
                    {
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                    else
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                    }
                }
            }
        }
    }

    #region Switch hand items
    private void OnSwitchHandItemMouse(InputAction.CallbackContext context)
    {
        Vector2 scrollValue = context.ReadValue<Vector2>();

        if (scrollValue.y > 0)
            SwitchNextItem?.Invoke();
        else if (scrollValue.y < 0)
            SwitchPreviousItem?.Invoke();
    }

    private void OnFirstHandItem(InputAction.CallbackContext context)
    {
        SwitchTargetItem?.Invoke(0);
    }

    private void OnSecondHandItem(InputAction.CallbackContext context)
    {
        SwitchTargetItem?.Invoke(1);
    }

    private void OnThirdHandItem(InputAction.CallbackContext context)
    {
        SwitchTargetItem?.Invoke(2);
    }

    private void OnFourthHandItem(InputAction.CallbackContext context)
    {
        SwitchTargetItem?.Invoke(3);
    }

    private void OnPreviousHandItemGamepad(InputAction.CallbackContext context)
    {
        SwitchPreviousItem?.Invoke();
    }

    private void OnNextHandItemGamepad(InputAction.CallbackContext context)
    {
        SwitchNextItem?.Invoke();
    }
    #endregion

    public void Cleanup()
    {
        DisableInput();
        Unsubscribe();
    }
}
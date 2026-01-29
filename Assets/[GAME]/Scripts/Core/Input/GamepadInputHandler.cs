using UnityEngine;

public class GamepadInputHandler
{
    private float _gamepadDeadzone = 0.1f;
    private float _snapSpeed = 10f;
    private float _aimRadius = 300f;
    private float _centerScreenOffsetY = 125f;
    private Vector2 _currentGamepadInput;

    public GamepadInputHandler(InputConfig inputConfig)
    {
        _gamepadDeadzone = inputConfig.GamepadDeadzone;
        _snapSpeed = inputConfig.SnapSpeed;
        _aimRadius = inputConfig.AimEdgeRadius;
        _centerScreenOffsetY = inputConfig.CenterScreenOffsetY;
    }

    public void Reset()
    {
        _currentGamepadInput = Vector2.zero;
    }

    public Vector2 CalculateGamepadPosition(Vector2 aimPosition, Vector2 inputVector)
    {
        _currentGamepadInput = inputVector;

        Vector2 normalizedInput = ApplyCircularDeadzone(_currentGamepadInput, _gamepadDeadzone);

        Vector2 screenCenter = new Vector2((Screen.width * 0.5f), (Screen.height * 0.5f) + _centerScreenOffsetY);

        if (normalizedInput.magnitude > 0)
        {
            Vector2 targetDirection = normalizedInput.normalized;
            Vector2 targetPosition = screenCenter + targetDirection * _aimRadius;

            aimPosition = Vector2.Lerp(aimPosition, targetPosition, Time.deltaTime * _snapSpeed);
        }
        else
        {
            aimPosition = Vector2.Lerp(aimPosition, screenCenter, Time.deltaTime * 5f);
        }

        return aimPosition;
    }

    private Vector2 ApplyCircularDeadzone(Vector2 input, float deadzone)
    {
        float magnitude = input.magnitude;

        if (magnitude < deadzone)
            return Vector2.zero;

        float scaledMagnitude = (magnitude - deadzone) / (1 - deadzone);
        return input.normalized * scaledMagnitude;
    }
}
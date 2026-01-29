using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpritesService : IService
{
    private SpritesConfig _config;

    public SpritesService(SpritesConfig spritesConfig)
    {
        _config = spritesConfig;
    }

    public void Initialize()
    {
    }

    public void Cleanup()
    {
    }

    public Sprite GetInputSprite(InputType type)
    {
        InputSpriteData data = _config.InputSpriteDatas.FirstOrDefault(c => c.Type == type);
        InputControl inputControl = SL.Get<InputProcessingService>().InputControl;

        return inputControl.device switch
        {
            Gamepad gamepad => data.GamepadSprite,
            Keyboard keyboard => data.KeyboardMouseSprite,
            Mouse mouse => data.KeyboardMouseSprite,
            _ => throw new ArgumentNullException($"unknown device {inputControl.device}")
        };
    }

    public Sprite GetHandItemSprite(HandItemType type)
    {
        return _config.HandItemSpriteDatas.FirstOrDefault(c => c.Type == type).Icon;
    }
}
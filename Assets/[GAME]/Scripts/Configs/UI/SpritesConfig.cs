using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpritesConfig", menuName = "Configs/UI/SpritesConfig")]
public class SpritesConfig : ScriptableObject
{
    [field: SerializeField] public InputSpriteData[] InputSpriteDatas { get; private set; }
    [field: SerializeField] public HandItemSpriteData[] HandItemSpriteDatas { get; private set; }

    private void OnValidate()
    {
        foreach (var data in InputSpriteDatas)
            data.Id = $"{data.Type}";

        foreach (var data in HandItemSpriteDatas) 
            data.Id = $"{data.Type}";
    }
}

[Serializable]
public class InputSpriteData
{
    [ReadOnlyField] public string Id;
    [field: SerializeField] public InputType Type { get; private set; }
    [field: SerializeField] public Sprite KeyboardMouseSprite { get; private set; }
    [field: SerializeField] public Sprite GamepadSprite { get; private set; }
}

[Serializable]
public class HandItemSpriteData
{
    [ReadOnlyField] public string Id;
    [field: SerializeField] public HandItemType Type { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
}
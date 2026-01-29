using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HandItemsConfig", menuName = "Configs/HandItemsConfig")]
public class HandItemsConfig : ScriptableObject
{
    [field: SerializeField] public JackhammerData JackhammerData { get; private set; }
    [field: SerializeField] public GasSuctionData GasSuctionData { get; private set; }
    [field: SerializeField] public HarpoonData HarpoonData { get; private set; }
    [field: SerializeField] public KnifeData KnifeData { get; private set; }

    public HandItemData GetData(HandItemType type)
    {
        return type switch
        {
            HandItemType.Jackhammer => JackhammerData,
            HandItemType.GasSuction => GasSuctionData,
            HandItemType.Harpoon => HarpoonData,
            HandItemType.Knife => KnifeData,
            _ => null
        };
    }

    private void OnValidate()
    {
        JackhammerData.Id = $"{JackhammerData.Type}";
        GasSuctionData.Id = $"{GasSuctionData.Type}";
        HarpoonData.Id = $"{HarpoonData.Type}";
        KnifeData.Id = $"{KnifeData.Type}";
    }
}

public abstract class HandItemData
{
    [ReadOnlyField] public string Id;
    public abstract HandItemType Type { get; }
}

[Serializable]
public class JackhammerData : HandItemData
{
    public override HandItemType Type => HandItemType.Jackhammer;
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float StartOperatingTimeBeforeOverheating { get; private set; }
    [field: SerializeField][Range(0, 1)] public float CoolingTime { get; private set; }
}

[Serializable]
public class GasSuctionData : HandItemData
{
    public override HandItemType Type => HandItemType.GasSuction;
    [field: SerializeField] public float StartMaxWorkload { get; private set; }
}

[Serializable]
public class HarpoonData : HandItemData
{
    public override HandItemType Type => HandItemType.Harpoon;
    [field: SerializeField] public float StartDamage { get; private set; }
    [field: SerializeField] public float BulletSpeed { get; private set; }
    [field: SerializeField] public float ReloadTime { get; private set; }
}

[Serializable]
public class KnifeData: HandItemData
{
    public override HandItemType Type => HandItemType.Knife;
    [field: SerializeField] public float StartDamage { get; private set; }
}
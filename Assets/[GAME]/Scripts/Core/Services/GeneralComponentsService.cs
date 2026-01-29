using System;
using System.Linq;
using UnityEngine;
using static UnityEngine.Analytics.IAnalytic;

public class GeneralComponentsService : IService
{
    private GeneralSettingsConfig _generalSettingsConfig;
    private PrefabsConfig _prefabsConfig;
    private HandItemsConfig _handItemsConfig;

    public GeneralComponentsService(
        GeneralSettingsConfig config, 
        PrefabsConfig prefabsConfig,
        HandItemsConfig handItemsConfig)
    {
        _generalSettingsConfig = config;
        _prefabsConfig = prefabsConfig;
        _handItemsConfig = handItemsConfig;
    }

    public void Initialize()
    {
        foreach (var data in _prefabsConfig.EntityDatas)
            if (_prefabsConfig.UIDatas.Contains(data))
                throw new ArgumentException($"The same type <color=yellow>{data.Type}</color> should not be in several lists!");
    }

    public void Cleanup()
    {

    }

    public GeneralSettingsConfig GetGeneralSettingsConfig()
    {
        return _generalSettingsConfig;
    }

    public PrefabsConfig GetPrefabsConfig()
    {
        return _prefabsConfig;
    }

    public GameObject GetPrefab(PrefabType type)
    {
        var prefabData = _prefabsConfig.EntityDatas
        .Concat(_prefabsConfig.UIDatas)
        .FirstOrDefault(d => d.Type == type);

        if (prefabData == null)
            throw new NullReferenceException($"There is no prefab with type <color=yellow>{type}</color> in the configuration!");

        return prefabData.Prefab;
    }

    public HandItemsConfig GetHandItemsConfig()
    {
        return _handItemsConfig;
    }
}
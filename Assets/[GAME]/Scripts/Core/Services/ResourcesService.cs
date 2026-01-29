using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesService : IService
{
    private Dictionary<ResourceType, int> _currency = new Dictionary<ResourceType, int>()
    {
        {ResourceType.Money, 0 }
    };

    private Dictionary<ResourceType, int> _resources = new Dictionary<ResourceType, int>()
    {
        { ResourceType.Plastic, 0 },
        { ResourceType.Metall, 0 }
    };

    private PlayerData _playerData;

    public void Initialize()
    {
        _playerData = SL.Get<CharactersService>().GetPlayerData();
    }

    public void Cleanup()
    {

    }

    public bool CheckAddResource(ResourceData resourceData)
    {
        if (_resources.ContainsKey(resourceData.Type))
        {
            int currentWorkload = _playerData.WorkloadParam.CurrentValue;
            bool isAvailable = _playerData.WorkloadParam.MaxValue > currentWorkload + resourceData.Value;

            if (isAvailable == false)
                SL.Get<EventProcessingService>().CantAddResourceInvoke(resourceData);

            return isAvailable;
        }
        else if (_currency.ContainsKey(resourceData.Type))
        {
            return true;
        }
        else
        {
            Debug.LogAssertion($"The resource({resourceData.id}) type({resourceData.Type}) does not fit more than one list!");
            return false;
        }
    }

    public void AddResource(ResourceData resourceData)
    {
        if (_resources.ContainsKey(resourceData.Type))
            _resources[resourceData.Type] += resourceData.Value;
        else if (_currency.ContainsKey(resourceData.Type))
            _currency[resourceData.Type] += resourceData.Value;

        SL.Get<EventProcessingService>().RecourceChangedInvoke(resourceData);
    }
}
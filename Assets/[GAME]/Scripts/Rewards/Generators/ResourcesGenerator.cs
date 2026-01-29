using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourcesGenerator : MonoBehaviour
{
    [SerializeField] private ResourcesConfig _config;
    [SerializeField] private ChestResourceGenerateData[] _generateDatas;

    [ReadOnlyField] public List<string> _rewardItemIds = new List<string>();

    public List<ResourceData> GetResourceDatas()
    {
        List<ResourceData> resources = new List<ResourceData>();

        foreach (var item in _rewardItemIds)
            resources.Add(_config.Datas.FirstOrDefault(c => c.id == item));

        return resources;
    }

#if UNITY_EDITOR
    public void GenerateReward()
    {
        Debug.Log($"Generate reward for chest");

        Undo.RecordObject(this, "Generate Chest Reward");
        ClearGeneratedItems();

        if (_generateDatas == null || _config == null)
        {
            Debug.LogError("Chest component, Datas or Config not found!");
            return;
        }

        for (int i = 0; i < _generateDatas.Length; i++)
        {
            var generatedItems = GenerateItemsForReward(_generateDatas[i]);
            if (generatedItems != null)
            {
                _rewardItemIds.AddRange(generatedItems);
            }
        }

        EditorUtility.SetDirty(this);
        Debug.Log($"Generated {_rewardItemIds.Count} items with total value: {CalculateTotalValue()}");
    }

    private List<string> GenerateItemsForReward(ChestResourceGenerateData chestResourceGenerateData)
    {
        List<ResourceData> items = new List<ResourceData>();
        List<ResourceData> availableDatas = new List<ResourceData>();
        List<string> ids = new List<string>();

        foreach (var data in _config.Datas)
        {
            if (data.Type == chestResourceGenerateData.ResourceType)
            {
                availableDatas.Add(data);
            }
        }

        if (availableDatas.Count == 0)
        {
            Debug.LogWarning($"There are no items set for resource type {chestResourceGenerateData.ResourceType} in ResourcesConfig!");
            return ids;
        }

        int remainingValue = chestResourceGenerateData.TotalValue;
        int attempts = 0;
        const int maxAttempts = 1000;

        while (remainingValue > 0 && attempts < maxAttempts)
        {
            attempts++;

            List<ResourceData> affordableItems = new List<ResourceData>();
            foreach (var data in availableDatas)
            {
                if (data.Value <= remainingValue)
                {
                    affordableItems.Add(data);
                }
            }

            if (affordableItems.Count == 0)
            {
                var smallestItem = GetSmallestItem(availableDatas);
                if (smallestItem != null)
                {
                    items.Add(smallestItem);
                    remainingValue -= smallestItem.Value;
                }
                else
                {
                    break;
                }
            }
            else
            {
                int randomIndex = Random.Range(0, affordableItems.Count);
                var selectedItem = affordableItems[randomIndex];
                items.Add(selectedItem);
                remainingValue -= selectedItem.Value;
            }
        }

        if (remainingValue != 0)
        {
            items = OptimizeItems(items, remainingValue, availableDatas, chestResourceGenerateData.TotalValue);
            remainingValue = 0;
        }

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning($"Max attempts reached for generating {chestResourceGenerateData.ResourceType}. Remaining value: {remainingValue}");
        }

        ShuffleList(items);

        foreach (var item in items)
            ids.Add(item.id);

        return ids;
    }

    private ResourceData GetSmallestItem(List<ResourceData> datas)
    {
        if (datas.Count == 0) return null;

        ResourceData smallest = datas[0];
        foreach (var data in datas)
        {
            if (data.Value < smallest.Value)
            {
                smallest = data;
            }
        }
        return smallest;
    }

    private List<ResourceData> OptimizeItems(List<ResourceData> currentItems, int remainingValue,
                                            List<ResourceData> availableDatas, int targetValue)
    {
        if (currentItems.Count == 0) return currentItems;

        int currentSum = 0;
        foreach (var item in currentItems)
        {
            currentSum += item.Value;
        }

        int difference = targetValue - currentSum;

        if (Mathf.Abs(difference) <= 0) return currentItems;

        for (int i = 0; i < currentItems.Count; i++)
        {
            for (int j = 0; j < availableDatas.Count; j++)
            {
                int newSum = currentSum - currentItems[i].Value + availableDatas[j].Value;
                int newDifference = targetValue - newSum;

                if (Mathf.Abs(newDifference) < Mathf.Abs(difference))
                {
                    currentItems[i] = availableDatas[j];
                    currentSum = newSum;
                    difference = newDifference;

                    if (difference == 0) return currentItems;
                }
            }
        }

        while (difference > 0)
        {
            ResourceData bestFit = null;
            int bestFitValue = int.MaxValue;

            foreach (var data in availableDatas)
            {
                if (data.Value <= difference && data.Value < bestFitValue)
                {
                    bestFit = data;
                    bestFitValue = data.Value;
                }
            }

            if (bestFit != null)
            {
                currentItems.Add(bestFit);
                difference -= bestFit.Value;
            }
            else
            {
                break;
            }
        }

        return currentItems;
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private int CalculateTotalValue()
    {
        int total = 0;
        foreach (var item in _rewardItemIds)
        {
            total += _config.Datas.FirstOrDefault(c => c.id == item).Value;
        }
        return total;
    }

    private void ClearGeneratedItems()
    {
        _rewardItemIds.Clear();
    }
#endif
}

[Serializable]
public class ChestResourceGenerateData
{
    [field: SerializeField] public ResourceType ResourceType { get; private set; }
    [field: SerializeField] public int TotalValue { get; private set; }
}
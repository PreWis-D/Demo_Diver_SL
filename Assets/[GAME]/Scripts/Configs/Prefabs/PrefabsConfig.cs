using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabsConfig", menuName = "Configs/PrefabsConfig")]
public class PrefabsConfig : ScriptableObject
{
    [field: SerializeField] public PrefabData[] EntityDatas { get; private set; }
    [field: SerializeField] public PrefabData[] UIDatas { get; private set; }

    private void OnValidate()
    {
        foreach (var data in EntityDatas)
            data.Id = $"{data.Type}";

        foreach (var data in UIDatas)
            data.Id = $"{data.Type}";
    }
}

[Serializable]
public class PrefabData
{
    [ReadOnlyField] public string Id;
    [field: SerializeField] public PrefabType Type { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
}
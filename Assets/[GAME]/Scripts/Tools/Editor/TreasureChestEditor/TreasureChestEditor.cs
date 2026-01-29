#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResourcesGenerator))]
public class TreasureChestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ResourcesGenerator generator = (ResourcesGenerator)target;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Statistics", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Generated Items: {generator.GetResourceDatas().Count}");

        if (generator.GetResourceDatas().Count > 0)
        {
            int totalValue = 0;
            foreach (var item in generator.GetResourceDatas())
            {
                totalValue += item.Value;
            }
            EditorGUILayout.LabelField($"Total Value: {totalValue}");
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Reward", GUILayout.Height(30)))
        {
            generator.GenerateReward();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Clear Generated Items", GUILayout.Height(25)))
        {
            Undo.RecordObject(generator, "Clear Generated Items");
            generator.GetComponent<ResourcesGenerator>().GetResourceDatas().Clear();
            EditorUtility.SetDirty(generator);
        }
    }
}
#endif
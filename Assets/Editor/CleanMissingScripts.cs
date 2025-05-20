using UnityEngine;
using UnityEditor;
using System.IO;

public class MissingScriptCleanerAdvanced
{
    [MenuItem("Tools/Cleanup/ Deep Clean Missing Scripts in All Prefabs")]
    static void DeepCleanPrefabs()
    {
        string[] prefabPaths = Directory.GetFiles("Assets", "*.prefab", SearchOption.AllDirectories);
        int totalRemoved = 0;
        int totalPrefabs = 0;

        foreach (string path in prefabPaths)
        {
            GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefabAsset == null) continue;

            GameObject prefabInstance = PrefabUtility.LoadPrefabContents(path);
            int removed = RemoveMissingInHierarchy(prefabInstance);

            if (removed > 0)
            {
                PrefabUtility.SaveAsPrefabAsset(prefabInstance, path);
                Debug.Log($" Cleaned {removed} missing scripts in: {path}");
                totalRemoved += removed;
            }

            PrefabUtility.UnloadPrefabContents(prefabInstance);
            totalPrefabs++;
        }

        Debug.Log($" Deep clean complete! {totalRemoved} missing scripts removed from {totalPrefabs} prefabs.");
    }

    static int RemoveMissingInHierarchy(GameObject go)
    {
        int count = 0;
        Transform[] allChildren = go.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in allChildren)
        {
            count += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
        }
        return count;
    }
}

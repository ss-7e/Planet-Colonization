using UnityEngine;
using UnityEditor;
using System.IO;

public class SetFBXReadWriteUtility
{
    [MenuItem("Tools/Set FBX Read/Write Enabled")]
    public static void SetAllFBXReadWrite()
    {
        string folderPath = EditorUtility.OpenFolderPanel("Select Folder with FBX files", "Assets", "");

        if (string.IsNullOrEmpty(folderPath))
            return;

        string projectPath = Application.dataPath;
        if (!folderPath.StartsWith(projectPath))
        {
            Debug.LogError("Selected folder must be inside the Unity project Assets folder.");
            return;
        }

        string relativePath = "Assets" + folderPath.Substring(projectPath.Length);

        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { relativePath });

        int modifiedCount = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;

            if (importer != null && !importer.isReadable)
            {
                importer.isReadable = true;
                importer.SaveAndReimport();
                modifiedCount++;
                Debug.Log($" Enabled Read/Write on: {assetPath}");
            }
        }

        Debug.Log($" finish: changed {modifiedCount} models.");
    }
}

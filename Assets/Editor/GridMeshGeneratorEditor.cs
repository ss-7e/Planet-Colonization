using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridMeshGenerator))]
public class GridMeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridMeshGenerator generator = (GridMeshGenerator)target;

        if (GUILayout.Button("Generate Grid Mesh (Editor)"))
        {
            generator.GenerateGridMesh();

            EditorUtility.SetDirty(generator.gameObject);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(generator.gameObject.scene);
        }
    }
}

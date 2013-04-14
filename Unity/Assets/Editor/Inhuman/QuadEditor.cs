using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuadMesh))]
public class QuadEditor : Editor
{
    [MenuItem("Inhuman/Quad Mesh")]
    static void DoIt()
    {
        // Create Mesh //
        GameObject go = new GameObject("Quad");
        go.AddComponent<QuadMesh>();
    }

    //======================================================================================================================================//
    void OnSceneGUI()
    {
    }

    //======================================================================================================================================//
    public override void OnInspectorGUI()
    {
        QuadMesh mesh = (QuadMesh)target;

        EditorGUILayout.BeginVertical();

            QuadMesh.PivotType pivot = (QuadMesh.PivotType)EditorGUILayout.EnumPopup("Pivot", mesh.Pivot);
            if (pivot != mesh.Pivot)
            {
                mesh.Pivot = pivot;
                mesh.GenerateMesh();
            }

            float buffer = EditorGUILayout.FloatField("Width", mesh.Width);
            if (buffer != mesh.Width)
            {
                mesh.Width = buffer;
                mesh.GenerateMesh();
            }

            buffer = EditorGUILayout.FloatField("Height", mesh.Height);
            if (buffer != mesh.Height)
            {
                mesh.Height = buffer;
                mesh.GenerateMesh();
            }

            Vector3 vectorBuffer = EditorGUILayout.Vector3Field("Offset", mesh.Offset);
            if (vectorBuffer != mesh.Offset)
            {
                mesh.Offset = vectorBuffer;
                mesh.GenerateMesh();
            }

            Vector2 vector2 = EditorGUILayout.Vector2Field("UV Scale", mesh.UVScale);
            if (vector2 != mesh.UVScale)
            {
                mesh.UVScale = vector2;
                mesh.GenerateMesh();
            }

            vector2 = EditorGUILayout.Vector2Field("UV Offset", mesh.UVOffset);
            if (vector2 != mesh.UVOffset)
            {
                mesh.UVOffset = vector2;
                mesh.GenerateMesh();
            }

        EditorGUILayout.EndVertical();
        Repaint();
    }
}
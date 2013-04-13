using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Inhuman
{
    public class Replacer : EditorWindow
    {
        List<Object> Replacements = new List<Object>();

        //============================================================================================================================================//
        [MenuItem("Inhuman/Replacer")]
        static void Init()
        {
            EditorWindow window = EditorWindow.GetWindow(typeof(Replacer));
            window.title = "Replacer";
        }

        //============================================================================================================================================//
        void OnGUI()
        {
            //EditorGUILayout.Foldout(true, "Selected Objects");
            GUILayout.Label("Selected Objects", EditorStyles.boldLabel);

            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                GUILayout.Label(Selection.gameObjects[i].name);
            }
            //GUILayout.Box(null, GUIStyle.none

            //EditorGUILayout.Foldout(true, "Replacement Objects");
            GUILayout.Label("Replacement Objects", EditorStyles.boldLabel);
            if (GUILayout.Button("New"))
            {
                Replacements.Add(null);
            }

            for (int i = 0; i < Replacements.Count; i++)
            {
                GUILayout.BeginHorizontal();

                Replacements[i] = EditorGUILayout.ObjectField(Replacements[i], typeof(Object), true);
                if (GUILayout.Button("Replace"))
                {
                    ReplaceAll(Replacements[i]);
                }
                if (GUILayout.Button("X"))
                {
                    Replacements.Remove(Replacements[i]);
                }

                GUILayout.EndHorizontal();
            }

            //EditorGUILayout.PropertyField(ColorA, true);

            //EditorGUILayout.Foldout(true, "Options");
            //GUILayout.Label("Options", EditorStyles.boldLabel);
            //GUILayout.BeginArea();
            //GUILayout.Toggle(true, "Position");
            //GUILayout.Toggle(true, "Rotation");
            //GUILayout.Toggle(true, "Scale");
            //GUILayout.Toggle(false, "Keep Original");
            //EditorGUILayout.EndVertical();
        }

        //============================================================================================================================================//
        void ReplaceAll(Object replacement)
        {
            Transform[] selected = Selection.transforms;

            if (Replacements.Count > 0 && selected.Length > 0)
            {
                Undo.RegisterSceneUndo("Replaced " + selected.Length + " with " + replacement.name);

                for (int i = 0; i < selected.Length; i++)
                {
                    ReplaceObject(selected[i], replacement);
                }
            }
        }

        //============================================================================================================================================//
        void ReplaceObject(Transform selected, Object replacement)
        {
            Transform parent = selected.parent;
            //GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(replacement);
            GameObject newObj = (GameObject)Instantiate(replacement);
           
            newObj.transform.localPosition = selected.position;
            newObj.transform.localRotation = selected.localRotation;
            newObj.transform.localScale = selected.lossyScale;
            newObj.transform.parent = parent;

            DestroyImmediate(selected.gameObject);
        }

        //============================================================================================================================================//
        void OnInspectorUpdate()
        {
            this.Repaint();
        }
    }
}
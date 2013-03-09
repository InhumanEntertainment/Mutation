using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameCurve : MonoBehaviour 
{
    public bool EditMode = false;
    public bool Snapping = true;
    public bool AutoGenerate = true;
    
    public List<Vector2> Vertex = new List<Vector2>();

    public string MeshName = "Default";
    public float BorderThickness = 1;
    public bool CollisionMesh = true;

    public float GridSpacing = 0.5f;
    public int GridSize = 40;

    //======================================================================================================================================//
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        for (int i = 0; i < Vertex.Count - 1; i++)
        {
            Debug.DrawLine(Vertex[i], Vertex[i + 1]);
        }
    }
}
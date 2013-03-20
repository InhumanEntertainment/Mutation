using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ParallaxLayer
{
    public string Name = "";
    public float Depth = 0.5f;
    public Vector3 Offset = new Vector3(0, 0, 0);

    public Color TopColor = Color.white;
    public Color BottomColor = Color.grey;
}

[ExecuteInEditMode]
public class Parallax : MonoBehaviour 
{
    public bool UpdateMesh = false;
	
    // Mesh //
    private MeshFilter Filter;
    private MeshRenderer Renderer;
    private Mesh Mesh;

    // Other //
    public float Width = 10;
    public float Height = 10;
    public Vector3 Amount = new Vector3(1, 1, 0);

    public ParallaxLayer[] Layers;

    //======================================================================================================================================//
    void Awake()
    {
        GenerateMesh();
    }

    //======================================================================================================================================//
    void GenerateMesh()
    {
        //print("Generating Mesh:");
        
        List<Vector3> Vertices = new List<Vector3>();
        List<Vector2> Uvs = new List<Vector2>();
        List<Color> Colors = new List<Color>();
        Dictionary<int, List<int>> Triangles = new Dictionary<int, List<int>>();
        
        for (int i = 0; i < Layers.Length; i++)
        {
            //print("Generating Mesh: Layer " + Layers[i].Name);

            // Create Vertices //
            float halfWidth = Width * 0.5f;
            float halfHeight = Height * 0.5f;

            Vector3 layerOffset = Vector3.Scale(Camera.main.transform.position, Amount) * Layers[i].Depth + Layers[i].Offset;

            Vertices.Add(new Vector3(-halfWidth, halfHeight, 0) + layerOffset);
            Vertices.Add(new Vector3(-halfWidth, -halfHeight, 0) + layerOffset);
            Vertices.Add(new Vector3(halfWidth, halfHeight, 0) + layerOffset);
            Vertices.Add(new Vector3(halfWidth, -halfHeight, 0) + layerOffset);

            // Create Uvs //
            Uvs.Add(new Vector2(0, 1));
            Uvs.Add(new Vector2(0, 0));
            Uvs.Add(new Vector2(1, 1));
            Uvs.Add(new Vector2(1, 0));

            // Create Colors //
            Colors.Add(Layers[i].TopColor);
            Colors.Add(Layers[i].BottomColor);
            Colors.Add(Layers[i].TopColor);
            Colors.Add(Layers[i].BottomColor);

            // Create Triangles //
            Triangles[i] = new List<int>();
            Triangles[i].Add(Vertices.Count - 4);
            Triangles[i].Add(Vertices.Count - 3);
            Triangles[i].Add(Vertices.Count - 2);
            Triangles[i].Add(Vertices.Count - 3);
            Triangles[i].Add(Vertices.Count - 2);
            Triangles[i].Add(Vertices.Count - 1);
        }

        // Generate Mesh //
        if (Mesh == null)
        {
            Mesh = new Mesh();        
        }
        else
        {
            Mesh.Clear();
        }

        Mesh.vertices = Vertices.ToArray();
        Mesh.uv = Uvs.ToArray();
        Mesh.colors = Colors.ToArray();

        if (Layers.Length == 1)
        {
            Mesh.triangles = Triangles[0].ToArray();
        }
        else
        {
            Mesh.subMeshCount = Layers.Length;
            for (int i = 0; i < Layers.Length; i++)
            {
                Mesh.SetTriangles(Triangles[i].ToArray(), i);
            }
        }

        Mesh.RecalculateNormals();
        Mesh.RecalculateBounds();
        Mesh.Optimize();

        Filter = GetComponent<MeshFilter>();
        if (Filter == null)
            Filter = gameObject.AddComponent<MeshFilter>();

        Renderer = GetComponent<MeshRenderer>();
        if (Renderer == null)
            Renderer = gameObject.AddComponent<MeshRenderer>();

        Filter.mesh = Mesh;
    }
    
    //======================================================================================================================================//
    void Update()
    {
        //if (UpdateMesh)
        //{
            GenerateMesh();
            //UpdateMesh = false;
        //}

        //transform.position = Vector3.Scale(Camera.main.transform.position, Amount) + Offset;   
	}
}

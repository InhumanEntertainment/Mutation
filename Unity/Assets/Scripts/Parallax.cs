using UnityEngine;
using System.Collections;

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
    public Vector3 Multiplier;
    public Vector3 Offset = new Vector3(0, 0, 10);

    public Color _TopColor = Color.white;
    public Color TopColor
    {
        get { return _TopColor; }
        set
        {
            _TopColor = value;
            UpdateMesh = true;
        }
    }

    public Color _BottomColor = Color.grey;
    public Color BottomColor
    {
        get { return _BottomColor; }
        set
        {
            _BottomColor = value;
            UpdateMesh = true;
        }
    }

    //======================================================================================================================================//
    void Awake()
    {
        GenerateMesh();
    }

    //======================================================================================================================================//
    void GenerateMesh()
    {
        print("Generated Mesh");

        // Generate Vertices //
        Vector3[] Vertices = new Vector3[4];
        /*Vertices[0] = new Vector3(0, Height, 0);
        Vertices[1] = new Vector3(0, 0, 0);
        Vertices[2] = new Vector3(Width, Height, 0);
        Vertices[3] = new Vector3(Width, 0, 0);*/

        float halfWidth = Width * 0.5f;
        float halfHeight = Height * 0.5f;

        Vertices[0] = new Vector3(-halfWidth, halfHeight, 0);
        Vertices[1] = new Vector3(-halfWidth, -halfHeight, 0);
        Vertices[2] = new Vector3(halfWidth, halfHeight, 0);
        Vertices[3] = new Vector3(halfWidth, -halfHeight, 0);

        // Gernerate Indexes //
        int[] Triangles = new int[6];
        Triangles[0] = 0;
        Triangles[1] = 1;
        Triangles[2] = 2;
        Triangles[3] = 1;
        Triangles[4] = 2;
        Triangles[5] = 3;

        // Gernerate Uvs //
        Vector2[] Uvs = new Vector2[4];
        Uvs[0] = new Vector2(0, 1);
        Uvs[1] = new Vector2(0, 0);
        Uvs[2] = new Vector2(1, 1);
        Uvs[3] = new Vector2(1, 0);

        // Gernerate Colors //
        Color[] Colors = new Color[4];
        Colors[0] = TopColor;
        Colors[1] = BottomColor;
        Colors[2] = TopColor;
        Colors[3] = BottomColor;      

        // Generate Mesh //
        Mesh = new Mesh();
        Mesh.vertices = Vertices;
        Mesh.uv = Uvs;
        Mesh.triangles = Triangles;
        Mesh.colors = Colors;
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
        if (UpdateMesh)
        {
            GenerateMesh();
            UpdateMesh = false;
        }

        transform.position = Vector3.Scale(Camera.main.transform.position, Multiplier) + Offset;   
	}
}

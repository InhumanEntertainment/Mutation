using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class QuadMesh : MonoBehaviour
{
    public bool Generate = false;
    public float Width = 2;
	public float Height = 2;
	public enum PivotType {TopLeft, TopCenter, TopRight, MiddleLeft, MiddleCenter, MiddleRight, BottonLeft, BottomCenter, BottomRight }
	public PivotType Pivot = PivotType.MiddleCenter;
    public Vector3 Offset = Vector3.zero;
    public Color Color = Color.white;

    public Vector2 UVOffset = Vector2.zero;
    public Vector2 UVScale = Vector2.one;
    // Orientation: XY, XZ, YZ //


    Dictionary<PivotType, Vector3> PivotOffset = new Dictionary<PivotType, Vector3> 
    { 
        {PivotType.TopLeft, new Vector3(-1, 1, 0)}, {PivotType.TopCenter, new Vector3(0, 1, 0)}, {PivotType.TopRight, new Vector3(1, 1, 0)},
        {PivotType.MiddleLeft, new Vector3(-1, 0, 0)}, {PivotType.MiddleCenter, new Vector3(0, 0, 0)}, {PivotType.MiddleRight, new Vector3(1, 0, 0)},
        {PivotType.BottonLeft, new Vector3(-1, -1, 0)}, {PivotType.BottomCenter, new Vector3(0, -1, 0)}, {PivotType.BottomRight, new Vector3(1, -1, 0)}
    };

    //======================================================================================================================================//
    void Awake()
	{
        GenerateMesh();
	}

    //======================================================================================================================================//
    void Update()
    {
        if (Generate)
        {
            Generate = false;
            GenerateMesh();
        }          
    }

    //======================================================================================================================================//
    public void GenerateMesh()
    {
        Vector3[] Vertices = new Vector3[4];
        Vector2[] Uvs = new Vector2[4];
        Color[] Colors = new Color[4];
        int[] Triangles = new int[6];

        // Create Vertices //
        float halfWidth = Width * 0.5f;
        float halfHeight = Height * 0.5f;
        Vector3 totalOffset = Offset - Vector3.Scale(PivotOffset[Pivot], new Vector3(halfWidth, halfHeight, 0));

        Vertices[0] = new Vector3(-halfWidth, -halfHeight, 0) + totalOffset;
        Vertices[1] = new Vector3(-halfWidth, halfHeight, 0) + totalOffset;
        Vertices[2] = new Vector3(halfWidth, -halfHeight, 0) + totalOffset;
        Vertices[3] = new Vector3(halfWidth, halfHeight, 0) + totalOffset;

        // Create Uvs //
        Uvs[0] = new Vector2(0, UVScale.y) + UVOffset;
        Uvs[1] = new Vector2(0, 0) + UVOffset;
        Uvs[2] = new Vector2(UVScale.x, UVScale.y) + UVOffset;
        Uvs[3] = new Vector2(UVScale.x, 0) + UVOffset;

        // Create Colors //
        Colors[0] = Color;
        Colors[1] = Color;
        Colors[2] = Color;
        Colors[3] = Color;

        // Create Triangles //
        Triangles[0] = 0;
        Triangles[1] = 1;
        Triangles[2] = 2;
        Triangles[3] = 1;
        Triangles[4] = 3;
        Triangles[5] = 2;

        // Generate Mesh //
        Mesh mesh = new Mesh();

        mesh.vertices = Vertices;
        mesh.uv = Uvs;
        mesh.colors = Colors;
        mesh.triangles = Triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter == null)
            filter = gameObject.AddComponent<MeshFilter>();

        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer == null)
            renderer = gameObject.AddComponent<MeshRenderer>();

        filter.mesh = mesh;
    }
}
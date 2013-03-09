using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor (typeof(GameCurve))]
class CurveEditor : Editor
{
    //======================================================================================================================================//
    int Selected = 0;
    int SelectedEdge = -1;
    bool vert = false;
    bool edge = false;

    Vector3[] StartCapVertices;
    Vector2[] StartCapUVs;
    int[] StartCapTriangles;

    Vector3[] EndCapVertices;
    Vector2[] EndCapUVs;
    int[] EndCapTriangles;

    Vector3[] BorderVertices;
    Vector2[] BorderUVs;
    int[] BorderTriangles;

    Vector3[] CollisionVertices;
    int[] CollisionTriangles;

    //======================================================================================================================================//
    void SelectClosestPoint(Vector2 position)
    {
        GameCurve Mesh = (GameCurve)target;

        float shortestdistance = 1000000;
        
        //Vertices //
        for (int i = 0; i < Mesh.Vertex.Count; i++)
        {
            Vector3 screenpos = Camera.current.WorldToScreenPoint(Mesh.Vertex[i]);

            float distance = Vector2.Distance(new Vector2(screenpos.x, Camera.current.pixelHeight - screenpos.y), position);

            if (distance < shortestdistance)
            {
                shortestdistance = distance;
                Selected = i;
                SelectedEdge = -1;
                vert = true;
                edge = false;
            }
        }

        // Edges //
        for (int i = 0; i < Mesh.Vertex.Count - 1; i++)
        {
            Vector3 screenpos = Camera.current.WorldToScreenPoint((Mesh.Vertex[i] + Mesh.Vertex[i + 1]) / 2);

            float distance = Vector2.Distance(new Vector2(screenpos.x, Camera.current.pixelHeight - screenpos.y), position);

            if (distance < shortestdistance)
            {
                shortestdistance = distance;
                SelectedEdge = i;
                Selected = -1;
                vert = false;
                edge = true;
            }
        }
        if (shortestdistance > 50)
        {
            Selected = -1;
            SelectedEdge = -1;
            vert = false;
            edge = false;
        }

    }

    //============================================================================================================================================//
    void GenerateMesh()
    {        
        GameCurve Mesh = (GameCurve)target;

        if (Mesh.MeshName == "Default")
        {
            EditorUtility.DisplayDialog("Mesh Generation Problem", "You must first change the generated mesh name", "Ok");
            return;
        }

        // Deleted Generated Meshes //
        //AssetDatabase.MoveAssetToTrash("Assets/Models/Generated/" + Mesh.MeshName + ".asset");
        AssetDatabase.MoveAssetToTrash("Assets/Models/Generated/" + Mesh.MeshName + "_Collision.asset");

        //BuildBorder();
        //BuildStartCap();
        //BuildEndCap();


        // Combined Vertices and uvs //
        /*Vector3[] CombinedVertices = new Vector3[BorderVertices.Length + StartCapVertices.Length + EndCapVertices.Length];
        Vector2[] CombinedUVs = new Vector2[BorderVertices.Length + StartCapVertices.Length + EndCapVertices.Length];
        for (int i = 0; i < BorderVertices.Length; i++)
        {
            CombinedVertices[i] = BorderVertices[i];
            CombinedUVs[i] = BorderUVs[i];
        }
        for (int i = 0; i < StartCapVertices.Length; i++)
        {
            CombinedVertices[BorderVertices.Length + i] = StartCapVertices[i];
            CombinedUVs[BorderVertices.Length + i] = StartCapUVs[i];

        }
        for (int i = 0; i < EndCapVertices.Length; i++)
        {
            CombinedVertices[BorderVertices.Length + StartCapVertices.Length + i] = EndCapVertices[i];
            CombinedUVs[BorderVertices.Length + StartCapVertices.Length + i] = EndCapUVs[i];
        }

        // Offset Triangle Indexes //
        for (int i = 0; i < StartCapTriangles.Length; i++)
        {
            StartCapTriangles[i] += BorderVertices.Length;
        }
        for (int i = 0; i < EndCapTriangles.Length; i++)
        {
            EndCapTriangles[i] += (BorderVertices.Length + StartCapVertices.Length);
        }*/

        // Create the mesh 
        /*Mesh mesh = new Mesh();
        mesh.subMeshCount = 3;
        mesh.vertices = CombinedVertices;
        mesh.uv = CombinedUVs;
        mesh.SetTriangles(BorderTriangles, 0);
        mesh.SetTriangles(StartCapTriangles, 1);
        mesh.SetTriangles(EndCapTriangles, 2);
       
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshFilter filter;
        if (Mesh.GetComponent<MeshFilter>() == null)
        {
            filter = Mesh.gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        }
        else
        {
            filter = Mesh.GetComponent<MeshFilter>();
        }
        filter.mesh = mesh; */ 


        // Build Collision //
        BuildCollision();

        Mesh collisionmesh = new Mesh();
        collisionmesh.vertices = CollisionVertices;
        collisionmesh.triangles = CollisionTriangles;
        collisionmesh.RecalculateBounds();

        if (Mesh.CollisionMesh)
        {
            MeshCollider col = Mesh.GetComponent<MeshCollider>();
            col.sharedMesh = collisionmesh;
        }
        
    
        // Save Files //
        //AssetDatabase.CreateAsset(mesh, "Assets/Models/Generated/" + Mesh.MeshName + ".asset");
        AssetDatabase.CreateAsset(collisionmesh, "Assets/Models/Generated/" + Mesh.MeshName + "_Collision.asset");
        AssetDatabase.SaveAssets();

    }

    //============================================================================================================================================//
    void BuildCollision()
    {
        GameCurve Mesh = (GameCurve)target;

        if (Mesh.Vertex.Count > 1)
        {
            Vector2[] vertices2D = Mesh.Vertex.ToArray();
            CollisionVertices = new Vector3[vertices2D.Length * 2];
            CollisionTriangles = new int[(vertices2D.Length - 1) * 6];

            // Generate Vertices //
            for (int i = 0; i < vertices2D.Length; i++)
            {
                Vector3 vfront = new Vector3(vertices2D[i].x, vertices2D[i].y, -5f);
                Vector3 vback = new Vector3(vertices2D[i].x, vertices2D[i].y, 5f);

                CollisionVertices[i * 2] = vfront;
                CollisionVertices[i * 2 + 1] = vback;
            }

            // Generate Triangles //
            for (int i = 0; i < vertices2D.Length - 1; i++)
            {
                short t1 = (short)(i * 2);
                short t2 = (short)(i * 2 + 1);
                short t3 = (short)(i * 2 + 2);
                short t4 = (short)(i * 2 + 3);

                CollisionTriangles[i * 6] = t1;
                CollisionTriangles[i * 6 + 1] = t2;
                CollisionTriangles[i * 6 + 2] = t3;

                CollisionTriangles[i * 6 + 3] = t3;
                CollisionTriangles[i * 6 + 4] = t2;
                CollisionTriangles[i * 6 + 5] = t4;
            }
        }
    }
    
    //======================================================================================================================================//
    void DrawLine()
    {
        GameCurve Mesh = (GameCurve)target;

        Color drawcolor = Color.white;

        for (int i = 0; i < Mesh.Vertex.Count - 1; i++)
        {
            drawcolor = i == SelectedEdge ? Color.red : Color.white;
            Debug.DrawLine(Mesh.Vertex[i], Mesh.Vertex[i + 1], drawcolor);        
        }
    }

    //======================================================================================================================================//
    void OnSceneGUI()
    {
        GameCurve Mesh = (GameCurve)target;

        serializedObject.Update();

        // Draw Grid //
        float AspectRatio = 1;

        for (float x = -Mesh.GridSize * AspectRatio; x <= Mesh.GridSize * AspectRatio; x += Mesh.GridSpacing)
        {
            Debug.DrawLine(new Vector3(x, -Mesh.GridSize, 100), new Vector3(x, Mesh.GridSize, 100), Color.grey);
        }
        for (float y = -Mesh.GridSize; y <= Mesh.GridSize; y += Mesh.GridSpacing)
        {
            Debug.DrawLine(new Vector3(-Mesh.GridSize * AspectRatio, y, 100), new Vector3(Mesh.GridSize * AspectRatio, y, 100), Color.grey);
        }

        
        if (Mesh.EditMode)
        {
            // User Controll //
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            if (Event.current.type == EventType.layout)
                HandleUtility.AddDefaultControl(controlID);

            // Draw Line //
            for (int i = 0; i < Mesh.Vertex.Count; i++)
            {
                Handles.color = Color.white;
                Handles.SphereCap(controlID, Mesh.Vertex[i], Quaternion.identity, 0.5f);

                if (i < Mesh.Vertex.Count - 1)
                {
                    Handles.color = SelectedEdge == i ? Color.red : Color.grey;                   
                    Handles.SphereCap(controlID, (Mesh.Vertex[i] + Mesh.Vertex[i + 1]) / 2, Quaternion.identity, 0.5f);
                }
            }
            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 1 && Selected > -1)
                {
                    Mesh.Vertex.RemoveAt(Selected);
                    Selected = -1;
                }
                if (Event.current.button == 0 && SelectedEdge > -1 && edge)
                {
                    Mesh.Vertex.Insert(SelectedEdge + 1, Camera.current.ScreenToWorldPoint(new Vector2(Event.current.mousePosition.x, Camera.current.pixelHeight - Event.current.mousePosition.y)));
                    vert = true;
                    edge = false;
                    Selected = SelectedEdge + 1;
                }
                if (Event.current.button == 0 && !vert && !edge)
                {
                    Mesh.Vertex.Add(Camera.current.ScreenToWorldPoint(new Vector2(Event.current.mousePosition.x, Camera.current.pixelHeight - Event.current.mousePosition.y)));
                    vert = true;
                    edge = false;
                    Selected = Mesh.Vertex.Count - 1;
                }
            }

            if (Event.current.type == EventType.MouseMove)
            {
                SelectClosestPoint(Event.current.mousePosition);
            }
            
            if (Event.current.type == EventType.MouseDrag)
            {     
                if (Event.current.button == 0)
                {
                    Vector2 pos = Camera.current.ScreenToWorldPoint(new Vector2(Event.current.mousePosition.x, Camera.current.pixelHeight - Event.current.mousePosition.y));
                    if (Mesh.Snapping)
                    {
                        pos.x = pos.x - (pos.x % Mesh.GridSpacing);
                        pos.y = pos.y - (pos.y % Mesh.GridSpacing);
                    }                   

                    if (Mesh.Vertex[Selected] != pos)
                    {
                        Mesh.Vertex[Selected] = pos;
                        Generate();
                    }
                    
                }
            }

            if (Selected > -1 && vert)
            {
                Handles.color = Color.red;                   
                Handles.SphereCap(controlID, Mesh.Vertex[Selected], Quaternion.identity, 0.5f);
            }

           
        }
        DrawLine();

        //if (GUI.changed)
        EditorUtility.SetDirty(target);
        //SceneView.RepaintAll();
    }

    //======================================================================================================================================//
    void Generate()
    {
        if (((GameCurve)target).AutoGenerate)
        {
            GenerateMesh();
        }
    }

    //======================================================================================================================================//
    // Inspector User Interface //
    //======================================================================================================================================//
    public override void OnInspectorGUI()
    {
        var Mesh = (GameCurve)target;

        if (GUILayout.Button(Mesh.EditMode ? "Stop" : "Edit Shape"))
        {
            Mesh.EditMode = Mesh.EditMode ? false : true;
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Generate Mesh"))
        {
            GenerateMesh();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Generated Mesh Name");
        Mesh.MeshName = EditorGUILayout.TextField(Mesh.MeshName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Auto Generate");
        Mesh.AutoGenerate = EditorGUILayout.Toggle(Mesh.AutoGenerate);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Attach Collision");
        bool bufferbool = EditorGUILayout.Toggle(Mesh.CollisionMesh);
        if (bufferbool != Mesh.CollisionMesh)
        {
            Mesh.CollisionMesh = bufferbool;
            Generate();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Width");
        float buffer = EditorGUILayout.Slider(Mesh.BorderThickness, 0.1f, 10f);
        if (buffer != Mesh.BorderThickness)
        {
            Mesh.BorderThickness = buffer;
            Generate();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Snapping");
        Mesh.Snapping = EditorGUILayout.Toggle(Mesh.Snapping);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Grid Spacing");
        Mesh.GridSpacing = EditorGUILayout.Slider(Mesh.GridSpacing, 0.1f, 2);

        EditorGUILayout.LabelField("Grid Size");
        Mesh.GridSize = EditorGUILayout.IntSlider(Mesh.GridSize, 1, 100);

        //this.DrawDefaultInspector();
        this.Repaint();

        //SceneView.RepaintAll();
    }
}
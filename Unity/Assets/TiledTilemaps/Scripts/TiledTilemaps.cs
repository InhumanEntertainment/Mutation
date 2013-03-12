using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

[ExecuteInEditMode]
public class TiledTilemaps : MonoBehaviour
{	
	public bool reloadAndCommit = false;
	
	private GameObject meshObject;
	private Camera mainCamera;
	
	private MeshFilter meshFilter;
	private MeshCollider meshCollider;
	private MeshRenderer meshRenderer;
	private Mesh mesh;
	
	public float BleedingOffset = 0.0065f;
	
	public float _cameraOrthoSize = 5f;
	public float CameraOrthoSize {
		set {
			_cameraOrthoSize = value;
			reloadAndCommit = true;
			Update();
		}
	}
	
	public float _targetSize = 640f;
	public float TargetSize {
		set {
			_targetSize = value;
			reloadAndCommit = true;
			Update();
		}
	}	
	
	public float _layerZ = -0.1f;
	public float LayerZ {
		set {
			_layerZ = value;
			reloadAndCommit = true;
			Update();
		}
	}
	
	public bool _generateMeshCollider = true;
	public bool GenerateMeshCollider {
		set {
			_generateMeshCollider = value;
			reloadAndCommit = true;
			Update();
		}
	}
	
	private float scale;
	private float scaleX;
	private float scaleY;
	private float anchorX; 
	private float anchorY;
	
	private struct TilelayerData {
		public string name;
		public int[,] gids;
	}
	
	private struct TilesetData {
		public string name;
		public int firstgid;
		public int tilewidth;
		public int tileheight;
		public int spacing;
		public int margin;
		public string imagesource;
		public float imagewidth;
		public float imageheight;		
		public int tilesPerRow;
		public int tilesPerColumn;
		public int amountTiles;
	}
	
	private int tilemapWidth;
	private int tilemapHeight;
	private float tileWidth;
	private float tileHeight;	
	private List<TilesetData> tilesets;
	private List<TilelayerData> layers;
	private Dictionary<int, int> tilesetIdToGid;
	
	public TextAsset _tiledXMLFile = null;		
    public TextAsset tiledXMLFile
    {
        get {
            return _tiledXMLFile;
        }
        set {
            _tiledXMLFile = value;	
            Update();
        }
	}
	
	private int _amountTilesets = 0;
	public Texture[] TilesetsTextures;	
	public Material[] TilesetsMaterials;

    private XmlDocument _xml;


    // Collision //
    List<List<Vector2>> CollisionCurves = new List<List<Vector2>>();

    //============================================================================================================================================//
    bool LoadTiledXML()
    {
        try {
            _xml.LoadXml(_tiledXMLFile.text);
            return true;
        }
        catch (System.Exception err) {
            Debug.LogError("Tiled Importer: An error ocurred when trying to load the Tiled XML file.");
			Debug.LogError(err.Message);
			
        }
		
        return false;
    }

    //============================================================================================================================================//
    void Awake()
	{
		if(meshObject == null)
		{
			meshObject = gameObject;
			_xml = new XmlDocument();			
			mainCamera = Camera.mainCamera;
		}
	}   

    //============================================================================================================================================//
    void ImportTiledXML()
    {		
        if(LoadTiledXML())
		{		
			if(_xml.DocumentElement.Name == "map")
			{
				tilemapWidth = System.Convert.ToInt32(_xml.DocumentElement.Attributes["width"].Value);
				tilemapHeight = System.Convert.ToInt32(_xml.DocumentElement.Attributes["height"].Value);
				tileWidth = System.Convert.ToInt32(_xml.DocumentElement.Attributes["tilewidth"].Value);
				tileHeight = System.Convert.ToInt32(_xml.DocumentElement.Attributes["tileheight"].Value);	
				
				scaleX = tileWidth * scale;
				scaleY = tileHeight * scale;
				
				anchorX = 0; 
				anchorY = tileHeight;
				
				XmlNodeList tilesetsNodes = _xml.DocumentElement.SelectNodes("tileset");
				XmlNodeList layersNodes = _xml.DocumentElement.SelectNodes("layer");


                LoadCollisionXML();






				
				XmlNode subNode = null;
				XmlNode subSubNode = null;
				
				tilesets = new List<TilesetData>();
				layers = new List<TilelayerData>();
				tilesetIdToGid = new Dictionary<int, int>();
				
				for(int tilesetId = 0; tilesetId < tilesetsNodes.Count; ++tilesetId)
				{
					subNode = tilesetsNodes[tilesetId];
					
					TilesetData tileset = new TilesetData();
					tileset.name = subNode.Attributes["name"].Value;
					tileset.firstgid = System.Convert.ToInt32(subNode.Attributes["firstgid"].Value);
					tileset.tilewidth = System.Convert.ToInt32(subNode.Attributes["tilewidth"].Value);
					tileset.tileheight = System.Convert.ToInt32(subNode.Attributes["tileheight"].Value);
					
					if(subNode.Attributes["margin"] != null)
						tileset.margin = System.Convert.ToInt32(subNode.Attributes["margin"].Value);
					else
						tileset.margin = 0;
					
					if(subNode.Attributes["spacing"] != null)
						tileset.spacing = System.Convert.ToInt32(subNode.Attributes["spacing"].Value);
					else
						tileset.spacing = 0;
	
					subSubNode = subNode.SelectSingleNode("image");
					tileset.imagesource = subSubNode.Attributes["source"].Value;
					tileset.imagewidth = (float)System.Convert.ToDouble(subSubNode.Attributes["width"].Value);
					tileset.imageheight = (float)System.Convert.ToDouble(subSubNode.Attributes["height"].Value);				
	
					tileset.tilesPerRow = (int)((tileset.imagewidth - tileset.margin * 2 + tileset.spacing) / 
												(tileset.tilewidth + tileset.spacing)
											   );
					
					tileset.tilesPerColumn = (int)((tileset.imageheight - tileset.margin * 2 + tileset.spacing) / 
												(tileset.tileheight + tileset.spacing)
											   );
					
					tileset.amountTiles = tileset.tilesPerRow * tileset.tilesPerColumn;
						
					tilesets.Add(tileset);
					
					// Map each gid of this tileset with the Tileset ID, so when building the tilemap
					// mesh, we can easily grab the tileset related to the gid.
					for (int gid = tileset.firstgid; gid <= (tileset.firstgid + tileset.amountTiles - 1); gid++) {
						tilesetIdToGid.Add(gid, tilesetId);
					}
				}
				
				for(int layerId = 0; layerId < layersNodes.Count; ++layerId)
				{
					subNode = layersNodes[layerId];
					
					TilelayerData layer = new TilelayerData();
					layer.name = subNode.Attributes["name"].Value;					
					XmlNodeList gidsNodes = subNode.SelectNodes("data/tile");										
					layer.gids = new int[tilemapHeight, tilemapWidth];

                    int index = 0;
                    for (int y = 0; y < tilemapHeight; y++)
                    {
                        for (int x = 0; x < tilemapWidth; x++)
                        {
                            layer.gids[y, x] = System.Convert.ToInt32(gidsNodes[index].Attributes["gid"].Value);
                            index++;
                        }
                    }
					
					layers.Add(layer);
				}
				
				_amountTilesets = tilesets.Count;
				
				if(TilesetsTextures != null && TilesetsTextures.Length != _amountTilesets) {
					for (int i = 0; i < TilesetsMaterials.Length; i++) {
						if(TilesetsMaterials[i] != null) {
							DestroyImmediate(TilesetsMaterials[i]);
						}
					}
					
					TilesetsTextures = null;
					TilesetsMaterials = null;	
				}
				
				if(TilesetsTextures == null) {
					TilesetsTextures = new Texture[_amountTilesets];
					TilesetsMaterials = new Material[TilesetsTextures.Length];
				}					
			}
		}
    }
    
    //============================================================================================================================================//
    void Update()
    {   
		if(reloadAndCommit)
		{		
			if(_xml == null) _xml = new XmlDocument();
			
			if(mainCamera.isOrthoGraphic)
				_cameraOrthoSize = mainCamera.orthographicSize;
			
			scale = 2.0f * _cameraOrthoSize / _targetSize;
			
            if (_tiledXMLFile != null) {
                ImportTiledXML();
            }
		
			// If a new texture is assigned, create and assign a material
			if(tilesets != null && TilesetsTextures.Length == tilesets.Count)
			{	
				bool generate = true;
				
				for (int i = 0; i < TilesetsTextures.Length; i++)
				{
					if(TilesetsTextures[i] == null) 
                    {
						generate = false;
						break;
					}
					
					if(TilesetsMaterials[i] != null) 
                    {
						DestroyImmediate(TilesetsMaterials[i]);
						TilesetsMaterials[i] = null;
					}
					
					TilesetsMaterials[i] = new Material(Shader.Find("Unlit/Transparent Cutout"));
					TilesetsMaterials[i].name = "Tileset " + tilesets[i].name;
					TilesetsMaterials[i].mainTexture = TilesetsTextures[i] as Texture2D;
				}
				
				if(generate) GenerateTilemap();
			}
			
			if (reloadAndCommit) reloadAndCommit = false;
        }		
    }

    //============================================================================================================================================//
    void GenerateTilemap()
	{		
		if(meshObject.GetComponent<MeshFilter>() != null) {
			DestroyImmediate(meshObject.GetComponent<MeshFilter>());
			mesh = null;
		}
		
		meshFilter = meshObject.AddComponent<MeshFilter>();		
		mesh = new Mesh();
		
		if(meshObject.GetComponent<MeshCollider>() == null) {
			if(_generateMeshCollider)
				meshCollider = meshObject.AddComponent<MeshCollider>();
		} else if(!_generateMeshCollider) {
			DestroyImmediate(meshObject.GetComponent<MeshCollider>());
			meshCollider = null;
		} else if (_generateMeshCollider) {
			meshCollider = meshObject.GetComponent<MeshCollider>();
		}
		
		if(meshObject.GetComponent<MeshRenderer>() == null) {
			meshRenderer = meshObject.AddComponent<MeshRenderer>();	
		} else {
			meshRenderer = meshObject.GetComponent<MeshRenderer>();	
		}
		
		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		
		// Tileset id, Triangles List (each tileset = submesh)
		Dictionary<int,List<int>> submeshesTriangles = new Dictionary<int, List<int>>();
		
		float actualZ = 0;
		int triangleFaces = -1;
		
		for (int layerIdx = 0; layerIdx < layers.Count; layerIdx++)
		{	
			actualZ += _layerZ;
			TilelayerData layer = layers[layerIdx];
	
			for(int row = 1; row <= tilemapHeight; ++row)
			{
				for(int col = 0; col < tilemapWidth; ++col)
				{	
					// Get GID
					int gid = layer.gids[row-1,col];			
					if(gid == 0) continue;
					
					// Get tileset for this gid
					int tilesetId = tilesetIdToGid[gid];					
					TilesetData tileset = tilesets[tilesetId];
					int textureTilesPerRow = tileset.tilesPerRow;
					int firstGid = tileset.firstgid;
					float textureWidth = tileset.imagewidth;
					float textureHeight = tileset.imageheight;
					
					if(!submeshesTriangles.ContainsKey(tilesetId)) 
                    {
						submeshesTriangles.Add(tilesetId, new List<int>());
					}
					
					// Tile position
					Vector3 pos0 = new Vector3(col * scaleX, (-(tileHeight * row - anchorY) * scale), 0);		
			        Vector3 pos1 = pos0 + new Vector3(scaleX, scaleY, 0);	
					
					// Vertices
					Vector3 p0 = new Vector3(pos0.x, pos0.y, actualZ);
					Vector3 p1 = new Vector3(pos1.x, pos0.y, actualZ);
					Vector3 p2 = new Vector3(pos0.x, pos1.y, actualZ);
					Vector3 p3 = new Vector3(pos1.x, pos1.y, actualZ);
					
					vertices.Add(p0);
					vertices.Add(p1);
					vertices.Add(p2);
					vertices.Add(p3);
					
					// Triangles
					triangleFaces += 4;
					
					submeshesTriangles[tilesetId].Add(triangleFaces - 3);
					submeshesTriangles[tilesetId].Add(triangleFaces - 1);
					submeshesTriangles[tilesetId].Add(triangleFaces - 2);		
					submeshesTriangles[tilesetId].Add(triangleFaces - 2);
					submeshesTriangles[tilesetId].Add(triangleFaces - 1);
					submeshesTriangles[tilesetId].Add(triangleFaces);
					
					// UV
					int equivalentGidFromFirstRow = gid - firstGid + 1;
					int tilesRow = 1; 
					
					for(; equivalentGidFromFirstRow > (textureTilesPerRow); equivalentGidFromFirstRow -= (textureTilesPerRow)) {
						tilesRow++;
					}
					
					float tileX = ((equivalentGidFromFirstRow - 1) * (tileWidth + tileset.spacing)) + tileset.margin;
					float tileYBottomLeft = (((tileHeight + tileset.spacing) * tilesRow) - tileset.spacing) + tileset.margin;
			
					float pixelMinX = textureWidth;
					float pixelMinY = textureHeight;
					
					if(tileX != 0f) 			pixelMinX = tileX / pixelMinX;
					if(tileYBottomLeft != 0f)	pixelMinY = tileYBottomLeft / pixelMinY;
					
					Vector2 pixelMin = new Vector2(pixelMinX, 1.0f - pixelMinY);
					Vector2 pixelDims = new Vector2((tileWidth / textureWidth), (tileHeight / textureHeight));
					
					Vector2 min = pixelMin;
					
					// Amount of UV's have to match the amount of verts
					// And their winding.
					// Fix gaps with eps: http://forum.unity3d.com/threads/41187-Web-demo-of-my-2D-platformer?p=263666&viewfull=1#post263666				
					float eps = BleedingOffset;
					uvs.Add(min + new Vector2(pixelDims.x * 0.0f + eps, pixelDims.y * 0.0f + eps));
					uvs.Add(min + new Vector2(pixelDims.x * 1.0f - eps, pixelDims.y * 0.0f + eps));
					uvs.Add(min + new Vector2(pixelDims.x * 0.0f + eps, pixelDims.y * 1.0f - eps));
					uvs.Add(min + new Vector2(pixelDims.x * 1.0f - eps, pixelDims.y * 1.0f - eps));	
				}
			}
		
		} // end iterate layers	
		
		mesh.vertices = vertices.ToArray();
		mesh.uv = uvs.ToArray();
		
		// Only one tileset, no need for more than one submesh
		if(_amountTilesets == 1)
		{
			mesh.triangles = submeshesTriangles[0].ToArray();
		}
		// A submesh to each tileset
		else
		{
			mesh.subMeshCount = _amountTilesets;
			
			for (int tilesetId = 0; tilesetId < _amountTilesets; tilesetId++)
			{
				if(submeshesTriangles.ContainsKey(tilesetId)) {
					mesh.SetTriangles(submeshesTriangles[tilesetId].ToArray(), tilesetId);
				} else {
					mesh.SetTriangles(new int[0], tilesetId);
				}
			}
		}
		
		meshFilter.mesh = mesh;			
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
		
		if(_generateMeshCollider)
		{
            BuildCollision();

           
		}
		
		meshRenderer.materials = TilesetsMaterials;
	}

    //============================================================================================================================================//
    void LoadCollisionXML()
    {
        CollisionCurves.Clear();

        // Collision Loading //
        XmlNodeList ObjectGroups = _xml.DocumentElement.SelectNodes("objectgroup");
        for (int groupIndex = 0; groupIndex < ObjectGroups.Count; ++groupIndex)
        {
            XmlNodeList Objects = ObjectGroups[groupIndex].SelectNodes("object");
            for (int objectIndex = 0; objectIndex < Objects.Count; ++objectIndex)
            {
                List<Vector2> curvePoints = new List<Vector2>();

                int x = System.Convert.ToInt32(Objects[objectIndex].Attributes["x"].Value);
                int y = System.Convert.ToInt32(Objects[objectIndex].Attributes["y"].Value);

                // Rectangle + Ellipse //
                if (Objects[objectIndex].Attributes.Count > 2)
                {
                    int w = System.Convert.ToInt32(Objects[objectIndex].Attributes["width"].Value);
                    int h = System.Convert.ToInt32(Objects[objectIndex].Attributes["height"].Value);

                    if (Objects[objectIndex].ChildNodes.Count > 0)
                    {
                        for (int i = 0; i < 370; i += 10)
                        {
                            float xx = Mathf.Cos(i * Mathf.Deg2Rad) * w * 0.5f + (w * 0.5f) + x;
                            float yy = Mathf.Sin(i * Mathf.Deg2Rad) * h * 0.5f + (h * 0.5f) + y;
                            curvePoints.Add(new Vector2(xx, yy * -1) * scale);                       
                        }
                    }
                    else
                    {
                        curvePoints.Add(new Vector2(x, -y) * scale);
                        curvePoints.Add(new Vector2(x + w, -y) * scale);
                        curvePoints.Add(new Vector2(x + w, -y - h) * scale);
                        curvePoints.Add(new Vector2(x, -y - h) * scale);
                        curvePoints.Add(new Vector2(x, -y) * scale);
                    }
                }
                // Polygon + Polyline //
                else
                {
                    XmlNode node = Objects[objectIndex].FirstChild;
                    string text = node.Attributes["points"].Value;
                    string[] points = text.Split(new string[] { " " }, StringSplitOptions.None);
                    Vector3[] Line = new Vector3[points.Length];

                    foreach (string point in points)
                    {
                        string[] xy = point.Split(',');
                        Vector2 v = new Vector2(x + float.Parse(xy[0]), -1f * (y + float.Parse(xy[1]))) * scale;
                        curvePoints.Add(v);
                    }

                    if (node.Name == "polygon")
                    {
                        string[] xy = points[0].Split(',');
                        Vector2 v = new Vector2(x + float.Parse(xy[0]), -1f * (y + float.Parse(xy[1]))) * scale;
                        curvePoints.Add(v);
                    }
                }

                CollisionCurves.Add(curvePoints);
            }
        }
    }

    //============================================================================================================================================//
    void BuildCollision()
    {
		// Destroy any collision mesh that might already have been created.
        int childs = transform.childCount;
        for (int i = childs - 1; i > 0; i--)
        {
			DestroyImmediate(transform.GetChild(i).gameObject);
            //Destroy(transform.GetChild(i).gameObject);
        }
		
		System.Diagnostics.Debug.Assert(transform.childCount == 0, "Not all colliders were cleaned up before generating new level colliders.");
		
		for (int curveIndex = 0; curveIndex < CollisionCurves.Count; curveIndex++)
		{
			int[] tris = new int[6] { 0, 1, 2, 2, 1, 3 };
			
			List<Vector2> points = CollisionCurves[curveIndex];
			
			for (int i = 0; i < points.Count - 1; i++)
			{
				List<Vector3> verts = new List<Vector3>();
			
				Vector3 start = new Vector3(points[i].x, points[i].y);
				
				float yOffset = scale * tileHeight;
				
				int index = i;
                verts.Add(new Vector3(points[index].x, points[index].y + yOffset, -5f) - start);
                verts.Add(new Vector3(points[index].x, points[index].y + yOffset, 5f) - start);
				
				float startY = points[index].y + yOffset;
				
				index++;
				if(index >= points.Count)
				{
					index = 0;
				}
				
                verts.Add(new Vector3(points[index].x, points[index].y + yOffset, -5f) - start);
                verts.Add(new Vector3(points[index].x, points[index].y + yOffset, 5f) - start);
				
				float endY = points[index].y + yOffset;
				
				Mesh collisionmesh = new Mesh();
		        collisionmesh.vertices = verts.ToArray();
		        collisionmesh.triangles = tris;
		        collisionmesh.RecalculateBounds();
				collisionmesh.RecalculateNormals();
				
				GameObject newObj = new GameObject();
				
				newObj.transform.position = start;
				
				newObj.AddComponent<MeshCollider>();
				newObj.GetComponent<MeshCollider>().sharedMesh = collisionmesh;
				
				if(startY != endY)
				{
					newObj.GetComponent<MeshCollider>().material = (PhysicMaterial)Resources.Load("Materials/Wall");
					newObj.name = "Wall";
				}
				else
				{
					newObj.GetComponent<MeshCollider>().material = (PhysicMaterial)Resources.Load("Materials/Floor");
					newObj.name = "Floor";
				}
				
				newObj.transform.parent = this.transform;
			}
		}
		
		/*
        List<Vector3> AllVertices = new List<Vector3>();
        List<int> AllTriangles = new List<int>();

        for (int curveIndex = 0; curveIndex < CollisionCurves.Count; curveIndex++)
        {
            List<Vector2> points = CollisionCurves[curveIndex];
            int offset = AllVertices.Count;

            // Generate Vertices //
            for (int i = 0; i < points.Count; i++)
            {
                float yOffset = scale * tileHeight;
                Vector3 vfront = new Vector3(points[i].x, points[i].y + yOffset, -5f);
                Vector3 vback = new Vector3(points[i].x, points[i].y + yOffset, 5f);

                AllVertices.Add(vfront);
                AllVertices.Add(vback);
            }

            // Generate Triangles //
            for (int i = 0; i < points.Count - 1; i++)
            {
                short t1 = (short)(i * 2);
                short t2 = (short)(i * 2 + 1);
                short t3 = (short)(i * 2 + 2);
                short t4 = (short)(i * 2 + 3);

                AllTriangles.Add(t1 + offset);
                AllTriangles.Add(t2 + offset);
                AllTriangles.Add(t3 + offset);

                AllTriangles.Add(t3 + offset);
                AllTriangles.Add(t2 + offset);
                AllTriangles.Add(t4 + offset);
            }
        }

        Vector3[] CollisionVertices = AllVertices.ToArray();
        int[] CollisionTriangles = AllTriangles.ToArray();

        Mesh collisionmesh = new Mesh();
        collisionmesh.vertices = CollisionVertices;
        collisionmesh.triangles = CollisionTriangles;
        collisionmesh.RecalculateBounds();

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = collisionmesh;
		
		meshCollider.material = (PhysicMaterial)Resources.Load("Materials/Floor");
		*/
    }
}
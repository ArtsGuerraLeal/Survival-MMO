using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class World : MonoBehaviour {

	public static World instance;

	public Material material;

 

    public int width;
	public int height;

    private MeshTile[] tiles;
    private MeshTile[,] tiles2d;
    public float noiseScale;
    public float moistScale;
    public float tempScale;

    public int octaves;
    public int moistOctaves;
    public int tempOctaves;

    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    [Range(0, 1)]
    public float moistPersistance;
    public float moistLacunarity;
    [Range(0, 1)]
    public float tempPersistance;
    public float tempLacunarity;

    public int seed;
    public int moistSeed;
    public int tempSeed;

    public Vector2 offset;
    public string streamPath;
    private MeshTileCollection tileCollection;
    private MeshTileCollection[,] tileCollectionArray;
    public bool autoUpdate;
    public MeshTerrainRegion[] regions;
    public BiomeType[] forests;

    public float[,] heightNoiseValues;
    public float[,] tempNoiseValues;
    public float[,] moistNoiseValues;
    public int chunkSize= 100;

 


    // Use this for initialization
    void Awake () {
        instance = this;
      //    tiles2d = new MeshTile[height, width];
        heightNoiseValues = Noise.GenerateNoiseMap(width, height, seed, noiseScale, octaves, persistance, lacunarity, offset);
        moistNoiseValues = Noise.GenerateNoiseMap(width, height, moistSeed, moistScale, moistOctaves, moistPersistance, moistLacunarity, offset);
        tempNoiseValues = Noise.GenerateNoiseMap(width, height, tempSeed, tempScale, tempOctaves, tempPersistance, tempLacunarity, offset);

        //  tileCollectionArray = new MeshTileCollection[3, 3];
        //   CreateTiles(heightNoiseValues);


        if (!File.Exists(streamPath))
        {
         //   heightNoiseValues = Noise.GenerateNoiseMap(width, height, seed, noiseScale, octaves, persistance, lacunarity, offset);

          //  CreateTiles(heightNoiseValues);
        }
        if (File.Exists(streamPath))
        {
          //  SpawnTiles();
           // WorldTrees.instance.SpawnTrees();
        }


        }

    public void SpawnTiles()
    {

        using (StreamReader stream = new StreamReader(streamPath))
        {
            string json = stream.ReadToEnd();

            tileCollection = JsonUtility.FromJson<MeshTileCollection>(json);
            
        }
    }

    public void SpawnTilesFromChunk(int x, int y,int chunkX, int chunkY)
    {

        using (StreamReader stream = new StreamReader(streamPath + x.ToString() + "_" + y.ToString() + ".json"))
        {
            string json = stream.ReadToEnd();

            tileCollectionArray[chunkX, chunkY] = JsonUtility.FromJson<MeshTileCollection>(json);

        }
    }

    void Start () {
        
        if (!File.Exists(streamPath + "0_0.json"))
        {

            // SpawnTiles();

            //for (int x = 0; x < width; x++)
            //{
            //    for (int y = 0; y < height; y++)
            //    {
            //        tiles2d[x, y] = GetTile(x, y);
            //    }
            //}

            //SubdivideTilesArray2(0, 0, 0, "Water");
            //SubdivideTilesArray2(0, 0, 1, "Sand");
            //SubdivideTilesArray2(0, 0, 2, "Grass");
            //SubdivideTilesArray2(0, 0, 3, "Rock");
            //SubdivideTilesArray2(0, 0, 4, "Snow");

        }
       // StartCoroutine(CreateTerrain());
        FastTerrain();


    }


    // Update is called once per frame
    void Update () {

    }


    void FastTerrain() {
        for (int x = 0; x < width; x = x + chunkSize)
        {
            for (int y = 0; y < height; y = y + chunkSize)
            {

                GenerateChunkedTiles(x, y, heightNoiseValues);
               

            }

        }
    }
    IEnumerator CreateTerrain()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);

        for (int x = 0; x < width; x = x + chunkSize)
        {
            for (int y = 0; y < height; y = y + chunkSize)
            {

                GenerateChunkedTiles(x, y, heightNoiseValues);
                yield return wait;

            }

        }

    }

    public void GenerateChunkedTiles(int startX, int startY, float[,] heightNoiseArray)
    {
        int count = 0;

        tiles = new MeshTile[chunkSize * chunkSize];

        for (int x = startX; x < startX + chunkSize; x++)
        {
            for (int y = startY; y < startY + chunkSize; y++)
            {
                float currentHeight = heightNoiseValues[x, y];
                float currentMoist = moistNoiseValues[x, y];
                float currentTemp = tempNoiseValues[x, y];

                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        //If region is grass spawn trees, rocks, and bushes

                        if (i == 3)
                        {
                            int biomeID = 4;

                            for (int f = 0; f < forests.Length; f++)
                            {

                                if (currentMoist >= forests[f].minMoist && currentMoist <= forests[f].maxMoist)
                                {

                                    if (currentTemp >= forests[f].minTemp && currentTemp <= forests[f].maxTemp)
                                    {
                                        biomeID = f;

                                    }

                                }
                            }

                            float r = Random.Range(0, 4000);
                            if (r <= 150)
                            {
                                tiles[count] = new MeshTile(i, x, y, 1,100,10, Mathf.RoundToInt(Random.Range(0, 4)), biomeID);
                            }
                            else if (r >= 750 && r < 900)
                            {
                                tiles[count] = new MeshTile(i, x, y, 2, 100, 10, Mathf.RoundToInt(Random.Range(0, 5)), biomeID);
                            }
                            else if (r >= 400 && r <= 500)
                            {
                                tiles[count] = new MeshTile(i, x, y, 3, 100, 10, Mathf.RoundToInt(Random.Range(0, 2)), biomeID);
                            }
                            else {
                                tiles[count] = new MeshTile(i, x, y, 0,0,0,0,biomeID);
                            }
                        }
                        else {
                            tiles[count] = new MeshTile(i, x, y, 0);
                        }
                        
                        count++;
                        break;
                    }
                }   
            }

        }

        tileCollection = new MeshTileCollection(tiles, "worldTiles");

        using (StreamWriter stream = new StreamWriter(streamPath + startX.ToString() + "_" + startY.ToString() + ".json"))
        {
            string json = JsonUtility.ToJson(tileCollection, false);
            stream.Write(json);
         //   Debug.Log(streamPath + startX.ToString() + "_" + startY.ToString() + ".json");

        }

    }

    public void DeleteStuff()
    {
        foreach (Transform child in this.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

 

	void CreateTiles (float [,] heightNoiseArray) {

        int count = 0;
        tiles = new MeshTile[width * height];


        for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {

                tiles[count] = LoadTile(i,j,heightNoiseArray[i, j]);
                count++;
               
            }
		}
        //y* width +x
        //Fix Tiles
        tileCollection = new MeshTileCollection(tiles, "MeshTiles");

        for (int x = 0; x < width-1; x++)
        {
            for (int y = 0; y < height-1; y++)
            {
                MeshTile[] neighbors = GetNeighbors(x, y,true);
                MeshTile tile = GetTile(x, y);
                int sameCount = 0;
                int diffCount = 0;
                int type1 = 0;
                int type2 = 0;
                foreach (MeshTile n in neighbors) {
                    if (n.t == tile.t)
                    {
                        sameCount++;
                    }
                    if(n.t != tile.t && n.t != MeshTile.Type.Empty) {
                        diffCount++;
                    }
                }

                if (diffCount > sameCount)
                {
                    int rand = (int)Random.Range(0, 7);

                    while (tile.t != GetNeighbors(x, y, true)[rand].t) {

                        if (GetNeighbors(x, y, true)[rand].t != MeshTile.Type.Empty)
                        {
                            tileCollection.tiles[y * width + x].t = GetNeighbors(x, y, true)[rand].t;
                            rand = (int)Random.Range(0, 7);
                        }
                        else {
                            rand = (int)Random.Range(0, 7);

                        }
                    }
                    
                    
                }

                


            }
        }

        using (StreamWriter stream = new StreamWriter(streamPath))
        {
            string json = JsonUtility.ToJson(tileCollection, true);
            stream.Write(json);

        }

    }

    MeshTile LoadTile(int x, int y, float currentHeight) {


        for (int i = 0; i < regions.Length; i++)
        {
            if (currentHeight <= regions[i].height)
            {
                return new MeshTile(i, y, x, 0);
            }
            
        }
        return new MeshTile(5, y, x, 0);


    }


    void SubdivideTilesArray(int i1 = 0, int i2 = 0, int type = 5, string layer="Height") {

		if (i1 > tiles2d.GetLength (0) && i2 > tiles2d.GetLength (1))
			return;

		//Get size of segment
		int sizeX, sizeY;

		if (tiles2d.GetLength (0) - i1 > 100) {

			sizeX = 100;
		} else
			sizeX = tiles2d.GetLength (0) - i1;

		if (tiles2d.GetLength (1) - i2 > 100) {

			sizeY = 100;
		} else
			sizeY = tiles2d.GetLength (1) - i2;

		GenerateTilesLayer (i1, i2, sizeX, sizeY, type,layer);

		if (tiles2d.GetLength (0) > i1 + 100) {
			SubdivideTilesArray (i1 + 100, i2, type,layer);
			return;
		}

		if (tiles2d.GetLength (1) > i2 + 100) {
			SubdivideTilesArray (0, i2 + 100, type,layer);
			return;
		}
	}

    void SubdivideTilesArray2(int i1 = 0, int i2 = 0, int type = 5, string layer = "Height")
    {
        
        if (i1 > width && i2 > height)
            return;

        //Get size of segment
        int sizeX, sizeY;

        if (width - i1 > 100)
        {

            sizeX = 100;
        }
        else
            sizeX = width - i1;

        if (height - i2 > 100)
        {

            sizeY = 100;
        }
        else
            sizeY = height - i2;

        GenerateTilesLayer(i1, i2, sizeX, sizeY, type, layer);

        if (width > i1 + 100)
        {
            SubdivideTilesArray2(i1 + 100, i2, type, layer);
            return;
        }

        if (height > i2 + 100)
        {
            SubdivideTilesArray2(0, i2 + 100, type, layer);
            return;
        }
    }

    public void SubdivideUpdatedTilesArray(int i1 = 0, int i2 = 0, string layer = "Height")
    {

        if (i1 > tiles.GetLength(0) && i2 > tiles.GetLength(1))
            return;

        //Get size of segment
        int sizeX, sizeY;

        if (tiles.GetLength(0) - i1 > 100)
        {

            sizeX = 100;
        }
        else
            sizeX = tiles.GetLength(0) - i1;

        if (tiles.GetLength(1) - i2 > 100)
        {

            sizeY = 100;
        }
        else
            sizeY = tiles.GetLength(1) - i2;

       // UpdateTiles(i1, i2, sizeX, sizeY,layer);

        if (tiles.GetLength(0) > i1 + 100)
        {
            SubdivideUpdatedTilesArray(i1 + 100, i2, layer);
            return;
        }

        if (tiles.GetLength(1) > i2 + 100)
        {
            SubdivideUpdatedTilesArray(0, i2 + 100, layer);
            return;
        }
    }

    public void GenerateTilesLayer (int x, int y, int width, int height, int type, string layer) {

		MeshData data = new MeshData (x, y, width, height, type);

		GameObject meshGO = new GameObject (layer + " "+ x + "_" + y);


        meshGO.transform.SetParent (this.transform);

		MeshFilter filter = meshGO.AddComponent<MeshFilter> ();
		MeshRenderer render = meshGO.AddComponent<MeshRenderer> ();
        MeshCollider collider = meshGO.AddComponent<MeshCollider>();

        render.material = material;

		Mesh mesh = filter.mesh;

        collider.sharedMesh = filter.mesh;

        mesh.vertices = data.vertices.ToArray (); 
        mesh.triangles = data.triangles.ToArray ();
		mesh.uv = data.UVs.ToArray ();


        if (x >= 500 || y >= 500) {
            meshGO.SetActive(false);
        }

    }

    public GameObject StartGenerator(int x, int y, int width, int height, int type, string layer,int z = 0)
    {
        Transform chunkTransform = this.transform;
        GameObject chunkGO = this.gameObject;
        if (!this.transform.Find("Chunk " + x + "_" + y))
        {
            chunkGO = new GameObject("Chunk " + x + "_" + y);
            chunkGO.transform.SetParent(this.transform);
            chunkTransform = chunkGO.transform;

        }
        else {
            
            chunkTransform = this.transform.Find("Chunk " + x + "_" + y);
            chunkGO = chunkTransform.gameObject;
        }

        MeshData data = new MeshData(x, y, width, height, type);

        GameObject meshGO = new GameObject(layer + " " + x + "_" + y);
        meshGO.transform.SetParent(chunkTransform);
        meshGO.transform.position = new Vector3(0, 0, z);


        MeshFilter filter = meshGO.AddComponent<MeshFilter>();
        MeshRenderer render = meshGO.AddComponent<MeshRenderer>();
        MeshCollider collider = meshGO.AddComponent<MeshCollider>();

        render.material = material;

        Mesh mesh = filter.mesh;

        collider.sharedMesh = filter.mesh;

        mesh.vertices = data.vertices.ToArray();
        mesh.triangles = data.triangles.ToArray();
        mesh.uv = data.UVs.ToArray();

        return chunkGO;

    }

    void SubdivideMountainArray (int i1 = 0, int i2 = 0) {

		if (i1 > tiles.GetLength (0) && i2 > tiles.GetLength (1))
			return;

		//Get size of segment
		int sizeX, sizeY;

		if (tiles.GetLength (0) - i1 > 25) {

			sizeX = 25;
		} else
			sizeX = tiles.GetLength (0) - i1;

		if (tiles.GetLength (1) - i2 > 25) {

			sizeY = 25;
		} else
			sizeY = tiles.GetLength (1) - i2;

		GenerateMountainLayer (i1, i2, sizeX, sizeY);

		if (tiles.GetLength (0) > i1 + 25) {
			SubdivideMountainArray (i1 + 25, i2);
			return;
		}

		if (tiles.GetLength (1) > i2 + 25) {
			SubdivideMountainArray (0, i2 + 25);
			return;
		}
	}

    public void RandomizeTerrain()
    {
        MeshTile[] neighbors;
        
        for (int i = 0; i < width-1; i++)
        {
            for (int j = 0; j < height-1; j++)
            {
                if (i - 1 > 0 && j - 1 > 0 )
                {
                    if (i < tiles.GetLength(0)-1 && j < tiles.GetLength(0)-1) {

                        int r = Random.Range(0, 8);

                        neighbors = GetNeighbors(i, j, true);
                        
                        tiles[i* j].t = neighbors[r].t;
                    }
                }
              //  foreach(Tile d in neighbors)
              // {
              //    if(d.type == tiles[i, j].type)
              //    {

                //      }
                //  }




            }
        }

    }

    public void UpdateTiles(int x, int y, string layer)
    {

        RaycastHit hit;
        Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray1, out hit, Mathf.Infinity))
        {
            MeshData data = new MeshData(x, y, 25, 25,1);

            GameObject go = hit.collider.gameObject;
        
            MeshFilter filter = go.GetComponent<MeshFilter>();

            Mesh mesh = filter.mesh;
            mesh.colors = filter.mesh.colors;
            
            mesh.vertices = data.vertices.ToArray();
            mesh.triangles = data.triangles.ToArray();
            mesh.uv = data.UVs.ToArray();
        }
            

        }

    void GenerateMountainLayer (int x, int y, int width, int height) {

		MeshData data = new MeshData (x, y, width, height, 1,true);

		GameObject meshGO = new GameObject ("MountainLayer_" + x + "_" + y);
		meshGO.transform.SetParent (this.transform);

		MeshFilter filter = meshGO.AddComponent<MeshFilter> ();
		MeshRenderer render = meshGO.AddComponent<MeshRenderer> ();
		render.material = material;

		Mesh mesh = filter.mesh;

		mesh.vertices = data.vertices.ToArray ();
		mesh.triangles = data.triangles.ToArray ();
		mesh.uv = data.UVs.ToArray ();
	}


	public MeshTile GetTileAts (int x, int y) {

		if (x < 0 || x >= width || y < 0 || y >= height) {

			return null;
		}

		return tiles [x * y];
	}

    public MeshTile GetTile(int x, int y)
    {
        //if (x > width)
        //{
        //    x--;
        //}
        //if (y > height)
        //{
        //    y--;
        //}

        //if (x < width)
        //{
        //    x++;
        //}
        //if (y < height)
        //{
        //    y++;
        //}

        if (x > width || x < 0) {
            return new MeshTile(5, 0, 0, 0);
        }

        if (y > height || y < 0)
        {
            return new MeshTile(5, 0, 0, 0);
        }

        return tileCollection.tiles[y * width + x];
    }

    public MeshTile[] GetNeighbors (int x, int y, bool diagonals = false) {

		MeshTile[] neighbors = new MeshTile[ diagonals ? 8 : 4];

        // N E S W

        //if (x > width)
        //{
        //    x--;
        //}
        //if (y > height)
        //{
        //    y--;
        //}

        //if (x < width)
        //{
        //    x++;
        //}
        //if (y < height)
        //{
        //    y++;
        //}


        neighbors [0] = GetTile (x + 0, y + 1);
		neighbors [1] = GetTile (x + 1, y + 0);
		neighbors [2] = GetTile (x + 0, y - 1);
		neighbors [3] = GetTile (x - 1, y + 0);

		//NE SE SW NW
		if (diagonals == true) {

			neighbors [4] = GetTile (x + 1, y + 1);
			neighbors [5] = GetTile (x + 1, y - 1);
			neighbors [6] = GetTile (x - 1, y - 1);
			neighbors [7] = GetTile (x - 1, y + 1);
		}

		return neighbors;
	}

}

[System.Serializable]
public struct MeshTerrainRegion
{
    public string name;
    public float height;

}

[System.Serializable]
public struct BiomeType
{
    public string name;
    public float minMoist;
    public float maxMoist;
    public float minTemp;
    public float maxTemp;

    public Color colour;
}
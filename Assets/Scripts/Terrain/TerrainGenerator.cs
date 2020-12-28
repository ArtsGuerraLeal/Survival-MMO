using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using System;
using System.IO;

public class TerrainGenerator : MonoBehaviour
{
    public static TerrainGenerator instance;
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public WorldTile[] tiles;
    public WorldTileCollection tileCollection;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;
    public TerrainRegion[] regions;

    public Tilemap tilemap;

    public float[,] heightNoiseValues;
  
    public float[,] moistureNoiseValues;
    
    public float[,] temperatureNoiseValues;

    public string streamPath;
    public int renderDistance;

    public int chunkSize;

    private Vector2 oldCurrentPosition;
    private Vector2 newCurrentPosition;
    [SerializeField]
    private int playerXFloor;
    [SerializeField]
    private int playerXFloorOld;
    [SerializeField]
    private int playerYFloor;
    [SerializeField]
    private int playerYFloorOld;

    void Awake()
    {
        instance = this;

        //if (!File.Exists(streamPath))
        //{
        //    heightNoiseValues = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        //    moistureNoiseValues = Noise.GenerateNoiseMap(mapWidth, mapHeight, (seed * 2) / 4, mapHeight / 10, 1, 1, 1, offset);
        //    temperatureNoiseValues = Noise.GenerateNoiseMap(mapWidth, mapHeight, (seed * 8) / 4, noiseScale, octaves, persistance, lacunarity, offset);

        //    GenerateTiles(heightNoiseValues, heightNoiseValues, heightNoiseValues);
        //}
        //else
        //{
        //    using (StreamReader stream = new StreamReader(streamPath))
        //    {
        //        string json = stream.ReadToEnd();

        //        tileCollection = JsonUtility.FromJson<WorldTileCollection>(json);

        //    }
        //}

    //    heightNoiseValues = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        
     //   StartCoroutine(CreateTerrain());

    }

    IEnumerator CreateTerrain()
    {
        WaitForSeconds wait = new WaitForSeconds(.01f);

        for (int x = 0; x < mapWidth; x = x + 100)
        {
            for (int y = 0; y < mapHeight; y = y + 100)
            {

                GenerateChunkedTiles(x, y, heightNoiseValues);
                yield return wait;

            }

        }

    }

    private void Start()
    {
        // playerXFloor = (int)Mathf.Round(oldCurrentPosition.x / 100) * 100;
        //  playerYFloor = (int)Mathf.Round(oldCurrentPosition.y / 100) * 100;
     //    SpawnTiles(renderDistance);

      

    }

    public void StartChunkGenerator(int x, int y, int size) {

        using (StreamReader stream = new StreamReader(streamPath + x.ToString() + "_" + y.ToString() + ".json"))
        {
            string json = stream.ReadToEnd();

            tileCollection = JsonUtility.FromJson<WorldTileCollection>(json);
            
        }


        for (int i = 0; i < tileCollection.tiles.Length; i++)
        {
            WorldTile wt = tileCollection.tiles[i];
            for (int type = 0; type < TerrainGenerator.instance.regions.Length; type++)
            {
                if ((int)wt.type == type)
                {
                    regions[type].grid.SetTile(new Vector3Int(wt.x, wt.y, (int)wt.type), regions[type].tile);
                }
            }
        }

       


    }

    public void StartGenerator(int x, int y, int type, int size) {
        for (int i = x; i < size + x; i++) {
            for (int j = y; j < size + y; j++)
            {
                WorldTile wt = GetTile(i, j);
                WorldTile[] neighbors = wt.GetNeighbors();
                int count = 0;
                //                Debug.Log("X: " + wt.x + "Y: " + wt.y + " Type:" + (int)wt.type);

                foreach (WorldTile w in neighbors) {
                    if ((int)w.type == (int)wt.type) {
                        count++;
                    }
                }
                if ((int)wt.type == type)
                {
                    if (count > 1)
                    {
                        regions[type].grid.SetTile(new Vector3Int(i, j, type * -1), regions[type].tile);
                    
                    if (j - 1 > 0)
                    {
                        regions[type].grid.SetTile(new Vector3Int(i, j - 1, type * -1), regions[type].tile);
                    }
                    if (j + 1 < mapHeight)
                    {
                        regions[type].grid.SetTile(new Vector3Int(i, j + 1, type * -1), regions[type].tile);
                    }

                    if (i - 1 > 0)
                    {
                        regions[type].grid.SetTile(new Vector3Int(i - 1, j, type * -1), regions[type].tile);
                    }

                    if (i + 1 < mapHeight)
                    {
                        regions[type].grid.SetTile(new Vector3Int(i + 1, j, type * -1), regions[type].tile);
                    }
                    }


                }





                //if ((int)wt.type == type && type > 0)
                //{
                //    regions[type].grid.SetTile(new Vector3Int(i, j, 0), regions[type].tile);
                //    regions[type - 1].grid.SetTile(new Vector3Int(i, j, 0), regions[type].tile);

                //}
                //else if ((int)wt.type == type)
                //{
                //    regions[type].grid.SetTile(new Vector3Int(i, j, 0), regions[type].tile);

                //}

            }
        }
    }

    public void GenerateChunkedTiles(int startX, int startY, float[,] heightNoiseArray)
    {
        int count = 0;
        int chunkXSize = 100;
        int chunkYSize = 100;

        tiles = new WorldTile[chunkXSize * chunkYSize];

        for (int x = startX; x < startX + chunkXSize; x++)
        {
            for (int y = startY; y < startY + chunkYSize; y++)
            {
                float currentHeight = heightNoiseValues[x, y];

                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        tiles[count] = new WorldTile(i, x, y);
                        count++;

                        break;
                    }
                }
            }
        }

        tileCollection = new WorldTileCollection(tiles, "worldTiles");

        using (StreamWriter stream = new StreamWriter(streamPath + startX.ToString()+"_"+ startY.ToString() + ".json"))
        {
            string json = JsonUtility.ToJson(tileCollection, true);
            stream.Write(json);

        }

    }

    public void GenerateTiles(float[,] heightNoiseArray, float[,] moistureNoiseArray, float[,] tempNoiseArray)
    {
        int count = 0;
        int chunkXSize = chunkSize;
        int chunkYSize = chunkSize;

        tiles = new WorldTile[mapWidth*mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = heightNoiseValues[x, y];

                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        tiles[count] = new WorldTile(i,x,y);
                        count++;    

                        break;
                    }
                }
            }
        }

        tileCollection = new WorldTileCollection(tiles, "worldTiles");

        using (StreamWriter stream = new StreamWriter(streamPath+".json"))
        {
            string json = JsonUtility.ToJson(tileCollection, true);
            stream.Write(json);

        }

    }

    public WorldTile GetTile(int x, int y)
    {
     

        if (x > mapWidth || x < 0)
        {
            return new WorldTile(5, 0, 0);
        }

        if (y > mapHeight || y < 0)
        {
            return new WorldTile(5, 0, 0);
        }

        return tileCollection.tiles[y * 100 + x];
    }

    public WorldTile GetChunkTile(int x, int y, int c)
    {


        if (x > c || x < 0)
        {
            return new WorldTile(5, 0, 0);
        }

        if (y > c || y < 0)
        {
            return new WorldTile(5, 0, 0);
        }

        

        return tileCollection.tiles[Math.Abs(y - c) * 100 + x];
    }

    public void SpawnTiles(int renderDistance)
    {

        //using (StreamReader stream = new StreamReader(streamPath))
        //{
        //    string json = stream.ReadToEnd();

        //    tileCollection = JsonUtility.FromJson<WorldTileCollection>(json);

        //}

        if (playerXFloor >= 0 && playerYFloor >= 0)
        {

            for (int x = 0; x <  mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    WorldTile wt = GetTile(x, y);
                    for (int i = 0; i < regions.Length; i++)
                    {
                        if ((int)wt.type == i)
                        {
                            regions[i].grid.SetTile(new Vector3Int(x, y, 0), regions[i].tile);
                        }
                        



                    }
                }
            }
        }
    }
}


    //for (int x = 0; x < mapWidth; x++)
    //{
    //    for (int y = 0; y < mapHeight; y++)
    //    {

    //        CreateTileAtHeight(x, y, heightNoiseArray[x, y], moistureNoiseArray[x, y], tempNoiseArray[x, y]);

    //    }

    //}

 



[System.Serializable]
public struct TerrainRegion
{
    public string name;
    public float height;
    public RuleTile tile;
    public Tilemap grid;
}




/*
    public static TerrainGenerator instance;
    public int height;
    public int width;
    public WorldTile[,] tiles;

    [Header("Terrain Parameters")]
    public string seed;
    public bool randomSeed;
    public float heightFrequency;
    public float heightAmplitude;
    public float heightLacunarity;
    public float heightPersistance;
    public int heightOctaves;

    [Header("Moisture Parameters")]
    public float moistFrequency;
    public float moistAmplitude;
    public float moistLacunarity;
    public float moistPersistance;
    public int moistOctaves;
    [Header("Temperature Parameters")]
    public float tempFrequency;
    public float tempAmplitude;
    public float tempLacunarity;
    public float tempPersistance;
    public int tempOctaves;
    
    [Header("Terrain Levels")]
    public float seaLevel;

    public float beachStartHeight;
    public float beachEndHeight;

    public float grassStartHeight;
    public float grassEndHeight;

    public float dirtStartHeight;
    public float dirtEndHeight;

    public float stoneStartHeight;
    public float stoneEndHeight;


    [Header("Biome Levels")]
    [Header("Swamp Levels")]
    public float swampTempStart;
    public float swampTempEnd;
    public float swampMoistStart;
    public float swampMoistEnd;
    [Header("Coniferous Levels")]
    public float coniferousTempStart;
    public float coniferousTempEnd;
    public float coniferousMoistStart;
    public float coniferousMoistEnd;
    [Header("Tropical Levels")]

    public float tropicalTempStart;
    public float tropicalTempEnd;
    public float tropicalMoistStart;
    public float tropicalMoistEnd;
    [Header("Taiga Levels")]

    public float taigaTempStart;
    public float taigaTempEnd;
    public float taigaMoistStart;
    public float taigaMoistEnd;
    [Header("Savannah Levels")]

    public float savannahTempStart;
    public float savannahTempEnd;
    public float savannahMoistStart;
    public float savannahMoistEnd;
    [Header("Tundra Levels")]

    public float tundraTempStart;
    public float tundraTempEnd;
    public float tundraMoistStart;
    public float tundraMoistEnd;
    [Header("Desert Levels")]

    public float desertTempStart;
    public float desertTempEnd;
    public float desertMoistStart;
    public float desertMoistEnd;

    public Tilemap tMap;
    public RuleTile grass;
    public RuleTile dirt;
    public RuleTile sand;
    public RuleTile water;
    public Tile swamp;
    public Tile taiga;
    public Tile tropical;
    public Tile savannah;
    public Tile tundra;
    public Tile desert;
    public Tile coniferous;
    public Tile stone;

    public int treeAmount;
    public GameObject treeApple;
    public GameObject treePine;




    Noise heightNoise;
    public float[,] heightNoiseValues;
    Noise moistureNoise;
    public float[,] moistureNoiseValues;
    Noise temperatureNoise;
    public float[,] temperatureNoiseValues;


    void Awake()
    {
        instance = this;

        if (randomSeed == true)
        {

            int value = UnityEngine.Random.Range(-5000, 5000);
            seed = value.ToString();
        }

        heightNoise = new Noise(seed.GetHashCode(), heightFrequency, heightAmplitude, heightLacunarity, heightPersistance, heightOctaves);
        moistureNoise = new Noise(seed.GetHashCode(), moistFrequency, moistAmplitude, moistLacunarity, moistPersistance, moistOctaves);
        temperatureNoise = new Noise(seed.GetHashCode(), tempFrequency, tempAmplitude, tempLacunarity, tempPersistance, tempOctaves);

        tiles = new WorldTile[width, height];

        heightNoiseValues = heightNoise.GetNoiseValues(width, height);
        moistureNoiseValues = moistureNoise.GetNoiseValues(width, height);
        temperatureNoiseValues = temperatureNoise.GetNoiseValues(width, height);

        GenerateTiles(heightNoiseValues, moistureNoiseValues, temperatureNoiseValues);
      //  GenerateTrees();
    }


    public void GenerateTiles(float[,] heightNoiseArray, float[,] moistureNoiseArray, float[,] tempNoiseArray)
    {

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                CreateTileAtHeight(x, y, heightNoiseArray[x, y], moistureNoiseArray[x, y], tempNoiseArray[x, y]);

            }

        }
    }

    public void GenerateTrees()
    {

        int count = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                if (Random.Range(0, 1000) > 998 && count < treeAmount)
                {
                    Instantiate(treeApple, new Vector3(x,y), Quaternion.identity );
                    count++;
                }
            }
        }
    
    }
    

    public void CreateTileAtHeight(int x, int y, float currentHeight, float currentMoist, float currentTemp)
    {

        if (currentHeight <= seaLevel)
        {
         //   tMap.SetTile(new Vector3Int(x, y, 0), water);
            tiles[x, y] = new WorldTile(WorldTile.Type.Water);
            
        }

        if (currentHeight >= beachStartHeight && currentHeight <= beachEndHeight)
        {
          //  tMap.SetTile(new Vector3Int(x, y, 0), sand);
            tiles[x, y] = new WorldTile(WorldTile.Type.Sand);
        }

        if (currentHeight >= grassStartHeight && currentHeight <= grassEndHeight)
        {
            if (currentMoist > swampMoistStart && currentMoist < swampMoistEnd && currentTemp > swampTempStart && currentTemp < swampTempEnd)
            {

             //   tMap.SetTile(new Vector3Int(x, y, 0), swamp);
                tiles[x, y] = new WorldTile(WorldTile.Type.Grass);
            }

            else if (currentMoist > taigaMoistStart && currentMoist < taigaMoistEnd && currentTemp > taigaTempStart && currentTemp < taigaTempEnd)
            {

              //  tMap.SetTile(new Vector3Int(x, y, 0), taiga);
                tiles[x, y] = new WorldTile(WorldTile.Type.Grass);

            }

            else if (currentMoist > tropicalMoistStart && currentMoist < tropicalMoistEnd && currentTemp > tropicalTempStart && currentTemp < tropicalTempEnd)
            {

              //  tMap.SetTile(new Vector3Int(x, y, 0), tropical);
                tiles[x, y] = new WorldTile(WorldTile.Type.Grass);

            }
            else if (currentMoist > savannahMoistStart && currentMoist < savannahMoistEnd && currentTemp > savannahTempStart && currentTemp < savannahTempEnd)
            {

             //   tMap.SetTile(new Vector3Int(x, y, 0), grass);
                tiles[x, y] = new WorldTile(WorldTile.Type.Grass);

            }
            else if (currentMoist > tundraMoistStart && currentMoist < tundraTempEnd && currentTemp > tundraTempStart && currentTemp < tundraTempEnd)
            {

              //  tMap.SetTile(new Vector3Int(x, y, 0), tundra);
                tiles[x, y] = new WorldTile(WorldTile.Type.Grass);


            }

            else if (currentMoist > desertMoistStart && currentMoist < desertMoistEnd && currentTemp > desertTempStart && currentTemp < desertTempEnd)
            {

              //  tMap.SetTile(new Vector3Int(x, y, 0), desert);
                tiles[x, y] = new WorldTile(WorldTile.Type.Grass);


            }
            else if (currentMoist > coniferousMoistStart && currentMoist < coniferousMoistEnd && currentTemp > coniferousTempStart && currentTemp < coniferousTempEnd)
            {

              //  tMap.SetTile(new Vector3Int(x, y, 0), coniferous);
                tiles[x, y] = new WorldTile(WorldTile.Type.Grass);


            }

            else
            {
              //  tMap.SetTile(new Vector3Int(x, y, 0), grass);
                tiles[x, y] = new WorldTile(WorldTile.Type.Grass);



            }

        }





        if (currentHeight >= dirtStartHeight && currentHeight <= dirtEndHeight)
        {
          //  tMap.SetTile(new Vector3Int(x, y, 0), dirt);
            tiles[x, y] = new WorldTile(WorldTile.Type.Dirt);


        }

        if (currentHeight >= stoneStartHeight && currentHeight <= stoneEndHeight)
        {
          //  tMap.SetTile(new Vector3Int(x, y, 0), stone);
            tiles[x, y] = new WorldTile(WorldTile.Type.Stone);




        }




    }


    // Start is called before the first frame update
    void Regen()
    {

        int value = UnityEngine.Random.Range(5000, 6000);
        seed = value.ToString();

        heightNoise = new Noise(seed.GetHashCode(), heightFrequency, heightAmplitude, heightLacunarity, heightPersistance, heightOctaves);
        moistureNoise = new Noise(seed.GetHashCode(), moistFrequency, moistAmplitude, moistLacunarity, moistPersistance, moistOctaves);
        temperatureNoise = new Noise(seed.GetHashCode(), tempFrequency, tempAmplitude, tempLacunarity, tempPersistance, tempOctaves);

        tiles = new WorldTile[width, height];

        heightNoiseValues = heightNoise.GetNoiseValues(width, height);
        moistureNoiseValues = moistureNoise.GetNoiseValues(width, height);
        temperatureNoiseValues = temperatureNoise.GetNoiseValues(width, height);

        GenerateTiles(heightNoiseValues, moistureNoiseValues, temperatureNoiseValues);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //   Regen();
         

        }
    }
}

    */

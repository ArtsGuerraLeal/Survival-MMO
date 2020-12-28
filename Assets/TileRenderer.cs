using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TileRenderer : MonoBehaviour
{

    [System.Serializable]
    public class Variants
    {
        //public SpriteRenderer spriteRenderer;
        public int gatherableId;
        public Sprite[] sprites;
    }


    public static TileRenderer instance;
    public List<Variants> variantList;
    public GameObject[] tiles;
    public int boundsX;
    public int boundsY;
    public Transform player;
    public GameObject chunk;
    public GameObject[,] chunkArray;
    public int worldSizeX;
    public int worldSizeY;
    public int playerX;
    public int playerY;
    public int playerXOld;
    public int playerYOld;
    public Sprite[] testSprites;
    public Sprite[] sprites;
    public Sprite[] forestSprites;
    public Texture2D texture;
    private Dictionary<string, Sprite> spriteDictionary;
    public string streamPath;
    public string streamPathObj;
    public BuildableDatabaseObject buildableDatabase;
    public BuildableObject bo;
    public GameObject sign;
    private MeshTileCollection tileCollection;
    private MeshTileCollection[,] tileCollectionArray;
    private PlacedObjectCollection[,] objCollectionArray;
    public int worldSize;

    public int chunkSize;

    private void Awake()
    {
        instance = this;
        sprites = Resources.LoadAll<Sprite>("TilesNew");
        forestSprites = Resources.LoadAll<Sprite>("TilesForest");
        chunkArray = new GameObject[worldSizeX,worldSizeY];
        spriteDictionary = new Dictionary<string, Sprite>();
        tileCollectionArray = new MeshTileCollection[worldSizeX, worldSizeY];
        objCollectionArray = new PlacedObjectCollection[worldSizeX, worldSizeY];

        foreach (Sprite s in sprites)
        {

            if (!spriteDictionary.ContainsKey(s.name))
            {
                spriteDictionary.Add(s.name, s);
            }

        }

        foreach (Sprite f in forestSprites)
        {

            if (!spriteDictionary.ContainsKey(f.name))
            {
                spriteDictionary.Add(f.name, f);
            }

        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        playerX = (int)Mathf.FloorToInt(player.position.x / 10) * 10;
        playerY = (int)Mathf.FloorToInt(player.position.y / 10) * 10;
        playerXOld = playerX;
        playerYOld = playerY;

      //  Debug.Log(playerX + " " + playerY);
        SpawnTiles();
        GenerateObjectsCollection();
        GenerateObjects();
        GenerateNeighbors();
        GenerateSprites();
        StartCoroutine(CheckChange());

    }

    private void GenerateObjectsCollection() {
        int xCounter = 0;
        int yCounter = 0;

        for (int x = playerX - worldSize; x <= playerX + worldSize; x = x + chunkSize)
        {
            for (int y = playerY - worldSize; y <= playerY + worldSize; y = y + chunkSize)
            {
     
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().objCollection = SpawnObjectsFromChunk(x, y, xCounter, yCounter);
              
                yCounter++;
            }
            yCounter = 0;
            xCounter++;
        }

    }

    private void GenerateObjects() {

        int xCounter = 0;
        int yCounter = 0;

        for (int x = playerX - worldSize; x <= playerX + worldSize; x = x + chunkSize)
        {
            for (int y = playerY - worldSize; y <= playerY + worldSize; y = y + chunkSize)
            {
                if (chunkArray[xCounter, yCounter].GetComponent<Chunk>().objCollection != null) {
                    PlacedObject[] obj = chunkArray[xCounter, yCounter].GetComponent<Chunk>().objCollection.placedObjects;
                    for (int i = 0; i < obj.Length; i++) {
                        if (obj[i].o != 0) {
                            GameObject go = Instantiate(buildableDatabase.GetBuild[obj[i].o].Result, new Vector3(obj[i].X, obj[i].Y), Quaternion.identity);
                            go.GetComponent<TemplateScript>().isBuilt = true;
                            GameObject tiles = chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetBaseTile(obj[i].X, obj[i].Y,10);
                            tiles.GetComponent<BaseTile>().placedObject = go;
                            //if (go.GetComponent<WallObject>().isActiveAndEnabled) {
                            //    go.GetComponent<WallObject>().neighbors = GetObjectNeighbors(obj[i].X, obj[i].Y, xCounter, yCounter, 10);

                            //}








                        }
                        if (obj[i].o == 0 && obj[i].h > 0) {
                            GameObject go = Instantiate(buildableDatabase.GetBuild[obj[i].o].Result, new Vector3(obj[i].X, obj[i].Y), Quaternion.identity);

                            go.GetComponentInChildren<BuildingSign>().buildableObject = buildableDatabase.GetBuild[obj[i].c];
                            go.GetComponentInChildren<BuildingSign>().materials = buildableDatabase.GetBuild[obj[i].c].Materials;
                            go.GetComponentInChildren<BuildingSign>().x = obj[i].X;
                            go.GetComponentInChildren<BuildingSign>().y = obj[i].Y;
                            go.GetComponentInChildren<BuildingSign>().isGenerated = true;

                            go.GetComponentInChildren<BuildingSign>().SetAmounts();

                            int[] currentAmounts = new int[go.GetComponentInChildren<BuildingSign>().materials.Count];

                            for (int c = 0; c < go.GetComponentInChildren<BuildingSign>().materials.Count; c++)
                            {
                                currentAmounts[c] = obj[i].m[c];
                                Debug.Log(currentAmounts[c]);
                            }

                                go.GetComponentInChildren<BuildingSign>().currentAmounts = currentAmounts;

                        }
                    }
                }
               

                yCounter++;
            }
            yCounter = 0;
            xCounter++;
        }

    }


    private void GenerateSprites()
    {
        
            int xCounter = 0;
            int yCounter = 0;
            for (int x = 0; x < worldSizeX; x++)
            {
                for (int y = 0; y < worldSizeY; y++)
                {
                    for (int i = chunkArray[xCounter, yCounter].GetComponent<Chunk>().x; i < chunkArray[xCounter, yCounter].GetComponent<Chunk>().x + chunkSize; i++)
                    {
                        for (int j = chunkArray[xCounter, yCounter].GetComponent<Chunk>().y; j < chunkArray[xCounter, yCounter].GetComponent<Chunk>().y + chunkSize; j++)
                        {
                            if (i >= 0 & j >= 0)
                            {
                           
                                string key = GetKeyForSprite(chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetBaseTile(i, j, chunkSize).GetComponent<BaseTile>());
                                chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetBaseTile(i, j, chunkSize).GetComponent<SpriteRenderer>().sprite = spriteDictionary[key];
                            
                            
                               


                            }
                        }
                    }

                    yCounter++;
                }
                yCounter = 0;
                xCounter++;
            }
        
    }

    public MeshTileCollection SpawnTilesFromChunk(int x, int y, int chunkX, int chunkY)
    {

        using (StreamReader stream = new StreamReader(streamPath + x.ToString() + "_" + y.ToString() + ".json"))
        {
            string json = stream.ReadToEnd();
            return tileCollectionArray[chunkX, chunkY] = JsonUtility.FromJson<MeshTileCollection>(json);

        }
    }

    public PlacedObjectCollection SpawnObjectsFromChunk(int x, int y, int chunkX, int chunkY)
    {
     

        if (File.Exists(streamPathObj + x.ToString() + "_" + y.ToString() + ".json"))
        {

            using (StreamReader stream = new StreamReader(streamPathObj + x.ToString() + "_" + y.ToString() + ".json"))
            {
                string json = stream.ReadToEnd();
                
                return objCollectionArray[chunkX, chunkY] = JsonUtility.FromJson<PlacedObjectCollection>(json);

            }
        }
        else {
            return null;

        }

    }



    public void SpawnTiles()
    {
        int xCounter = 0;
        int yCounter = 0;
        for (int x = playerX - worldSize; x <= playerX + worldSize; x = x + chunkSize)
        {
            for (int y = playerY - worldSize; y <= playerY + worldSize; y = y + chunkSize)
            {

                int tileCounter = 0;
                chunkArray[xCounter, yCounter] = Instantiate(chunk, new Vector2(x, y), Quaternion.identity);
                chunkArray[xCounter, yCounter].name = "Chunk " + xCounter + " " + yCounter;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().x = (int)chunkArray[xCounter, yCounter].transform.position.x;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().y = (int)chunkArray[xCounter, yCounter].transform.position.y;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().distanceFromPlayer = Vector2.Distance(chunkArray[xCounter, yCounter].transform.position, player.position);

                chunkArray[xCounter, yCounter].GetComponent<Chunk>().tileCollection = SpawnTilesFromChunk(x, y, xCounter, yCounter);
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().tiles = new GameObject[chunkSize*chunkSize];
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().gatherables = new GameObject[chunkSize*chunkSize];

                for (int i = chunkArray[xCounter, yCounter].GetComponent<Chunk>().x; i < chunkArray[xCounter, yCounter].GetComponent<Chunk>().x + chunkSize; i++)
                {
                    for (int j = chunkArray[xCounter, yCounter].GetComponent<Chunk>().y; j < chunkArray[xCounter, yCounter].GetComponent<Chunk>().y + chunkSize; j++)
                    {
                        if (i >= 0 & j >= 0)
                        {
 
                            GameObject tiles = TileGrid.instance.SpawnFromPool("Tiles", new Vector2(i, j));
                            int gatherableID = (int)chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).g;
                            tiles.transform.parent = chunkArray[xCounter, yCounter].transform;
                  //          Debug.Log("X: " + i + " Y: " + j + " " + GetKeyForSprite(World.instance.GetTile(i, j)));

                            tiles.GetComponent<BaseTile>().x = i;
                            tiles.GetComponent<BaseTile>().y = j;
                            tiles.GetComponent<BaseTile>().variant = chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).v;
                            tiles.GetComponent<BaseTile>().type = (int)chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).t;
                            tiles.GetComponent<BaseTile>().biome = (int)chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).b;
                            if (tiles.GetComponent<BaseTile>().type == 0 || tiles.GetComponent<BaseTile>().type == 5)
                            {
                                tiles.GetComponent<BoxCollider2D>().isTrigger = false;
                            }
                            else
                            {
                                tiles.GetComponent<BoxCollider2D>().isTrigger = true;
                            }
                            //if (tiles.GetComponent<BaseTile>().type == 2 && chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).gatherable == 0)
                            //{
                            //    float r = UnityEngine.Random.Range(-.50f, .50f);
                            //    r = 0;
                            //    if(UnityEngine.Random.Range(0, 100) < 10)
                            //    {
                            //        GameObject detail = TileGrid.instance.SpawnFromPool("Detail", new Vector2(i + r, j + r));
                            //        tiles.GetComponent<BaseTile>().placedObject = detail;

                            //    }

                            //}
                            if (chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).g != 0)
                            {
                                int health = chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).h;

                                //  float r = UnityEngine.Random.Range(-.50f, .50f);
                                float r = 0;
                                if (gatherableID == 1)
                                {
                                    GameObject gatherable = TileGrid.instance.SpawnFromPool("Trees", new Vector2(i + r, j + r));
                                    tiles.GetComponent<BaseTile>().placedObject = gatherable;
                                    gatherable.GetComponentInChildren<Gatherable>().health = health;

                                    foreach (Transform child in gatherable.transform)
                                    {
                                        if (child.tag == "VariantTag")
                                        {
                                            child.GetComponent<SpriteRenderer>().sprite = variantList[0].sprites[tiles.GetComponent<BaseTile>().variant];
                                        }

                                    }

                                }
                                if (gatherableID == 2)
                                {
                                    GameObject gatherable = TileGrid.instance.SpawnFromPool("Rocks", new Vector2(i + r, j + r));
                                    //       gatherable.transform.parent = tiles.transform;

                                    tiles.GetComponent<BaseTile>().placedObject = gatherable;
                                //    chunkArray[xCounter, yCounter].GetComponent<Chunk>().gatherables[tileCounter] = gatherable;
                                    gatherable.GetComponentInChildren<Gatherable>().health = health;

                                    foreach (Transform child in gatherable.transform)
                                    {
                                        if (child.tag == "VariantTag")
                                        {
                                            child.GetComponent<SpriteRenderer>().sprite = variantList[1].sprites[tiles.GetComponent<BaseTile>().variant];
                                        }

                                    }

                                }

                                if (gatherableID == 3)
                                {
                                    GameObject gatherable = TileGrid.instance.SpawnFromPool("Bushes", new Vector2(i + r, j + r));
                                    //      gatherable.transform.parent = tiles.transform;
                                    tiles.GetComponent<BaseTile>().placedObject = gatherable;
                                //    chunkArray[xCounter, yCounter].GetComponent<Chunk>().gatherables[tileCounter] = gatherable;
                                    gatherable.GetComponentInChildren<Gatherable>().health = health;

                                    foreach (Transform child in gatherable.transform)
                                    {
                                        if (child.tag == "VariantTag")
                                        {
                                            child.GetComponent<SpriteRenderer>().sprite = variantList[2].sprites[tiles.GetComponent<BaseTile>().variant];
                                        }

                                    }
                                }
                            }
                           
                            chunkArray[xCounter, yCounter].GetComponent<Chunk>().tiles[tileCounter] = tiles;
                            tileCounter++;
                        }
                    }
                }

                yCounter++;
            }
            yCounter = 0;
            xCounter++;
        }
    }

    public void GenerateTile(int x, int y) {

        TileGrid.instance.SpawnFromPool("Tiles", new Vector2(x, y));

    }

    public void GenerateTiles(int chunkX, int chunkY, GameObject chunk)
    {
        for (int x = chunkX; x < chunkSize; x++)
        {
            for (int y = chunkX; y < chunkSize; y++)
            {
                GameObject tiles = TileGrid.instance.SpawnFromPool("Tiles", new Vector2(x, y));
                tiles.transform.parent = chunk.transform;
            //    string key = GetKeyForSprite(chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetBaseTile(i, j, chunkSize).GetComponent<BaseTile>());
                tiles.GetComponent<BaseTile>().x = x;
                tiles.GetComponent<BaseTile>().y = y;
                tiles.GetComponent<BaseTile>().type = (int)World.instance.GetTile(x, y).t;
           //     tiles.GetComponent<BaseTile>().neighbors = World.instance.GetNeighbors(x, y);
            }
        }


    }

   
    void FixedUpdate()
    {
  
        
    }

    public void GenerateChunk()
    {
        int xCounter = 0;
        int yCounter = 0;
        for (int x = playerX - worldSize; x <= playerX + worldSize; x = x + chunkSize)
        {
            for (int y = playerY - worldSize; y <= playerY + worldSize; y = y + chunkSize)
            {
                chunkArray[xCounter, yCounter].transform.position = new Vector2(x, y);
                chunkArray[xCounter, yCounter].name = "Chunk " + xCounter + " " + yCounter;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().x = (int)chunkArray[xCounter, yCounter].transform.position.x;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().y = (int)chunkArray[xCounter, yCounter].transform.position.y;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().distanceFromPlayer = Vector2.Distance(chunkArray[xCounter, yCounter].transform.position, player.position);

                chunkArray[xCounter, yCounter].GetComponent<Chunk>().tileCollection = SpawnTilesFromChunk(x, y, xCounter, yCounter);


                for (int i = chunkArray[xCounter, yCounter].GetComponent<Chunk>().x; i < chunkArray[xCounter, yCounter].GetComponent<Chunk>().x + chunkSize; i++)
                {
                    for (int j = chunkArray[xCounter, yCounter].GetComponent<Chunk>().y; j < chunkArray[xCounter, yCounter].GetComponent<Chunk>().y + chunkSize; j++)
                    {
                        if (i >= 0 & j >= 0)
                        {
                            GameObject tiles = TileGrid.instance.SpawnFromPool("Tiles", new Vector2(i, j));
                            int gatherableID = (int)chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).g;

                            tiles.transform.parent = chunkArray[xCounter, yCounter].transform;
                            tiles.GetComponent<BaseTile>().x = i;
                            tiles.GetComponent<BaseTile>().y = j;
                            tiles.GetComponent<BaseTile>().type = (int)chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).t;
                            tiles.GetComponent<BaseTile>().biome = (int)chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).b;

                            tiles.GetComponent<BaseTile>().variant = chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).v;
                            if (tiles.GetComponent<BaseTile>().type == 0 || tiles.GetComponent<BaseTile>().type == 5)
                            {
                                tiles.GetComponent<BoxCollider2D>().isTrigger = false;
                            }
                            else {
                                tiles.GetComponent<BoxCollider2D>().isTrigger = true;
                            }

                            if (tiles.GetComponent<BaseTile>().placedObject != null) {
                                tiles.GetComponent<BaseTile>().placedObject.SetActive(false);
                                tiles.GetComponent<BaseTile>().placedObject = null;
                            }

                            //if (tiles.GetComponent<BaseTile>().type == 2 && UnityEngine.Random.Range(0, 100) < 10 && chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).gatherable == 0)
                            //{
                            //    float r = UnityEngine.Random.Range(-.50f, .50f);
                            //    r = 0;
                            //    GameObject detail = TileGrid.instance.SpawnFromPool("Detail", new Vector2(i + r, j + r));
                            //    tiles.GetComponent<BaseTile>().placedObject = detail;
                            //}

                            if (chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).g != 0)
                            {
                                int health = chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).h;
                                // float r = UnityEngine.Random.Range(-1.50f, 1.50f);
                                float r = 0;
                                if (gatherableID == 1)
                                {
                                    GameObject gatherable = TileGrid.instance.SpawnFromPool("Trees", new Vector2(i + r, j + r));
                                    //      gatherable.transform.parent = tiles.transform;
                                    tiles.GetComponent<BaseTile>().placedObject = gatherable;
                                    gatherable.GetComponentInChildren<Gatherable>().health = health;

                                    foreach (Transform child in gatherable.transform)
                                    {
                                        if (child.tag == "VariantTag")
                                        {
                                            child.GetComponent<SpriteRenderer>().sprite = variantList[0].sprites[tiles.GetComponent<BaseTile>().variant];
                                        }

                                    }

                                }
                                if (gatherableID == 2)
                                {
                                    GameObject gatherable = TileGrid.instance.SpawnFromPool("Rocks", new Vector2(i + r, j + r));
                                    //   gatherable.transform.parent = tiles.transform;

                                    tiles.GetComponent<BaseTile>().placedObject = gatherable;
                                    gatherable.GetComponentInChildren<Gatherable>().health = health;

                                    foreach (Transform child in gatherable.transform)
                                    {
                                        if (child.tag == "VariantTag")
                                        {
                                            child.GetComponent<SpriteRenderer>().sprite = variantList[1].sprites[tiles.GetComponent<BaseTile>().variant];
                                        }

                                    }

                                }

                                if (gatherableID == 3)
                                {
                                    GameObject gatherable = TileGrid.instance.SpawnFromPool("Bushes", new Vector2(i + r, j + r));
                                    //    gatherable.transform.parent = tiles.transform;
                                    tiles.GetComponent<BaseTile>().placedObject = gatherable;
                                    gatherable.GetComponentInChildren<Gatherable>().health = health;

                                    foreach (Transform child in gatherable.transform)
                                    {
                                        if (child.tag == "VariantTag")
                                        {
                                            child.GetComponent<SpriteRenderer>().sprite = variantList[2].sprites[tiles.GetComponent<BaseTile>().variant];
                                        }

                                    }

                                }
                            }

                            string key = GetKeyForSprite(chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetBaseTile(i, j, chunkSize).GetComponent<BaseTile>());
                            tiles.GetComponent<SpriteRenderer>().sprite = spriteDictionary[key];

                          //  tiles.GetComponent<BaseTile>().neighbors = GetNeighbors(i, j, xCounter, yCounter, chunkSize);

                        }
                    }
                }

                yCounter++;
            }
            yCounter = 0;
            xCounter++;
        }
    }

    public void GenerateNeighbors()
    {
        int xCounter = 0;
        int yCounter = 0;
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                //Debug.Log("X counter: " + x + " Y Counter " + y);
                for (int i = chunkArray[xCounter, yCounter].GetComponent<Chunk>().x; i < chunkArray[xCounter, yCounter].GetComponent<Chunk>().x + chunkSize; i++)
                {
                    for (int j = chunkArray[xCounter, yCounter].GetComponent<Chunk>().y; j < chunkArray[xCounter, yCounter].GetComponent<Chunk>().y + chunkSize; j++)
                    {
                        if (i >= 0 & j >= 0)
                        {
                            chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetBaseTile(i, j, chunkSize).GetComponent<BaseTile>().neighbors = GetNeighbors(i, j, x, y, chunkSize);
                            chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetBaseTile(i, j, chunkSize).GetComponent<BaseTile>().neighborObjects = GetBaseTileNeighbors(i, j, x, y, chunkSize);

                        }
                    }
                }

                yCounter++;
            }
            yCounter = 0;
            xCounter++;
        }
    }

    public MeshTile[] GetNeighbors(int tileX, int tileY) {
        MeshTile[] neighbors = new MeshTile[4];
        neighbors[0] = new MeshTile(GetTile(tileX + 0, tileY + 1).GetComponent<BaseTile>().type, GetTile(tileX + 0, tileY + 1).GetComponent<BaseTile>().x, GetTile(tileX + 0, tileY + 1).GetComponent<BaseTile>().y, 0,0,0,0, GetTile(tileX + 0, tileY + 1).GetComponent<BaseTile>().biome);
        neighbors[1] = new MeshTile(GetTile(tileX + 1, tileY + 0).GetComponent<BaseTile>().type, GetTile(tileX + 1, tileY + 0).GetComponent<BaseTile>().x, GetTile(tileX + 1, tileY + 0).GetComponent<BaseTile>().y, 0, 0, 0, 0, GetTile(tileX + 1, tileY + 0).GetComponent<BaseTile>().biome);
        neighbors[2] = new MeshTile(GetTile(tileX + 0, tileY - 1).GetComponent<BaseTile>().type, GetTile(tileX + 0, tileY - 1).GetComponent<BaseTile>().x, GetTile(tileX + 0, tileY - 1).GetComponent<BaseTile>().y, 0, 0, 0, 0, GetTile(tileX + 0, tileY - 1).GetComponent<BaseTile>().biome);
        neighbors[3] = new MeshTile(GetTile(tileX - 1, tileY + 0).GetComponent<BaseTile>().type, GetTile(tileX - 1, tileY + 0).GetComponent<BaseTile>().x, GetTile(tileX - 1, tileY + 0).GetComponent<BaseTile>().y, 0, 0, 0, 0, GetTile(tileX - 1, tileY + 0).GetComponent<BaseTile>().biome);

        return neighbors;
    }

    //public PlacedObject[] GetObjectNeighbors(int tileX, int tileY)
    //{
    //    PlacedObject[] neighbors = new PlacedObject[4];
    //    neighbors[0] = new PlacedObject(GetTile(tileX + 0, tileY + 1).GetComponent<BaseTile>().type, GetTile(tileX + 0, tileY + 1).GetComponent<BaseTile>().x, GetTile(tileX + 0, tileY + 1).GetComponent<BaseTile>().y, 0, 0, 0, 0, GetTile(tileX + 0, tileY + 1).GetComponent<BaseTile>().biome);
    //    neighbors[1] = new PlacedObject(GetTile(tileX + 1, tileY + 0).GetComponent<BaseTile>().type, GetTile(tileX + 1, tileY + 0).GetComponent<BaseTile>().x, GetTile(tileX + 1, tileY + 0).GetComponent<BaseTile>().y, 0, 0, 0, 0, GetTile(tileX + 1, tileY + 0).GetComponent<BaseTile>().biome);
    //    neighbors[2] = new PlacedObject(GetTile(tileX + 0, tileY - 1).GetComponent<BaseTile>().type, GetTile(tileX + 0, tileY - 1).GetComponent<BaseTile>().x, GetTile(tileX + 0, tileY - 1).GetComponent<BaseTile>().y, 0, 0, 0, 0, GetTile(tileX + 0, tileY - 1).GetComponent<BaseTile>().biome);
    //    neighbors[3] = new MeshTile(GetTile(tileX - 1, tileY + 0).GetComponent<BaseTile>().type, GetTile(tileX - 1, tileY + 0).GetComponent<BaseTile>().x, GetTile(tileX - 1, tileY + 0).GetComponent<BaseTile>().y, 0, 0, 0, 0, GetTile(tileX - 1, tileY + 0).GetComponent<BaseTile>().biome);

    //    return neighbors;
    //}

    public GameObject[] GetBaseTileNeighbors(int tileX, int tileY, int chunkX, int chunkY, int chunkSize)
    {

        GameObject[] neighbors = new GameObject[4];
        //  Debug.Log("X: "+ tileX + " Y: "+tileY+" ChunkX: "+ chunkX + " ChunkY " + chunkY + " Answer "+ ((tileY + 1) - (chunkY * chunkSize)));
        //if (((tileY + 1) - (chunkY * chunkSize)) == chunkSize)
        //{
        //    // neighbors[0] = TileRenderer.instance.chunkArray[];
        //    neighbors[0] = chunkArray[chunkX, chunkY+1].GetComponent<Chunk>().GetTile(tileX + 0, 0, chunkSize);

        //}
        //else
        //{
        //    neighbors[0] = chunkArray[chunkX, chunkY].GetComponent<Chunk>().GetTile(tileX + 0, tileY + 1, chunkSize);

        //}
        if (tileY + 1 >= chunkArray[chunkX, chunkY].GetComponent<Chunk>().y + chunkSize && chunkY + 1 < worldSizeY)
        {
            neighbors[0] = chunkArray[chunkX, chunkY + 1].GetComponent<Chunk>().GetBaseTile(tileX + 0, tileY + 1, chunkSize);

        }
        else
        {
            neighbors[0] = chunkArray[chunkX, chunkY].GetComponent<Chunk>().GetBaseTile(tileX + 0, tileY + 1, chunkSize);

        }

        if (tileX + 1 >= chunkArray[chunkX, chunkY].GetComponent<Chunk>().x + chunkSize && chunkX + 1 < worldSizeX)
        {
            neighbors[1] = chunkArray[chunkX + 1, chunkY].GetComponent<Chunk>().GetBaseTile(tileX + 1, tileY + 0, chunkSize);

        }
        else
        {
            neighbors[1] = chunkArray[chunkX, chunkY].GetComponent<Chunk>().GetBaseTile(tileX + 1, tileY + 0, chunkSize);

        }

        if (tileY - 1 < 0)
        {
            neighbors[2] = null;
        }
        else if (tileY - 1 < chunkArray[chunkX, chunkY].GetComponent<Chunk>().y && chunkY - 1 >= 0)
        {

            neighbors[2] = chunkArray[chunkX, chunkY - 1].GetComponent<Chunk>().GetBaseTile(tileX + 0, tileY - 1, chunkSize);


        }
        else
        {
            neighbors[2] = chunkArray[chunkX, chunkY].GetComponent<Chunk>().GetBaseTile(tileX + 0, tileY - 1, chunkSize);
        }

        if (tileX - 1 < 0)
        {
            neighbors[3] = null;
        }
        else if (tileX - 1 < chunkArray[chunkX, chunkY].GetComponent<Chunk>().x && chunkX - 1 >= 0)
        {
            neighbors[3] = chunkArray[chunkX - 1, chunkY].GetComponent<Chunk>().GetBaseTile(tileX - 1, tileY + 0, chunkSize);

        }
        else
        {
            neighbors[3] = chunkArray[chunkX, chunkY].GetComponent<Chunk>().GetBaseTile(tileX - 1, tileY + 0, chunkSize);

        }

        return neighbors;
    }

    public MeshTile[] GetNeighbors(int tileX, int tileY, int chunkX, int chunkY, int chunkSize)
    {

        MeshTile[] neighbors = new MeshTile[4];
        //  Debug.Log("X: "+ tileX + " Y: "+tileY+" ChunkX: "+ chunkX + " ChunkY " + chunkY + " Answer "+ ((tileY + 1) - (chunkY * chunkSize)));
        //if (((tileY + 1) - (chunkY * chunkSize)) == chunkSize)
        //{
        //    // neighbors[0] = TileRenderer.instance.chunkArray[];
        //    neighbors[0] = chunkArray[chunkX, chunkY+1].GetComponent<Chunk>().GetTile(tileX + 0, 0, chunkSize);

        //}
        //else
        //{
        //    neighbors[0] = chunkArray[chunkX, chunkY].GetComponent<Chunk>().GetTile(tileX + 0, tileY + 1, chunkSize);

        //}
        if (tileY + 1 >= chunkArray[chunkX, chunkY].GetComponent<Chunk>().y + chunkSize && chunkY+1 < worldSizeY)
        {
            neighbors[0] = chunkArray[chunkX, chunkY + 1].GetComponent<Chunk>().GetTile(tileX + 0, tileY + 1, chunkSize);

        }
        else {
            neighbors[0] = chunkArray[chunkX, chunkY].GetComponent<Chunk>().GetTile(tileX + 0, tileY + 1, chunkSize);

        }

        if (tileX + 1 >= chunkArray[chunkX, chunkY].GetComponent<Chunk>().x + chunkSize && chunkX + 1 < worldSizeX)
        {
            neighbors[1] = chunkArray[chunkX + 1, chunkY].GetComponent<Chunk>().GetTile(tileX + 1, tileY + 0, chunkSize);

        }
        else
        {
            neighbors[1] = chunkArray[chunkX, chunkY].GetComponent<Chunk>().GetTile(tileX + 1, tileY + 0, chunkSize);

        }

        if (tileY - 1 < 0)
        {
            neighbors[2] = new MeshTile(5, tileX, -1,0);
        }
        else if (tileY - 1 < chunkArray[chunkX, chunkY].GetComponent<Chunk>().y && chunkY - 1 >= 0)
        {

            neighbors[2] = chunkArray[chunkX, chunkY - 1].GetComponent<Chunk>().GetTile(tileX + 0, tileY - 1, chunkSize);


        }
        else
        {
            neighbors[2] = chunkArray[chunkX, chunkY].GetComponent<Chunk>().GetTile(tileX + 0, tileY - 1, chunkSize);
        }

        if (tileX - 1 < 0)
        {
            neighbors[3] = new MeshTile(5, -1, tileY, 0);
        }
        else if (tileX - 1 < chunkArray[chunkX, chunkY].GetComponent<Chunk>().x && chunkX - 1 >= 0)
        {
            neighbors[3] = chunkArray[chunkX - 1, chunkY].GetComponent<Chunk>().GetTile(tileX - 1, tileY + 0, chunkSize);

        }
        else
        {
            neighbors[3] = chunkArray[chunkX, chunkY].GetComponent<Chunk>().GetTile(tileX - 1, tileY + 0, chunkSize);

        }

        return neighbors;
    }

    public PlacedObject[] GetObjectNeighbors(int tileX, int tileY, int chunkX, int chunkY, int chunkSize)
    {

        PlacedObject[] neighbors = new PlacedObject[4];
        //  Debug.Log("X: "+ tileX + " Y: "+tileY+" ChunkX: "+ chunkX + " ChunkY " + chunkY + " Answer "+ ((tileY + 1) - (chunkY * chunkSize)));
        //if (((tileY + 1) - (chunkY * chunkSize)) == chunkSize)
        //{
        //    // neighbors[0] = TileRenderer.instance.chunkArray[];
        //    neighbors[0] = chunkArray[chunkX, chunkY+1].GetComponent<Chunk>().GetTile(tileX + 0, 0, chunkSize);

        //}
        //else
        //{
        //    neighbors[0] = chunkArray[chunkX, chunkY].GetComponent<Chunk>().GetTile(tileX + 0, tileY + 1, chunkSize);

        //}
        if (tileY + 1 >= chunkArray[chunkX, chunkY].GetComponent<Chunk>().y + chunkSize && chunkY + 1 < worldSizeY)
        {
            neighbors[0] = chunkArray[chunkX, chunkY + 1].GetComponent<Chunk>().GetObject(tileX + 0, tileY + 1, chunkSize);

        }
        else
        {
            neighbors[0] = chunkArray[chunkX, chunkY].GetComponent<Chunk>().GetObject(tileX + 0, tileY + 1, chunkSize);

        }

        if (tileX + 1 >= chunkArray[chunkX, chunkY].GetComponent<Chunk>().x + chunkSize && chunkX + 1 < worldSizeX)
        {
            neighbors[1] = chunkArray[chunkX + 1, chunkY].GetComponent<Chunk>().GetObject(tileX + 1, tileY + 0, chunkSize);

        }
        else
        {
            neighbors[1] = chunkArray[chunkX, chunkY].GetComponent<Chunk>().GetObject(tileX + 1, tileY + 0, chunkSize);

        }

        if (tileY - 1 < 0)
        {
            neighbors[2] = new PlacedObject(0, 0, 0);
        }
        else if (tileY - 1 < chunkArray[chunkX, chunkY].GetComponent<Chunk>().y && chunkY - 1 >= 0)
        {

            neighbors[2] = chunkArray[chunkX, chunkY - 1].GetComponent<Chunk>().GetObject(tileX + 0, tileY - 1, chunkSize);


        }
        else
        {
            neighbors[2] = chunkArray[chunkX, chunkY].GetComponent<Chunk>().GetObject(tileX + 0, tileY - 1, chunkSize);
        }

        if (tileX - 1 < 0)
        {
            neighbors[3] = new PlacedObject(0, 0, 0);
        }
        else if (tileX - 1 < chunkArray[chunkX, chunkY].GetComponent<Chunk>().x && chunkX - 1 >= 0)
        {
            neighbors[3] = chunkArray[chunkX - 1, chunkY].GetComponent<Chunk>().GetObject(tileX - 1, tileY + 0, chunkSize);

        }
        else
        {
            neighbors[3] = chunkArray[chunkX, chunkY].GetComponent<Chunk>().GetObject(tileX - 1, tileY + 0, chunkSize);

        }

        return neighbors;
    }

    string GetKeyForSprite(BaseTile tile)
    {
       
        MeshTile t = new MeshTile(tile.type, 0, 0, 0,0,0,0,tile.biome);
        string key = "";
        if (tile.type == 3)
        {
             key = t.b.ToString();
        }
        else {
             key = t.t.ToString();
        }

        if (tile.type == 3)
        {
            if ((int)tile.neighbors[0].b == tile.biome
            && (int)tile.neighbors[1].b == tile.biome
            && (int)tile.neighbors[2].b == tile.biome
            && (int)tile.neighbors[3].b == tile.biome)
            {
                key += "_Center";
                return key;
            }

            if ((int)tile.neighbors[0].b != tile.biome
            && (int)tile.neighbors[1].b == tile.biome
            && (int)tile.neighbors[2].b == tile.biome
            && (int)tile.neighbors[3].b == tile.biome)
            {
                key += "_3";

                return key;
            }

            if ((int)tile.neighbors[0].b != tile.biome
            && (int)tile.neighbors[1].b != tile.biome
            && (int)tile.neighbors[2].b == tile.biome
            && (int)tile.neighbors[3].b == tile.biome)
            {
                key += "_7";

                return key;
            }

            if ((int)tile.neighbors[0].b == tile.biome
            && (int)tile.neighbors[1].b != tile.biome
            && (int)tile.neighbors[2].b == tile.biome
            && (int)tile.neighbors[3].b == tile.biome)
            {
                key += "_4";

                return key;
            }

            if ((int)tile.neighbors[0].b == tile.biome
            && (int)tile.neighbors[1].b != tile.biome
            && (int)tile.neighbors[2].b != tile.biome
            && (int)tile.neighbors[3].b == tile.biome)
            {
                key += "_9";

                return key;
            }

            if ((int)tile.neighbors[0].b == tile.biome
            && (int)tile.neighbors[1].b == tile.biome
            && (int)tile.neighbors[2].b != tile.biome
            && (int)tile.neighbors[3].b == tile.biome)
            {
                key += "_5";

                return key;
            }

            if ((int)tile.neighbors[0].b == tile.biome
            && (int)tile.neighbors[1].b == tile.biome
            && (int)tile.neighbors[2].b != tile.biome
            && (int)tile.neighbors[3].b != tile.biome)
            {
                key += "_8";

                return key;
            }

            if ((int)tile.neighbors[0].b == tile.biome
            && (int)tile.neighbors[1].b == tile.biome
            && (int)tile.neighbors[2].b == tile.biome
            && (int)tile.neighbors[3].b != tile.biome)
            {
                key += "_2";

                return key;
            }

            if ((int)tile.neighbors[0].b != tile.biome
            && (int)tile.neighbors[1].b == tile.biome
            && (int)tile.neighbors[2].b == tile.biome
            && (int)tile.neighbors[3].b != tile.biome)
            {
                key += "_6";

                return key;
            }

            if ((int)tile.neighbors[0].b != tile.biome
                && (int)tile.neighbors[1].b != tile.biome
                && (int)tile.neighbors[2].b != tile.biome
                && (int)tile.neighbors[3].b != tile.biome)
            {
                key += "_1";
                return key;
            }

            if ((int)tile.neighbors[0].b == tile.biome
                && (int)tile.neighbors[1].b != tile.biome
                && (int)tile.neighbors[2].b == tile.biome
                && (int)tile.neighbors[3].b != tile.biome)
            {
                key += "_10";
                return key;
            }

            if ((int)tile.neighbors[0].b != tile.biome
               && (int)tile.neighbors[1].b == tile.biome
               && (int)tile.neighbors[2].b != tile.biome
               && (int)tile.neighbors[3].b == tile.biome)
            {
                key += "_11";
                return key;
            }

            if ((int)tile.neighbors[0].b != tile.biome
               && (int)tile.neighbors[1].b != tile.biome
               && (int)tile.neighbors[2].b == tile.biome
               && (int)tile.neighbors[3].b != tile.biome)
            {
                key += "_12";
                return key;
            }

            if ((int)tile.neighbors[0].b != tile.biome
               && (int)tile.neighbors[1].b != tile.biome
               && (int)tile.neighbors[2].b != tile.biome
               && (int)tile.neighbors[3].b == tile.biome)
            {
                key += "_13";
                return key;
            }

            if ((int)tile.neighbors[0].b == tile.biome
               && (int)tile.neighbors[1].b != tile.biome
               && (int)tile.neighbors[2].b != tile.biome
               && (int)tile.neighbors[3].b != tile.biome)
            {
                key += "_14";
                return key;
            }

            if ((int)tile.neighbors[0].b != tile.biome
               && (int)tile.neighbors[1].b == tile.biome
               && (int)tile.neighbors[2].b != tile.biome
               && (int)tile.neighbors[3].b != tile.biome)
            {
                key += "_15";
                return key;
            }



            key = "Empty_Center";

            return key;
        }
        else {
            if ((int)tile.neighbors[0].t == tile.type
            && (int)tile.neighbors[1].t == tile.type
            && (int)tile.neighbors[2].t == tile.type
            && (int)tile.neighbors[3].t == tile.type)
            {
                key += "_Center";
                return key;
            }

            if ((int)tile.neighbors[0].t != tile.type
            && (int)tile.neighbors[1].t == tile.type
            && (int)tile.neighbors[2].t == tile.type
            && (int)tile.neighbors[3].t == tile.type)
            {
                key += "_3";

                return key;
            }

            if ((int)tile.neighbors[0].t != tile.type
            && (int)tile.neighbors[1].t != tile.type
            && (int)tile.neighbors[2].t == tile.type
            && (int)tile.neighbors[3].t == tile.type)
            {
                key += "_7";

                return key;
            }

            if ((int)tile.neighbors[0].t == tile.type
            && (int)tile.neighbors[1].t != tile.type
            && (int)tile.neighbors[2].t == tile.type
            && (int)tile.neighbors[3].t == tile.type)
            {
                key += "_4";

                return key;
            }

            if ((int)tile.neighbors[0].t == tile.type
            && (int)tile.neighbors[1].t != tile.type
            && (int)tile.neighbors[2].t != tile.type
            && (int)tile.neighbors[3].t == tile.type)
            {
                key += "_9";

                return key;
            }

            if ((int)tile.neighbors[0].t == tile.type
            && (int)tile.neighbors[1].t == tile.type
            && (int)tile.neighbors[2].t != tile.type
            && (int)tile.neighbors[3].t == tile.type)
            {
                key += "_5";

                return key;
            }

            if ((int)tile.neighbors[0].t == tile.type
            && (int)tile.neighbors[1].t == tile.type
            && (int)tile.neighbors[2].t != tile.type
            && (int)tile.neighbors[3].t != tile.type)
            {
                key += "_8";

                return key;
            }

            if ((int)tile.neighbors[0].t == tile.type
            && (int)tile.neighbors[1].t == tile.type
            && (int)tile.neighbors[2].t == tile.type
            && (int)tile.neighbors[3].t != tile.type)
            {
                key += "_2";

                return key;
            }

            if ((int)tile.neighbors[0].t != tile.type
            && (int)tile.neighbors[1].t == tile.type
            && (int)tile.neighbors[2].t == tile.type
            && (int)tile.neighbors[3].t != tile.type)
            {
                key += "_6";

                return key;
            }

            if ((int)tile.neighbors[0].t != tile.type
                && (int)tile.neighbors[1].t != tile.type
                && (int)tile.neighbors[2].t != tile.type
                && (int)tile.neighbors[3].t != tile.type)
            {
                key += "_1";
                return key;
            }

            if ((int)tile.neighbors[0].t == tile.type
                && (int)tile.neighbors[1].t != tile.type
                && (int)tile.neighbors[2].t == tile.type
                && (int)tile.neighbors[3].t != tile.type)
            {
                key += "_10";
                return key;
            }

            if ((int)tile.neighbors[0].t != tile.type
               && (int)tile.neighbors[1].t == tile.type
               && (int)tile.neighbors[2].t != tile.type
               && (int)tile.neighbors[3].t == tile.type)
            {
                key += "_11";
                return key;
            }

            if ((int)tile.neighbors[0].t != tile.type
               && (int)tile.neighbors[1].t != tile.type
               && (int)tile.neighbors[2].t == tile.type
               && (int)tile.neighbors[3].t != tile.type)
            {
                key += "_12";
                return key;
            }

            if ((int)tile.neighbors[0].t != tile.type
               && (int)tile.neighbors[1].t != tile.type
               && (int)tile.neighbors[2].t != tile.type
               && (int)tile.neighbors[3].t == tile.type)
            {
                key += "_13";
                return key;
            }

            if ((int)tile.neighbors[0].t == tile.type
               && (int)tile.neighbors[1].t != tile.type
               && (int)tile.neighbors[2].t != tile.type
               && (int)tile.neighbors[3].t != tile.type)
            {
                key += "_14";
                return key;
            }

            if ((int)tile.neighbors[0].t != tile.type
               && (int)tile.neighbors[1].t == tile.type
               && (int)tile.neighbors[2].t != tile.type
               && (int)tile.neighbors[3].t != tile.type)
            {
                key += "_15";
                return key;
            }



            key = "Empty_Center";

            return key;
        }

        
    }

    IEnumerator CheckChange()
    {

        while (true)
        {

            if (playerX == playerXOld && playerY == playerYOld)
            {
                playerX = (int)Mathf.FloorToInt(player.position.x / 10) * 10;
                playerY = (int)Mathf.FloorToInt(player.position.y / 10) * 10;

                yield return new WaitForSeconds(.2f);
            }
            else
            {
                playerXOld = playerX;
                playerYOld = playerY;
                if (playerX >= 10 && playerX <= 4980 && playerY >= 10 && playerY <= 4980)
                {
                    GenerateChunk();
                    GenerateObjectsCollection();
                    GenerateObjects();
                    GenerateNeighbors();
                    GenerateSprites();

                }

                yield return new WaitForSeconds(.2f);
            }


        }

    }

    public GameObject GetTile(int tileX, int tileY)
    {

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                if (((tileX / 10) * 10) == chunkArray[x, y].GetComponent<Chunk>().x && ((tileY / 10) * 10) == chunkArray[x, y].GetComponent<Chunk>().y)
                {
                    return chunkArray[x, y].GetComponent<Chunk>().GetBaseTile(tileX, tileY, chunkSize);
                }
            }
        }
        return gameObject;
    }

    public PlacedObject GetObject(int tileX, int tileY, int type)
    {
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                if (((tileX / 10) * 10) == chunkArray[x, y].GetComponent<Chunk>().x && ((tileY / 10) * 10) == chunkArray[x, y].GetComponent<Chunk>().y)
                {
                    return chunkArray[x, y].GetComponent<Chunk>().GetObject(tileX,tileY,10);
                }
            }
        }
        return new PlacedObject();

    }


    public void ChangeTile(int tileX, int tileY,int type) {
        GameObject tile = GetTile(tileX, tileY);
        tile.GetComponent<BaseTile>().type = type;
        string key = GetKeyForSprite(tile.GetComponent<BaseTile>());
        GetTile(tileX, tileY).GetComponent<SpriteRenderer>().sprite = spriteDictionary[key];
        ReloadNeighbors(tileX, tileY);

        if (tile.GetComponent<BaseTile>().type == 0 || tile.GetComponent<BaseTile>().type == 5)
        {
            tile.GetComponent<BoxCollider2D>().isTrigger = false;
        }
        else
        {
            tile.GetComponent<BoxCollider2D>().isTrigger = true;
        }
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                if (((tileX / 10) * 10) == chunkArray[x, y].GetComponent<Chunk>().x && ((tileY / 10) * 10) == chunkArray[x, y].GetComponent<Chunk>().y)
                {

                    chunkArray[x, y].GetComponent<Chunk>().ChangeTile(tile.GetComponent<BaseTile>().type, tileX, tileY, chunkSize);
                    Debug.Log(streamPath + chunkArray[x, y].GetComponent<Chunk>().x + "_" + chunkArray[x, y].GetComponent<Chunk>().y + ".json");
                    using (StreamWriter stream = new StreamWriter(streamPath + chunkArray[x, y].GetComponent<Chunk>().x + "_" + chunkArray[x, y].GetComponent<Chunk>().y + ".json"))
                    {

                        string json = JsonUtility.ToJson(chunkArray[x, y].GetComponent<Chunk>().tileCollection, true);
                        stream.Write(json);
                    }

                }
            }
        }

    }

    public void ChangeGatherable(int tileX, int tileY, int damage = 10)
    {
        GameObject tile = GetTile(tileX, tileY);

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                if (((tileX / 10) * 10) == chunkArray[x, y].GetComponent<Chunk>().x && ((tileY / 10) * 10) == chunkArray[x, y].GetComponent<Chunk>().y)
                {

                    chunkArray[x, y].GetComponent<Chunk>().DamageGatherable(tileX, tileY, chunkSize,damage);
                    Debug.Log(streamPath + chunkArray[x, y].GetComponent<Chunk>().x + "_" + chunkArray[x, y].GetComponent<Chunk>().y + ".json");
                    using (StreamWriter stream = new StreamWriter(streamPath + chunkArray[x, y].GetComponent<Chunk>().x + "_" + chunkArray[x, y].GetComponent<Chunk>().y + ".json"))
                    {

                        string json = JsonUtility.ToJson(chunkArray[x, y].GetComponent<Chunk>().tileCollection, true);
                        stream.Write(json);
                    }

                }
            }
        }

    }

    public void ReloadNeighbors(int tileX, int tileY) {
        GameObject tile = GetTile(tileX, tileY);
       
        foreach (MeshTile neighbor in tile.GetComponent<BaseTile>().neighbors) {

            GameObject t = GetTile(neighbor.X, neighbor.Y);
            t.GetComponent<BaseTile>().neighbors = GetNeighbors(neighbor.X, neighbor.Y);
            string key = GetKeyForSprite(t.GetComponent<BaseTile>());
            t.GetComponent<SpriteRenderer>().sprite = spriteDictionary[key];

        }

    }

    private void SaveTerrainChanges(int tileX, int tileY) {
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                if (((tileX / 10) * 10) == chunkArray[x, y].GetComponent<Chunk>().x && ((tileY / 10) * 10) == chunkArray[x, y].GetComponent<Chunk>().y)
                {

                    using (StreamWriter stream = new StreamWriter(streamPath + x.ToString() + "_" + y.ToString() + ".json"))
                    {
 
                        string json = JsonUtility.ToJson(chunkArray[x, y].GetComponent<Chunk>().tileCollection, true);
                        stream.Write(json);
                    }

                }
            }
        }
    }

    IEnumerator UpdateTerrain()
    {
        WaitForSeconds wait = new WaitForSeconds(.1f);

        for (int x = playerX - 10; x <= playerX + 10; x = x + 10)
        {
            for (int y = playerY - 10; y <= playerY + 10; y = y + 10)
            {

                if (x < 0)
                {
                    x = 0;
                }
                if (y < 0)
                {
                    y = 0;
                }

                //   if (x + 100 > World.instance.width || y + 100 > World.instance.height) break;
                //MoveChunks

                yield return wait;
            }
        }

    }
}

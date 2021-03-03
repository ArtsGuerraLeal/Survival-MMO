using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



public class NetworkTerrainRenderer : NetworkBehaviour
{

    [System.Serializable]
    public class Variants
    {
        //public SpriteRenderer spriteRenderer;
        public int gatherableId;
        public Sprite[] sprites;
    }

    public struct TerrainMessage : NetworkMessage
    {
        public int x;
        public int y;
        public int chunkXCounter;
        public int chunkYCounter;
        public string content;
    }

    public struct ChangeTileMessage : NetworkMessage
    {
        public int x;
        public int y;
        public int chunkX;
        public int chunkY;
        public int type;
    }



    public static NetworkTerrainRenderer instance;
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
    public Sprite[] forestVariants;

    public Texture2D texture;
    private Dictionary<string, Sprite> spriteDictionary;
    public BuildableDatabaseObject buildableDatabase;
    private MeshTileCollection tileCollection;
    private MeshTileCollection[,] tileCollectionArray;
    public GameObject terrainData;
    private NetworkReader data;
    public string streamPath;
    public string tempChunksPath;
    public NetworkConnectionToServer n;
    public bool serverResponse = false;
    public int terrainsGenerated = 0;
    public bool checkChanged = false;
    
    [SyncVar]
    public int health = 100;
    public int worldSize;
    public string jsonField;
    public int chunkSize;

    [Client]
    private void Awake()
    {
       
        
        if (isLocalPlayer)
        {
        instance = this;
        player = this.transform;
        }
        sprites = Resources.LoadAll<Sprite>("TilesNew");
        forestSprites = Resources.LoadAll<Sprite>("TilesForest");
        forestVariants = Resources.LoadAll<Sprite>("TileVariantsConnected");

        chunkArray = new GameObject[worldSizeX, worldSizeY];
        spriteDictionary = new Dictionary<string, Sprite>();
        tileCollectionArray = new MeshTileCollection[worldSizeX, worldSizeY];
        

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

        foreach (Sprite f in forestVariants)
        {

            if (!spriteDictionary.ContainsKey(f.name))
            {
                spriteDictionary.Add(f.name, f);
            }

        }
        
    }

    // Start is called before the first frame update
    [Client]
    void Start()
    {
        if (isLocalPlayer) { 
        n = new NetworkConnectionToServer();
        terrainData = GameObject.FindGameObjectWithTag("TerrainManagerTag");

        playerX = (int)Mathf.FloorToInt(player.position.x / 10) * 10;
        playerY = (int)Mathf.FloorToInt(player.position.y / 10) * 10;
        playerXOld = playerX;
        playerYOld = playerY;
        SpawnInitialChunks();
        //    SpawnNetworkTiles();
        //    GenerateNeighbors();
        //     GenerateSprites();



       
            //     NetworkClient.RegisterHandler<TerrainMessage>(OnDataSend);

        }
    }

    [Server]
    public override void OnStartServer()
    {
        Debug.Log("Server HI");
     
        NetworkServer.RegisterHandler<TerrainMessage>(OnRequestTerrain, false);
        NetworkServer.RegisterHandler<ChangeTileMessage>(OnRequestTileChange, false);

    }

    [Client]
    public override void OnStartClient()
    {
        Debug.Log("Client HI");
        NetworkClient.RegisterHandler<TerrainMessage>(OnResponseTerrain, false);
        NetworkClient.RegisterHandler<ChangeTileMessage>(OnResponseTileChange, false);

    }

    //Request Tile to be changed on server
    [Server]
    public void OnRequestTileChange(NetworkConnection conn, ChangeTileMessage msg)
    {
        if (File.Exists(NetworkManagerMMO.instance.chunkPath + msg.chunkX + "_" + msg.chunkY + ".json"))
        {
            MeshTileCollection m = new MeshTileCollection(new MeshTile[100], "test");
            using (StreamReader stream = new StreamReader(NetworkManagerMMO.instance.streamPath + msg.chunkX + "_" + msg.chunkY + ".json"))
            {
                string json = stream.ReadToEnd();

                 m = JsonUtility.FromJson<MeshTileCollection>(json);

          
            }

            m.tiles[(msg.y - msg.chunkY) + ((msg.x - msg.chunkX) * chunkSize)].t = (MeshTile.Type)msg.type; 

            using (StreamWriter streamOut = new StreamWriter(NetworkManagerMMO.instance.streamPath + msg.chunkX + "_" + msg.chunkY + ".json"))
            {
                
                string jsonOut = JsonUtility.ToJson(m, false);
                streamOut.Write(jsonOut);

            }

            ChangeTileMessage ResponseMessage = new ChangeTileMessage
            {
         

            };
            
            conn.Send(ResponseMessage);
            


        }



    }


    //What happens on client
    [Client]
    public void OnResponseTileChange(NetworkConnection conn, ChangeTileMessage msg)
    {

     
    }

    [Client]
    void SendChangeTileRequest(int tileX, int tileY, int cX, int cY, int t)
    {
        NetworkClient.Send(new ChangeTileMessage { x = tileX, y = tileY, chunkX = cX, chunkY = cY, type = t});
    }

    [Server]
    public void OnRequestTerrain(NetworkConnection conn, TerrainMessage msg)
    {

        string data;
        if (File.Exists(NetworkManagerMMO.instance.streamPath + msg.x + "_" + msg.y + ".json"))
        {
            using (StreamReader stream = new StreamReader(NetworkManagerMMO.instance.streamPath + msg.x + "_" + msg.y + ".json"))
            {
                string json = stream.ReadToEnd();
                data = json;
                //JsonUtility.FromJson<string>(json);
            }

            TerrainMessage ResponseMessage = new TerrainMessage
            {
                content = data,
                chunkXCounter = msg.chunkXCounter,
                chunkYCounter = msg.chunkYCounter,
                x = msg.x,
                y = msg.y

            };

            conn.Send(ResponseMessage);

        }
    }


    //Client gets file content from server;
    [Client]
    public void OnResponseTerrain(NetworkConnection conn, TerrainMessage msg)
    {
        terrainsGenerated++;

     //   Debug.Log(terrainsGenerated);

        //   chunkArray[msg.chunkXCounter, msg.chunkYCounter].GetComponent<Chunk>().tileCollection = JsonUtility.FromJson<MeshTileCollection>(msg.content);
        int xCounter = msg.chunkXCounter;
        int yCounter = msg.chunkYCounter;

        using (StreamWriter stream = new StreamWriter(tempChunksPath + xCounter + "_" + yCounter + ".json"))
        {

            string json = JsonUtility.ToJson(JsonToMeshTileCollection(msg.content), false);


            stream.Write(json);

        }

        if (terrainsGenerated == 9)
        {
            terrainsGenerated = 0;
            SpawnInitialTiles();
            if (!checkChanged)
            {
                StartCoroutine(CheckChange());
            }
            else {
                GenerateChunk();
            }

        }

    }


    [Client]
    public MeshTileCollection SpawnTilesFromChunk(int x, int y, int chunkX, int chunkY)
    {

        using (StreamReader stream = new StreamReader(tempChunksPath + chunkX + "_" + chunkY + ".json"))
        {
            string json = stream.ReadToEnd();
            return tileCollectionArray[chunkX, chunkY] = JsonUtility.FromJson<MeshTileCollection>(json);

        }

    }

    void DoTerrainStuff(int xCount, int yCount) {
        int xCounter = xCount;
        int yCounter = yCount;
        int tileCounter = 0;
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

                    //if (chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).g != 0)
                    //{
                    //    int health = chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).h;

                    //    //  float r = UnityEngine.Random.Range(-.50f, .50f);
                    //    float r = 0;
                    //    if (gatherableID == 1)
                    //    {
                    //        GameObject gatherable = TileGrid.instance.SpawnFromPool("Trees", new Vector2(i + r, j + r));
                    //        tiles.GetComponent<BaseTile>().placedObject = gatherable;
                    //        gatherable.GetComponentInChildren<Gatherable>().health = health;

                    //        foreach (Transform child in gatherable.transform)
                    //        {
                    //            if (child.tag == "VariantTag")
                    //            {
                    //                child.GetComponent<SpriteRenderer>().sprite = variantList[0].sprites[tiles.GetComponent<BaseTile>().variant];
                    //            }

                    //        }

                    //    }
                    //    if (gatherableID == 2)
                    //    {
                    //        GameObject gatherable = TileGrid.instance.SpawnFromPool("Rocks", new Vector2(i + r, j + r));
                    //        //       gatherable.transform.parent = tiles.transform;

                    //        tiles.GetComponent<BaseTile>().placedObject = gatherable;
                    //        //    chunkArray[xCounter, yCounter].GetComponent<Chunk>().gatherables[tileCounter] = gatherable;
                    //        gatherable.GetComponentInChildren<Gatherable>().health = health;

                    //        foreach (Transform child in gatherable.transform)
                    //        {
                    //            if (child.tag == "VariantTag")
                    //            {
                    //                child.GetComponent<SpriteRenderer>().sprite = variantList[1].sprites[tiles.GetComponent<BaseTile>().variant];
                    //            }

                    //        }

                    //    }

                    //    if (gatherableID == 3)
                    //    {
                    //        GameObject gatherable = TileGrid.instance.SpawnFromPool("Bushes", new Vector2(i + r, j + r));
                    //        //      gatherable.transform.parent = tiles.transform;
                    //        tiles.GetComponent<BaseTile>().placedObject = gatherable;
                    //        //    chunkArray[xCounter, yCounter].GetComponent<Chunk>().gatherables[tileCounter] = gatherable;
                    //        gatherable.GetComponentInChildren<Gatherable>().health = health;

                    //        foreach (Transform child in gatherable.transform)
                    //        {
                    //            if (child.tag == "VariantTag")
                    //            {
                    //                child.GetComponent<SpriteRenderer>().sprite = variantList[2].sprites[tiles.GetComponent<BaseTile>().variant];
                    //            }

                    //        }
                    //    }
                    //}

                    chunkArray[xCounter, yCounter].GetComponent<Chunk>().tiles[tileCounter] = tiles;
                    tileCounter++;
                }
            }
        }
        terrainsGenerated++;


        if (terrainsGenerated == 9)
        {
            GenerateNeighbors();
            GenerateSprites();

            if (!checkChanged)
            {
                StartCoroutine(CheckChange());
            }
            terrainsGenerated = 0;
        }
        //conn.Disconnect();
    }



    [Client]
    void SendTerrainRequest(int chunkX, int chunkY, int xCounter, int yCounter) {
        NetworkClient.Send(new TerrainMessage { x = chunkX, y = chunkY, chunkXCounter = xCounter, chunkYCounter = yCounter });
    }

    [Client]
    public void ReloadTerrain()
    {
        int xCounter = 0;
        int yCounter = 0;
        for (int x = playerX - worldSize; x <= playerX + worldSize; x = x + chunkSize)
        {
            for (int y = playerY - worldSize; y <= playerY + worldSize; y = y + chunkSize)
            {

                chunkArray[xCounter, yCounter] = Instantiate(chunk, new Vector2(x, y), Quaternion.identity);
                chunkArray[xCounter, yCounter].name = "Chunk " + xCounter + " " + yCounter;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().x = (int)chunkArray[xCounter, yCounter].transform.position.x;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().y = (int)chunkArray[xCounter, yCounter].transform.position.y;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().distanceFromPlayer = Vector2.Distance(chunkArray[xCounter, yCounter].transform.position, player.position);
                SendTerrainRequest(x, y, xCounter, yCounter);
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().tiles = new GameObject[chunkSize * chunkSize];
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().gatherables = new GameObject[chunkSize * chunkSize];
                yCounter++;
            }
            yCounter = 0;
            xCounter++;
        }
    }

    [Client]
    public void SpawnInitialChunks()
    {
        int xCounter = 0;
        int yCounter = 0;
        for (int x = playerX - worldSize; x <= playerX + worldSize; x = x + chunkSize)
        {
            for (int y = playerY - worldSize; y <= playerY + worldSize; y = y + chunkSize)
            {
                chunkArray[xCounter, yCounter] = Instantiate(chunk, new Vector2(x, y), Quaternion.identity);
                chunkArray[xCounter, yCounter].name = "Chunk " + xCounter + " " + yCounter;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().x = (int)chunkArray[xCounter, yCounter].transform.position.x;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().y = (int)chunkArray[xCounter, yCounter].transform.position.y;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().distanceFromPlayer = Vector2.Distance(chunkArray[xCounter, yCounter].transform.position, player.position);

                chunkArray[xCounter, yCounter].GetComponent<Chunk>().tiles = new GameObject[chunkSize * chunkSize];
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().gatherables = new GameObject[chunkSize * chunkSize];

                SendTerrainRequest(x, y, xCounter, yCounter);

                yCounter++;
            }
            yCounter = 0;
            xCounter++;
        }
    }

    [Client]
    public void SpawnInitialTiles()
    {
        int xCounter = 0;
        int yCounter = 0;
        for (int x = playerX - worldSize; x <= playerX + worldSize; x = x + chunkSize)
        {
            for (int y = playerY - worldSize; y <= playerY + worldSize; y = y + chunkSize)
            {
                int tileCounter = 0;

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
                            tiles.GetComponent<BaseTile>().seed = (i*j)/2;

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

                            if (tiles.GetComponent<BaseTile>().placedObject != null)
                            {
                                tiles.GetComponent<BaseTile>().placedObject.SetActive(false);
                                tiles.GetComponent<BaseTile>().placedObject = null;
                            }

                            //if (tiles.GetComponent<BaseTile>().type == 3 && chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).g == 0)
                            //{
                            //    if (((tiles.GetComponent<BaseTile>().x*9 + tiles.GetComponent<BaseTile>().y*5)%2) == 0) {
                            //        GameObject detail = TileGrid.instance.SpawnFromPool("Detail", new Vector2(i, j));
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
                                    gatherable.transform.SetParent(GameObject.FindGameObjectWithTag("TreePool").transform);
                                    tiles.GetComponent<BaseTile>().placedObject = gatherable;
                                    gatherable.GetComponentInChildren<Gatherable>().health = health;
                                    gatherable.GetComponentInChildren<Gatherable>().parentTile = tiles.GetComponent<BaseTile>().gameObject;

                                    foreach (Transform child in gatherable.transform)
                                    {
                                        if (child.tag == "VariantTag")
                                        {
                                            child.GetComponent<SpriteRenderer>().sprite = variantList[0].sprites[tiles.GetComponent<BaseTile>().variant];
                                        }

                                        if (child.tag == "VariantTag2")
                                        {

                                            Random.InitState(tiles.GetComponent<BaseTile>().seed);

                                            child.GetComponent<SpriteRenderer>().sprite = variantList[3].sprites[Random.Range(0, 7)];
                                        }

                                    }

                                }
                                if (gatherableID == 2)
                                {
                                    GameObject gatherable = TileGrid.instance.SpawnFromPool("Rocks", new Vector2(i + r, j + r));
                                    //       gatherable.transform.parent = tiles.transform;
                                    gatherable.transform.SetParent(GameObject.FindGameObjectWithTag("RockPool").transform);

                                    tiles.GetComponent<BaseTile>().placedObject = gatherable;
                                    //    chunkArray[xCounter, yCounter].GetComponent<Chunk>().gatherables[tileCounter] = gatherable;
                                    gatherable.GetComponentInChildren<Gatherable>().health = health;
                                    gatherable.GetComponentInChildren<Gatherable>().parentTile = tiles.GetComponent<BaseTile>().gameObject;

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
                                    gatherable.transform.SetParent(GameObject.FindGameObjectWithTag("BushPool").transform);

                                    //      gatherable.transform.parent = tiles.transform;
                                    tiles.GetComponent<BaseTile>().placedObject = gatherable;
                                    //    chunkArray[xCounter, yCounter].GetComponent<Chunk>().gatherables[tileCounter] = gatherable;
                                    gatherable.GetComponentInChildren<Gatherable>().health = health;
                                    gatherable.GetComponentInChildren<Gatherable>().parentTile = tiles.GetComponent<BaseTile>().gameObject;

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
        GenerateNeighbors();
        GenerateSprites();


    }


    [Client]
    public void GenNextChunks()
    {
        int xCounter = 0;
        int yCounter = 0;
        for (int x = playerX - worldSize; x <= playerX + worldSize; x = x + chunkSize)
        {
            for (int y = playerY - worldSize; y <= playerY + worldSize; y = y + chunkSize)
            {
                chunkArray[xCounter, yCounter] = Instantiate(chunk, new Vector2(x, y), Quaternion.identity);
                chunkArray[xCounter, yCounter].name = "Chunk " + xCounter + " " + yCounter;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().x = (int)chunkArray[xCounter, yCounter].transform.position.x;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().y = (int)chunkArray[xCounter, yCounter].transform.position.y;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().distanceFromPlayer = Vector2.Distance(chunkArray[xCounter, yCounter].transform.position, player.position);

                chunkArray[xCounter, yCounter].GetComponent<Chunk>().tiles = new GameObject[chunkSize * chunkSize];
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().gatherables = new GameObject[chunkSize * chunkSize];

                SendTerrainRequest(x, y, xCounter, yCounter);

                yCounter++;
            }
            yCounter = 0;
            xCounter++;
        }

        //if (terrainsGenerated == 9)
        //{

        //    if (!checkChanged)
        //    {
        //        StartCoroutine(CheckChange());
        //    }
        //    terrainsGenerated = 0;
        //}
    }


    [Client]
    public void SpawnNetworkTiles() {
        int xCounter = 0;
        int yCounter = 0;
        for (int x = playerX - worldSize; x <= playerX + worldSize; x = x + chunkSize)
        {
            for (int y = playerY - worldSize; y <= playerY + worldSize; y = y + chunkSize)
            {

                chunkArray[xCounter, yCounter] = Instantiate(chunk, new Vector2(x, y), Quaternion.identity);
                chunkArray[xCounter, yCounter].name = "Chunk " + xCounter + " " + yCounter;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().x = (int)chunkArray[xCounter, yCounter].transform.position.x;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().y = (int)chunkArray[xCounter, yCounter].transform.position.y;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().distanceFromPlayer = Vector2.Distance(chunkArray[xCounter, yCounter].transform.position, player.position);
                SendTerrainRequest(x, y, xCounter, yCounter);
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().tileCollection = SpawnTilesFromChunk(x, y, xCounter, yCounter);

                chunkArray[xCounter, yCounter].GetComponent<Chunk>().tiles = new GameObject[chunkSize * chunkSize];
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().gatherables = new GameObject[chunkSize * chunkSize];



                for (int i = chunkArray[xCounter, yCounter].GetComponent<Chunk>().x; i < chunkArray[xCounter, yCounter].GetComponent<Chunk>().x + chunkSize; i++)
                {
                    for (int j = chunkArray[xCounter, yCounter].GetComponent<Chunk>().y; j < chunkArray[xCounter, yCounter].GetComponent<Chunk>().y + chunkSize; j++)
                    {
                        if (i >= 0 & j >= 0)
                        {
                            int tileCounter = 0;
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
        //  terrainsGenerated++;
        GenerateNeighbors();



    }

    [Client]
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
                            tiles.GetComponent<BaseTile>().seed = (i * j) / 2;
                            tiles.GetComponent<BaseTile>().type = (int)chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).t;
                            tiles.GetComponent<BaseTile>().biome = (int)chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).b;

                            tiles.GetComponent<BaseTile>().variant = chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).v;
                            if (tiles.GetComponent<BaseTile>().type == 0 || tiles.GetComponent<BaseTile>().type == 5)
                            {
                                tiles.GetComponent<BoxCollider2D>().isTrigger = false;
                            }
                            else
                            {
                                tiles.GetComponent<BoxCollider2D>().isTrigger = true;
                            }

                            if (tiles.GetComponent<BaseTile>().placedObject != null)
                            {
                                tiles.GetComponent<BaseTile>().placedObject.SetActive(false);
                                tiles.GetComponent<BaseTile>().placedObject = null;
                            }

                            //if (tiles.GetComponent<BaseTile>().type == 3 && chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).g == 0)
                            //{
                            //    if (((tiles.GetComponent<BaseTile>().x * 9 + tiles.GetComponent<BaseTile>().y * 5) % 2) == 0)
                            //    {
                            //        GameObject detail = TileGrid.instance.SpawnFromPool("Detail", new Vector2(i, j));
                            //        tiles.GetComponent<BaseTile>().placedObject = detail;
                            //    }

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
                                    gatherable.GetComponentInChildren<Gatherable>().parentTile = tiles.GetComponent<BaseTile>().gameObject;

                                    foreach (Transform child in gatherable.transform)
                                    {
                                        if (child.tag == "VariantTag")
                                        {
                                            child.GetComponent<SpriteRenderer>().sprite = variantList[0].sprites[tiles.GetComponent<BaseTile>().variant];
                                        }

                                        if (child.tag == "VariantTag2")
                                        {
                                         

                                            Random.InitState(tiles.GetComponent<BaseTile>().seed);

                                            child.GetComponent<SpriteRenderer>().sprite = variantList[3].sprites[Random.Range(0, 7)];
                                        }

                                    }

                                }
                                if (gatherableID == 2)
                                {
                                    GameObject gatherable = TileGrid.instance.SpawnFromPool("Rocks", new Vector2(i + r, j + r));
                                    //   gatherable.transform.parent = tiles.transform;

                                    tiles.GetComponent<BaseTile>().placedObject = gatherable;
                                    gatherable.GetComponentInChildren<Gatherable>().health = health;
                                    gatherable.GetComponentInChildren<Gatherable>().parentTile = tiles.GetComponent<BaseTile>().gameObject;

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
                                    gatherable.GetComponentInChildren<Gatherable>().parentTile = tiles.GetComponent<BaseTile>().gameObject;

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

    [Client]
    public void GenerateNewChunk() {
       
            int xCounter = 0;
            int yCounter = 0;
            for (int x = playerX - worldSize; x <= playerX + worldSize; x = x + chunkSize)
            {
                for (int y = playerY - worldSize; y <= playerY + worldSize; y = y + chunkSize)
                {        
                    SendTerrainRequest(x, y, xCounter, yCounter);

                    yCounter++;
                }
                yCounter = 0;
                xCounter++;
            }

        
        
    }

    [Client]
    public void GenerateNet()
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
                SendTerrainRequest(x, y, xCounter, yCounter);

                chunkArray[xCounter, yCounter].GetComponent<Chunk>().tileCollection = SpawnTilesFromChunk(x,y,xCounter, yCounter);


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
                            else
                            {
                                tiles.GetComponent<BoxCollider2D>().isTrigger = true;
                            }


                            //if (tiles.GetComponent<BaseTile>().type == 2 && UnityEngine.Random.Range(0, 100) < 10 && chunkArray[xCounter, yCounter].GetComponent<Chunk>().GetTile(i, j, chunkSize).gatherable == 0)
                            //{
                            //    float r = UnityEngine.Random.Range(-.50f, .50f);
                            //    r = 0;
                            //    GameObject detail = TileGrid.instance.SpawnFromPool("Detail", new Vector2(i + r, j + r));
                            //    tiles.GetComponent<BaseTile>().placedObject = detail;
                            //}

                            

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

    IEnumerator ChunkGen()
    {
        int xCounter = 0;
        int yCounter = 0;
        for (int x = playerX - worldSize; x <= playerX + worldSize; x = x + chunkSize)
        {
            for (int y = playerY - worldSize; y <= playerY + worldSize; y = y + chunkSize)
            {
                SendTerrainRequest(x, y, xCounter, yCounter);
          //      chunkArray[xCounter, yCounter].transform.position = new Vector2(x, y);
                chunkArray[xCounter, yCounter].name = "Chunk " + xCounter + " " + yCounter;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().x = (int)chunkArray[xCounter, yCounter].transform.position.x;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().y = (int)chunkArray[xCounter, yCounter].transform.position.y;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().distanceFromPlayer = Vector2.Distance(chunkArray[xCounter, yCounter].transform.position, player.position);
                
                //chunkArray[xCounter, yCounter].GetComponent<Chunk>().tileCollection = SpawnTilesFromChunk(x, y, xCounter, yCounter);

                yield return new WaitForSeconds(1f);

                yCounter++;
            }
            yCounter = 0;
            xCounter++;
            yield return new WaitForSeconds(1f);

        }

        yield return new WaitForSeconds(1f);

    }

    [Client]
    IEnumerator CheckChange()
    {
        checkChanged = true;
        while (true)
        {

            if (playerX == playerXOld && playerY == playerYOld)
            {
                playerX = (int)Mathf.FloorToInt(player.position.x / 10) * 10;
                playerY = (int)Mathf.FloorToInt(player.position.y / 10) * 10;

                yield return new WaitForSeconds(.02f);
            }
            else
            {
                playerXOld = playerX;
                playerYOld = playerY;
                if (playerX >= 10 && playerX <= 4980 && playerY >= 10 && playerY <= 4980)
                {
                    GenerateNewChunk();
               //     GenerateNet();
               //     StartCoroutine(ChunkGen());

                    // GenerateChunk();
                    //GenerateObjectsCollection();
                    //GenerateObjects();
                    //GenerateNeighbors();
                    //GenerateSprites();

                }

                yield return new WaitForSeconds(.02f);
            }


        }

    }

    [Client]
    public void SpawnTiles()
    {
        int xCounter = 0;
        int yCounter = 0;
        for (int x = playerX - worldSize; x <= playerX + worldSize; x = x + chunkSize)
        {
            for (int y = playerY - worldSize; y <= playerY + worldSize; y = y + chunkSize)
            {
                serverResponse = false;
                int tileCounter = 0;
               
                chunkArray[xCounter, yCounter] = Instantiate(chunk, new Vector2(x, y), Quaternion.identity);
                chunkArray[xCounter, yCounter].name = "Chunk " + xCounter + " " + yCounter;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().x = (int)chunkArray[xCounter, yCounter].transform.position.x;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().y = (int)chunkArray[xCounter, yCounter].transform.position.y;
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().distanceFromPlayer = Vector2.Distance(chunkArray[xCounter, yCounter].transform.position, player.position);
                

                chunkArray[xCounter, yCounter].GetComponent<Chunk>().tileCollection = SpawnTilesFromChunk(x,y, xCounter, yCounter);

                chunkArray[xCounter, yCounter].GetComponent<Chunk>().tiles = new GameObject[chunkSize * chunkSize];
                chunkArray[xCounter, yCounter].GetComponent<Chunk>().gatherables = new GameObject[chunkSize * chunkSize];

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

    //Ask Server for data

    [Command]
    void CmdSpawnTilesFromChunk(int x, int y, int chunkX, int chunkY)
    {
        Debug.Log("Call");
        RpcSpawnTilesFromChunk(x,y,chunkX,chunkY);

    }



    [ClientRpc]
    public void RpcSpawnTilesFromChunk(int x, int y, int chunkX, int chunkY)
    {
        //Debug.Log("Response");
        //using (StreamReader stream = new StreamReader(NetworkManagerMMO.isntance.streamPath + x.ToString() + "_" + y.ToString() + ".json"))
        //{
        //    string json = stream.ReadToEnd();
        //    tileCollectionArray[chunkX, chunkY] = JsonToMeshTileCollection(json);
        //    Debug.Log(json);   
        //}

    }

    [Client]
    public MeshTileCollection JsonToMeshTileCollection(string json)
    {
         return JsonUtility.FromJson<MeshTileCollection>(json);
    }

    [Client]
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
    [Client]
    public GameObject[] GetBaseTileNeighbors(int tileX, int tileY, int chunkX, int chunkY, int chunkSize)
    {

        GameObject[] neighbors = new GameObject[4];

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
    [Client]
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
        if (tileY + 1 >= chunkArray[chunkX, chunkY].GetComponent<Chunk>().y + chunkSize && chunkY + 1 < worldSizeY)
        {
            neighbors[0] = chunkArray[chunkX, chunkY + 1].GetComponent<Chunk>().GetTile(tileX + 0, tileY + 1, chunkSize);

        }
        else
        {
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
            neighbors[2] = new MeshTile(5, tileX, -1, 0);
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
    [Client]
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
    [Client]
    public MeshTile[] GetNeighbors(int tileX, int tileY)
    {
        MeshTile[] neighbors = new MeshTile[4];
        neighbors[0] = new MeshTile(GetTile(tileX + 0, tileY + 1).GetComponent<BaseTile>().type, GetTile(tileX + 0, tileY + 1).GetComponent<BaseTile>().x, GetTile(tileX + 0, tileY + 1).GetComponent<BaseTile>().y, 0, 0, 0, 0, GetTile(tileX + 0, tileY + 1).GetComponent<BaseTile>().biome);
        neighbors[1] = new MeshTile(GetTile(tileX + 1, tileY + 0).GetComponent<BaseTile>().type, GetTile(tileX + 1, tileY + 0).GetComponent<BaseTile>().x, GetTile(tileX + 1, tileY + 0).GetComponent<BaseTile>().y, 0, 0, 0, 0, GetTile(tileX + 1, tileY + 0).GetComponent<BaseTile>().biome);
        neighbors[2] = new MeshTile(GetTile(tileX + 0, tileY - 1).GetComponent<BaseTile>().type, GetTile(tileX + 0, tileY - 1).GetComponent<BaseTile>().x, GetTile(tileX + 0, tileY - 1).GetComponent<BaseTile>().y, 0, 0, 0, 0, GetTile(tileX + 0, tileY - 1).GetComponent<BaseTile>().biome);
        neighbors[3] = new MeshTile(GetTile(tileX - 1, tileY + 0).GetComponent<BaseTile>().type, GetTile(tileX - 1, tileY + 0).GetComponent<BaseTile>().x, GetTile(tileX - 1, tileY + 0).GetComponent<BaseTile>().y, 0, 0, 0, 0, GetTile(tileX - 1, tileY + 0).GetComponent<BaseTile>().biome);

        return neighbors;
    }



    [Client]
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

    [Client]
    public void ReloadNeighbors(int tileX, int tileY)
    {
        GameObject tile = GetTile(tileX, tileY);

        foreach (MeshTile neighbor in tile.GetComponent<BaseTile>().neighbors)
        {

            GameObject t = GetTile(neighbor.X, neighbor.Y);
            t.GetComponent<BaseTile>().neighbors = GetNeighbors(neighbor.X, neighbor.Y);
            string key = GetKeyForSprite(t.GetComponent<BaseTile>());
            t.GetComponent<SpriteRenderer>().sprite = spriteDictionary[key];

        }

    }


    [Command]
    public void CmdChangeTile(int tileX, int tileY, int type)
    {
     //   Debug.Log("Call");
        RpcChangeTile( tileX,  tileY,  type);

    }

    //This shows up on client
    [ClientRpc]
    public void RpcChangeTile(int tileX, int tileY, int type)
    {
        GameObject tile = GetTile(tileX, tileY);
        tile.GetComponent<BaseTile>().type = type;
        string key = GetKeyForSprite(tile.GetComponent<BaseTile>());
        GetTile(tileX, tileY).GetComponent<SpriteRenderer>().sprite = spriteDictionary[key];
      //  GenerateNeighbors();
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
                  //  Debug.Log(streamPath + chunkArray[x, y].GetComponent<Chunk>().x + "_" + chunkArray[x, y].GetComponent<Chunk>().y + ".json");
                    SendChangeTileRequest(tileX, tileY, chunkArray[x, y].GetComponent<Chunk>().x, chunkArray[x, y].GetComponent<Chunk>().y, type);
                 

                }
            }
        }

    }


    [Client]
    string GetKeyForSprite(BaseTile tile)
    {

        MeshTile t = new MeshTile(tile.type, 0, 0, 0, 0, 0, 0, tile.biome);
        string key = "";
        if (tile.type == 3)
        {
            key = t.b.ToString();
        }
        else
        {
            key = t.t.ToString();
        }

        if (tile.type == 3)
        {
            if ((int)tile.neighbors[0].b == tile.biome
            && (int)tile.neighbors[1].b == tile.biome
            && (int)tile.neighbors[2].b == tile.biome
            && (int)tile.neighbors[3].b == tile.biome)
            {

                //  System.Random random = new System.Random(tile.GetComponent<BaseTile>().seed);
                Random.InitState(tile.GetComponent<BaseTile>().seed);
                if (Random.Range(0, 100) < 75 && tile.placedObject == null) {
                    key += "_Variant_" + Random.Range(1, 7);
                    return key;
                }

            

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
        else
        {
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

}


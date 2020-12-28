using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TilemapRenderer : MonoBehaviour
{
    public static TilemapRenderer instance;
    public const float maxViewDst = 450;
    public Transform viewer;


    private TilemapChunk[,] chunks;

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

    public string streamPath;
    public WorldTileCollection tileCollection;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        chunks = new TilemapChunk[TerrainGenerator.instance.mapWidth / 100, TerrainGenerator.instance.mapHeight / 100];

        oldCurrentPosition = viewer.transform.position;
        playerXFloor = (int)Mathf.Round(oldCurrentPosition.x / 100) * 100;
        playerYFloor = (int)Mathf.Round(oldCurrentPosition.y / 100) * 100;
        LoadInitialTerrain(0, 0);
       StartCoroutine(CreateChunks());
      // StartCoroutine(CheckChange());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadInitialTerrain(int x, int y) {

        TerrainGenerator.instance.StartChunkGenerator(x,y,100);
    }

    IEnumerator CreateChunks()
    {
        WaitForSeconds wait = new WaitForSeconds(.0001f);

        for (int x = playerXFloor - 100; x <= playerXFloor + 1900; x = x + 100)
        {
            for (int y = playerYFloor - 100; y <= playerYFloor + 1900; y = y + 100)
            {
                for (int i = 0; i < TerrainGenerator.instance.regions.Length; i++)
                {

                    if (x < 0)
                    {
                        x = 0;
                    }

                    if (y < 0)
                    {
                        y = 0;
                    }


                    if (x > TerrainGenerator.instance.mapWidth || y > TerrainGenerator.instance.mapHeight) break;

                    TerrainGenerator.instance.StartChunkGenerator(x, y, 100);
                    yield return wait;

                }

            }
        }
    }

    IEnumerator CreateTerrain()
    {
        WaitForSeconds wait = new WaitForSeconds(.001f);

        if (playerXFloor < 0)
        {
            playerXFloor = 0;

            if (playerYFloor < 0)
            {

                playerYFloor = 0;

            }
        }

        for (int x = playerXFloor - 100; x <= playerXFloor + 100; x = x + 100)
        {
            for (int y = playerYFloor - 100; y <= playerYFloor + 100; y = y + 100)
            {
                for (int i = 0; i < TerrainGenerator.instance.regions.Length; i++)
                {

                if (x < 0)
                {
                    x = 0;
                }

                if (y < 0)
                {
                    y = 0;
                }


                if (x > TerrainGenerator.instance.mapWidth || y > TerrainGenerator.instance.mapHeight) break;

                  TerrainGenerator.instance.StartGenerator(x, y, i, 100);
                  //  chunks[x / 100, y / 100] = new TerrainChunk(new Vector2(x, y), 100, mesh);
                    yield return wait;

                }
            }
        }


    }


}

[System.Serializable]
public class TilemapChunk
{
    public GameObject meshObject;
    public Vector2 position;
    public Bounds bounds;

    public TilemapChunk(Vector2 coord, int size, GameObject parent)
    {
        position = coord;
        bounds = new Bounds(position, Vector2.one * size);

        meshObject = parent;

        //SetVisible(false);
    }

    public void UpdateTerrainChunk()
    {
        float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(TilemapRenderer.instance.viewer.position));
        bool visible = viewerDstFromNearestEdge <= 450;
        SetVisible(visible);
    }

    public void SetVisible(bool visible)
    {
        meshObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return meshObject.activeSelf;
    }

    public float GetDistance()
    {
        float viewerDstFromNearestEdge = Vector2.Distance(position, TerrainRenderer.instance.viewer.position);

        return viewerDstFromNearestEdge;

    }

}

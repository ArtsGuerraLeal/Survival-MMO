using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainRenderer : MonoBehaviour
{
    public static TerrainRenderer instance;
    public const float maxViewDst = 450;
    public Transform viewer;


    private TerrainChunk[,] chunks;

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
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        chunks = new TerrainChunk[World.instance.width/100, World.instance.height / 100];

        oldCurrentPosition = viewer.transform.position;
        playerXFloor = (int)Mathf.Round(oldCurrentPosition.x / 100) * 100;
        playerYFloor = (int)Mathf.Round(oldCurrentPosition.y / 100) * 100;

        StartCoroutine(CreateTerrain());
        StartCoroutine(CheckChange());
        StartCoroutine(UnloadTerrain());

      //  LoadStartingChunks();
    }

    void LoadStartingChunks() {

        if (playerXFloor >= 0 && playerYFloor >= 0)
        {

            for(int x = 0; x <= playerXFloor + 100; x = x + 100) {
                for (int y = 0; y <= playerYFloor + 100; y = y + 100)
                {
                    for (int i = 0; i < World.instance.regions.Length; i++)
                    {
                        string name = World.instance.regions[i].name;
                        World.instance.StartGenerator(x, y, 100, 100, i, name, i * -1);
                    }
                }
            }
        }

    }

    void UpdateChunks()
    {

        if (playerXFloor >= 0 && playerYFloor >= 0)
        {
            for (int x = 0; x <= playerXFloor + 500; x = x + 100)
            {
                for (int y = 0; y <= playerYFloor + 500; y = y + 100)
                {
                    for (int i = 0; i < World.instance.regions.Length; i++)
                    {
                        string name = World.instance.regions[i].name;
                        World.instance.StartGenerator(x, y, 100, 100, i, name, i * -1);
                    }
                }
            }
        }

    }


    IEnumerator CheckChange()
    {

        while (true)
        {

            if (playerXFloor == playerXFloorOld && playerYFloor == playerYFloorOld)
            {
                newCurrentPosition = viewer.transform.position;
                playerXFloor = (int)Mathf.Round(newCurrentPosition.x / 100) * 100;
                playerYFloor = (int)Mathf.Round(newCurrentPosition.y / 100) * 100;

                yield return new WaitForSeconds(1);
            }
            else {
                playerXFloorOld = playerXFloor;
                playerYFloorOld = playerYFloor;
                StartCoroutine(UpdateTerrain());
                StartCoroutine(UnloadTerrain());

                yield return new WaitForSeconds(1);
            }

            
        }

    }

    IEnumerator CreateTerrain()
    {
        WaitForSeconds wait = new WaitForSeconds(.3f);

        if (playerXFloor < 0)
        {
            playerXFloor = 0;

            if (playerYFloor < 0) {

                playerYFloor = 0;

            }
        }
        
            for (int x = playerXFloor - 100; x <= playerXFloor + 100; x = x + 100)
            {
                for (int y = playerYFloor - 100; y <= playerYFloor + 100; y = y + 100)
                {
                    for (int i = 0; i < World.instance.regions.Length; i++)
                    {

                    if (x < 0)
                    {
                        x = 0;
                    }

                    if (y < 0)
                    {
                        y = 0;
                    }

                    
                    if (x > World.instance.width || y > World.instance.height) break;
                    string name = World.instance.regions[i].name;
                            GameObject mesh = World.instance.StartGenerator(x, y, 100, 100, i, name, i *-1);
                            chunks[x / 100, y / 100] = new TerrainChunk(new Vector2(x, y), 100, mesh);
                            yield return wait;
                        
                    }
                }
            }
        

    }

    IEnumerator UpdateTerrain()
    {
        WaitForSeconds wait = new WaitForSeconds(.1f);

        for (int x = playerXFloor - 100; x <= playerXFloor + 100; x = x + 100)
        {
            for (int y = playerYFloor - 100; y <= playerYFloor + 100; y = y + 100)
            {

                if (x < 0)
                {
                    x = 0;
                }
                if (y < 0)
                {
                    y = 0;
                }

                if (x + 100 > World.instance.width || y + 100 > World.instance.height) break;

                if (chunks[x / 100, y / 100] == null) {
                    for (int i = 0; i < World.instance.regions.Length; i++)
                    {
                        string name = World.instance.regions[i].name;
                        GameObject mesh = World.instance.StartGenerator(x, y, 100, 100, i, name, i * -1);
                        chunks[x / 100, y / 100] = new TerrainChunk(new Vector2(x, y), 100, mesh);
                        yield return wait;

                    }
                }
                if (chunks[x / 100, y / 100].IsVisible() == false && chunks[x / 100, y / 100].GetDistance() < 200)
                {
                    chunks[x / 100, y / 100].SetVisible(true);
                    chunks[x / 100, y / 100].meshObject.SetActive(false);
                    chunks[x / 100, y / 100].meshObject.SetActive(true);

                    yield return wait;
                }
            }
        }

    }

    IEnumerator UnloadTerrain()
    {
        WaitForSeconds wait = new WaitForSeconds(.1f);
        int Xchunks = chunks.GetLength(0);
        int Ychunks = chunks.GetLength(1);
        //Debug.Log(Xchunks);
        //Debug.Log(Ychunks);

        for (int x = 0; x < Xchunks; x++)
        {
            for (int y = 0; y < Ychunks; y++)
            {
                    if (chunks[x, y] != null && chunks[x, y].GetDistance() > 200)
                    {
                        Debug.Log("Chunk X: " + x + " Chunk Y: " + y + " Position: " + chunks[x, y].position + " Distance: " + chunks[x, y].GetDistance());
                        chunks[x, y].SetVisible(false);
                        yield return wait;
                    }
                    
                        yield return wait;
                    
            }
        }


        
        //for (int x = playerXFloor - 200; x <= playerXFloor - 100; x = x + 100)
        //{
        //    for (int y = playerYFloor - 200; y <= playerYFloor - 100; y = y + 100)
        //    {

        //        if (x < 0)
        //        {
        //            x = 0;
        //        }
        //        if (y < 0)
        //        {
        //            y = 0;
        //        }


        //        chunks[x / 100, y / 100].SetVisible(false);
        //        Debug.Log("Would unload: " + x / 100 + " and " + x / 100);

        //        yield return wait;


        //    }
        //}

    }
}

[System.Serializable]
public class TerrainChunk
{
    public GameObject meshObject;
    public Vector2 position;
    public Bounds bounds;

    public TerrainChunk(Vector2 coord, int size, GameObject parent)
    {
        position = coord;
        bounds = new Bounds(position, Vector2.one * size);

        meshObject = parent;

        //SetVisible(false);
    }

    public void UpdateTerrainChunk()
    {
        float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(TerrainRenderer.instance.viewer.position));
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

    public float GetDistance() {
        float viewerDstFromNearestEdge = Vector2.Distance(position, TerrainRenderer.instance.viewer.position);

        return viewerDstFromNearestEdge;

    }

}

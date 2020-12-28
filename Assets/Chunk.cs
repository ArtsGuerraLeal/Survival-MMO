using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public int x;
    public int y;
    public float distanceFromPlayer;
    public MeshTileCollection tileCollection;
    public PlacedObjectCollection objCollection;
    public GameObject[] tiles;
    public GameObject[] gatherables;
 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public PlacedObject GetObject(int tileX, int tileY, int chunkSize)
    {
           Debug.Log("X: " + (tileX -x) + "Y: " + (tileY - y) + "  Answer " + ((tileY - y) + ((tileX - x) * chunkSize)) );

        if ((tileX - x) > (chunkSize - 1))
        {
            tileX--; ;

        }
        else if ((tileX - x) < 0)
        {
            tileX++;

        }

        if ((tileY - y) > (chunkSize - 1))
        {
            tileY--;
        }
        else if ((tileY - y) < 0)
        {
            tileY++;
        }

        return objCollection.placedObjects[(tileY - y) + ((tileX - x) * chunkSize)];
    }

    public GameObject GetBaseTile(int tileX, int tileY, int chunkSize)
    {

        if ((tileX - x) > (chunkSize - 1))
        {
            tileX--; ;

        }
        else if ((tileX - x) < 0)
        {
            tileX++;

        }

        if ((tileY - y) > (chunkSize - 1))
        {
            tileY--;
        }
        else if ((tileY - y) < 0)
        {
            tileY++;
        }

        return tiles[(tileY - y) + ((tileX - x) * chunkSize)];
    }

    public MeshTile GetTile(int tileX, int tileY, int chunkSize) {
     //   Debug.Log("X: " + (tileX -x) + "Y: " + (tileY - y) + "  Answer " + ((tileY - y) + ((tileX - x) * chunkSize)) );

        if ((tileX - x) > (chunkSize - 1))
        {
            tileX--; ;

        }
        else if ((tileX - x) < 0)
        {
            tileX++;

        }

        if ((tileY - y) > (chunkSize - 1))
        {
            tileY--;
        }
        else if ((tileY - y) < 0)
        {
            tileY++;
        }

        return tileCollection.tiles[(tileY - y) + ((tileX - x) * chunkSize)];
    }

    public void ChangeTile(int tileType, int tileX, int tileY, int chunkSize)
    {
        //   Debug.Log("X: " + (tileX -x) + "Y: " + (tileY - y) + "  Answer " + ((tileY - y) + ((tileX - x) * chunkSize)) );

        if ((tileX - x) > (chunkSize - 1))
        {
            tileX--; ;

        }
        else if ((tileX - x) < 0)
        {
            tileX++;

        }

        if ((tileY - y) > (chunkSize - 1))
        {
            tileY--;
        }
        else if ((tileY - y) < 0)
        {
            tileY++;
        }

        tileCollection.tiles[(tileY - y) + ((tileX - x) * chunkSize)].t = (MeshTile.Type)tileType;
    }

    public void DamageGatherable( int tileX, int tileY, int chunkSize, int damage = 10)
    {
        //   Debug.Log("X: " + (tileX -x) + "Y: " + (tileY - y) + "  Answer " + ((tileY - y) + ((tileX - x) * chunkSize)) );

        if ((tileX - x) > (chunkSize - 1))
        {
            tileX--; ;

        }
        else if ((tileX - x) < 0)
        {
            tileX++;

        }

        if ((tileY - y) > (chunkSize - 1))
        {
            tileY--;
        }
        else if ((tileY - y) < 0)
        {
            tileY++;
        }

        

        tileCollection.tiles[(tileY - y) + ((tileX - x) * chunkSize)].h -= damage;
        if (tileCollection.tiles[(tileY - y) + ((tileX - x) * chunkSize)].h <= 0) {
            tileCollection.tiles[(tileY - y) + ((tileX - x) * chunkSize)].g = 0;
        }
    }

    public MeshTile[] GetNeighbors(int tileX, int tileY, int chunkSize) {

        MeshTile[] neighbors = new MeshTile[4];
    //    if ((tileY - y) > (chunkSize - 1))
     //   {
     //       neighbors[0] = TileRenderer.instance.chunkArray[];

    //    }
    //   else {
            neighbors[0] = GetTile(tileX + 0, tileY + 1, chunkSize);

      //  }
        neighbors[1] = GetTile(tileX + 1, tileY + 0, chunkSize);
        neighbors[2] = GetTile(tileX + 0, tileY - 1, chunkSize);
        neighbors[3] = GetTile(tileX - 1, tileY + 0, chunkSize);

        return neighbors;
    }
}

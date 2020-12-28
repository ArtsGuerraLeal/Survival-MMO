using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldTile
{
    public enum Type{Water,Sand,Grass,Rock,Snow}

    public int quality = 10;
    public int x;
    public int y;

    public Type type;

    public WorldTile(int i, int x, int y)
    {
        this.type = (Type)i;
        this.x = x;
        this.y = y;

    }

    public WorldTile[] GetNeighbors()
    {

        WorldTile[] neighbors = new WorldTile[4];

        // N E S W

        neighbors[0] = TerrainGenerator.instance.GetTile(x + 0, y + 1);
        neighbors[1] = TerrainGenerator.instance.GetTile(x + 1, y + 0);
        neighbors[2] = TerrainGenerator.instance.GetTile(x + 0, y - 1);
        neighbors[3] = TerrainGenerator.instance.GetTile(x - 1, y + 0);
        //neighbors[4] = World.instance.GetTile(x + 0, y + 1);
        //neighbors[5] = World.instance.GetTile(x + 1, y + 0);
        //neighbors[6] = World.instance.GetTile(x + 0, y - 1);
        //neighbors[7] = World.instance.GetTile(x - 1, y + 0);


        

        return neighbors;
    }

    public WorldTile[] GetNeighborsInChunk(int c)
    {

        WorldTile[] neighbors = new WorldTile[4];

        // N E S W

        neighbors[0] = TerrainGenerator.instance.GetChunkTile(x + 0, y + 1, c);
        neighbors[1] = TerrainGenerator.instance.GetChunkTile(x + 1, y + 0, c);
        neighbors[2] = TerrainGenerator.instance.GetChunkTile(x + 0, y - 1, c);
        neighbors[3] = TerrainGenerator.instance.GetChunkTile(x - 1, y + 0, c);
        //neighbors[4] = World.instance.GetTile(x + 0, y + 1);
        //neighbors[5] = World.instance.GetTile(x + 1, y + 0);
        //neighbors[6] = World.instance.GetTile(x + 0, y - 1);
        //neighbors[7] = World.instance.GetTile(x - 1, y + 0);




        return neighbors;
    }

    //public WorldTile(Type type){

    //   this.type = type;

    //  }
}

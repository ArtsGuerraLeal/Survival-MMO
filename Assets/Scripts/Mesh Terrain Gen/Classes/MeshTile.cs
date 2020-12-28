using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MeshTile
{
    //Is Tile a wall?
    //If it is a wall give it HP
    public enum Type { Deep_Water,Shallow_Water, Sand, Grass,Dirt, Rock,High_Rock, Snow, Empty }
    public enum Biome { Tundra,Cold_Forest, Pine_Forest,Swamp,Coniferous_Forest,Plains, Savanah, Desert,None }
    //Every once in a while set some tiles to foragables
    public enum Gatherable { Empty, Tree, Rock, Bush, Foragable }
    public Type t;
    public int X;
    public int Y;
    public Biome b;
    //Gatherable
    public Gatherable g;
    //Gatherable Health
    public int h;
    //Gatherable Amount
    public int a;
    //Variant
    public int v;

    
    

    public MeshTile(int type, int tileX, int tileY, int gatherable, int gatherableHealth = 0, int gatherAmount = 0, int variant = 0, int biome = 0)
    {

        this.t = (Type)type;
  
        this.X = tileX;
        this.Y = tileY;
        this.g = (Gatherable)gatherable;
        this.h = gatherableHealth;
        this.a = gatherAmount;
        this.v = variant;
        this.b = (Biome)biome;

    }

    public void SetTile(Type newType)
    {
        t = newType;
    }

    public MeshTile[] GetNeighbors(int x, int y)
    {

        MeshTile[] neighbors = new MeshTile[4];

        // N E S W
        //neighbors[0] = World.instance.tiles[x + 0, y + 1];
        //neighbors[1] = World.instance.tiles[x + 1, y + 0];
        // neighbors[2] = World.instance.tiles[x + 0, y - 1];
        //neighbors[3] = World.instance.tiles[x - 1, y + 0];

        neighbors[0] = World.instance.GetTile(x + 0, y + 1);
        neighbors[1] = World.instance.GetTile(x + 1, y + 0);
        neighbors[2] = World.instance.GetTile(x + 0, y - 1);
        neighbors[3] = World.instance.GetTile(x - 1, y + 0);
        //neighbors[4] = World.instance.GetTile(x + 0, y + 1);
        //neighbors[5] = World.instance.GetTile(x + 1, y + 0);
        //neighbors[6] = World.instance.GetTile(x + 0, y - 1);
        //neighbors[7] = World.instance.GetTile(x - 1, y + 0);

        return neighbors;
    }

}


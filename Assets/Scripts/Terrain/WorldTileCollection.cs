using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldTileCollection
{
    public WorldTile[] tiles;
    public string collectionName;
    private string v;

    public WorldTileCollection(WorldTile[] tiles, string v)
    {
        this.tiles = tiles;
        this.v = v;
    }

    public override string ToString()
    {
        return base.ToString();
    }

}

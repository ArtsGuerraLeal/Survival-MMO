using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MeshTileCollection
{
    public MeshTile[] tiles;
    public string collectionName;
    private string v;

    public MeshTileCollection(MeshTile[] tiles, string v)
    {
        this.tiles = tiles;
        this.v = v;
    }

    public override string ToString()
    {
        return base.ToString();
    }
}

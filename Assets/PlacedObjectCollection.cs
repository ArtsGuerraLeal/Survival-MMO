using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlacedObjectCollection
{
    public PlacedObject[] placedObjects;
    public string collectionName;
    private string v;

    public PlacedObjectCollection(PlacedObject[] placedObjects, string v)
    {
        this.placedObjects = placedObjects;
        this.v = v;
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
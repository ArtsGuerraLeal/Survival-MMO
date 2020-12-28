using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTreeCollection
{
    public BaseTree[] trees;
    public string collectionName;
    private string v;

    public BaseTreeCollection(BaseTree[] trees, string v)
    {
        this.trees = trees;
        this.v = v;
    }

    public override string ToString()
    {
        return base.ToString();
    }
}

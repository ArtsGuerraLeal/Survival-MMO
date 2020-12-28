using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseTree
{

    public enum Type { Oak,Fir,Apple }
    public Type type;
    public int tileX;
    public int tileY;
    public int quality;


    public BaseTree(int type,int tileX, int tileY)
    {

        this.type = (Type)type;
        this.tileX = tileX;
        this.tileY = tileY;

    }
}

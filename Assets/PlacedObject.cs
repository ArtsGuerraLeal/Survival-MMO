using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlacedObject
{
    //Objects Crafted ID
    public int c;

    //Objects Position
    public int X;
    public int Y;

    //Objects ID
    public int o;

    //Objects health
    public int h;

    //Objects quality
    public float q;

    //Material ID
    public int[] i;
    //Material Amounts
    public int[] m;

    public PlacedObject(int c = 0, int X = 0, int Y = 0, int o = 0, int[] i = null, int[] m = null, int h = 100, float q = 10)
    {
        this.c = c;
        this.X = X;
        this.Y = Y;
        this.o = o;
        this.h = h;
        this.q = q;
        this.i = i;
        this.m = m;

    }

}


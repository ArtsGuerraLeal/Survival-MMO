using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{

    public List<Vector3> vertices;
    public List<Vector2> UVs;
    float blendFactor = 1;
    public List<int> triangles;
    public int tileType;
    public MeshData(int x, int y, int width, int height, int type, bool mountainLayer = false)
    {

        vertices = new List<Vector3>();
        UVs = new List<Vector2>();
        triangles = new List<int>();
        tileType = type;

        if (mountainLayer)
        {

            for (int i = x; i < width + x; i++)
            {
                for (int j = y; j < height + y; j++)
                {

                    CreateSquareWithQuadrants(i, j);
                }
            }

            return;
        }


        for (int i = x; i < width + x; i++)
        {
            for (int j = y; j < height + y; j++)
            {

                CreateSquare(i, j);
            }
        }
    }

    void CreateSquare(int x, int y)
    {

        MeshTile tile = World.instance.GetTile(x, y);

        if ((int)tile.t == tileType)
        {
            tile = new MeshTile(tileType, x, y, 0);
        }
        else if ((int)tile.t == tileType + 1 && (int)tile.t != 0)
        {
            tile = new MeshTile(tileType, x, y, 0);

        }
        else
        {

            tile = new MeshTile(5, x, y, 0);

        }

        vertices.Add(new Vector3(x + 0, y + 0));
        vertices.Add(new Vector3(x + blendFactor, y + 0));

        vertices.Add(new Vector3(x + 0, y + blendFactor));
        vertices.Add(new Vector3(x + blendFactor, y + blendFactor));

        vertices.Add(new Vector3(x + blendFactor / 2f, y + blendFactor / 2f));

        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 5);
        triangles.Add(vertices.Count - 4);

        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 3);

        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 4);

        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 5);
        triangles.Add(vertices.Count - 3);


        /*
                 vertices.Add (new Vector3(x + 0, y + 0));
                 vertices.Add (new Vector3(x + 1.5f, y + 0));
                 vertices.Add (new Vector3(x + 0, y + 1.5f));
                 vertices.Add (new Vector3(x + 1.5f, y + 1.5f));

                 triangles.Add (vertices.Count - 1);
                 triangles.Add (vertices.Count - 3);
                 triangles.Add (vertices.Count - 4);

                 triangles.Add (vertices.Count - 2);
                 triangles.Add (vertices.Count - 1);
                 triangles.Add (vertices.Count - 4);
         */


        UVs.AddRange(SpriteLoader.instance.GetTileUVs(tile));



    }

    void CreateSquareWithQuadrants(int x, int y)
    {

        MeshTile tile = World.instance.GetTile(x, y);
        MeshTile[] neighbors = World.instance.GetNeighbors(x, y, true);

        //In order of quadrants
        //1st quad: x += 0.5f, y += 0.5f
        CreateQuadrant(tile, neighbors, x + 0.5f, y + 0.5f, 1);

        //In order of quadrants
        //2nd quad: x += 0.5f, y += 0f
        CreateQuadrant(tile, neighbors, x + 0.5f, y + 0f, 2);

        //In order of quadrants
        //3rd quad: x += 0f, y += 0f
        CreateQuadrant(tile, neighbors, x + 0f, y + 0f, 3);

        //In order of quadrants
        //4th quad: x += 0f, y += 0.5f
        CreateQuadrant(tile, neighbors, x + 0f, y + 0.5f, 4);
    }

    void CreateQuadrant(MeshTile tile, MeshTile[] neighbors, float x, float y, int quadrant)
    {

        vertices.Add(new Vector3(x + 0, y + 0));
        vertices.Add(new Vector3(x + 0.5f, y + 0));
        vertices.Add(new Vector3(x + 0, y + 0.5f));
        vertices.Add(new Vector3(x + 0.5f, y + 0.5f));

        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 4);

        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);
        triangles.Add(vertices.Count - 4);

     
    }
}

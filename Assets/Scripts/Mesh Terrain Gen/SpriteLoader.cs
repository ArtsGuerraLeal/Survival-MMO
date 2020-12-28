using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLoader : MonoBehaviour
{

    public static SpriteLoader instance;

    Dictionary<string, Vector2[]> tileUVMap;

    // Use this for initialization
    void Awake()
    {
        Debug.Log("Loaded");
        instance = this;

        tileUVMap = new Dictionary<string, Vector2[]>();

        Sprite[] sprites = Resources.LoadAll<Sprite>("");

        float imageWidth = 0f;
        float imageHeight = 0f;

        foreach (Sprite s in sprites)
        {

            if (s.rect.x + s.rect.width > imageWidth)
                imageWidth = s.rect.x + s.rect.width;

            if (s.rect.y + s.rect.height > imageHeight)
                imageHeight = s.rect.y + s.rect.height;
        }

        foreach (Sprite s in sprites)
        {

            Vector2[] uvs = new Vector2[5];

            uvs[0] = new Vector2(s.rect.x / imageWidth, s.rect.y / imageHeight);
            uvs[1] = new Vector2((s.rect.x + s.rect.width) / imageWidth, s.rect.y / imageHeight);
            uvs[2] = new Vector2(s.rect.x / imageWidth, (s.rect.y + s.rect.height) / imageHeight);
            uvs[3] = new Vector2((s.rect.x + s.rect.width) / imageWidth, (s.rect.y + s.rect.height) / imageHeight);
            uvs[4] = new Vector2((s.rect.x + s.rect.width / 2) / imageWidth, (s.rect.y + s.rect.height / 2) / imageHeight);


            if (!tileUVMap.ContainsKey(s.name))
            {
                tileUVMap.Add(s.name, uvs);

            }


        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector2[] GetTileUVs(MeshTile tile)
    {

        //string key = GetKeyForSprite(tile);
        string key = tile.t.ToString();

        if (tileUVMap.ContainsKey(key) == true)
        {

            return tileUVMap[key];
        }
        else
        {

            Debug.LogError("There is no UV map for tile type: " + key);
            return tileUVMap["Void"];
        }
    }

    string GetKeyForSprite(MeshTile tile)
    {

        string key = tile.t.ToString();

        if (tile.GetNeighbors(tile.X, tile.Y)[0].t == tile.t
            && tile.GetNeighbors(tile.X, tile.Y)[1].t == tile.t
            && tile.GetNeighbors(tile.X, tile.Y)[2].t == tile.t
            && tile.GetNeighbors(tile.X, tile.Y)[3].t == tile.t)
        {
            key += "_Center";
            return key;
        }

        if (tile.GetNeighbors(tile.X, tile.Y)[0].t == MeshTile.Type.Empty
        && tile.GetNeighbors(tile.X, tile.Y)[1].t == tile.t
        && tile.GetNeighbors(tile.X, tile.Y)[2].t == tile.t
        && tile.GetNeighbors(tile.X, tile.Y)[3].t == tile.t)
        {
            key += "_Top";

            return key;
        }

        if (tile.GetNeighbors(tile.X, tile.Y)[0].t == MeshTile.Type.Empty
        && tile.GetNeighbors(tile.X, tile.Y)[1].t == MeshTile.Type.Empty
        && tile.GetNeighbors(tile.X, tile.Y)[2].t == tile.t
        && tile.GetNeighbors(tile.X, tile.Y)[3].t == tile.t)
        {
            key += "_Top_Right_Corner";

            return key;
        }

        if (tile.GetNeighbors(tile.X, tile.Y)[0].t == tile.t
        && tile.GetNeighbors(tile.X, tile.Y)[1].t == MeshTile.Type.Empty
        && tile.GetNeighbors(tile.X, tile.Y)[2].t == tile.t
        && tile.GetNeighbors(tile.X, tile.Y)[3].t == tile.t)
        {
            key += "_Right";

            return key;
        }

        if (tile.GetNeighbors(tile.X, tile.Y)[0].t == tile.t
        && tile.GetNeighbors(tile.X, tile.Y)[1].t == MeshTile.Type.Empty
        && tile.GetNeighbors(tile.X, tile.Y)[2].t == MeshTile.Type.Empty
        && tile.GetNeighbors(tile.X, tile.Y)[3].t == tile.t)
        {
            key += "_Bot_Right_Corner";

            return key;
        }

        if (tile.GetNeighbors(tile.X, tile.Y)[0].t == tile.t
        && tile.GetNeighbors(tile.X, tile.Y)[1].t == tile.t
        && tile.GetNeighbors(tile.X, tile.Y)[2].t == MeshTile.Type.Empty
        && tile.GetNeighbors(tile.X, tile.Y)[3].t == tile.t)
        {
            key += "_Bottom";

            return key;
        }

        if (tile.GetNeighbors(tile.X, tile.Y)[0].t == tile.t
        && tile.GetNeighbors(tile.X, tile.Y)[1].t == tile.t
        && tile.GetNeighbors(tile.X, tile.Y)[2].t == MeshTile.Type.Empty
        && tile.GetNeighbors(tile.X, tile.Y)[3].t == MeshTile.Type.Empty)
        {
            key += "_Bot_Left_Corner";

            return key;
        }

        if (tile.GetNeighbors(tile.X, tile.Y)[0].t == tile.t
        && tile.GetNeighbors(tile.X, tile.Y)[1].t == tile.t
        && tile.GetNeighbors(tile.X, tile.Y)[2].t == tile.t
        && tile.GetNeighbors(tile.X, tile.Y)[3].t == MeshTile.Type.Empty)
        {
            key += "_Left";

            return key;
        }

        if (tile.GetNeighbors(tile.X, tile.Y)[0].t == MeshTile.Type.Empty
        && tile.GetNeighbors(tile.X, tile.Y)[1].t == tile.t
        && tile.GetNeighbors(tile.X, tile.Y)[2].t == tile.t
        && tile.GetNeighbors(tile.X, tile.Y)[3].t == MeshTile.Type.Empty)
        {
            key += "_Top_Left_Corner";

            return key;
        }

        return key;
    }







}

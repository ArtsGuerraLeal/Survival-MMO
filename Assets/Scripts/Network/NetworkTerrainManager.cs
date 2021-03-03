using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NetworkTerrainManager : NetworkBehaviour
{
    public struct TerrainData : NetworkMessage
    {
        public string chunkData;
       
    }

    public static NetworkTerrainManager instance;

    public string streamPath;

    public void Awake()
    {
        instance = this;
    }

    
    public MeshTileCollection GetTerrainData(int x, int y) {

        using (StreamReader stream = new StreamReader(streamPath + x.ToString() + "_" + y.ToString() + ".json"))
        {
            string json = stream.ReadToEnd();
            MeshTileCollection tc = JsonUtility.FromJson<MeshTileCollection>(json);
            return tc;

        }
       
    }

    
    void SendChunk(int x, int y)
    {

        using (StreamReader stream = new StreamReader(streamPath + x.ToString() + "_" + y.ToString() + ".json"))
        {
            string json = stream.ReadToEnd();
            string tiles = JsonUtility.FromJson<string>(json);
         //   var bytes = System.Text.Encoding.UTF8.GetBytes(tiles);

            TerrainData msg = new TerrainData()
            {
                chunkData = tiles
              
            };

        }



    }



}

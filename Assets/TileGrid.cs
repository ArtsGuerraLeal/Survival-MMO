using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid: NetworkBehaviour
{

    [System.Serializable]
    public class Pool {
        public string tag;
        public GameObject prefab;
        public Sprite[] variant;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public static TileGrid instance;
    public int width;
    public int height;
    private int[,] gridArray;
    public GameObject tile;


    private void Awake()
    {
        instance = this;
        CreatePool();

    }

    [Client]
    private void CreatePool() {

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
              //  NetworkServer.Spawn(obj, NetworkClient.connection);

                obj.SetActive(false);
 
                objectPool.Enqueue(obj);

            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    [Client]
    public GameObject SpawnFromPool(string tag, Vector2 position) {
        if (isLocalPlayer) { 

        if (!poolDictionary.ContainsKey(tag)) {
            Debug.LogWarning("No Key Found");
            return null;
        }
        
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
        return null;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTrees : MonoBehaviour
{
    public static WorldTrees instance;
    public GameObject[] treePrefab;
    GameObject treePool;
    // Start is called before the first frame update
    void Awake()
    {

        instance = this;
    }

    public void SpawnTrees() {
      
        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                Random.Range(1, 100);
                MeshTile mt = World.instance.GetTile(x, y);
                if (mt.t == MeshTile.Type.Grass && Random.Range(1, 100) < 10)

                {
                    GameObject tree = Instantiate(treePrefab[Random.Range(0, 2)], new Vector3(x + .5f, y + .8f, 0), Quaternion.identity);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTile : MonoBehaviour
{
    public int x, y;
    public int type;
    public int variant;
    public int biome;
    public bool passable;
    public bool slow;
    public int seed;
    public MeshTile[] neighbors;
    public GameObject[] neighborObjects;
    public GameObject placedObject;
    public bool isWall;
    public bool northWall = false;
    public bool eastWall = false;
    public bool southWall = false;
    public bool westWall = false;

    // Start is called before the first frame update
    void Start()
    {
        if (placedObject != null) {
            if (placedObject.GetComponent<WallObject>() != null) {
                if (neighborObjects[0].GetComponent<BaseTile>().placedObject != null) {
                    northWall = true;
                }
                if (neighborObjects[1].GetComponent<BaseTile>().placedObject != null)
                {
                    eastWall = true;
                }
                if (neighborObjects[2].GetComponent<BaseTile>().placedObject != null)
                {
                    southWall = true;
                }
                if (neighborObjects[3].GetComponent<BaseTile>().placedObject != null)
                {
                    westWall = true;
                }

                if (northWall == true && eastWall == false && southWall == true && westWall == false) {
                    placedObject.GetComponentInChildren<SpriteRenderer>().sprite = placedObject.GetComponent<WallObject>().sprites[0];
                }
                if (northWall == true && eastWall == false && southWall == false && westWall == false)
                {
                    placedObject.GetComponentInChildren<SpriteRenderer>().sprite = placedObject.GetComponent<WallObject>().sprites[4];
                }
                if (northWall == false && eastWall == false && southWall == false && westWall == true)
                {
                    placedObject.GetComponentInChildren<SpriteRenderer>().sprite = placedObject.GetComponent<WallObject>().sprites[2];
                }
                if (northWall == false && eastWall == true && southWall == true && westWall == false)
                {
                    placedObject.GetComponentInChildren<SpriteRenderer>().sprite = placedObject.GetComponent<WallObject>().sprites[6];
                }
                

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (placedObject != null)
        {
            if (placedObject.GetComponent<WallObject>() != null)
            {
                if (neighborObjects[0].GetComponent<BaseTile>().placedObject != null)
                {
                    northWall = true;
                }
                if (neighborObjects[1].GetComponent<BaseTile>().placedObject != null)
                {
                    eastWall = true;
                }
                if (neighborObjects[2].GetComponent<BaseTile>().placedObject != null)
                {
                    southWall = true;
                }
                if (neighborObjects[3].GetComponent<BaseTile>().placedObject != null)
                {
                    westWall = true;
                }

                if (northWall == true && eastWall == false && southWall == true && westWall == false)
                {
                    placedObject.GetComponentInChildren<SpriteRenderer>().sprite = placedObject.GetComponent<WallObject>().sprites[0];
                }
                if (northWall == true && eastWall == false && southWall == false && westWall == false)
                {
                    placedObject.GetComponentInChildren<SpriteRenderer>().sprite = placedObject.GetComponent<WallObject>().sprites[4];
                }
                if (northWall == false && eastWall == false && southWall == false && westWall == true)
                {
                    placedObject.GetComponentInChildren<SpriteRenderer>().sprite = placedObject.GetComponent<WallObject>().sprites[2];
                }
                if (northWall == false && eastWall == true && southWall == true && westWall == false)
                {
                    placedObject.GetComponentInChildren<SpriteRenderer>().sprite = placedObject.GetComponent<WallObject>().sprites[6];
                }


            }
        }
    }
}

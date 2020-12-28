using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public string streamPath;
    public static PlacementManager instance;
    private PlacedObjectCollection objectCollection;
    private PlacedObject[] placedObjects;
    public BuildableDatabaseObject database;
    private List<MaterialsAmount> materials;
    
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
      

    }

    
    void Update()
    {
        
    }

    public void CreateObject(int x, int y, int id) {
        int chunkX = (((x / 10) * 10));
        int chunkY = (((y / 10) * 10));
        if (!File.Exists(streamPath + chunkX.ToString() + "_" + chunkY.ToString() + ".json"))
        {
            placedObjects = new PlacedObject[10 * 10];

            PlacedObject placed = new PlacedObject(0, x, y, id);
            objectCollection = new PlacedObjectCollection(placedObjects, "PlacedObjects");
            placedObjects[(y - chunkY) + ((x - chunkX) * 10)] = placed;

            using (StreamWriter stream = new StreamWriter(streamPath + chunkX.ToString() + "_" + chunkY.ToString() + ".json"))
            {
                string json = JsonUtility.ToJson(objectCollection, false);
                stream.Write(json);

            }
        }
        else {

            using (StreamReader stream = new StreamReader(streamPath + chunkX.ToString() + "_" + chunkY.ToString() + ".json"))
            {
                string json = stream.ReadToEnd();

                objectCollection = JsonUtility.FromJson<PlacedObjectCollection>(json);
               
            }

            PlacedObject placed = new PlacedObject(0,x, y, id);

            if (objectCollection.placedObjects[(y - chunkY) + ((x - chunkX) * 10)].q <= 0) {

                objectCollection.placedObjects[(y - chunkY) + ((x - chunkX) * 10)] = placed;
                using (StreamWriter stream = new StreamWriter(streamPath + chunkX.ToString() + "_" + chunkY.ToString() + ".json"))
                {
                    string json = JsonUtility.ToJson(objectCollection, false);
                    stream.Write(json);

                }

            }

        }
  
    }

    public void AddMaterials(int x, int y,int i, int m) {
        int chunkX = (((x / 10) * 10));
        int chunkY = (((y / 10) * 10));
      
            using (StreamReader stream = new StreamReader(streamPath + chunkX.ToString() + "_" + chunkY.ToString() + ".json"))
            {
                string json = stream.ReadToEnd();

                objectCollection = JsonUtility.FromJson<PlacedObjectCollection>(json);

            }
        

            PlacedObject placed = objectCollection.placedObjects[(y - chunkY) + ((x - chunkX) * 10)];
            placed.m[i] = m;

        

            objectCollection.placedObjects[(y - chunkY) + ((x - chunkX) * 10)] = placed;
            using (StreamWriter stream = new StreamWriter(streamPath + chunkX.ToString() + "_" + chunkY.ToString() + ".json"))
            {
                string json = JsonUtility.ToJson(objectCollection, false);
                stream.Write(json);

            }

        



    }

    public void CreateSign(int x, int y, int id)
    {
        int chunkX = (((x / 10) * 10));
        int chunkY = (((y / 10) * 10));
        if (!File.Exists(streamPath + chunkX.ToString() + "_" + chunkY.ToString() + ".json"))
        {
            placedObjects = new PlacedObject[10 * 10];


            int[] items = new int[database.GetBuild[id].Materials.Count];
            int[] current = new int[database.GetBuild[id].Materials.Count];
            
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = database.GetBuild[id].Materials[i].item.data.Id;
                current[i] = 0;
            }

            
            PlacedObject placed = new PlacedObject(id, x, y, 0,items, current);
            objectCollection = new PlacedObjectCollection(placedObjects, "PlacedObjects");
            placedObjects[(y - chunkY) + ((x - chunkX) * 10)] = placed;

            using (StreamWriter stream = new StreamWriter(streamPath + chunkX.ToString() + "_" + chunkY.ToString() + ".json"))
            {
                string json = JsonUtility.ToJson(objectCollection, false);
                stream.Write(json);

            }
        }
        else
        {
       //     Debug.Log(id);
            using (StreamReader stream = new StreamReader(streamPath + chunkX.ToString() + "_" + chunkY.ToString() + ".json"))
            {
                string json = stream.ReadToEnd();

                objectCollection = JsonUtility.FromJson<PlacedObjectCollection>(json);

            }
            int[] items = new int[database.GetBuild[id].Materials.Count];
            int[] current = new int[database.GetBuild[id].Materials.Count];

            Debug.Log(items.Length);

            for (int i = 0; i < items.Length; i++)
            {
                items[i] = database.GetBuild[id].Materials[i].item.data.Id;
                current[i] = 0;
                Debug.Log(items[i]);
            }

            
            PlacedObject placed = new PlacedObject(id, x, y, 0, items, current);

            if (objectCollection.placedObjects[(y - chunkY) + ((x - chunkX) * 10)].q <= 0)
            {

                objectCollection.placedObjects[(y - chunkY) + ((x - chunkX) * 10)] = placed;
                using (StreamWriter stream = new StreamWriter(streamPath + chunkX.ToString() + "_" + chunkY.ToString() + ".json"))
                {
                    string json = JsonUtility.ToJson(objectCollection, false);
                    stream.Write(json);

                }

            }

        }

    }

    public void UpdateObject(int x, int y, int id)
    {
        int chunkX = (((x / 10) * 10));
        int chunkY = (((y / 10) * 10));

        if (File.Exists(streamPath + chunkX.ToString() + "_" + chunkY.ToString() + ".json"))
        {
            using (StreamReader stream = new StreamReader(streamPath + chunkX.ToString() + "_" + chunkY.ToString() + ".json"))
            {
                string json = stream.ReadToEnd();

                objectCollection = JsonUtility.FromJson<PlacedObjectCollection>(json);

            }

            PlacedObject placed = new PlacedObject(0, x, y, id);

                objectCollection.placedObjects[(y - chunkY) + ((x - chunkX) * 10)] = placed;
                using (StreamWriter stream = new StreamWriter(streamPath + chunkX.ToString() + "_" + chunkY.ToString() + ".json"))
                {
                    string json = JsonUtility.ToJson(objectCollection, false);
                    stream.Write(json);

                }

            
        }

    }

    public void RemoveObject(int x, int y)
    {
        int chunkX = (((x / 10) * 10));
        int chunkY = (((y / 10) * 10));

        if (File.Exists(streamPath + chunkX.ToString() + "_" + chunkY.ToString() + ".json"))
        {
            using (StreamReader stream = new StreamReader(streamPath + chunkX.ToString() + "_" + chunkY.ToString() + ".json"))
            {
                string json = stream.ReadToEnd();

                objectCollection = JsonUtility.FromJson<PlacedObjectCollection>(json);

            }


            objectCollection.placedObjects[(y - chunkY) + ((x - chunkX) * 10)] = null;
            using (StreamWriter stream = new StreamWriter(streamPath + chunkX.ToString() + "_" + chunkY.ToString() + ".json"))
            {
                string json = JsonUtility.ToJson(objectCollection, false);
                stream.Write(json);

            }


        }

    }

    public PlacedObject GetObject(int x, int y)
    {
        int chunkX = (((x / 10) * 10));
        int chunkY = (((y / 10) * 10));

        if (File.Exists(streamPath + chunkX.ToString() + "_" + chunkY.ToString() + ".json"))
        {
            using (StreamReader stream = new StreamReader(streamPath + chunkX.ToString() + "_" + chunkY.ToString() + ".json"))
            {
                string json = stream.ReadToEnd();

                objectCollection = JsonUtility.FromJson<PlacedObjectCollection>(json);

            }
            if (objectCollection.placedObjects[(y - chunkY) + ((x - chunkX) * 10)].q > 0) {
                return objectCollection.placedObjects[(y - chunkY) + ((x - chunkX) * 10)];
            }
            else
            {
                return null;
            }

        }
        else {
            return null;
        }

    }

}

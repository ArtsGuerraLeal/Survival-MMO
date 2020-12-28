using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour
{

    public enum DrawMode { NoiseMap, ColourMap };
    public DrawMode drawMode;

    public int mapWidth;
    public int mapHeight;

    public float noiseScale;
    public float moistScale;
    public float tempScale;

    public int octaves;
    public int moistOctaves;
    public int tempOctaves;

    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    [Range(0, 1)]
    public float moistPersistance;
    public float moistLacunarity;
    [Range(0, 1)]
    public float tempPersistance;
    public float tempLacunarity;

    public int seed;
    public int moistSeed;
    public int tempSeed;

    public Vector2 offset;

    public bool autoUpdate;

    public TerrainType[] regions;

    public ForestType[] forests;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] moistMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, moistSeed, moistScale, moistOctaves, moistPersistance, moistLacunarity, offset);
        float[,] tempMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, tempSeed, tempScale, tempOctaves, tempPersistance, tempLacunarity, offset);
        Debug.Log("Height: "+ noiseMap[0,0]);
        Debug.Log("Moist: " + moistMap[0,0]);
        Debug.Log("Temp: " + tempMap[0,0]);

        // noiseMap = World.instance.heightNoiseValues;
        // mapWidth = World.instance.width ;
        //  mapHeight = World.instance.height;
        //string json = JsonUtility.ToJson(this);
        // Debug.Log("tst");

        Color[] colourMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // 50
                float currentHeight = noiseMap[x, y];
                // 70
                float currentMoist = moistMap[x, y];
                
                // 25
                float currentTemp = tempMap[x, y];

                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {

                        if (i == 3)
                        {
                            Color c = new Color(1,1,1);
                            Color finalColor = new Color(1, 1, 1);
                            for (int f = 0; f < forests.Length; f++) {

                                   if(currentMoist >= forests[f].minMoist && currentMoist <= forests[f].maxMoist)
                                    {
                                       
                                            if (currentTemp >= forests[f].minTemp && currentTemp <= forests[f].maxTemp)
                                            {
                                                finalColor = forests[f].colour;
                                                
                                            }

                                        

                                    }
                            }
                            c = finalColor;
                            colourMap[y * mapWidth + x] = c;
                            break;
                        }
                        else {
                            colourMap[y * mapWidth + x] = regions[i].colour;
                            break;
                        }
                        


                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
        }
    }

    void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

[System.Serializable]
public struct ForestType
{
    public string name;
    public float minMoist;
    public float maxMoist;
    public float minTemp;
    public float maxTemp;

    public Color colour;
}
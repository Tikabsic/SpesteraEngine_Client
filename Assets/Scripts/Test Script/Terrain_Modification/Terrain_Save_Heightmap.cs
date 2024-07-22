using UnityEngine;
using System.IO;

public class Terrain_Save_Heightmap : MonoBehaviour
{
    public Terrain terrain;
    public string filePath = "Assets/heightmap.json";

    private void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            ExportHeightmapToJson();
        }
    }

    public void ExportHeightmapToJson()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain reference not set!");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int length = terrainData.heightmapResolution;
        float[,] heights = terrainData.GetHeights(0, 0, width, length);

        float[] heightmapData = new float[width * length];

        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                heightmapData[z * width + x] = heights[z, x];
            }
        }

        HeightmapData data = new HeightmapData
        {
            width = width,
            length = length,
            heightmap = heightmapData
        };

        string json = JsonUtility.ToJson(data);

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log("Heightmap data exported to JSON: " + filePath);
        }
        catch (IOException e)
        {
            Debug.LogError("Error saving heightmap data to JSON file: " + e.Message);
        }
    }

    [System.Serializable]
    public class HeightmapData
    {
        public int width;
        public int length;
        public float[] heightmap;
    }
}



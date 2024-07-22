
using System.Diagnostics;
using UnityEngine;

public class Terrain_Read_Heightmap_Data : MonoBehaviour
{
    public Terrain terrain;
    public string jsonFilePath = "Assets/heightmap.json";

    private void Update()
    {
        if (Input.GetKey(KeyCode.L))
        {
            ReadHeightMapFromFile();
        }
    }

    private void ReadHeightMapFromFile()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        if (terrain == null)
        {
            UnityEngine.Debug.LogError("Terrain reference not set!");
            return;
        }

        string jsonText = System.IO.File.ReadAllText(jsonFilePath);
        HeightmapData heightmapData = JsonUtility.FromJson<HeightmapData>(jsonText);

        int width = heightmapData.width;
        int length = heightmapData.length;

        TerrainData terrainData = terrain.terrainData;
        float[,] heights = new float[width, length];

        for (int z = 0; z < length; z++)
        {
            for (int x = 0; x < width; x++)
            {
                heights[z, x] = heightmapData.heightmap[z * width + x];
            }
        }

        terrainData.SetHeights(0, 0, heights);

        int jsonSizeBytes = System.Text.Encoding.UTF8.GetBytes(jsonText).Length;
        UnityEngine.Debug.Log($"File byte size: {jsonSizeBytes} bytes");
        UnityEngine.Debug.Log($"Terrain loaded from : {jsonFilePath} in {stopwatch.ElapsedMilliseconds} ms.");
    }

}

[System.Serializable]
public class HeightmapData
{
    public int width;
    public int length;
    public float[] heightmap;
}

using UnityEngine;
using Unity.Mathematics;

public class WorldChunk : MonoBehaviour
{
    private void Awake()
    {
        var terrain = GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogError("Terrain component not found.");
            return;
        }

        TerrainData terrainData = new TerrainData();

        int heightmapResolution = 1025;
        terrainData.heightmapResolution = heightmapResolution;

        terrainData.size = new Vector3(terrain.terrainData.size.x, terrain.terrainData.size.y, terrain.terrainData.size.z);

        // Create an empty heightmap
        float[,] heights = new float[heightmapResolution, heightmapResolution];
        for (int y = 0; y < heightmapResolution; y++)
        {
            for (int x = 0; x < heightmapResolution; x++)
            {
                heights[x, y] = 0;
            }
        }

        terrainData.SetHeights(0, 0, heights);

        terrain.terrainData = terrainData;
    }
}
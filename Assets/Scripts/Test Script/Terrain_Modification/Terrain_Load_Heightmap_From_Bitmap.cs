using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class Terrain_Load_Heightmap_From_Bitmap : MonoBehaviour
{
    public Texture2D heightmapTexture;
    public Texture2D detailmapTexture;
    public Terrain terrain;
    public float heightmapThreshold;
    public float smooth;

    void Update()
    {
        if (Input.GetKey(KeyCode.L))
        {
            LoadHeightmapFromBitmap();
        }


        if (Input.GetKey(KeyCode.D))
        {
            GenerateDetails(heightmapTexture.width, heightmapTexture.height, terrain.terrainData);
        }

    }

    void LoadHeightmapFromBitmap()
    {
        if (heightmapTexture == null || terrain == null)
        {
            Debug.LogError("Heightmap texture or terrain not assigned.");
            return;
        }

        int width = heightmapTexture.width;
        int height = heightmapTexture.height;

        TerrainData terrainData = terrain.terrainData;

        terrainData.heightmapResolution = width + 1;

        float[,] heights = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float pixelValue = heightmapTexture.GetPixel(x, y).grayscale;
                heights[x, y] = pixelValue;
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }

    void GenerateDetails(int width, int height, TerrainData terrainData)
    {
        float[,] heights = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float baseHeight = heightmapTexture.GetPixel(x, y).grayscale;
                float detailHeight = detailmapTexture.GetPixel(x, y).grayscale;

                float finalHeight = baseHeight;
                if (baseHeight > heightmapThreshold)
                {
                    finalHeight += (detailHeight / smooth);
                }

                heights[x, y] = finalHeight;
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }
}

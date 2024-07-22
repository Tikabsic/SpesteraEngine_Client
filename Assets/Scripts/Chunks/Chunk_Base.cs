using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk_Base : MonoBehaviour
{
    public int ID;

    public Texture2D heightmapTexture;
    public Texture2D detailmapTexture;
    public Terrain terrain;
    public float heightmapThreshold;
    public float smooth;

    private void Start()
    {
        terrain = GetComponent<Terrain>();
        this.gameObject.name = $"Chunk_{this.transform.position.x},{this.transform.position.z},";
    }

    public void LoadHeightmapFromBitmap()
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

    public void GenerateDetails()
    {
        float[,] heights = new float[heightmapTexture.width, heightmapTexture.height];

        for (int y = 0; y < heightmapTexture.height; y++)
        {
            for (int x = 0; x < heightmapTexture.width; x++)
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

        terrain.terrainData.SetHeights(0, 0, heights);
    }

    public void SetSize(int width, int height, int y)
    {
        Vector3 newsize = new Vector3(width, y, height);
        terrain.terrainData.size = newsize;
    }
}

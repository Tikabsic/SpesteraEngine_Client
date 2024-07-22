using System.Collections.Generic;
using UnityEngine;

//public class ChunkManager : MonoBehaviour
//{
//    public Terrain terrainPrefab;
//    public int chunkSize = 1000;
//    public int seed;
//    public float scale = 20f;
//    public Texture2D heightmapTexture;
//    public Texture2D detailmapTexture;
//    public float heightmapThreshold;
//    public float smooth;

//    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

//    void Start()
//    {
//        CreateChunk(Vector2Int.zero);
//    }

//    void CreateChunk(Vector2Int coord)
//    {
//        if (chunks.ContainsKey(coord)) return;

//        Vector3 position = new Vector3(coord.x * chunkSize, 0, coord.y * chunkSize);
//        Terrain terrain = Instantiate(terrainPrefab, position, Quaternion.identity);

//        terrain.terrainData = LoadHeightmapFromBitmap();

//        Chunk newChunk = new Chunk(terrain, coord);
//        chunks.Add(coord, newChunk);

//        ConnectNeighbors(newChunk);
//    }

//    TerrainData LoadHeightmapFromBitmap()
//    {
//        if (heightmapTexture == null || terrainPrefab == null)
//        {
//            Debug.LogError("Heightmap texture or terrain not assigned.");
//            return null;
//        }

//        int width = heightmapTexture.width;
//        int height = heightmapTexture.height;

//        TerrainData terrainData = terrainPrefab.terrainData;

//        terrainData.heightmapResolution = width + 1;

//        float[,] heights = new float[width, height];

//        for (int y = 0; y < height; y++)
//        {
//            for (int x = 0; x < width; x++)
//            {
//                float pixelValue = heightmapTexture.GetPixel(x, y).grayscale;
//                heights[x, y] = pixelValue;
//            }
//        }

//        terrainData.SetHeights(0, 0, heights);
//        GenerateDetails(width, height, terrainData);
//        return terrainData;
//    }

//    void GenerateDetails(int width, int height, TerrainData terrainData)
//    {
//        float[,] heights = new float[width, height];

//        for (int y = 0; y < height; y++)
//        {
//            for (int x = 0; x < width; x++)
//            {
//                float baseHeight = heightmapTexture.GetPixel(x, y).grayscale;
//                float detailHeight = detailmapTexture.GetPixel(x, y).grayscale;

//                float finalHeight = baseHeight;
//                if (baseHeight > heightmapThreshold) // Mo¿esz dostosowaæ próg maski
//                {
//                    finalHeight += (detailHeight / smooth);
//                }

//                heights[x, y] = finalHeight;
//            }
//        }

//        terrainData.SetHeights(0, 0, heights);
//    }

//    void ConnectNeighbors(Chunk chunk)
//    {
//        Vector2Int coord = chunk.coord;

//        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
//        foreach (Vector2Int dir in directions)
//        {
//            Vector2Int neighborCoord = coord + dir;
//            if (chunks.TryGetValue(neighborCoord, out Chunk neighbor))
//            {
//                chunk.SetNeighbor(dir, neighbor);
//                neighbor.SetNeighbor(-dir, chunk);
//            }
//        }
//    }

    //[System.Serializable]
    //public class Chunk
    //{
    //    public Terrain terrain;
    //    public Vector2Int coord;
    //    public Dictionary<Vector2Int, Chunk> neighbors = new Dictionary<Vector2Int, Chunk>();

    //    public Chunk(Terrain terrain, Vector2Int coord)
    //    {
    //        this.terrain = terrain;
    //        this.coord = coord;
    //    }

    //    public void SetNeighbor(Vector2Int direction, Chunk neighbor)
    //    {
    //        neighbors[direction] = neighbor;
    //        StitchEdges(neighbor, direction);
    //    }

    //    void StitchEdges(Chunk neighbor, Vector2Int direction)
    //    {
    //        int resolution = terrain.terrainData.heightmapResolution;
    //        float[,] heights = terrain.terrainData.GetHeights(0, 0, resolution, resolution);
    //        float[,] neighborHeights = neighbor.terrain.terrainData.GetHeights(0, 0, resolution, resolution);

    //        if (direction == Vector2Int.up)
    //        {
    //            for (int x = 0; x < resolution; x++)
    //            {
    //                heights[resolution - 1, x] = neighborHeights[0, x];
    //            }
    //        }
    //        else if (direction == Vector2Int.right)
    //        {
    //            for (int y = 0; y < resolution; y++)
    //            {
    //                heights[y, resolution - 1] = neighborHeights[y, 0];
    //            }
    //        }
    //        else if (direction == Vector2Int.down)
    //        {
    //            for (int x = 0; x < resolution; x++)
    //            {
    //                heights[0, x] = neighborHeights[resolution - 1, x];
    //            }
    //        }
    //        else if (direction == Vector2Int.left)
    //        {
    //            for (int y = 0; y < resolution; y++)
    //            {
    //                heights[y, 0] = neighborHeights[y, resolution - 1];
    //            }
    //        }

    //        terrain.terrainData.SetHeights(0, 0, heights);
    //    }
    //}
//}
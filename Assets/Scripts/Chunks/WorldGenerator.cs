//using UnityEngine;
//using System.Collections.Generic;
//using Unity.Mathematics;

//public class WorldGenerator : MonoBehaviour
//{
//    public int ChunkSize = 75;
//    public string Seed = "sdfkj234kja12";
//    [Range(1f, 300f)]
//    public float PerlinScale = 100f;
//    [Range(1, 100)]
//    public int PerlinOctaves = 5;
//    public float persistence = 2f;
//    public float lacunarity = 2f;
//    [Range(0.001f, 3.000f)]
//    public float PerlinBaseAmplitude = 1f;
//    [Range(1, 256)]
//    public int GrayScaleLevels = 3;

//    private System.Random pseudoRandom;
//    public List<WorldChunk> chunks = new List<WorldChunk>();

//    private void Awake()
//    {
//        pseudoRandom = new System.Random(Seed.GetHashCode());

//        if (!string.IsNullOrEmpty(Seed))
//        {
//            Seed = GenerateRandomSeed();
//        }
//    }

//    private void Update()
//    {
//        if (Input.GetKey(KeyCode.Space))
//        {
//            GenerateWorld();
//        }
//    }

//    private void GenerateWorld()
//    {
//        var fchunks = FindObjectsOfType<WorldChunk>();

//        if (fchunks.Length == 0)
//        {
//            Debug.Log("not found any chunk!");
//            return;
//        }

//        Debug.Log("Chunks found!");

//        foreach (var fchunk in fchunks)
//        {
//            GenerateChunkData(fchunk);
//        }
//    }

//    private void GenerateChunkData(WorldChunk chunk)
//    {
//        float maxNoiseHeight = float.MinValue;
//        float minNoiseHeight = float.MaxValue;

//        for (int x = 0; x <= ChunkSize; x++)
//        {
//            for (int y = 0; y <= ChunkSize; y++)
//            {
//                float noiseHeight = GeneratePerlinNoise(chunk.CellPosition(x, y), out float currentAmplitude);

//                if (noiseHeight > maxNoiseHeight)
//                {
//                    maxNoiseHeight = noiseHeight;
//                }
//                if (noiseHeight < minNoiseHeight)
//                {
//                    minNoiseHeight = noiseHeight;
//                }

//                chunk.Sample[x, y] = noiseHeight;
//            }
//        }

//        for (int x = 0; x <= ChunkSize; x++)
//        {
//            for (int y = 0; y <= ChunkSize; y++)
//            {
//                float normalizedHeight = (chunk.Sample[x, y] - minNoiseHeight) / (maxNoiseHeight - minNoiseHeight);
//                float grayScaleStep = 1.0f / (GrayScaleLevels - 1);
//                normalizedHeight = Mathf.Round(normalizedHeight / grayScaleStep) * grayScaleStep;
//                chunk.Sample[x, y] = Mathf.Clamp(normalizedHeight, 0, 1);
//            }
//        }

//        chunk.SetHeight();
//    }

//    private float GeneratePerlinNoise(int2 position, out float amplitude)
//    {
//        amplitude = PerlinBaseAmplitude;
//        float noiseHeight = 0;
//        float frequency = 1;

//        for (int i = 0; i < PerlinOctaves; i++)
//        {
//            float px = (float)position.x / PerlinScale * frequency;
//            float py = (float)position.y / PerlinScale * frequency;
//            float perlinValue = Mathf.PerlinNoise(px, py) * 2 - 1;
//            noiseHeight += perlinValue * amplitude;

//            amplitude *= persistence;
//            frequency *= lacunarity;
//        }

//        return noiseHeight;
//    }

//    private string GenerateRandomSeed()
//    {
//        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
//        char[] stringChars = new char[10];
//        for (int i = 0; i < stringChars.Length; i++)
//        {
//            stringChars[i] = chars[pseudoRandom.Next(chars.Length)];
//        }
//        return new string(stringChars);
//    }
//}
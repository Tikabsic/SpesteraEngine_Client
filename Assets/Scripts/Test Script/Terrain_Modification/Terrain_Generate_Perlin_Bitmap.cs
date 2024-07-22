using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Terrain_Generate_Perlin_Bitmap : MonoBehaviour
{
    public int textureWidth = 256;
    public int textureHeight = 256;
    public float scale = 20f;
    public int seed = 0;
    public int numberOfShades;
    public float heightInterval;

    private float offsetX;
    private float offsetY;

    void Start()
    {
        GenerateOffsets();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.H))
        {
            GenerateOffsets();
        }

        if (Input.GetKey(KeyCode.G))
        {
            Texture2D texture = GeneratePerlinBitmap();
            SaveTextureToFile(texture, "Map_Data/detailmap.png");
        }
    }

    void GenerateOffsets()
    {
        int randomseed = Random.Range(0, 999999999);
        System.Random prng = new System.Random(randomseed);
        seed = randomseed;
        offsetX = prng.Next(0, 10000);
        offsetY = prng.Next(0, 10000);
    }

    Texture2D GeneratePerlinBitmap()
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight);

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                float xCoord = (float)x / textureWidth * scale + offsetX;
                float yCoord = (float)y / textureHeight * scale + offsetY;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                float shadeValue = sample * numberOfShades;

                shadeValue = Mathf.Clamp01(shadeValue / numberOfShades);

                float height = shadeValue * heightInterval;

                Color color = new Color(height, height, height);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = texture;

        return texture;
    }

    void SaveTextureToFile(Texture2D texture, string fileName)
    {
        byte[] bytes = texture.EncodeToPNG();
        string filePath = Path.Combine(Application.dataPath, fileName);
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Texture saved to " + filePath);
    }
}

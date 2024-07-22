using System;
using System.IO;
using UnityEngine;

public class Terrain_Seamless_Heightmap_Generator : MonoBehaviour
{
    public int cellAmount = 20;
    public Vector2 period = new Vector2(5f, 10f);
    public string savePath = "Assets/Map_Data/heightmap.png";
    public int seed = 0;
    public int numberOfShades = 10;
    public float heightInterval = 0.1f;
    public int hsize = 0;
    public int wsize = 0;


    private System.Random prng;

    public void Start()
    {
        Debug.Log("start");
        GenerateNoiseMap(hsize, wsize);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Alpha0))
        {
            GenerateNoiseMap(hsize, wsize);
        }
    }

    public void GenerateNoiseMap(int width, int height)
    {
        prng = new System.Random(seed);

        float[,] noiseMap = new float[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 uv = new Vector2((float)x / width, (float)y / height);
                float noiseValue = SeamlessNoise(uv, period);
                float shadeValue = Mathf.Round(noiseValue * numberOfShades) / numberOfShades;

                float heightValue = shadeValue * heightInterval;

                noiseMap[x, y] = heightValue;
            }
        }
        SaveNoiseMapToPNG(noiseMap, width, height, savePath);
    }

    private Vector2 Modulo(Vector2 divident, Vector2 divisor)
    {
        Vector2 positiveDivident = new Vector2(
            (divident.x % divisor.x + divisor.x) % divisor.x,
            (divident.y % divisor.y + divisor.y) % divisor.y
        );
        return positiveDivident;
    }

    private Vector2 Random(Vector2 value)
    {
        value = new Vector2(
            Vector2.Dot(value, new Vector2(127.1f, 311.7f)),
            Vector2.Dot(value, new Vector2(269.5f, 183.3f))
        );

        value = new Vector2(
            Mathf.Sin(value.x + seed) * 43758.5453123f % 1.0f,
            Mathf.Sin(value.y + seed) * 43758.5453123f % 1.0f
        );
        return value * 2.0f - Vector2.one;
    }

    private float SeamlessNoise(Vector2 uv, Vector2 _period)
    {
        uv *= cellAmount;
        Vector2 cellsMinimum = new Vector2(Mathf.Floor(uv.x), Mathf.Floor(uv.y));
        Vector2 cellsMaximum = new Vector2(Mathf.Ceil(uv.x), Mathf.Ceil(uv.y));
        Vector2 uv_fract = new Vector2(uv.x - Mathf.Floor(uv.x), uv.y - Mathf.Floor(uv.y));

        cellsMinimum = Modulo(cellsMinimum, _period);
        cellsMaximum = Modulo(cellsMaximum, _period);

        Vector2 blur = new Vector2(Mathf.SmoothStep(0.0f, 1.0f, uv_fract.x), Mathf.SmoothStep(0.0f, 1.0f, uv_fract.y));

        Vector2 lowerLeftDirection = Random(new Vector2(cellsMinimum.x, cellsMinimum.y));
        Vector2 lowerRightDirection = Random(new Vector2(cellsMaximum.x, cellsMinimum.y));
        Vector2 upperLeftDirection = Random(new Vector2(cellsMinimum.x, cellsMaximum.y));
        Vector2 upperRightDirection = Random(new Vector2(cellsMaximum.x, cellsMaximum.y));

        Vector2 fraction = new Vector2(uv.x - Mathf.Floor(uv.x), uv.y - Mathf.Floor(uv.y));

        float value = Mathf.Lerp(
            Mathf.Lerp(Vector2.Dot(lowerLeftDirection, fraction - new Vector2(0, 0)), Vector2.Dot(lowerRightDirection, fraction - new Vector2(1, 0)), blur.x),
            Mathf.Lerp(Vector2.Dot(upperLeftDirection, fraction - new Vector2(0, 1)), Vector2.Dot(upperRightDirection, fraction - new Vector2(1, 1)), blur.x),
            blur.y
        ) * 0.8f + 0.5f;

        return value;
    }

    public void SaveNoiseMapToPNG(float[,] noiseMap, int width, int height, string path)
    {
        Texture2D texture = new Texture2D(width, height);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float value = noiseMap[x, y];
                texture.SetPixel(x, y, new Color(value, value, value));
            }
        }
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        Debug.Log($"Noise map saved to {path}");
    }
}
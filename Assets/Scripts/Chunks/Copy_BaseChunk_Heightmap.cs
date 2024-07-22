using UnityEngine;

public class TerrainMirrorNeighbourSynchronizer : MonoBehaviour
{
    public Terrain mainTerrain;
    public Terrain leftNeighbour;
    public Terrain rightNeighbour;
    public Terrain topNeighbour;
    public Terrain bottomNeighbour;

    private void Start()
    {
        SyncNeighbours();
    }

    private void SyncNeighbours()
    {
        if (mainTerrain == null) return;

        int resolution = mainTerrain.terrainData.heightmapResolution;

        if (leftNeighbour != null)
        {
            MirrorHeightmapEdge(mainTerrain, leftNeighbour, Vector2.left, resolution);
        }
        if (rightNeighbour != null)
        {
            MirrorHeightmapEdge(mainTerrain, rightNeighbour, Vector2.right, resolution);
        }
        if (topNeighbour != null)
        {
            MirrorHeightmapEdge(mainTerrain, topNeighbour, Vector2.up, resolution);
        }
        if (bottomNeighbour != null)
        {
            MirrorHeightmapEdge(mainTerrain, bottomNeighbour, Vector2.down, resolution);
        }
    }

    private void MirrorHeightmapEdge(Terrain source, Terrain target, Vector2 direction, int resolution)
    {
        float[,] sourceHeights = source.terrainData.GetHeights(0, 0, resolution, resolution);
        float[,] targetHeights = target.terrainData.GetHeights(0, 0, resolution, resolution);

        if (direction == Vector2.left)
        {
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    targetHeights[y, x] = sourceHeights[y, resolution - 1 - x];
                }
            }
        }
        else if (direction == Vector2.right)
        {
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    targetHeights[y, x] = sourceHeights[y, x];
                }
            }
        }
        else if (direction == Vector2.up)
        {
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    targetHeights[y, x] = sourceHeights[resolution - 1 - y, x];
                }
            }
        }
        else if (direction == Vector2.down)
        {
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    targetHeights[y, x] = sourceHeights[y, x];
                }
            }
        }

        target.terrainData.SetHeights(0, 0, targetHeights);
    }
}
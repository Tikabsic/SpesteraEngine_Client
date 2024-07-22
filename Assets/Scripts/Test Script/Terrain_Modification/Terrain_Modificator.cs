using UnityEngine;

public class Terrain_Modificator : MonoBehaviour
{
    private Terrain mainTerrain;
    public bool showVertices;
    [SerializeField] private float heightDelta = 0.1f;
    [SerializeField] private int brushSize = 1;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ModifyTerrainHeight();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            showVertices = !showVertices;
            ToggleVertexDebug(showVertices);
        }
    }

    void ModifyTerrainHeight()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            mainTerrain = hit.collider.gameObject.GetComponent<Terrain>();
            if (mainTerrain != null)
            {
                Vector3 terrainLocalPosition = hit.point - mainTerrain.transform.position;

                TerrainData terrainData = mainTerrain.terrainData;
                Vector3 terrainSize = terrainData.size;

                int mapXMain = (int)((terrainLocalPosition.x / terrainSize.x) * terrainData.heightmapResolution);
                int mapZMain = (int)((terrainLocalPosition.z / terrainSize.z) * terrainData.heightmapResolution);

                int startX = Mathf.Clamp(mapXMain - brushSize, 0, terrainData.heightmapResolution - 1);
                int endX = Mathf.Clamp(mapXMain + brushSize, 0, terrainData.heightmapResolution - 1);
                int startZ = Mathf.Clamp(mapZMain - brushSize, 0, terrainData.heightmapResolution - 1);
                int endZ = Mathf.Clamp(mapZMain + brushSize, 0, terrainData.heightmapResolution - 1);

                float[,] heightsMain = terrainData.GetHeights(startX, startZ, endX - startX + 1, endZ - startZ + 1);


                for (int z = 0; z <= endZ - startZ; z++)
                {
                    for (int x = 0; x <= endX - startX; x++)
                    {
                        heightsMain[z, x] += heightDelta;

                        heightsMain[z, x] = Mathf.Clamp01(heightsMain[z, x]);
                    }
                }


                terrainData.SetHeights(startX, startZ, heightsMain);
            }
        }
    }

    void ToggleVertexDebug(bool enable)
    {
        TerrainCollider terrainCollider = mainTerrain.GetComponent<TerrainCollider>();
        if (terrainCollider != null)
        {
            terrainCollider.enabled = !enable;
        }
    }
}
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class Terrain_SaveData : MonoBehaviour
{
    [SerializeField] private Terrain _map;
    [SerializeField] private string _filePath;
    [SerializeField] private string _staticObjectsTag;
    [SerializeField] private List<Net_MapObject> _net_MapObjects = new List<Net_MapObject>();

    // Struktura musi mieæ publiczne pola, aby by³y dostêpne
    private struct Net_MapObject
    {
        public int Id;
        public Vector3 Position;
        public float ColliderSize;
    }

    private struct MapData
    {
        public int MapSize;
        public int MapHeight;
        public int ChunkSize;
        public List<Net_MapObject> MapObjects;
    }

    void Start()
    {
        var objects = GameObject.FindGameObjectsWithTag(_staticObjectsTag);
        if (objects.Length > 0)
        {
            foreach (var obj in objects)
            {
                Net_MapObject net_object = new Net_MapObject()
                {
                    Id = obj.GetComponent<Net_ObjectData>().GetObjectId(),
                    Position = obj.transform.position,
                    ColliderSize = obj.GetComponent<Net_ObjectData>().GetColliderSize()
                };
                _net_MapObjects.Add(net_object);
            }
            Debug.Log($"found {_net_MapObjects.Count} static objects.");

            SaveMapFile();
        }


    }


    void SaveMapFile()
    {
        MapData mapData = new MapData()
        {
            MapSize = (int)_map.terrainData.size.x,
            MapHeight = (int)_map.terrainData.size.y,
            ChunkSize = 250,
            MapObjects = _net_MapObjects
        };

        using (FileStream mapbin = new FileStream(_filePath, FileMode.Create))
        using (BinaryWriter writer = new BinaryWriter(mapbin))
        {
            writer.Write(mapData.MapSize);
            writer.Write(mapData.MapHeight);
            writer.Write(mapData.ChunkSize);

            writer.Write(mapData.MapObjects.Count);
            foreach (var staticObject in mapData.MapObjects)
            {
                writer.Write(staticObject.Id);
                writer.Write(staticObject.Position.x);
                writer.Write(staticObject.Position.y);
                writer.Write(staticObject.Position.z);
                writer.Write(staticObject.ColliderSize);
            }
        }

        Debug.Log("Map data saved to " + _filePath);
    }
}
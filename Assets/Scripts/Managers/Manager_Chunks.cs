
//using System.Collections.Generic;
//using UnityEngine;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//[ExecuteInEditMode]
//public class Manager_Chunks : MonoBehaviour
//{
//    public List<Terrain> Chunks = new List<Terrain>();

//    private void Start()
//    {
//        RenameChunks();
//    }

//    private void RenameChunks()
//    {
//        var fchunks = GetComponentsInChildren<Terrain>();

//        for (int i = 0; i < fchunks.Length; i++)
//        {
//            fchunks[i].name = $"Chunk_{i + 1}";
//#if UNITY_EDITOR
//            EditorUtility.SetDirty(fchunks[i].gameObject);
//#endif
//        }

//#if UNITY_EDITOR
//        EditorUtility.SetDirty(this.gameObject);
//        AssetDatabase.SaveAssets();
//#endif
//    }
//}

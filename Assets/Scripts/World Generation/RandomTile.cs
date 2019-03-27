using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomTile : TileBase {
    public List<Sprite> possibilities;    
    //public static int?[,] indicies = new int?[World.width,World.height];

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        //if (indicies[position.x, position.y] == null) indicies[position.x, position.y] = Random.Range(0, possibilities.Count);

        tileData.sprite = possibilities[Random.Range(0, possibilities.Count)];        
    }
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        tilemap.RefreshTile(position);
    }

#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create an Asset
    [MenuItem("Assets/Create/RandomTile")]
    public static void CreateRandomTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Random Tile", "New Random Tile", "Asset", "Save Random Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<RandomTile>(), path);
    }
#endif
}

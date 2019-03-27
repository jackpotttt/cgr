using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SingleTile : TileBase
{
    public Sprite sprite;    

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = sprite;
    }
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        tilemap.RefreshTile(position);
    }
}

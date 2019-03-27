using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoadTile : TileBase {

    public Sprite spriteUD;
    public Sprite spriteLR;
    public Sprite spriteUL;
    public Sprite spriteUR;
    public Sprite spriteDL;
    public Sprite spriteDR;
    public Sprite spriteUDL;
    public Sprite spriteUDR;
    public Sprite spriteULR;
    public Sprite spriteDLR;
    public Sprite spriteUDLR;
    public Sprite spriteUDW;
    public Sprite spriteLRW;

    public int id;
    public int roadClass;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = GetSprite(position, tilemap);
    }

    public Sprite GetSprite(Vector3Int position, ITilemap tilemap)
    {
        if ((tilemap.GetTile(position + Vector3Int.up) as RoadTile)?.id == this.id)
        {
            if ((tilemap.GetTile(position + Vector3Int.down) as RoadTile)?.id == this.id)
            {
                if ((tilemap.GetTile(position + Vector3Int.left) as RoadTile)?.id == this.id)
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) as RoadTile)?.id == this.id)
                    {
                        return spriteUDLR;
                    }
                    else
                    {
                        return spriteUDL;
                    }
                }
                else
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) as RoadTile)?.id == this.id)
                    {
                        return spriteUDR;
                    }
                    else
                    {
                        if (tilemap.GetTile(position + Vector3Int.left) is RiverTile && tilemap.GetTile(position + Vector3Int.right) is RiverTile)
                            return spriteUDW;
                        else
                            return spriteUD;
                    }
                }
            }
            else
            {
                if ((tilemap.GetTile(position + Vector3Int.left) as RoadTile)?.id == this.id)
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) as RoadTile)?.id == this.id)
                    {
                        return spriteULR;
                    }
                    else
                    {
                        return spriteUL;
                    }
                }
                else
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) as RoadTile)?.id == this.id)
                    {
                        return spriteUR;
                    }
                    else
                    {
                        if (tilemap.GetTile(position + Vector3Int.left) is RiverTile && tilemap.GetTile(position + Vector3Int.right) is RiverTile)
                            return spriteUDW;
                        else
                            return spriteUD;
                    }
                }
            }
        }
        else
        {
            if ((tilemap.GetTile(position + Vector3Int.down) as RoadTile)?.id == this.id)
            {
                if ((tilemap.GetTile(position + Vector3Int.left) as RoadTile)?.id == this.id)
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) as RoadTile)?.id == this.id)
                    {
                        return spriteDLR;
                    }
                    else
                    {
                        return spriteDL;
                    }
                }
                else
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) as RoadTile)?.id == this.id)
                    {
                        return spriteDR;
                    }
                    else
                    {
                        if (tilemap.GetTile(position + Vector3Int.left) is RiverTile && tilemap.GetTile(position + Vector3Int.right) is RiverTile)
                            return spriteUDW;
                        else
                            return spriteUD;
                    }
                }
            }
            else
            {
                if (tilemap.GetTile(position + Vector3Int.up) is RiverTile && tilemap.GetTile(position + Vector3Int.down) is RiverTile)
                    return spriteLRW;
                else
                    return spriteLR;
            }
        }
    }

    public Sprite GetSprite(Vector3Int position, Tilemap tilemap)
    {
        if ((tilemap.GetTile(position + Vector3Int.up) as RoadTile)?.id == this.id)
        {
            if ((tilemap.GetTile(position + Vector3Int.down) as RoadTile)?.id == this.id)
            {
                if ((tilemap.GetTile(position + Vector3Int.left) as RoadTile)?.id == this.id)
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) as RoadTile)?.id == this.id)
                    {
                        return spriteUDLR;
                    }
                    else
                    {
                        return spriteUDL;
                    }
                }
                else
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) as RoadTile)?.id == this.id)
                    {
                        return spriteUDR;
                    }
                    else
                    {
                        if (tilemap.GetTile(position + Vector3Int.left) is RiverTile && tilemap.GetTile(position + Vector3Int.right) is RiverTile)
                            return spriteUDW;
                        else
                            return spriteUD;
                    }
                }
            }
            else
            {
                if ((tilemap.GetTile(position + Vector3Int.left) as RoadTile)?.id == this.id)
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) as RoadTile)?.id == this.id)
                    {
                        return spriteULR;
                    }
                    else
                    {
                        return spriteUL;
                    }
                }
                else
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) as RoadTile)?.id == this.id)
                    {
                        return spriteUR;
                    }
                    else
                    {
                        if (tilemap.GetTile(position + Vector3Int.left) is RiverTile && tilemap.GetTile(position + Vector3Int.right) is RiverTile)
                            return spriteUDW;
                        else
                            return spriteUD;
                    }
                }
            }
        }
        else
        {
            if ((tilemap.GetTile(position + Vector3Int.down) as RoadTile)?.id == this.id)
            {
                if ((tilemap.GetTile(position + Vector3Int.left) as RoadTile)?.id == this.id)
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) as RoadTile)?.id == this.id)
                    {
                        return spriteDLR;
                    }
                    else
                    {
                        return spriteDL;
                    }
                }
                else
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) as RoadTile)?.id == this.id)
                    {
                        return spriteDR;
                    }
                    else
                    {
                        if (tilemap.GetTile(position + Vector3Int.left) is RiverTile && tilemap.GetTile(position + Vector3Int.right) is RiverTile)
                            return spriteUDW;
                        else
                            return spriteUD;
                    }
                }
            }
            else
            {
                if (tilemap.GetTile(position + Vector3Int.up) is RiverTile && tilemap.GetTile(position + Vector3Int.down) is RiverTile)
                    return spriteLRW;
                else
                    return spriteLR;
            }
        }
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        tilemap.RefreshTile(position);

        var up = position + Vector3Int.up;        
        tilemap.RefreshTile(up);

        var down = position + Vector3Int.down;        
        tilemap.RefreshTile(down);

        var left = position + Vector3Int.left;        
        tilemap.RefreshTile(left);

        var right = position + Vector3Int.right;
        tilemap.RefreshTile(right);
    }


#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a RoadTile Asset
    [MenuItem("Assets/Create/RoadTile")]
    public static void CreateRoadTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Road Tile", "New Road Tile", "Asset", "Save Road Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<RoadTile>(), path);
    }
#endif
}

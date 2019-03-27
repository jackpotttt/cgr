using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RiverTile : TileBase
{
    //end
    public Sprite spriteU;
    public Sprite spriteD;
    public Sprite spriteL;
    public Sprite spriteR;



    //straight
    public Sprite spriteUD;
    public Sprite spriteLR;



    //turns
    public Sprite spriteUL;
    public Sprite spriteUR;
    public Sprite spriteDL;
    public Sprite spriteDR;

    public Sprite spriteULW;
    public Sprite spriteURN;
    public Sprite spriteDLS;
    public Sprite spriteDRE;



    //fork
    public Sprite spriteUDL;
    public Sprite spriteUDR;
    public Sprite spriteULR;
    public Sprite spriteDLR;

    public Sprite spriteUDLS;
    public Sprite spriteUDLW;
    public Sprite spriteUDLSW;

    public Sprite spriteUDRN;
    public Sprite spriteUDRE;
    public Sprite spriteUDRNE;

    public Sprite spriteULRN;
    public Sprite spriteULRW;
    public Sprite spriteULRNW;

    public Sprite spriteDLRS;
    public Sprite spriteDLRE;
    public Sprite spriteDLRSE;



    //branch
    public Sprite spriteUDLR;

    public Sprite spriteUDLRN;
    public Sprite spriteUDLRS;
    public Sprite spriteUDLRE;
    public Sprite spriteUDLRW;

    public Sprite spriteUDLRNS;
    public Sprite spriteUDLREW;
    public Sprite spriteUDLRNE;
    public Sprite spriteUDLRNW;
    public Sprite spriteUDLRSE;
    public Sprite spriteUDLRSW;

    public Sprite spriteUDLRNSE;
    public Sprite spriteUDLRNSW;
    public Sprite spriteUDLRNEW;
    public Sprite spriteUDLRSEW;
    public Sprite spriteUDLRNSEW;

    public bool hasBridge(ITilemap tilemap, Vector3Int pos, Vector3Int dir)
    {
        return (tilemap.GetTile(pos + dir) is RoadTile) && (tilemap.GetTile(pos + dir + dir) is RiverTile);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if ((tilemap.GetTile(position + Vector3Int.up) is RiverTile) || hasBridge(tilemap, position, Vector3Int.up))
        {
            if ((tilemap.GetTile(position + Vector3Int.down) is RiverTile) || hasBridge(tilemap, position, Vector3Int.down))
            {
                if ((tilemap.GetTile(position + Vector3Int.left) is RiverTile) || hasBridge(tilemap, position, Vector3Int.left))
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) is RiverTile) || hasBridge(tilemap, position, Vector3Int.right))
                    {
                        if (tilemap.GetTile(position + Vector3Int.up + Vector3Int.right) is RiverTile && !hasBridge(tilemap, position, Vector3Int.up) && !hasBridge(tilemap, position, Vector3Int.right))
                        {
                            if (tilemap.GetTile(position + Vector3Int.down + Vector3Int.left) is RiverTile && !hasBridge(tilemap, position, Vector3Int.down) && !hasBridge(tilemap, position, Vector3Int.left))
                            {
                                if (tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) is RiverTile && !hasBridge(tilemap, position, Vector3Int.up) && !hasBridge(tilemap, position, Vector3Int.left))
                                {
                                    if (tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) is RiverTile && !hasBridge(tilemap, position, Vector3Int.down) && !hasBridge(tilemap, position, Vector3Int.right))
                                    {
                                        tileData.sprite = spriteUDLRNSEW;
                                    }
                                    else
                                    {
                                        tileData.sprite = spriteUDLRNSW;
                                    }
                                }
                                else
                                {
                                    if (tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) is RiverTile && !hasBridge(tilemap, position, Vector3Int.down) && !hasBridge(tilemap, position, Vector3Int.right))
                                    {
                                        tileData.sprite = spriteUDLRNSE;
                                    }
                                    else
                                    {
                                        tileData.sprite = spriteUDLRNS;
                                    }
                                }
                            }
                            else
                            {
                                if (tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) is RiverTile && !hasBridge(tilemap, position, Vector3Int.up) && !hasBridge(tilemap, position, Vector3Int.left))
                                {
                                    if (tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) is RiverTile && !hasBridge(tilemap, position, Vector3Int.down) && !hasBridge(tilemap, position, Vector3Int.right))
                                    {
                                        tileData.sprite = spriteUDLRNEW;
                                    }
                                    else
                                    {
                                        tileData.sprite = spriteUDLRNW;
                                    }
                                }
                                else
                                {
                                    if (tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) is RiverTile && !hasBridge(tilemap, position, Vector3Int.down) && !hasBridge(tilemap, position, Vector3Int.right))
                                    {
                                        tileData.sprite = spriteUDLRNE;
                                    }
                                    else
                                    {
                                        tileData.sprite = spriteUDLRN;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (tilemap.GetTile(position + Vector3Int.down + Vector3Int.left) is RiverTile && !hasBridge(tilemap, position, Vector3Int.down) && !hasBridge(tilemap, position, Vector3Int.left))
                            {
                                if (tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) is RiverTile && !hasBridge(tilemap, position, Vector3Int.up) && !hasBridge(tilemap, position, Vector3Int.left))
                                {
                                    if (tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) is RiverTile && !hasBridge(tilemap, position, Vector3Int.down) && !hasBridge(tilemap, position, Vector3Int.right))
                                    {
                                        tileData.sprite = spriteUDLRSEW;
                                    }
                                    else
                                    {
                                        tileData.sprite = spriteUDLRSW;
                                    }
                                }
                                else
                                {
                                    if (tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) is RiverTile && !hasBridge(tilemap, position, Vector3Int.down) && !hasBridge(tilemap, position, Vector3Int.right))
                                    {
                                        tileData.sprite = spriteUDLRSE;
                                    }
                                    else
                                    {
                                        tileData.sprite = spriteUDLRS;
                                    }
                                }
                            }
                            else
                            {
                                if (tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) is RiverTile && !hasBridge(tilemap, position, Vector3Int.up) && !hasBridge(tilemap, position, Vector3Int.left))
                                {
                                    if (tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) is RiverTile && !hasBridge(tilemap, position, Vector3Int.down) && !hasBridge(tilemap, position, Vector3Int.right))
                                    {
                                        tileData.sprite = spriteUDLREW;
                                    }
                                    else
                                    {
                                        tileData.sprite = spriteUDLRW;
                                    }
                                }
                                else
                                {
                                    if (tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) is RiverTile && !hasBridge(tilemap, position, Vector3Int.down) && !hasBridge(tilemap, position, Vector3Int.right))
                                    {
                                        tileData.sprite = spriteUDLRE;
                                    }
                                    else
                                    {
                                        tileData.sprite = spriteUDLR;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (tilemap.GetTile(position + Vector3Int.down + Vector3Int.left) is RiverTile && !hasBridge(tilemap, position, Vector3Int.down) && !hasBridge(tilemap, position, Vector3Int.left))
                        {
                            if (tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) is RiverTile && !hasBridge(tilemap, position, Vector3Int.up) && !hasBridge(tilemap, position, Vector3Int.left))
                            {
                                tileData.sprite = spriteUDLSW;
                            }
                            else
                            {
                                tileData.sprite = spriteUDLS;
                            }
                        }
                        else
                        {
                            if (tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) is RiverTile && !hasBridge(tilemap, position, Vector3Int.up) && !hasBridge(tilemap, position, Vector3Int.left))
                            {
                                tileData.sprite = spriteUDLW;
                            }
                            else
                            {
                                tileData.sprite = spriteUDL;
                            }
                        }
                    }
                }
                else
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) is RiverTile) || hasBridge(tilemap, position, Vector3Int.right))
                    {
                        if (tilemap.GetTile(position + Vector3Int.up + Vector3Int.right) is RiverTile && !hasBridge(tilemap, position, Vector3Int.up) && !hasBridge(tilemap, position, Vector3Int.right))
                        {
                            if (tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) is RiverTile && !hasBridge(tilemap, position, Vector3Int.down) && !hasBridge(tilemap, position, Vector3Int.right))
                            {
                                tileData.sprite = spriteUDRNE;
                            }
                            else
                            {
                                tileData.sprite = spriteUDRN;
                            }
                        }
                        else
                        {
                            if (tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) is RiverTile && !hasBridge(tilemap, position, Vector3Int.down) && !hasBridge(tilemap, position, Vector3Int.right))
                            {
                                tileData.sprite = spriteUDRE;
                            }
                            else
                            {
                                tileData.sprite = spriteUDR;
                            }
                        }
                    }
                    else
                    {
                        tileData.sprite = spriteUD;
                    }
                }
            }
            else
            {
                if ((tilemap.GetTile(position + Vector3Int.left) is RiverTile) || hasBridge(tilemap, position, Vector3Int.left))
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) is RiverTile) || hasBridge(tilemap, position, Vector3Int.right))
                    {
                        if (tilemap.GetTile(position + Vector3Int.up + Vector3Int.right) is RiverTile && !hasBridge(tilemap, position, Vector3Int.up) && !hasBridge(tilemap, position, Vector3Int.right))
                        {
                            if (tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) is RiverTile && !hasBridge(tilemap, position, Vector3Int.up) && !hasBridge(tilemap, position, Vector3Int.left))
                            {
                                tileData.sprite = spriteULRNW;
                            }
                            else
                            {
                                tileData.sprite = spriteULRN;
                            }
                        }
                        else
                        {
                            if (tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) is RiverTile && !hasBridge(tilemap, position, Vector3Int.up) && !hasBridge(tilemap, position, Vector3Int.left))
                            {
                                tileData.sprite = spriteULRW;
                            }
                            else
                            {
                                tileData.sprite = spriteULR;
                            }
                        }
                    }
                    else
                    {
                        if (tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) is RiverTile && !hasBridge(tilemap, position, Vector3Int.up) && !hasBridge(tilemap, position, Vector3Int.left))
                        {
                            tileData.sprite = spriteULW;
                        }
                        else
                        {
                            tileData.sprite = spriteUL;
                        }
                    }
                }
                else
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) is RiverTile) || hasBridge(tilemap, position, Vector3Int.right))
                    {
                        if (tilemap.GetTile(position + Vector3Int.up + Vector3Int.right) is RiverTile && !hasBridge(tilemap, position, Vector3Int.up) && !hasBridge(tilemap, position, Vector3Int.right))
                        {
                            tileData.sprite = spriteURN;
                        }
                        else
                        {
                            tileData.sprite = spriteUR;
                        }
                    }
                    else
                    {
                        tileData.sprite = spriteU;
                    }
                }
            }
        }
        else
        {
            if ((tilemap.GetTile(position + Vector3Int.down) is RiverTile) || hasBridge(tilemap, position, Vector3Int.down))
            {
                if ((tilemap.GetTile(position + Vector3Int.left) is RiverTile) || hasBridge(tilemap, position, Vector3Int.left))
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) is RiverTile) || hasBridge(tilemap, position, Vector3Int.right))
                    {
                        if (tilemap.GetTile(position + Vector3Int.down + Vector3Int.left) is RiverTile && !hasBridge(tilemap, position, Vector3Int.down) && !hasBridge(tilemap, position, Vector3Int.left))
                        {
                            if (tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) is RiverTile && !hasBridge(tilemap, position, Vector3Int.down) && !hasBridge(tilemap, position, Vector3Int.right))
                            {
                                tileData.sprite = spriteDLRSE;
                            }
                            else
                            {
                                tileData.sprite = spriteDLRS;
                            }
                        }
                        else
                        {
                            if (tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) is RiverTile && !hasBridge(tilemap, position, Vector3Int.down) && !hasBridge(tilemap, position, Vector3Int.right))
                            {
                                tileData.sprite = spriteDLRE;
                            }
                            else
                            {
                                tileData.sprite = spriteDLR;
                            }
                        }
                    }
                    else
                    {
                        if (tilemap.GetTile(position + Vector3Int.down + Vector3Int.left) is RiverTile && !hasBridge(tilemap, position, Vector3Int.down) && !hasBridge(tilemap, position, Vector3Int.left))
                        {
                            tileData.sprite = spriteDLS;
                        }
                        else
                        {
                            tileData.sprite = spriteDL;
                        }
                    }
                }
                else
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) is RiverTile) || hasBridge(tilemap, position, Vector3Int.right))
                    {
                        if (tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) is RiverTile && !hasBridge(tilemap, position, Vector3Int.down) && !hasBridge(tilemap, position, Vector3Int.right))
                        {
                            tileData.sprite = spriteDRE;
                        }
                        else
                        {
                            tileData.sprite = spriteDR;
                        }                        
                    }
                    else
                    {
                        tileData.sprite = spriteD;
                    }
                }
            }
            else
            {
                if ((tilemap.GetTile(position + Vector3Int.left) is RiverTile) || hasBridge(tilemap, position, Vector3Int.left))
                {                    
                    if ((tilemap.GetTile(position + Vector3Int.right) is RiverTile) || hasBridge(tilemap, position, Vector3Int.right))
                    {
                        tileData.sprite = spriteLR;
                    }
                    else
                    {
                        tileData.sprite = spriteL;
                    }
                }
                else
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) is RiverTile) || hasBridge(tilemap, position, Vector3Int.right))
                    {
                        tileData.sprite = spriteR;
                    }
                    else
                    {
                        tileData.sprite = spriteUD;
                    }
                }
            }
        }
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        tilemap.RefreshTile(position);

        var up = position + Vector3Int.up;
        tilemap.RefreshTile(up);
        tilemap.RefreshTile(up + Vector3Int.left);
        tilemap.RefreshTile(up + Vector3Int.right);

        var down = position + Vector3Int.down;
        tilemap.RefreshTile(down);
        tilemap.RefreshTile(down + Vector3Int.left);
        tilemap.RefreshTile(down + Vector3Int.right);

        var left = position + Vector3Int.left;
        tilemap.RefreshTile(left);

        var right = position + Vector3Int.right;
        tilemap.RefreshTile(right);
    }

#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a RoadTile Asset
    [MenuItem("Assets/Create/RiverTile")]
    public static void CreateRiverTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save River Tile", "New River Tile", "Asset", "Save River Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<RiverTile>(), path);
    }
#endif
}

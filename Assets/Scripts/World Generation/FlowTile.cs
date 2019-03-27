using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FlowTile : TileBase {

    public Sprite spriteN;

    public Sprite spriteU;
    public Sprite spriteD;
    public Sprite spriteL;
    public Sprite spriteR;

    public Sprite spriteUD;
    public Sprite spriteLR;

    public Sprite spriteUL;
    public Sprite spriteUR;
    public Sprite spriteDL;
    public Sprite spriteDR;
    public Sprite spriteULC;
    public Sprite spriteURC;
    public Sprite spriteDLC;
    public Sprite spriteDRC;

    public Sprite spriteUDL;
    public Sprite spriteUDLS;
    public Sprite spriteUDLW;
    public Sprite spriteUDLSW;
    public Sprite spriteUDR;
    public Sprite spriteUDRN;
    public Sprite spriteUDRE;
    public Sprite spriteUDRNE;
    public Sprite spriteULR;
    public Sprite spriteULRN;
    public Sprite spriteULRW;
    public Sprite spriteULRNW;
    public Sprite spriteDLR;
    public Sprite spriteDLRS;
    public Sprite spriteDLRE;
    public Sprite spriteDLRSE;

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

    public int id;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        if ((tilemap.GetTile(position + Vector3Int.up) as FlowTile)?.id == this.id)
        {
            if ((tilemap.GetTile(position + Vector3Int.down) as FlowTile)?.id == this.id)
            {
                if ((tilemap.GetTile(position + Vector3Int.left) as FlowTile)?.id == this.id)
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) as FlowTile)?.id == this.id)
                    {
                        if ((tilemap.GetTile(position + Vector3Int.up + Vector3Int.right) as FlowTile)?.id == this.id)
                        {
                            if ((tilemap.GetTile(position + Vector3Int.down + Vector3Int.left) as FlowTile)?.id == this.id)
                            {
                                if ((tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) as FlowTile)?.id == this.id)
                                {
                                    if ((tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) as FlowTile)?.id == this.id)
                                    {
                                        tileData.sprite = spriteUDLRNSEW;
                                    }
                                    else
                                    {
                                        tileData.sprite = spriteUDLRNSE;
                                    }
                                }
                                else
                                {
                                    if ((tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) as FlowTile)?.id == this.id)
                                    {
                                        tileData.sprite = spriteUDLRNSW;
                                    }
                                    else
                                    {
                                        tileData.sprite = spriteUDLRNS;
                                    }
                                }
                            }
                            else
                            {
                                if ((tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) as FlowTile)?.id == this.id)
                                {
                                    if ((tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) as FlowTile)?.id == this.id)
                                    {
                                        tileData.sprite = spriteUDLRNEW;
                                    }
                                    else
                                    {
                                        tileData.sprite = spriteUDLRNE;
                                    }
                                }
                                else
                                {
                                    if ((tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) as FlowTile)?.id == this.id)
                                    {
                                        tileData.sprite = spriteUDLRNW;
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
                            if ((tilemap.GetTile(position + Vector3Int.down + Vector3Int.left) as FlowTile)?.id == this.id)
                            {
                                if ((tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) as FlowTile)?.id == this.id)
                                {
                                    if ((tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) as FlowTile)?.id == this.id)
                                    {
                                        tileData.sprite = spriteUDLRSEW;
                                    }
                                    else
                                    {
                                        tileData.sprite = spriteUDLRSE;
                                    }
                                }
                                else
                                {
                                    if ((tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) as FlowTile)?.id == this.id)
                                    {
                                        tileData.sprite = spriteUDLRSW;
                                    }
                                    else
                                    {
                                        tileData.sprite = spriteUDLRS;
                                    }
                                }
                            }
                            else
                            {
                                if ((tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) as FlowTile)?.id == this.id)
                                {
                                    if ((tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) as FlowTile)?.id == this.id)
                                    {
                                        tileData.sprite = spriteUDLREW;
                                    }
                                    else
                                    {
                                        tileData.sprite = spriteUDLRE;
                                    }
                                }
                                else
                                {
                                    if ((tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) as FlowTile)?.id == this.id)
                                    {
                                        tileData.sprite = spriteUDLRW;
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
                        if ((tilemap.GetTile(position + Vector3Int.down + Vector3Int.left) as FlowTile)?.id == this.id)
                        {
                            if ((tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) as FlowTile)?.id == this.id)
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
                            if ((tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) as FlowTile)?.id == this.id)
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
                    if ((tilemap.GetTile(position + Vector3Int.right) as FlowTile)?.id == this.id)
                    {                        
                        if ((tilemap.GetTile(position + Vector3Int.up + Vector3Int.right) as FlowTile)?.id == this.id)
                        {
                            if ((tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) as FlowTile)?.id == this.id)
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
                            if ((tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) as FlowTile)?.id == this.id)
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
                if ((tilemap.GetTile(position + Vector3Int.left) as FlowTile)?.id == this.id)
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) as FlowTile)?.id == this.id)
                    {                        
                        if ((tilemap.GetTile(position + Vector3Int.up + Vector3Int.right) as FlowTile)?.id == this.id)
                        {
                            if ((tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) as FlowTile)?.id == this.id)
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
                            if ((tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) as FlowTile)?.id == this.id)
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
                        if ((tilemap.GetTile(position + Vector3Int.up + Vector3Int.left) as FlowTile)?.id == this.id)
                        {
                            tileData.sprite = spriteULC;
                        }
                        else
                        {
                            tileData.sprite = spriteUL;
                        }                        
                    }
                }
                else
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) as FlowTile)?.id == this.id)
                    {
                        if ((tilemap.GetTile(position + Vector3Int.up + Vector3Int.right) as FlowTile)?.id == this.id)
                        {
                            tileData.sprite = spriteURC;
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
            if ((tilemap.GetTile(position + Vector3Int.down) as FlowTile)?.id == this.id)
            {
                if ((tilemap.GetTile(position + Vector3Int.left) as FlowTile)?.id == this.id)
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) as FlowTile)?.id == this.id)
                    {
                        if ((tilemap.GetTile(position + Vector3Int.down + Vector3Int.left) as FlowTile)?.id == this.id)
                        {
                            if ((tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) as FlowTile)?.id == this.id)
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
                            if ((tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) as FlowTile)?.id == this.id)
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
                        if ((tilemap.GetTile(position + Vector3Int.down + Vector3Int.left) as FlowTile)?.id == this.id)
                        {
                            tileData.sprite = spriteDLC;
                        }
                        else
                        {
                            tileData.sprite = spriteDL;
                        }
                    }
                }
                else
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) as FlowTile)?.id == this.id)
                    {

                        if ((tilemap.GetTile(position + Vector3Int.down + Vector3Int.right) as FlowTile)?.id == this.id)
                        {
                            tileData.sprite = spriteDRC;
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
                if ((tilemap.GetTile(position + Vector3Int.left) as FlowTile)?.id == this.id)
                {
                    if ((tilemap.GetTile(position + Vector3Int.right) as FlowTile)?.id == this.id)
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
                    if ((tilemap.GetTile(position + Vector3Int.right) as FlowTile)?.id == this.id)
                    {
                        tileData.sprite = spriteR;
                    }
                    else
                    {
                        tileData.sprite = spriteN;
                    }
                }
            }
        }
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        for(int i = -1; i<2; i++)
            for (int j = -1; j < 2; j++)
                tilemap.RefreshTile(position + new Vector3Int(i,j,0));
    }


#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create an Asset
    [MenuItem("Assets/Create/FlowTile")]
    public static void CreateFlowTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Flow Tile", "New Flow Tile", "Asset", "Save Flow Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<FlowTile>(), path);
    }
#endif
}

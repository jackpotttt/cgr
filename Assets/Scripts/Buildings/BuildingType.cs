using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public enum BuildingCategory
{
    Null = 0,
    House = 1,
    Supplier = 2,
    Utility = 3,
    Restaurant = 4,
} 

[Serializable]
public struct GenerationData
{    
    public float frequency;   
    public bool requiresSmallRoad;
    public bool requiresNonSmallRoad;
    public bool requiresWater;
    public int minDistanceFromBuildings;
    public int minPastures;
    public int maxPastures;
    public bool makePasturesFields;
    public bool doesntRequireRoad;
    public bool mustBeAttachedToHouse;
}

[Serializable]
public class BuildingType {

    public string name;
    public BuildingCategory category;
    public Texture2D texture;
    public Texture2D[] textures;
    public GenerationData genData;
    public int spawnsHires;
    
    [HideInInspector]
    public int id;
    internal Sprite splash;
    internal SingleTile[,] tiles;
    internal SingleTile[,][,] tileSets;
    internal int count = 0;
    internal int height;
    internal int width;
    internal int size { get { return height * width; } }
    
    public const int MAX_WIDTH = 3;
    public const int MAX_HEIGHT = 3;

    public void Initialize(int id)
    {        
        this.id = id;
        splash = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 16);

        if (textures.Length == 0)
        {
            width = texture.width / 16;
            height = texture.height / 16;

            tiles = new SingleTile[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var s = Sprite.Create(texture, new Rect(i * 16f, j * 16f, 16, 16), new Vector2(0.5f, 0.5f), 16);
                    var tile = (SingleTile)ScriptableObject.CreateInstance(typeof(SingleTile));
                    tile.sprite = s;
                    tiles[i, j] = tile;
                }
            }
        }
        else
        {
            tiles = null;
            width = 2;
            height = 1;

            tileSets = new SingleTile[MAX_WIDTH, MAX_HEIGHT][,];

            foreach (var tex in textures)
            {
                int width = tex.width / 16;
                int height = tex.height / 16;

                tileSets[width - 1, height - 1] = new SingleTile[width, height];

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        var s = Sprite.Create(tex, new Rect(i * 16f, j * 16f, 16, 16), new Vector2(0.5f, 0.5f), 16);
                        var tile = (SingleTile)ScriptableObject.CreateInstance(typeof(SingleTile));
                        tile.sprite = s;
                        tileSets[width - 1, height - 1][i, j] = tile;
                    }
                }
            }
        }
    }

    public override string ToString()
    {
        return $"{name} - [{width} x {height}]";
    }

    public TileBase GetTile(int i, int j, Building b)
    {
        if(tiles != null)
            return tiles[i, j];
        
        return tileSets[1, 0][i, j];
    }
}

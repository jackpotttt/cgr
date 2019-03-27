using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType
{
    empty = 0,
    tree = 1,
    smallRoad = 2,
    mediumRoad = 3,
    largeRoad = 4,
    smallRoadRiver = 5,
    mediumRoadRiver = 6,
    largeRoadRiver = 7,
    river = 8,
    building = 9,
    pasture = 10
};

public struct MapObject
{
    public TileType type;
    public IBuilding building;
    public Employee employee;
}

public class World : MonoBehaviour {

    public Tilemap baseTilemap;
    public Tilemap buildingTilemap;
    public Tilemap tileSelectorOverlay;
    public Tilemap overlay;
        
    public static int width { get; private set; }
    public static int height { get; private set; }
    public static MapObject[,] map;
    public static List<Building> houses;
    public static List<Building> suppliers;
    public static List<Building> utilities;
    public static List<Building> restaurants;
    public static List<Building> allBuildings;
    public static bool ready = false;
    public static World instance;


    private void Awake()
    {
        instance = this;
    }

    void Start () {
        TileSelectorMouse.instance.tileSelectorOverlay = tileSelectorOverlay;
        TileSelectorMultipleChoice.instance.tileSelectorOverlay = tileSelectorOverlay;    

        houses = new List<Building>();
        suppliers = new List<Building>();
        utilities = new List<Building>();
        restaurants = new List<Building>();
    }

    internal static void SortBuildings()
    {
        houses.Clear();
        suppliers.Clear();
        utilities.Clear();
        foreach (var b in allBuildings)
        {
            if (b != null)
            {
                switch (b.type.category)
                {
                    case BuildingCategory.House:
                        houses.Add(b);
                        break;
                    case BuildingCategory.Supplier:
                        suppliers.Add(b);
                        break;
                    case BuildingCategory.Utility:
                        utilities.Add(b);
                        break;
                    case BuildingCategory.Restaurant:
                        restaurants.Add(b);
                        break;
                }
            }
        }
    }

    public static List<Vector2Int> GetValidPointsWithin(Vector2Int loc, int dist, bool mustBeEmpty = false)
    {
        var points = new List<Vector2Int>();
        for (int i = loc.x - dist; i <= loc.x + dist; i++)
        {
            for (int j = loc.y - dist; j <= loc.y + dist; j++)
            {
                if (withinBounds(i, j) && ((i - loc.x) * (i - loc.x)) + ((j - loc.y) * (j - loc.y)) <= dist * dist)
                {
                    if(!mustBeEmpty)
                        points.Add(new Vector2Int(i, j));
                    else
                    {
                        var mo = map[i, j];
                        if(mo.type == TileType.empty || mo.type == TileType.tree)
                        {
                            points.Add(new Vector2Int(i, j));
                        }
                    }
                }
            }
        }
        return points;
    }

    public static List<Building> GetBuildingsWithin(Vector2Int loc, int dist)
    {        
        var ps = GetValidPointsWithin(loc, dist);
        return GetBuildingsWithin(ps);
    }

    public static List<Building> GetBuildingsWithin(List<Vector2Int> points)
    {
        var buildings = new List<Building>();
        foreach (var p in points)
        {
            var b = map[p.x, p.y].building as Building;
            if (b != null) buildings.Add(b);
        }

        return buildings;
    }

    #region Utility

    internal static void Initialize(int width, int height)
    {
        World.width = width;
        World.height = height;
        map = new MapObject[width, height];
    }

    public static bool withinBounds(float x, float y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    public static bool withinBounds(Vector2Int v)
    {
        return withinBounds(v.x, v.y);
    }
    public static bool withinBounds(Vector3Int v)
    {
        return withinBounds(v.x, v.y);
    }
    public static bool withinBounds(Vector2 v)
    {
        return withinBounds(v.x, v.y);
    }
    public static bool withinBounds(Vector3 v)
    {
        return withinBounds(v.x, v.y);
    }

    internal static void FlashTile(Vector3Int errorPos, Color color, float duration, float interval)
    {        
        instance.StartCoroutine(FlashRoutine(errorPos, color, duration, interval));
    }

    private static IEnumerator FlashRoutine(Vector3Int pos, Color color, float duration, float interval)
    {
        var time = 0f;
        while (time < duration)
        {
            instance.overlay.SetTile(pos, WorldGenerator.Instance.blankTile);
            instance.overlay.SetColor(pos, color);
            yield return new WaitForSeconds(interval);
            time += interval;
            instance.overlay.SetTile(pos, null);
            instance.overlay.SetColor(pos, Color.white);
            yield return new WaitForSeconds(interval);
            time += interval;
        }
    }

    public static MapObject GetMapObject(Vector2Int v, out bool inBounds)
    {
        if (withinBounds(v))
        {
            inBounds = true;
            return map[v.x, v.y];
        }
        inBounds = false;
        return new MapObject();
    }

    #endregion
}

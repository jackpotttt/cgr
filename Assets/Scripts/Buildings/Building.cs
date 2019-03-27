using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Building : IBuilding {

    public enum PlacementResult { Success, OutOfBounds, SomethingInTheWay, NoRoad, RequiresWater, RequiresHouse };

    public static int nextId = 0;

    public int id;
    public Vector2Int origin;
    public BuildingType type;
    public List<Vector2Int> entrances = new List<Vector2Int>();
    public List<Vector2Int> borders = new List<Vector2Int>();
    public List<Vector2Int> pastures = new List<Vector2Int>();
    public List<Vector2Int> tiles = new List<Vector2Int>();
    public Color color;
    public List<Hireable> hireables = new List<Hireable>();
    public List<Employee> employees = new List<Employee>();

    internal void AddHireable(EmployeeType type)
    {
        if(hireables == null) hireables = new List<Hireable>();
        var h = GameObject.Instantiate(EmployeeManager.instance.hireablePrefab, World.instance.transform).GetComponent<Hireable>();
        h.Init(type, this, hireables.Count);
        hireables.Add(h);
        ArrangeHireables();
    }

    private void ArrangeHireables()
    {
        var spacing = 1.1f;

        if (type.height > type.width)
        {
            float i = 0f;
            foreach (var h in hireables)
            {
                var x = (float)origin.x + (type.width / 2f);
                var y = (float)origin.y + (type.height / 2f) + ((i - ((hireables.Count - 1f) / 2f)) * spacing);
                h.transform.position = new Vector3(x, y);
                i++;
            }
        }
        else
        {
            float i = 0f;
            foreach (var h in hireables)
            {
                var x = (float)origin.x + (type.width / 2f) + ((i - ((hireables.Count - 1f) / 2f)) * spacing);
                var y = (float)origin.y + (type.height / 2f);
                h.transform.position = new Vector3(x, y);
                i++;
            }
        }
    }

    #region Constructors

    public Building(BuildingType type)
    {
        id = nextId++;
        color = Color.HSVToRGB(UnityEngine.Random.value, UnityEngine.Random.Range(0.3f, 0.75f), UnityEngine.Random.Range(0.7f, 0.9f));

        this.type = type;

        employees = new List<Employee>();
    }

    public Building(BuildingType type, Vector2Int origin, Color color)
    {
        id = nextId++;

        this.type = type;
        this.origin = origin;
        this.color = color;

        employees = new List<Employee>();
    }

    public Building(int id, BuildingType type, Vector2Int origin, Color baseColor, List<Vector2Int> pastures)
    {
        this.id = id;
        this.type = type;
        this.origin = origin;
        this.color = baseColor;
        this.pastures = pastures;

        employees = new List<Employee>();
    }

    #endregion

    #region Utility

    public void SetColor(Color color)
    {
        foreach (var point in tiles)
        {
            World.instance.buildingTilemap.SetColor(new Vector3Int(point.x, point.y, 0), color);
        }
    }
    public List<Vector2Int> GetNearbyTiles(Vector2Int orig, int distance)
    {
        var nearby = new List<Vector2Int>();

        for (int i = 0 - distance; i < type.width + distance; i++)
        {
            for (int j = 0 - distance; j < type.height + distance; j++)
            {
                if (i >= 0 && i < type.width && j >= 0 && j < type.height) continue;
                var x = orig.x + j;
                var y = orig.y + i;
                if (!World.withinBounds(x, y)) continue;
                nearby.Add(new Vector2Int(x, y));
            }
        }

        return nearby;
    }

    #endregion

    #region Initialization

    public static PlacementResult CanPlace(Vector2Int origin, BuildingType type, out Vector2Int errorPos)
    {
        errorPos = new Vector2Int(-1, -1);
        for (int i = 0; i < type.width; i++)
        {
            for (int j = 0; j < type.height; j++)
            {
                var x = origin.x + i;
                var y = origin.y + j;
                if (x < 0 || x >= World.width || y < 0 || y >= World.height)
                {
                    return PlacementResult.OutOfBounds;
                }
                var pos = World.map[x, y].type;
                if (!(pos == TileType.empty || pos == TileType.tree))
                {
                    errorPos = new Vector2Int(x, y);
                    return PlacementResult.SomethingInTheWay;
                }
            }
        }

        var tempBorders = GetBorders(origin, type);
        var found = false;

        if (!type.genData.doesntRequireRoad)
        {
            foreach (var b in tempBorders)
            {
                var tiletype = (int)World.map[b.x, b.y].type;
                if (tiletype >= 2 && tiletype <= 7)
                {
                    //is on a road
                    found = true;
                    break;
                }
            }
            if (!found) return PlacementResult.NoRoad;
        }

        if (type.genData.requiresWater)
        {
            found = false;
            foreach (var b in tempBorders)
            {
                var tiletype = World.map[b.x, b.y].type;
                if (tiletype == TileType.river)
                {
                    found = true;
                    break;
                }
            }
            if (!found) return PlacementResult.RequiresWater;
        }

        if (type.genData.mustBeAttachedToHouse)
        {
            found = false;
            foreach (var b in tempBorders)
            {
                var bldg = World.map[b.x, b.y].building as Building;
                if (bldg != null && bldg.type.category == BuildingCategory.House)
                {
                    found = true;
                    break;
                }
            }
            if (!found) return PlacementResult.RequiresWater;
        }

        return PlacementResult.Success;
    }

    public void Place(bool putInGlobalCatalog = true)
    {
        Place(origin, putInGlobalCatalog);
    }

    public void Place(Vector2Int orig, bool putInGlobalCatalog = true)
    {
        origin = orig;
        if (putInGlobalCatalog)
        {
            while (World.allBuildings.Count <= id)
                World.allBuildings.Add(null);
            
            World.allBuildings[id] = this;
        }

        type.count++;
        for (int i = 0; i < type.width; i++)
        {
            for (int j = 0; j < type.height; j++)
            {
                int x = origin.x + i;
                int y = origin.y + j;
                var v = new Vector3Int(x, y, 0);
                World.instance.buildingTilemap.SetTile(v, type.GetTile(i, j, this));
                World.instance.buildingTilemap.SetColor(v, color);
                World.instance.buildingTilemap.RefreshTile(v);
                World.map[x, y].type = TileType.building;
                World.map[x, y].building = this;
                tiles.Add(new Vector2Int(x, y));
            }
        }

        if(pastures != null)
        {
            foreach(var p in pastures)
            {
                PlacePasture(p);
            }
        }

        borders = GetBorders(origin, type);
        entrances = GetEntrances(origin, type);
    }
    public void PlacePasture(Vector2Int pos)
    {        
        World.instance.buildingTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), WorldGenerator.Instance.pastureTile);
        if (type.genData.makePasturesFields)
            World.instance.baseTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), WorldGenerator.Instance.fieldTile);
        else
            World.instance.baseTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), WorldGenerator.Instance.emptyTile);

        World.map[pos.x, pos.y].type = TileType.pasture;
        World.map[pos.x, pos.y].building = this;
    }

    public static List<Vector2Int> GetBorders(Vector2Int orig, BuildingType type)
    {
        var borders = new List<Vector2Int>();
        for (int i = 0; i < type.width; i++)
        {
            var js = new int[2] { -1, type.height };
            foreach (var j in js)
            {
                var x = orig.x + i;
                var y = orig.y + j;
                if (!World.withinBounds(x, y)) continue;
                borders.Add(new Vector2Int(x, y));
            }
        }
        for (int i = 0; i < type.height; i++)
        {
            var js = new int[2] { -1, type.width };
            foreach (var j in js)
            {
                var x = orig.x + j;
                var y = orig.y + i;
                if (!World.withinBounds(x, y)) continue;
                borders.Add(new Vector2Int(x, y));
            }
        }
        return borders;
    }
    public static List<Vector2Int> GetEntrances(Vector2Int orig, BuildingType t)
    {
        var entrances = new List<Vector2Int>();
        var borders = GetBorders(orig, t);
        foreach (var b in borders)
        {
            var type = (int)World.map[b.x, b.y].type;
            if (type >= 2 && type <= 7) entrances.Add(b);
        }

        return entrances;
    }

    #endregion

    #region Procedural Generation Stuff

    public bool CanGenerate(Vector2Int orig)
    {
        origin = orig;
        if (CanPlace(origin, type, out Vector2Int errorPos) != PlacementResult.Success) return false;

        var tempBorders = GetBorders(origin, type);

        if (type.genData.requiresSmallRoad)
        {
            var found = false;
            foreach (var b in tempBorders)
            {
                var tiletype = World.map[b.x, b.y].type;
                if (tiletype == TileType.smallRoad)
                {
                    found = true;
                    break;
                }
            }
            if (!found) return false;
        }

        if (type.genData.requiresNonSmallRoad)
        {
            var found = false;
            foreach (var b in tempBorders)
            {
                var tiletype = World.map[b.x, b.y].type;
                if (tiletype == TileType.mediumRoad || tiletype == TileType.largeRoad)
                {
                    found = true;
                    break;
                }
            }
            if (!found) return false;
        }

        if (type.genData.minDistanceFromBuildings > 0)
        {
            var nearby = GetNearbyTiles(orig, type.genData.minDistanceFromBuildings);
            foreach (var pos in nearby)
            {
                var tiletype = World.map[pos.x, pos.y].type;
                if (tiletype == TileType.building)
                {
                    return false;
                }
            }
        }

        return true;
    }
    public void Generate(Vector2Int orig, Tilemap tilemap, bool[,] occupied)
    {
        Place(origin, false); //No global catalog because it hasnt been initialized yet 

        for (int i = 0; i < type.width; i++)
        {
            for (int j = 0; j < type.height; j++)
            {
                int x = origin.x + i;
                int y = origin.y + j;
                if (occupied != null) occupied[x, y] = true;
            }
        }

        if (type.genData.minPastures != 0 || type.genData.maxPastures != 0)
        {
            GeneratePastures();
        }
    }    

    private void GeneratePastures()
    {
        pastures = new List<Vector2Int>();
        var count = Mathf.Round(UnityEngine.Random.value * (Mathf.Max(type.genData.maxPastures, type.genData.minPastures) - type.genData.minPastures)) + type.genData.minPastures;
        var potential = new List<Vector2Int>();
        foreach (var b in borders)
        {
            var t = World.map[b.x, b.y].type;
            if (t == TileType.empty || t == TileType.tree)
            {
                potential.Add(b);
            }
        }

        if (World.withinBounds(origin.x - 1, origin.y - 1))
        {
            var t = World.map[origin.x - 1, origin.y - 1].type;
            if (t == TileType.empty || t == TileType.tree)
            {
                potential.Add(new Vector2Int(origin.x - 1, origin.y - 1));
            }
        }
        if (World.withinBounds(origin.x - 1, origin.y + type.height))
        {
            var t = World.map[origin.x - 1, origin.y + type.height].type;
            if (t == TileType.empty || t == TileType.tree)
            {
                potential.Add(new Vector2Int(origin.x - 1, origin.y + type.height));
            }
        }
        if (World.withinBounds(origin.x + type.width, origin.y + type.height))
        {
            var t = World.map[origin.x + type.width, origin.y + type.height].type;
            if (t == TileType.empty || t == TileType.tree)
            {
                potential.Add(new Vector2Int(origin.x + type.width, origin.y + type.height));
            }
        }
        if (World.withinBounds(origin.x + type.width, origin.y - 1))
        {
            var t = World.map[origin.x + type.width, origin.y - 1].type;
            if (t == TileType.empty || t == TileType.tree)
            {
                potential.Add(new Vector2Int(origin.x + type.width, origin.y - 1));
            }
        }


        var nextPotential = new List<Vector2Int>();

        while (count > 0 && potential.Count + nextPotential.Count > 0)
        {
            if (potential.Count > 0)
            {
                var index = UnityEngine.Random.Range(0, potential.Count);
                var pos = potential[index];

                pastures.Add(pos);
                PlacePasture(pos);

                if (World.withinBounds(pos.x, pos.y + 1))
                {
                    var up = World.map[pos.x, pos.y + 1].type;
                    if (up == TileType.empty || up == TileType.tree) nextPotential.Add(new Vector2Int(pos.x, pos.y + 1));
                }
                if (World.withinBounds(pos.x, pos.y - 1))
                {
                    var down = World.map[pos.x, pos.y - 1].type;
                    if (down == TileType.empty || down == TileType.tree) nextPotential.Add(new Vector2Int(pos.x, pos.y - 1));
                }
                if (World.withinBounds(pos.x - 1, pos.y))
                {
                    var left = World.map[pos.x - 1, pos.y].type;
                    if (left == TileType.empty || left == TileType.tree) nextPotential.Add(new Vector2Int(pos.x - 1, pos.y));
                }
                if (World.withinBounds(pos.x + 1, pos.y))
                {
                    var right = World.map[pos.x + 1, pos.y].type;
                    if (right == TileType.empty || right == TileType.tree) nextPotential.Add(new Vector2Int(pos.x + 1, pos.y));
                }

                potential.RemoveAt(index);
            }
            else
            {
                var index = UnityEngine.Random.Range(0, nextPotential.Count);
                var pos = nextPotential[index];

                pastures.Add(pos);
                PlacePasture(pos);
                nextPotential.RemoveAt(index);

                if (World.withinBounds(pos.x, pos.y + 1))
                {
                    var up = World.map[pos.x, pos.y + 1].type;
                    if (up == TileType.empty || up == TileType.tree) nextPotential.Add(new Vector2Int(pos.x, pos.y + 1));
                }
                if (World.withinBounds(pos.x, pos.y - 1))
                {
                    var down = World.map[pos.x, pos.y - 1].type;
                    if (down == TileType.empty || down == TileType.tree) nextPotential.Add(new Vector2Int(pos.x, pos.y - 1));
                }
                if (World.withinBounds(pos.x - 1, pos.y))
                {
                    var left = World.map[pos.x - 1, pos.y].type;
                    if (left == TileType.empty || left == TileType.tree) nextPotential.Add(new Vector2Int(pos.x - 1, pos.y));
                }
                if (World.withinBounds(pos.x + 1, pos.y))
                {
                    var right = World.map[pos.x + 1, pos.y].type;
                    if (right == TileType.empty || right == TileType.tree) nextPotential.Add(new Vector2Int(pos.x + 1, pos.y));
                }
            }
            count--;
        }
    }

    #endregion

    #region Json

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
    public static Building FromJson(string s)
    {
        try
        {
            var b = JsonUtility.FromJson<Building>(s);
            b.type = BuildingTypeManager.instance.allBuildingTypes[b.type.id];            
            return b;
        }
        catch
        {
            return null;
        }
    }

    #endregion
}

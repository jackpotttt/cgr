using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    public int largeRoadLength;
    public float largeRoadTurn;
    public float largeRoadSplit;
    public float largeRoadDowngrade;
    public int mediumRoadLength;
    public float mediumRoadTurn;
    public float mediumRoadSplit;
    public float mediumRoadDowngrade;
    public int smallRoadLength;
    public float smallRoadTurn;
    public float smallRoadSplit;
    
    public int lakeCount;
    public int lakeLength;
    public float lakeTurn;
    public float lakeSplit;
    public float lakeDegradation;
    public int minBridge;
    public int maxBridge;
    public int minIslandSize;
    public float buildingDensity;
    public RoadTile largeRoad;
    public RoadTile mediumRoad;
    public RoadTile smallRoad;
    public TileBase emptyTile;
    public TileBase treeTile;
    public TileBase riverTile;
    public TileBase pastureTile;
    public TileBase fieldTile;
    public TileBase blankTile;

    public static BuildingTypeManager BuildingReader;
    public static WorldGenerator Instance;
        
    void Awake()
    {
        Instance = this;        
    }

    public IEnumerator GetMap()
    {
        var i = 0;
        GameUI.instance.ShowStaticCenterMessage($"Attemping to connect");
        while (NetworkPlayer.localPlayer == null)
        {            
            Debug.Log($"Waiting on connection to request initial map: {++i}");
            if (i == 1000)
            {
                GameUI.instance.ShowStaticCenterMessage($"Could not connect");
                yield break;
            }
            yield return null;
        }
        GameUI.instance.ShowStaticCenterMessage($"Connected, loading game from server");
        NetworkPlayer.localPlayer.Cmd_WaitForInitialMap();
    }

    private class Heading
    {
        public Heading(Vector3Int p, Vector3Int d, TileBase t, bool split)
        {
            pos = p;
            dir = d;
            type = t;
            length = 0;
            HasSplit = split;   
        }

        public Vector3Int pos;
        public Vector3Int dir;
        public TileBase type;
        public int length;
        public Vector3Int next { get { return pos + dir; } }

        public bool HasSplit;
    }

    public IEnumerator Generate()
    {
        GameUI.instance.ShowStaticCenterMessage($"Generating World");
        ParseInputParameters();
        yield return null;

        FillEmpty();                
        
        yield return StartCoroutine(MakeWaterways());
        yield return StartCoroutine(MakeRoads());
        yield return StartCoroutine(FixIslands(largeRoadLength));
        yield return StartCoroutine(InitWorldMap());
        yield return StartCoroutine(PlaceAllBuildings());

        GameController.WorldIsReady();        
    }

    private void ParseInputParameters()
    {
        var width = (int)(200f * Mathf.Pow(WorldGenData.Size * 2, 2f));
        var height = (int)(width * 0.75f);
        World.Initialize(width, height);
        minIslandSize = (int)(25 * Mathf.Pow(WorldGenData.Size * 2, 2f));

        var baseRoads = ((float)World.width * (float)World.height / 75f);
        var roadFactor = Mathf.Pow(WorldGenData.Roads * 2, 3f);

        largeRoadLength = (int)(baseRoads * roadFactor);
        mediumRoadLength = (int)(WorldGenData.Size * 120 * WorldGenData.Roads);
        smallRoadLength = (int)(WorldGenData.Size * 120 * WorldGenData.Roads );

        var baseLakes = (float)(World.width / 40f) + 3;
        lakeCount = (int)(baseLakes * Mathf.Pow(WorldGenData.Water * 2, 1f));

        var baseLakeSize = Mathf.Max((float)((World.width - 30f) * 0.7f), 12f);
        lakeLength = (int)(baseLakeSize * Mathf.Pow(WorldGenData.Water * 2, 1f));

        buildingDensity = (0.25f * Mathf.Pow(WorldGenData.Buildings * 2, 1.2f));
    }

    public IEnumerator FixIslands(int max)
    {
        max = (int)(max * 0.4f);
        yield return StartCoroutine(InitWorldMap());

        var visited = new bool[World.width, World.height];
        var islands = new List<List<Vector2Int>>();
        for (int i = 0; i < World.width; i++)
        {
            for (int j = 0; j < World.height; j++)
            {
                if (!visited[i, j] && (World.map[i, j].type == TileType.empty || World.map[i, j].type == TileType.tree))
                {
                    var island = FloodFill(new Vector2Int(i, j), visited);
                    if(island != null)
                        islands.Add(island);
                }
            }
        }

        bool changed = false;
        foreach(var island in islands)
        {
            if (island.Count > minIslandSize)
            {
                if (changed)
                {
                    yield return StartCoroutine(FixIslands(max));
                    yield break;
                }

                var start = island[Random.Range(0, island.Count)];
                yield return StartCoroutine(MakeRoads(new Vector3Int(start.x, start.y, 0), max));
                changed = true;
            }
        }
    }

    private List<Vector2Int> FloodFill(Vector2Int start, bool[,] visited)
    {
        var queue = new List<Vector2Int>();
        var island = new List<Vector2Int>();

        queue.Add(start);
        visited[start.x, start.y] = true;        

        while (queue.Count > 0)
        {
            var pos = queue[0];
            queue.RemoveAt(0);
            island.Add(pos);


            if (pos.y < World.height - 1)
            {
                var up = World.map[pos.x, pos.y + 1].type;
                if ((int)up > 1 && (int)up < 8)
                {
                    return null;
                }
                else if ((int)up < 2 && !visited[pos.x, pos.y + 1])
                {
                    visited[pos.x, pos.y + 1] = true;
                    queue.Add(new Vector2Int(pos.x, pos.y + 1));
                }
            }



            if (pos.y > 0)
            {
                var down = World.map[pos.x, pos.y - 1].type;
                if ((int)down > 1 && (int)down < 8)
                {
                    return null;
                }
                else if ((int)down < 2 && !visited[pos.x, pos.y - 1])
                {
                    visited[pos.x, pos.y - 1] = true;
                    queue.Add(new Vector2Int(pos.x, pos.y - 1));
                }
            }



            if (pos.x > 0)
            {
                var left = World.map[pos.x - 1, pos.y].type;
                if ((int)left > 1 && (int)left < 8)
                {
                    return null;
                }
                else if ((int)left < 2 && !visited[pos.x - 1, pos.y])
                {
                    visited[pos.x - 1, pos.y] = true;
                    queue.Add(new Vector2Int(pos.x - 1, pos.y));
                }
            }



            if (pos.x < World.width - 1)
            {
                var right = World.map[pos.x + 1, pos.y].type;
                if ((int)right > 1 && (int)right < 8)
                {
                    return null;
                }
                else if ((int)right < 2 && !visited[pos.x + 1, pos.y])
                {
                    visited[pos.x + 1, pos.y] = true;
                    queue.Add(new Vector2Int(pos.x + 1, pos.y));
                }
            }
        }

        return island;
    }

    private IEnumerator InitWorldMap()
    {
        for (int i = 0; i < World.width; i++)
        {
            for (int j = 0; j < World.height; j++)
            {
                var pos = new Vector3Int(i, j, 0);
                var type = World.instance.baseTilemap.GetTile(pos);
                if (type == emptyTile)
                {
                    World.map[i, j].type = TileType.empty;
                }
                else if (type == treeTile)
                {
                    World.map[i, j].type = TileType.tree;
                }
                else if (type == smallRoad)
                {
                    var sprite = smallRoad.GetSprite(pos, World.instance.baseTilemap);
                    if (sprite == smallRoad.spriteLRW || sprite == smallRoad.spriteUDW)
                        World.map[i, j].type = TileType.smallRoadRiver;
                    else
                        World.map[i, j].type = TileType.smallRoad;
                }
                else if (type == mediumRoad)
                {
                    var sprite = mediumRoad.GetSprite(pos, World.instance.baseTilemap);
                    if (sprite == mediumRoad.spriteLRW || sprite == mediumRoad.spriteUDW)
                        World.map[i, j].type = TileType.mediumRoadRiver;
                    else
                        World.map[i, j].type = TileType.mediumRoad;
                }
                else if (type == largeRoad)
                {
                    var sprite = largeRoad.GetSprite(pos, World.instance.baseTilemap);
                    if (sprite == largeRoad.spriteLRW || sprite == largeRoad.spriteUDW)
                        World.map[i, j].type = TileType.largeRoadRiver;
                    else
                        World.map[i, j].type = TileType.largeRoad;
                }
                else if (type == riverTile) World.map[i, j].type = TileType.river;
            }
        }
        yield return null;
    }

    private IEnumerator PlaceAllBuildings()
    {
        var visited = new bool[World.width, World.height];
        var blocks = new List<Block>();
        for (int i = 0; i < World.width; i++)
        {
            for (int j = 0; j < World.height; j++)
            {
                if (!visited[i, j] && (World.map[i, j].type == TileType.empty || World.map[i, j].type == TileType.tree))
                {
                    blocks.Add(FloodFillBlocks(new Vector2Int(i, j), visited));
                }
            }
        }
        yield return null;

        var occupied = new bool[World.width, World.height];
        yield return StartCoroutine(PlaceBuildings(blocks, occupied, buildingDensity));

        World.allBuildings = new List<Building>(new Building[Building.nextId]);

        foreach(var b in World.houses)        
            World.allBuildings[b.id] = b;
        foreach (var b in World.suppliers)
            World.allBuildings[b.id] = b;
        foreach (var b in World.utilities)
            World.allBuildings[b.id] = b;

        CleanupBuildingIds();        
    }

    private void CleanupBuildingIds()
    {
        var firstEmpty = CleanupBuildingIdsWrapped();
        World.allBuildings.RemoveRange(firstEmpty, World.allBuildings.Count - firstEmpty);

        Building.nextId = World.allBuildings.Count;
    }

    private int CleanupBuildingIdsWrapped()
    {
        int i, j = World.allBuildings.Count - 1;
        for (i = 0; i < j; i++)
        {
            if (World.allBuildings[i] == null)
            {
                while (World.allBuildings[j] == null)
                {
                    j--;
                    if (i >= j) return i;
                }

                World.allBuildings[i] = World.allBuildings[j];
                World.allBuildings[i].id = i;
                World.allBuildings[j--] = null;
            }
        }

        return i;
    }

    private IEnumerator PlaceBuildings(List<Block> blocks, bool[,] occupied, float density)
    {
        var visited = new bool[World.width, World.height];
        var types = new List<BuildingType>();
        var buildingTypes = BuildingTypeManager.GetGenerationTypes();

        types.AddRange(buildingTypes);
        int count = 0;
        bool first = true;

        while (types.Count > 0)
        {
            for (int i = 0; i < types.Count; i++)
            {
                var min = density * roads / buildingTypes.Count * types[i].genData.frequency;
                if (types[i].category != BuildingCategory.House) min /= 2f;
                if (Random.value < (types[i].count - min) / min)
                {
                    types.RemoveAt(i--);
                    continue;
                }

                var tempBlocks = new List<Block>();
                tempBlocks.AddRange(blocks);
                visited = new bool[World.width, World.height];

                var placed = false;
                while (tempBlocks.Count > 0)
                {
                    var index = Random.Range(0, tempBlocks.Count);

                    if (types[i].size <= tempBlocks[index].volume && PlaceBuildingInBlock(types[i], tempBlocks[index], visited, occupied))
                    {
                        if (tempBlocks[index].volume < types[i].size) tempBlocks.RemoveAt(index);
                        if (tempBlocks[index].volume <= 0) blocks.Remove(tempBlocks[index]);
                        placed = true;
                        break;
                    }
                    else tempBlocks.RemoveAt(index);
                }
                if (!placed && !first) types.RemoveAt(i--);
                if (++count >= 25)
                {
                    yield return null;
                    count = 0;
                }
            }
            first = false;
        }
    }

    private bool PlaceBuildingInBlock(BuildingType type, Block block, bool[,] visited, bool[,] occupied)
    {        
        Building building = new Building(type);
        var spot = new Vector2Int();

        List<Building> worldList;
        switch (type.category)
        {
            case BuildingCategory.House:
                worldList = World.houses;
                break;
            case BuildingCategory.Supplier:
                worldList = World.suppliers;
                break;
            default:
                worldList = World.utilities;
                break;
        }

        if (type.genData.doesntRequireRoad)
        {
            var seed = Random.Range(0, block.positions.Count);
            for (int i = 0; i < block.positions.Count; i++)
            {
                var index = seed + i;
                if (index >= block.positions.Count) index -= block.positions.Count;
                var pos = block.positions[index];

                if (building.CanGenerate(pos))
                {
                    if (worldList != null) worldList.Add(building);
                    building.Generate(pos, World.instance.buildingTilemap, occupied);
                    block.volume -= type.size;
                    return true;
                }
            }
        }
        else
        {
            var seed = Random.Range(0, block.borders.Count);
            for (int i = 0; i < block.borders.Count; i++)
            {
                var index = seed + i;
                if (index >= block.borders.Count) index -= block.borders.Count;

                var offset = new Vector2Int();
                var border = block.borders[index];

                switch (border.neighbors)
                {
                    case 1: //up
                        offset.y = -(type.height - 1);
                        for (int j = 0; j < type.width; j++)
                        {
                            offset.x = -j;
                            spot = border.pos + offset;
                            if (spot.x > 0 && spot.y > 0)
                            {
                                if (!visited[spot.x, spot.y] && !occupied[spot.x, spot.y] && building.CanGenerate(spot))
                                {
                                    if (worldList != null) worldList.Add(building);
                                    building.Generate(spot, World.instance.buildingTilemap, occupied);
                                    block.borders.RemoveAt(index);
                                    block.volume -= type.size;
                                    return true;
                                }
                                else visited[spot.x, spot.y] = true;
                            }
                        }
                        break;

                    case 2:  //down                         
                        for (int j = 0; j < type.width; j++)
                        {
                            offset.x = -j;
                            spot = border.pos + offset;
                            if (spot.x > 0)
                            {
                                if (!visited[spot.x, spot.y] && !occupied[spot.x, spot.y] && building.CanGenerate(spot))
                                {
                                    if (worldList != null) worldList.Add(building);
                                    building.Generate(spot, World.instance.buildingTilemap, occupied);
                                    block.borders.RemoveAt(index);
                                    block.volume -= type.size;
                                    return true;
                                }
                                else visited[spot.x, spot.y] = true;
                            }
                        }
                        break;

                    case 3: // up and down
                        if (type.height > 1)
                        {
                            visited[border.pos.x, border.pos.y] = true;
                            return false;
                        }

                        for (int j = 0; j < type.width; j++)
                        {
                            offset.x = -j;
                            spot = border.pos + offset;
                            if (spot.x > 0)
                            {
                                if (!visited[spot.x, spot.y] && !occupied[spot.x, spot.y] && building.CanGenerate(spot))
                                {
                                    if (worldList != null) worldList.Add(building);
                                    building.Generate(spot, World.instance.buildingTilemap, occupied);
                                    block.borders.RemoveAt(index);
                                    block.volume -= type.size;
                                    return true;
                                }
                                else visited[spot.x, spot.y] = true;
                            }
                        }
                        break;

                    case 4: // left                    
                        for (int j = 0; j < type.height; j++)
                        {
                            offset.y = -j;
                            spot = border.pos + offset;
                            if (spot.y > 0)
                            {
                                if (!visited[spot.x, spot.y] && !occupied[spot.x, spot.y] && building.CanGenerate(spot))
                                {
                                    if (worldList != null) worldList.Add(building);
                                    building.Generate(spot, World.instance.buildingTilemap, occupied);
                                    block.borders.RemoveAt(index);
                                    block.volume -= type.size;
                                    return true;
                                }
                                else visited[spot.x, spot.y] = true;
                            }
                        }
                        break;

                    case 5: //left and up                    
                        offset.y -= (type.height - 1);
                        spot = border.pos + offset;
                        if (spot.y > 0)
                        {
                            if (!visited[spot.x, spot.y] && !occupied[spot.x, spot.y] && building.CanGenerate(spot))
                            {
                                if (worldList != null) worldList.Add(building);
                                building.Generate(spot, World.instance.buildingTilemap, occupied);
                                block.borders.RemoveAt(index);
                                block.volume -= type.size;
                                return true;
                            }
                            else visited[spot.x, spot.y] = true;
                        }
                        break;

                    case 6: // left and down                                                      
                        if (!visited[border.pos.x, border.pos.y] && !occupied[border.pos.x, border.pos.y] && building.CanGenerate(border.pos))
                        {
                            if (worldList != null) worldList.Add(building);
                            building.Generate(border.pos, World.instance.buildingTilemap, occupied);
                            block.borders.RemoveAt(index);
                            block.volume -= type.size;
                            return true;
                        }
                        else visited[border.pos.x, border.pos.y] = true;
                        break;

                    case 7: //left up down
                        if (type.height > 1)
                        {
                            visited[border.pos.x, border.pos.y] = true;
                            return false;
                        }
                        if (!visited[border.pos.x, border.pos.y] && !occupied[border.pos.x, border.pos.y] && building.CanGenerate(border.pos))
                        {
                            if (worldList != null) worldList.Add(building);
                            building.Generate(border.pos, World.instance.buildingTilemap, occupied);
                            block.borders.RemoveAt(index);
                            block.volume -= type.size;
                            return true;
                        }
                        else visited[border.pos.x, border.pos.y] = true;
                        break;

                    case 8: //right
                        offset.x = -(type.width - 1);
                        for (int j = 0; j < type.height; j++)
                        {
                            offset.y = -j;
                            spot = border.pos + offset;
                            if (spot.x > 0 && spot.y > 0)
                            {
                                if (!visited[spot.x, spot.y] && !occupied[spot.x, spot.y] && building.CanGenerate(spot))
                                {
                                    if (worldList != null) worldList.Add(building);
                                    building.Generate(spot, World.instance.buildingTilemap, occupied);
                                    block.borders.RemoveAt(index);
                                    block.volume -= type.size;
                                    return true;
                                }
                                else visited[spot.x, spot.y] = true;
                            }
                        }
                        break;

                    case 9: //up right
                        offset.x -= (type.width - 1);
                        offset.y -= (type.height - 1);
                        spot = border.pos + offset;
                        if (spot.x > 0 && spot.y > 0)
                        {
                            if (!visited[spot.x, spot.y] && !occupied[spot.x, spot.y] && building.CanGenerate(spot))
                            {
                                if (worldList != null) worldList.Add(building);
                                building.Generate(spot, World.instance.buildingTilemap, occupied);
                                block.borders.RemoveAt(index);
                                block.volume -= type.size;
                                return true;
                            }
                            else visited[spot.x, spot.y] = true;
                        }
                        break;

                    case 10: //down right
                        offset.x -= (type.width - 1);
                        spot = border.pos + offset;
                        if (spot.x > 0)
                        {
                            if (!visited[spot.x, spot.y] && !occupied[spot.x, spot.y] && building.CanGenerate(spot))
                            {
                                if (worldList != null) worldList.Add(building);
                                building.Generate(spot, World.instance.buildingTilemap, occupied);
                                block.borders.RemoveAt(index);
                                block.volume -= type.size;
                                return true;
                            }
                            else visited[spot.x, spot.y] = true;
                        }
                        break;

                    case 11: //up down right
                        if (type.height > 1)
                        {
                            visited[border.pos.x, border.pos.y] = true;
                            return false;
                        }
                        offset.x -= (type.width - 1);
                        spot = border.pos + offset;
                        if (spot.x > 0)
                        {
                            if (!visited[spot.x, spot.y] && !occupied[spot.x, spot.y] && building.CanGenerate(spot))
                            {
                                if (worldList != null) worldList.Add(building);
                                building.Generate(spot, World.instance.buildingTilemap, occupied);
                                block.borders.RemoveAt(index);
                                block.volume -= type.size;
                                return true;
                            }
                            else visited[spot.x, spot.y] = true;
                        }
                        break;

                    case 12: //right left
                        if (type.width > 1)
                        {
                            visited[border.pos.x, border.pos.y] = true;
                            return false;
                        }
                        for (int j = 0; j < type.height; j++)
                        {
                            offset.y = -j;
                            spot = border.pos + offset;
                            if (spot.y > 0)
                            {
                                if (!visited[spot.x, spot.y] && !occupied[spot.x, spot.y] && building.CanGenerate(spot))
                                {
                                    if (worldList != null) worldList.Add(building);
                                    building.Generate(spot, World.instance.buildingTilemap, occupied);
                                    block.borders.RemoveAt(index);
                                    block.volume -= type.size;
                                    return true;
                                }
                                else visited[spot.x, spot.y] = true;
                            }
                        }
                        break;

                    case 13: //up right left
                        if (type.width > 1)
                        {
                            visited[border.pos.x, border.pos.y] = true;
                            return false;
                        }
                        offset.y -= (type.height - 1);
                        spot = border.pos + offset;
                        if (spot.y > 0)
                        {
                            if (!visited[spot.x, spot.y] && !occupied[spot.x, spot.y] && building.CanGenerate(spot))
                            {
                                if (worldList != null) worldList.Add(building);
                                building.Generate(spot, World.instance.buildingTilemap, occupied);
                                block.borders.RemoveAt(index);
                                block.volume -= type.size;
                                return true;
                            }
                            else visited[spot.x, spot.y] = true;
                        }
                        break;

                    case 14: //down right left
                        if (type.width > 1)
                        {
                            visited[border.pos.x, border.pos.y] = true;
                            return false;
                        }
                        spot = border.pos;
                        if (!visited[spot.x, spot.y] && !occupied[spot.x, spot.y] && building.CanGenerate(spot))
                        {
                            if (worldList != null) worldList.Add(building);
                            building.Generate(spot, World.instance.buildingTilemap, occupied);
                            block.borders.RemoveAt(index);
                            block.volume -= type.size;
                            return true;
                        }
                        else visited[spot.x, spot.y] = true;
                        break;

                    case 15: //all
                        if (type.size == 1)
                        {
                            if (!visited[border.pos.x, border.pos.y] && !occupied[border.pos.x, border.pos.y] && building.CanGenerate(border.pos))
                            {
                                if (worldList != null) worldList.Add(building);
                                building.Generate(border.pos, World.instance.buildingTilemap, occupied);
                                block.borders.RemoveAt(index);
                                block.volume -= type.size;
                                return true;
                            }
                            else visited[border.pos.x, border.pos.y] = true;
                        }
                        break;
                }
            }
        }
        return false;
    }

    struct Block
    {
        public int volume;
        public List<Border> borders;
        public List<Vector2Int> positions;
    }
    struct Border
    {
        public Vector2Int pos;
        public Block block;
        public byte neighbors;
    }
    
    private Block FloodFillBlocks(Vector2Int start, bool[,] visited)
    {
        var queue = new List<Vector2Int>();
        queue.Add(start);
        visited[start.x, start.y] = true;
        Block b = new Block();
        b.volume = 0;
        b.borders = new List<Border>();
        b.positions = new List<Vector2Int>();

        while (queue.Count > 0)
        {
            var pos = queue[0];
            queue.RemoveAt(0);            

            var bdr = new Border();
            bdr.pos = pos;            
            bdr.neighbors = 0;
            bool isBorder = false;            

            if (pos.y < World.height - 1)
            {
                var up = World.map[pos.x, pos.y + 1].type;
                if ((int)up > 1 && (int)up < 8)
                {
                    isBorder = true;
                    bdr.neighbors += 1;
                }
                else if ((int)up < 2 && !visited[pos.x, pos.y + 1])
                {
                    visited[pos.x, pos.y + 1] = true;
                    queue.Add(new Vector2Int(pos.x, pos.y + 1));
                }                    
            }



            if (pos.y > 0)
            {
                var down = World.map[pos.x, pos.y - 1].type;
                if ((int)down > 1 && (int)down < 8)
                {
                    isBorder = true;
                    bdr.neighbors += 2;
                }
                else if ((int)down < 2 && !visited[pos.x, pos.y - 1])
                {
                    visited[pos.x, pos.y - 1] = true;
                    queue.Add(new Vector2Int(pos.x, pos.y - 1));
                }
            }



            if (pos.x > 0)
            {
                var left = World.map[pos.x - 1, pos.y].type;
                if ((int)left > 1 && (int)left < 8)
                {
                    isBorder = true;
                    bdr.neighbors += 4;
                }
                else if ((int)left < 2 && !visited[pos.x - 1, pos.y])
                {
                    visited[pos.x - 1, pos.y] = true;
                    queue.Add(new Vector2Int(pos.x - 1, pos.y));
                }
            }



            if (pos.x < World.width - 1)
            {
                var right = World.map[pos.x + 1, pos.y].type;
                if ((int)right > 1 && (int)right < 8)
                {
                    isBorder = true;
                    bdr.neighbors += 8;
                }
                else if ((int)right < 2 && !visited[pos.x + 1, pos.y])
                {
                    visited[pos.x + 1, pos.y] = true;
                    queue.Add(new Vector2Int(pos.x + 1, pos.y));
                }
            }

            b.volume++;
            if (isBorder) b.borders.Add(bdr);
            b.positions.Add(pos);
        }

        return b;
    }

    private IEnumerator MakeWaterways()
    {
        for (int j = 0; j < lakeCount; j++)
        {
            int count = 0;
            var heading = new Heading(new Vector3Int(), new Vector3Int(), riverTile, false);
            do
            {
                var startX = Random.Range(0, World.width);
                var startY = Random.Range(0, World.height);
                var start = new Vector3Int(startX, startY, 0);

                heading.dir = randomDirection();
                heading.pos = start;
            } while (!isValidRiverStart(heading) && count++ < 50);
            if (count >= 50) yield break;


            var time = 0f;

            var headings = new List<Heading>();
            headings.Add(heading);

            while (headings.Count > 0)
            {
                time += Time.deltaTime;
                if (time > 0.2f)
                {
                    time = 0f;
                    yield return null;
                }

                int i = 0;
                while (i < headings.Count)
                {
                    MutateLake(headings[i], headings);
                    if (withinBounds(headings[i]) && isValidRiverSpot(headings[i]))
                    {                        
                        advanceRiver(headings[i]);                        
                    }
                    else
                    {
                        headings[i].dir = getPerpendicular(headings[i].dir, Random.value > 0.5);
                        if (withinBounds(headings[i]) && isValidRiverSpot(headings[i]))
                        {
                            advanceRiver(headings[i]);
                        }
                        else
                        {
                            headings[i].dir = getOpposite(headings[i].dir);
                            if (withinBounds(headings[i]) && isValidRiverSpot(headings[i]))
                            {
                                advanceRiver(headings[i]);
                            }
                            else
                            {
                                headings.RemoveAt(i);
                                continue;
                            }
                        }
                    }
                    if (headings[i].length > lakeLength) headings.RemoveAt(i);
                    i++;
                }
            }

            yield return null;
        }
    }
    
    private bool notTooCloseRiver(Heading heading)
    {
        //if (World.Instance.tilemap.GetTile(heading.next) is RiverTile) return true;
        var positions = new Vector3Int[6]
        {
            heading.next + heading.dir,
            heading.next + heading.dir + heading.dir,
            heading.next + getPerpendicular(heading.dir, true),
            heading.next + (getPerpendicular(heading.dir, true) * 2),
            heading.next + (getPerpendicular(heading.dir, false) * 2),
            heading.next + getPerpendicular(heading.dir, false)
        };
        foreach (var pos in positions)
        {
            if (World.instance.baseTilemap.GetTile(pos) is RiverTile)
            {
                //if (World.Instance.tilemap.GetTile(pos + heading.dir) is RoadTile || World.Instance.tilemap.GetTile(pos + getOpposite(heading.dir)) is RoadTile)
                    return false;
            }
        }
        return true;
    }

    private void MutateLake(Heading heading, List<Heading> headings)
    {
        if (Random.value < lakeTurn)
        {
            heading.dir = getPerpendicular(heading.dir, Random.value > 0.5f);
        }
        if (Random.value < lakeSplit)
        {
            var h = new Heading(heading.pos, getPerpendicular(heading.dir, Random.value > 0.5f), riverTile, true);
            h.length = heading.length + (int)((lakeLength - heading.length) * lakeDegradation);
            headings.Add(h);
        }
    }

       

    private void advanceRiver(Heading heading)
    {
        if (!(World.instance.baseTilemap.GetTile(heading.next) is RoadTile)) World.instance.baseTilemap.SetTile(heading.next, riverTile);
        heading.pos = heading.next;
        heading.length++;
    }

    private bool isValidRiverStart(Heading heading)
    {
        return (World.instance.baseTilemap.GetTile(heading.pos) is RandomTile) && isValidRiverSpot(heading);
    }

    private bool isValidRiverSpot(Heading heading)
    {
        var tile = World.instance.baseTilemap.GetTile(heading.next) as RoadTile;
        var sprite = tile?.GetSprite(heading.next, World.instance.baseTilemap);
        if (sprite != null && (sprite == tile?.spriteUD || sprite == tile?.spriteLR || sprite == tile?.spriteUDW || sprite == tile?.spriteLRW)) return true;

        var positions = new Vector3Int[4]
        {
            heading.next,
            heading.next + heading.dir,
            heading.next + getPerpendicular(heading.dir, true),
            heading.next + getPerpendicular(heading.dir, false)
        };

        foreach(var pos in positions)
        {
            var tile2 = World.instance.baseTilemap.GetTile(pos) as RoadTile;
            var sprite2 = tile2?.GetSprite(pos, World.instance.baseTilemap);

            if (sprite2 != null && sprite2 != tile2?.spriteUD && sprite2 != tile2?.spriteLR && sprite2 != tile2?.spriteUDW && sprite2 != tile2?.spriteLRW)
            {
                return false;
            }
        }

        return true;
    }


    public static int roads = 0;
    

    private IEnumerator MakeRoads()
    {
        var startX = Random.Range((int)(0.35f * World.width), (int)(0.65f * World.width));
        var startY = Random.Range((int)(0.35f * World.height), (int)(0.65f * World.height));
        var start = new Vector3Int(startX, startY, 0);

        while (World.instance.baseTilemap.GetTile(start) is RiverTile)
        {
            startX = Random.Range((int)(0.35f * World.width), (int)(0.65f * World.width));
            startY = Random.Range((int)(0.35f * World.height), (int)(0.65f * World.height));
            start = new Vector3Int(startX, startY, 0);
        }

        yield return StartCoroutine(MakeRoads(start, largeRoadLength));
    }

    private IEnumerator MakeRoads(Vector3Int start, int max)
    {      
        var headings = new List<Heading>();
        World.instance.baseTilemap.SetTile(start, largeRoad);
        bool one = false;
        if (Random.value > 0.1f) { headings.Add(new Heading(start, Vector3Int.up, largeRoad, false)); one = true; }
        if (Random.value > 0.1f) { headings.Add(new Heading(start, Vector3Int.left, largeRoad, false)); one = true; }
        if (Random.value > 0.1f) { headings.Add(new Heading(start, Vector3Int.right, largeRoad, false)); one = true; }
        if (Random.value > 0.1f || !one) headings.Add(new Heading(start, Vector3Int.down, largeRoad, false));

        var time = 0f;
        while (headings.Count > 0)
        {
            time += Time.deltaTime;
            if (time > 0.05f)
            {
                time = 0f;
                yield return null;
            }

            var i = 0;
            while (i < headings.Count)
            {
                Mutate(headings, i, max);
                var h = headings[i];

                if (withinBounds(h) && isValidRoadSpot(h))
                {                    
                    h.pos = h.next;
                    h.length++;
                    if (withinBounds(h.pos))
                    {                                           
                        var t = World.instance.baseTilemap.GetTile(h.pos) as RoadTile;
                        if (t == null)
                        {
                            World.instance.baseTilemap.SetTile(h.pos, h.type);
                            roads++;
                        }
                        else if(t.roadClass < ((RoadTile)h.type).roadClass)
                            World.instance.baseTilemap.SetTile(h.pos, h.type);
                    }
                }
                else
                {
                    h.dir = getPerpendicular(h.dir, Random.value > 0.5f);
                    if (withinBounds(h) && isValidRoadSpot(h))
                    {
                        h.pos = h.next;
                        h.length++;
                        if (withinBounds(h.pos))
                        {
                            var t = World.instance.baseTilemap.GetTile(h.pos) as RoadTile;
                            if (t == null)
                            {
                                World.instance.baseTilemap.SetTile(h.pos, h.type);
                                roads++;
                            }
                            else if (t.roadClass < ((RoadTile)h.type).roadClass)
                                World.instance.baseTilemap.SetTile(h.pos, h.type);
                        }
                    }
                    else
                    {
                        h.dir = getOpposite(h.dir);
                        if (withinBounds(h) && isValidRoadSpot(h))
                        {
                            h.pos = h.next;
                            h.length++;
                            if (withinBounds(h.pos))
                            {
                                var t = World.instance.baseTilemap.GetTile(h.pos) as RoadTile;
                                if (t == null)
                                {
                                    World.instance.baseTilemap.SetTile(h.pos, h.type);
                                    roads++;
                                }
                                else if (t.roadClass < ((RoadTile)h.type).roadClass)
                                    World.instance.baseTilemap.SetTile(h.pos, h.type);
                            }
                        }
                        else
                        {
                            headings.RemoveAt(i);
                            continue;
                        }
                    }                    
                }

                if (h.HasSplit && headings[i].length > max)
                    headings.RemoveAt(i);

                i++;
            }
        }
    }

    private void FillEmpty()
    {
        for (float i = 0; i < World.width; i++)
        {
            for (float j = 0; j < World.height; j++)
            {
                var noise = Mathf.PerlinNoise(i / 6f, j / 6f);
                if (noise > 0.6f && (noise * 100) % 10 > 4) World.instance.baseTilemap.SetTile(new Vector3Int((int)i, (int)j, 0), treeTile);
                else World.instance.baseTilemap.SetTile(new Vector3Int((int)i, (int)j, 0), emptyTile);
            }
        }
    }

    public int DistanceFromEdge(int x, int y, bool u, bool d, bool l, bool r)
    {
        List<int> distances = new List<int>();

        if (u) distances.Add(x - 0);
        if (d) distances.Add(y - 0);
        if (l) distances.Add((World.width - 1) - x);
        if (r) distances.Add((World.height - 1) - y);

        return Mathf.Min(distances.ToArray());
    }

    private bool isValidRoadSpot(Heading heading)
    {
        var nextTile = World.instance.baseTilemap.GetTile(heading.next);
        if (nextTile is RoadTile)
        {
            //if (nextTile == largeRoad) return heading.type == largeRoad;
            //if (nextTile == mediumRoad) return heading.type == largeRoad || heading.type == mediumRoad;
            //if (nextTile == smallRoad)
            return true;
        }

        var distance = 3;

        var positions = new List<Vector3Int>();
        for (int i = 1; i <= distance; i++)
        {
            positions.Add(heading.next + (getPerpendicular(heading.dir, true) * i));
            positions.Add(heading.next + (getPerpendicular(heading.dir, false) * i));
        }

        foreach (var pos in positions)
        {
            if (World.instance.baseTilemap.GetTile(pos) is RoadTile)
            {
                if (World.instance.baseTilemap.GetTile(pos + heading.dir) is RoadTile || World.instance.baseTilemap.GetTile(pos + getOpposite(heading.dir)) is RoadTile)
                    return false;
            }
        }


        positions = new List<Vector3Int>();
        for (int i = distance; i <= distance * 2; i++)
        {
            positions.Add(heading.next + (getPerpendicular(heading.dir, true) * i));
            positions.Add(heading.next + (getPerpendicular(heading.dir, false) * i));
        }


        foreach (var pos in positions)
        {
            if (World.instance.baseTilemap.GetTile(pos) == mediumRoad || World.instance.baseTilemap.GetTile(pos) == largeRoad)
            {
                return false;
            }
        }

        positions = new List<Vector3Int>();
        for (int i = distance * 2; i <= distance * 3; i++)
        {
            positions.Add(heading.next + (getPerpendicular(heading.dir, true) * i));
            positions.Add(heading.next + (getPerpendicular(heading.dir, false) * i));
        }

        foreach (var pos in positions)
        {
            if (World.instance.baseTilemap.GetTile(pos) == largeRoad)
            {
                return false;
            }
        }

        if (World.instance.baseTilemap.GetTile(heading.next) is RiverTile)
        {
            var bridge = new List<Vector3Int>();
            var temp = heading.next;
            bridge.Add(temp);
            do
            {
                temp += heading.dir;
                bridge.Add(temp);
            } while (World.instance.baseTilemap.GetTile(temp) is RiverTile);

            if ((bridge.Count > World.width / 8f) || (heading.type != largeRoad && Random.value < ((float)(bridge.Count - minBridge)) / ((float)(maxBridge - minBridge))))
            {
                return false;
            }

            else
            {
                var i = -1;
                foreach (var piece in bridge)
                {
                    if (World.instance.baseTilemap.GetTile(piece) is RoadTile || !withinBounds(piece)) break;
                    World.instance.baseTilemap.SetTile(piece, heading.type);
                    heading.length++;
                    i++;
                    if (World.instance.baseTilemap.GetTile(piece + getPerpendicular(heading.dir, true)) is RoadTile || World.instance.baseTilemap.GetTile(piece + getPerpendicular(heading.dir, false)) is RoadTile) break;
                }
                heading.pos = bridge[i];
            }
        }
        return true;
    }

    private void Mutate(List<Heading> headings, int i, int max)
    {
        var heading = headings[i];
        var turnChance = largeRoadTurn;
        var splitChance = largeRoadSplit;
        var downgradeChance = largeRoadDowngrade;
        if (heading.type == smallRoad)
        {
            turnChance = smallRoadTurn;
            splitChance = smallRoadSplit;
            downgradeChance = 0;
        }
        else if (heading.type == mediumRoad)
        {
            turnChance = mediumRoadTurn;
            splitChance = mediumRoadSplit;
            downgradeChance = mediumRoadDowngrade;
        }


        if (Random.value < turnChance)
        {
            var prevDir = heading.dir;
            heading.dir = getPerpendicular(heading.dir, Random.value > 0.5f);
            if (!isValidRoadSpot(heading)) heading.dir = prevDir;
        }
        else if(Random.value < splitChance)
        {
            var h = new Heading(heading.pos, getPerpendicular(heading.dir, Random.value > 0.5f), heading.type, true);
            h.length = heading.length;
            h.HasSplit = heading.HasSplit;
            headings.Add(h);

            if (Random.value < downgradeChance)
            {                
                heading.HasSplit = true;
                h.HasSplit = true;
                if (h.type == largeRoad)
                {
                    h.length = max - mediumRoadLength;
                    h.type = mediumRoad;
                }
                else
                {
                    h.length = max - smallRoadLength;
                    h.type = smallRoad;
                }
            }
        }        
    }

    private Vector3Int randomDirection()
    {
        switch (Random.Range(0, 4))
        {
            case 0:
                return Vector3Int.up;
            case 1:
                return Vector3Int.down;
            case 2:
                return Vector3Int.left;
            default:
                return Vector3Int.right;
        }
    }

    private bool withinBounds(Heading h)
    {
        if (h.next.x < 0 || h.next.x >= World.width || h.next.y < 0 || h.next.y >= World.height) return false;
        return true;
    }

    private bool withinBounds(Vector3Int pos)
    {
        if (pos.x < 0 || pos.x >= World.width || pos.y < 0 || pos.y >= World.height) return false;
        return true;
    }

    private Vector3Int getPerpendicular(Vector3Int v, bool first)
    {
        if (first) return new Vector3Int(v.y, -v.x, 0);
        return new Vector3Int(-v.y, v.x, 0);
    }

    private Vector3Int getOpposite(Vector3Int v)
    {
        return new Vector3Int(-v.x, -v.y, 0);
    }
}

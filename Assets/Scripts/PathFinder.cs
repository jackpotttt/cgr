using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public delegate void PathCallback(PathFinder.PathNode node);

public static class PathFinder
{
    public class PathNode
    {
        public Vector2Int pos;
        public PathNode parent;
        public int cost;

        internal Vector2[] Flatten()
        {
            var node = this;
            int i = 1;
            while(node.parent != null)
            {
                node = node.parent;
                i++;
            }

            var nodes = new Vector2[i];

            node = this;            
            while (node!=null)
            {
                nodes[--i] = node.pos;
                node = node.parent;                
            }

            return nodes;
        }
    }

    public const int LARGE_ROAD_WEIGHT = 3;
    public const int MEDIUM_ROAD_WEIGHT = 4;
    public const int SMALL_ROAD_WEIGHT = 5;
    public const int BUILDING_WEIGHT = 0;

    public enum PathFinderRules { Offroad, UnweightedRoad, WeightedRoad, WaterOnly, Amphibious };

    public static List<Vector2Int> GetPath(List<Vector2Int> from, List<Vector2Int> to, PathFinderRules rules)
    {
        var pathRoot = PathFind(from, to, rules);

        if (pathRoot == null) return new List<Vector2Int>();
        else
        {
            var list = new List<Vector2Int>();
            TraceParentRecursive(pathRoot, list);
            return list;
        }
    }

    public static List<PathNode> GetWithin(List<Vector2Int> from, int distance, PathFinderRules rules, List<Vector2Int> closed, Employee employee = null)
    {
        var openSet = new SortedList<int, List<PathNode>>();
        var closedSet = new List<PathNode>();
        foreach (var c in closed) closedSet.Add(new PathNode { pos = c });

        foreach (var fro in from)
        {
            var p = 0; // GetPointCost(fro, rules);
            AddToOpenSet(openSet, p, new PathNode() { cost = p, parent = null, pos = fro });
        }

        while (openSet.Count > 0)
        {
            var best = openSet.First().Value[0];
            var currentPoint = best.pos;
            
            RemoveFromOpenSet(openSet, 0);
            if(!ClosedSetContains(closedSet, currentPoint))
                closedSet.Add(best);

            foreach (var point in GetNeighbors(currentPoint, rules))
            {
                if (!ClosedSetContains(closedSet, point) && (employee == null || employee.AllowedToMoveThrough(point.x, point.y)))
                {
                    var dist = best.cost + GetPointCost(point, rules);
                    if (dist > distance) continue;

                    int foundKey = -1;
                    PathNode found = null;
                    foreach (var set in openSet)
                    {
                        foreach (var node in set.Value)
                        {
                            if (node.pos == point)
                            {
                                found = node;
                                foundKey = set.Key;
                                break;
                            }
                        }
                    }
                    if (found != null)
                    {
                        if (found.cost > dist)
                        {
                            RemoveFromOpenSet(openSet, foundKey, found);
                            AddToOpenSet(openSet, dist, new PathNode() { cost = dist, parent = best, pos = point });
                        }
                    }
                    else
                    {
                        AddToOpenSet(openSet, dist, new PathNode() { cost = dist, parent = best, pos = point });
                    }
                }
            }
        }

        foreach (var close in closed) closedSet.RemoveAll(p => p.pos == close);

        return closedSet;
    }

    private static void TraceParentRecursive(PathNode pathNode, List<Vector2Int> list)
    {
        if(pathNode.parent != null)
        {
            TraceParentRecursive(pathNode.parent, list);
        }

        list.Add(pathNode.pos);
    }

    public static PathNode PathFind(List<Vector2Int> from, List<Vector2Int> to, PathFinderRules rules)
    {
        var openSet = new SortedList<int, List<PathNode>>();
        var closedSet = new List<Vector2Int>();

        foreach (var fro in from)
        {
            AddToOpenSet(openSet, CalculateEstimatedTotalPathDistance(fro, to, 0, rules, out int initialCost), new PathNode() { cost = 0, parent = null, pos = fro });
        }

        while (openSet.Count > 0)
        {
            var best = openSet.First().Value[0];
            var currentPoint = best.pos;
            if (to.Contains(currentPoint)) return best;
            RemoveFromOpenSet(openSet, 0);
            closedSet.Add(currentPoint);

            foreach (var point in GetNeighbors(currentPoint, rules))
            {
                if (!closedSet.Contains(point))
                {
                    var dist = CalculateEstimatedTotalPathDistance(point, to, best.cost, rules, out int nextCost);

                    int foundKey = -1;
                    PathNode found = null;
                    foreach(var set in openSet)
                    {
                        foreach (var node in set.Value)
                        {                            
                            if (node.pos == point)
                            {
                                found = node;
                                foundKey = set.Key;
                                break;
                            }
                        }
                    }
                    if (found != null)
                    {                        
                        if(found.cost > nextCost)
                        {
                            RemoveFromOpenSet(openSet, foundKey, found);
                            AddToOpenSet(openSet, dist, new PathNode() { cost = nextCost, parent = best, pos = point });
                        }                        
                    }
                    else
                    {
                        AddToOpenSet(openSet, dist, new PathNode() { cost = nextCost, parent = best, pos = point });
                    }
                }
            }
        }

        return null;
    }

    private static bool ClosedSetContains(List<PathNode> closedSet, Vector2 point)
    {
        foreach(var n in closedSet)
        {
            if (n.pos == point) return true;
        }
        return false;
    }

    private static void RemoveFromOpenSet(SortedList<int, List<PathNode>> openSet, int foundKey, PathNode found)
    {
        if (openSet.ContainsKey(foundKey))
        {
            if (openSet[foundKey].Count < 1) return;
            if (openSet[foundKey].Count > 1)
            {
                openSet[foundKey].Remove(found);
            }
            else if(openSet[foundKey][0].pos == found.pos)
            {
                openSet.Remove(foundKey);
            }
        }        
    }

    private static void RemoveFromOpenSet(SortedList<int, List<PathNode>> openSet, int index)
    {
        if (openSet.ElementAt(index).Value.Count < 1) return;
        if (openSet.ElementAt(index).Value.Count > 1)
        {
            openSet.ElementAt(index).Value.RemoveAt(0);
        }
        else openSet.RemoveAt(index);
    }

    private static void AddToOpenSet(SortedList<int, List<PathNode>> openSet, int cost, PathNode node)
    {
        if (openSet.ContainsKey(cost))
        {
            openSet[cost].Add(node);
        }
        else
        {
            openSet.Add(cost, new List<PathNode>() { node });
        }
    }

    private static int CalculateEstimatedTotalPathDistance(Vector2Int point, List<Vector2Int> to, int cost, PathFinderRules rules, out int costSoFar)
    {
        var estDist = cost + MinManhattan(point, to);
        costSoFar = cost;

        var stepCost = GetPointCost(point, rules);

        estDist += stepCost;
        costSoFar += stepCost;        

        return estDist;
    }

    private static List<Vector2Int> GetNeighbors(Vector2Int currentPoint, PathFinderRules rules)
    {
        if (World.GetMapObject(currentPoint, out bool ignored).type == TileType.building) return new List<Vector2Int>();

        var neighbors = new Vector2Int[]
            {
                new Vector2Int(currentPoint.x, currentPoint.y+1),
                new Vector2Int(currentPoint.x, currentPoint.y-1),
                new Vector2Int(currentPoint.x+1, currentPoint.y),
                new Vector2Int(currentPoint.x-1, currentPoint.y),
            };

        List<TileType> allowedTypes;
        if(rules == PathFinderRules.WaterOnly)
        {
            allowedTypes = new List<TileType>() { TileType.river, TileType.smallRoadRiver, TileType.mediumRoadRiver, TileType.largeRoadRiver };
        }
        else
        {
            allowedTypes = new List<TileType>() { TileType.smallRoad, TileType.mediumRoad, TileType.largeRoad, TileType.smallRoadRiver, TileType.mediumRoadRiver, TileType.largeRoadRiver };
            if (rules == PathFinderRules.Amphibious) allowedTypes.Add(TileType.river);
            else if (rules == PathFinderRules.Offroad)
            {
                allowedTypes.Add(TileType.empty);
                allowedTypes.Add(TileType.tree);
            }
        }

        var allowedNeighbors = new List<Vector2Int>();
        foreach (var point in neighbors)
        {
            var obj = World.GetMapObject(point, out bool inbounds);
            if (inbounds)
            {
                if (allowedTypes.Contains(obj.type))
                    allowedNeighbors.Add(point);

                else
                {
                    var b = obj.building as Building;
                    if (b != null && NetworkPlayer.localPlayer.buildings.Contains(b))
                        allowedNeighbors.Add(point);
                }
            }
        }
        return allowedNeighbors;
    }

    private static int MinManhattan(Vector2Int origin, List<Vector2Int> destinations)
    {
        var min = int.MaxValue;
        foreach(var dest in destinations)
        {
            var manhat = Math.Abs(origin.x - dest.x) + Math.Abs(origin.y - dest.y);
            if (manhat < min) min = manhat;
        }

        return min;
    }

    private static int GetPointCost(Vector2Int point, PathFinderRules rules)
    {
        if (rules == PathFinderRules.WeightedRoad)
        {
            switch (World.GetMapObject(point, out bool ignored).type)
            {
                case TileType.building:
                    return BUILDING_WEIGHT;
                case TileType.smallRoad:
                case TileType.smallRoadRiver:
                    return SMALL_ROAD_WEIGHT;
                case TileType.mediumRoad:
                case TileType.mediumRoadRiver:
                    return MEDIUM_ROAD_WEIGHT;
                default:
                    return LARGE_ROAD_WEIGHT;
            }
        }
        else
            return 1;
    }
}

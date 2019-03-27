using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static PathFinder;

public class TileSelectorMultipleChoice : MonoBehaviour
{
    public Color highlightedColor;
    public TileBase selectorTile;
    [HideInInspector]
    public Tilemap tileSelectorOverlay;


    public static TileSelectorMultipleChoice instance;
    private static bool isHighlighting;
    static List<Vector2Int> options;    
    static ClickCallback callback;
    static PathCallback pathCallback;
    static List<PathNode> storedPaths;

    void Awake()
    {
        instance = this;
    }

    public static void Setup(List<Vector2Int> points, ClickCallback cb, bool highlight)
    {
        TileSelectorMouse.instance.enabled = false;

        isHighlighting = highlight;
        options = points;
        storedPaths = null;
        callback = cb;
        pathCallback = null;
        instance.tileSelectorOverlay.ClearAllTiles();
        foreach (var p in options)
        {
            if (World.withinBounds(p))
                instance.tileSelectorOverlay.SetTile(new Vector3Int(p.x, p.y, 0), instance.selectorTile);
        }

        instance.enabled = true;
    }



    public static void Setup(List<PathNode> points, PathCallback pcb, bool highlight)
    {
        TileSelectorMouse.instance.enabled = false;

        isHighlighting = highlight;
        options = points.Select( p => p.pos ).ToList();
        storedPaths = points;
        callback = null;
        pathCallback = pcb;
        instance.tileSelectorOverlay.ClearAllTiles();
        foreach (var p in options)
        {
            if (World.withinBounds(p))
                instance.tileSelectorOverlay.SetTile(new Vector3Int(p.x, p.y, 0), instance.selectorTile);
        }

        instance.enabled = true;
    }

    private void OnDisable()
    {
        if (_isHighlighted) tileSelectorOverlay.SetColor(_highlighted, Color.white);
        _isHighlighted = false;
    }

    private bool _isHighlighted = false;
    private Vector3Int _highlighted;

    private void Update()
    {
        var point = CameraControl.instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var intPoint = new Vector2Int((int)point.x, (int)point.y);

        if (isHighlighting)
        {
            if (_isHighlighted) tileSelectorOverlay.SetColor(_highlighted, Color.white);
            _isHighlighted = false;
        }

        if (World.withinBounds(intPoint) && options.Contains(intPoint))
        {
            var intPoint3 = new Vector3Int(intPoint.x, intPoint.y, 0);

            if (isHighlighting)
            {
                tileSelectorOverlay.SetColor(intPoint3, highlightedColor);
                _isHighlighted = true;
                _highlighted = intPoint3;
            }

            if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                callback?.Invoke(intPoint.x, intPoint.y);
                if(pathCallback != null)
                {
                    var node = storedPaths.Find(p => p.pos == intPoint);
                    pathCallback(node);
                }
            }
        }
    }

    internal void TurnOff()
    {
        enabled = false;
        tileSelectorOverlay.ClearAllTiles();
    }
}

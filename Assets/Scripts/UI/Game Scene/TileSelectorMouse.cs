using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public delegate void ClickCallback(int x, int y);

public class TileSelectorMouse : MonoBehaviour {

    public TileBase selectorTile;

    [HideInInspector]
    public Tilemap tileSelectorOverlay;

    public static TileSelectorMouse instance;

    private static int width;
    private static int height;
    private static ClickCallback callback;
    private static int prevX, prevY;

    public static void Setup(int w, int h, ClickCallback c)
    {
        instance.enabled = true;
        TileSelectorMultipleChoice.instance.enabled = false;

        width = w;
        height = h;
        callback = c;
    }

	void Awake () {
        instance = this;
	}


    void Update()
    {
        var point = CameraControl.instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var x = (int)point.x;
        var y = (int)point.y;

        if (x != prevX || y != prevY)
        {
            tileSelectorOverlay.ClearAllTiles();
            prevX = x;
            prevY = y;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (World.withinBounds(x + i, y + j))
                        tileSelectorOverlay.SetTile(new Vector3Int(x + i, y + j, 0), selectorTile);
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            callback?.Invoke(x, y);
        }
    }
    
    internal static void Flash(Color color, float duration, float interval)
    {     
        instance.StartCoroutine(FlashRoutine(color, duration, interval));
    }
    
    private static IEnumerator FlashRoutine(Color color, float duration, float interval)
    {
        var time = 0f;        
        while (time < duration) {
            instance.tileSelectorOverlay.color = color;
            yield return new WaitForSeconds(interval);
            time += interval;
            instance.tileSelectorOverlay.color = Color.white;
            yield return new WaitForSeconds(interval);
            time += interval;
        }
    }

    internal void TurnOff()
    {
        enabled = false;
        tileSelectorOverlay.ClearAllTiles();
    }
}

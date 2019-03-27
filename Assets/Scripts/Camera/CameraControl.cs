using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    public float zoomSensitivity;
    public float minZoom = 5;
    public float maxZoom = 50;

    
    internal Camera mainCamera;    
    internal bool worldView = true;
    internal bool hierarchyView = false;

    private Vector3 cameraMovementSavedPoint;
    private const string CameraPanButtonName = "Camera";
    private const string CameraZoomAxisName = "Zoom";

    private Vector3 savedPos = new Vector3(0f, -5f, -10f);
    private float savedZoom = 10f;
    

    public static CameraControl instance;

    private void Awake()
    {
        instance = this;
    }

    void Start () {
        mainCamera = GetComponent<Camera>();
	}
    
    // Update is called once per frame
    void Update () {
        if (Input.GetButtonDown(CameraPanButtonName))
        {
            cameraMovementSavedPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetButton(CameraPanButtonName))
        {
            var diff = cameraMovementSavedPoint - mainCamera.ScreenToWorldPoint(Input.mousePosition);
            transform.position += diff;
            checkBounds();
        }

        var zoomAmount = zoomSensitivity * Input.GetAxis(CameraZoomAxisName);
        if (zoomAmount != 0)
        {
            var p = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mainCamera.orthographicSize -= zoomAmount;
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
            transform.position += p - mainCamera.ScreenToWorldPoint(Input.mousePosition);
            checkBounds();
        }

    }

    private void checkBounds()
    {
        var lowerLeft = mainCamera.ScreenToWorldPoint(Vector3.zero);
        var upperRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        var size = upperRight - lowerLeft;

        var minX = worldView ? -10 : -500;
        var maxX = worldView ? World.width + 10 : 500;
        var minY = worldView ? -10 : -500;
        var maxY = worldView ? World.height + 10 : 500;

        if (size.x < maxX - minX)
        {
            if (lowerLeft.x < minX)
                transform.position = new Vector3(transform.position.x + (minX - lowerLeft.x), transform.position.y, transform.position.z);
            else if (upperRight.x > maxX)
                transform.position = new Vector3(transform.position.x + (maxX - upperRight.x), transform.position.y, transform.position.z);
        }
        if (size.y < maxY - minY)
        {
            if (lowerLeft.y < minY)
                transform.position = new Vector3(transform.position.x, transform.position.y + (minY - lowerLeft.y), transform.position.z);
            else if (upperRight.y > maxY)
                transform.position = new Vector3(transform.position.x, transform.position.y + (maxY - upperRight.y), transform.position.z);
        }
    }

    public void SwapBetweenWorldAndHierarchyView()
    {
        worldView = !worldView;
        hierarchyView = !hierarchyView;

        var oldPos = savedPos;
        var oldZoom = savedZoom;

        savedPos = transform.position;
        savedZoom = mainCamera.orthographicSize;

        transform.position = oldPos;
        mainCamera.orthographicSize = oldZoom;
    }
}

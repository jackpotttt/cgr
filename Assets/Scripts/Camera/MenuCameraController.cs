using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraController : MonoBehaviour {

    Camera mainCamera;
    Quaternion defaultRotation;

    public int xSensitivity;
    public int ySensitivity;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        defaultRotation = mainCamera.transform.rotation;
    }

    void Update () {
        var offsetx = -((Input.mousePosition.y - (Screen.height / 2)) / Screen.height);
        var offsety = ((Input.mousePosition.x - (Screen.width / 2)) / Screen.width);

        mainCamera.transform.rotation = Quaternion.Euler((offsetx * xSensitivity) + defaultRotation.eulerAngles.x, (offsety * ySensitivity) + defaultRotation.eulerAngles.y, 0);
        //camera.transform.rotation = Quaternion.Euler(defaultRotation.eulerAngles.x, defaultRotation.eulerAngles.y, 0);
    }
}

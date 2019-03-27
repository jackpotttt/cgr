using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public Color houseColor = Color.HSVToRGB(0, 0, 0.7f);
    public Color supplierColor = Color.HSVToRGB(1, 0.9f, 0.95f);
    public Color utilityColor = Color.HSVToRGB(111f/360f, 0.9f, 0.8f);
    
    public static event ClickCallback LeftClickPixelEvent;
    public static event ClickCallback LeftClickGridEvent;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Normal View"))
        {
            showBuildingsNormal();
        }

        else if (Input.GetButtonDown("Type View"))
        {
            showBuildingsByType();
        }

        if (Input.GetMouseButtonUp(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            LeftClickPixelEvent?.Invoke((int)Input.mousePosition.x, (int)Input.mousePosition.y);

            var point = CameraControl.instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var intPoint = new Vector2Int((int)point.x, (int)point.y);

            LeftClickGridEvent?.Invoke(intPoint.x, intPoint.y);
        }

        
    }

    internal void showBuildingsByType()
    {
        foreach (var b in World.houses)
            b.SetColor(houseColor);
        foreach (var b in World.suppliers)
            b.SetColor(supplierColor);
        foreach (var b in World.utilities)
            b.SetColor(utilityColor);
    }

    internal void showBuildingsNormal()
    {
        foreach (var b in World.houses)
            b.SetColor(b.color);
        foreach (var b in World.suppliers)
            b.SetColor(b.color);
        foreach (var b in World.utilities)
            b.SetColor(b.color);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestaurantCustomizationMenu : MonoBehaviour {
        
    public Slider Hue;
    public Slider Saturation;
    public Slider Value;
    public Image RestaurantSprite;
    public InputField NameField;

    int BuildingTypeIndex;
    Image hueFill;
    Image satFill;
    Image valFill;    

    void Start () {
        
		hueFill = Hue.fillRect.GetComponent<Image>();
        satFill = Saturation.fillRect.GetComponent<Image>();
        valFill = Value.fillRect.GetComponent<Image>();

        BuildingTypeIndex = Random.Range(0, BuildingTypeManager.RestaurantTypes.Count);
        UpdateSprite();

        Hue.value = Random.value;
        Saturation.value = (Random.value * 0.5f) + 0.3f;
        Value.value = (Random.value * 0.4f) + 0.6f;
    }

    public void ColorChanged()
    {
        hueFill.color = Color.HSVToRGB(Hue.value, 1, 1);
        satFill.color = Color.HSVToRGB(Hue.value, Saturation.value, 0.6f);
        valFill.color = Color.HSVToRGB(Hue.value, 0.6f, Value.value);

        RestaurantSprite.color = Color.HSVToRGB(Hue.value, Saturation.value, Value.value);
    }

    public void NextButtonClicked()
    {
        BuildingTypeIndex++;
        UpdateSprite();
    }

    public void PrevButtonClicked()
    {
        BuildingTypeIndex--;
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        if (BuildingTypeIndex < 0) BuildingTypeIndex += BuildingTypeManager.RestaurantTypes.Count;
        BuildingTypeIndex %= BuildingTypeManager.RestaurantTypes.Count;


        RestaurantSprite.sprite = BuildingTypeManager.RestaurantTypes[BuildingTypeIndex].splash;
    }
	
    public void ReadyButtonClicked()
    {
        NetworkPlayer.localPlayer.Cmd_SyncSetupProperties(NameField.text, RestaurantSprite.color, BuildingTypeManager.RestaurantTypes[BuildingTypeIndex].id, NetworkPlayer.localPlayer.ready);
        GameController.PlayerIsCustomized();
    }
}

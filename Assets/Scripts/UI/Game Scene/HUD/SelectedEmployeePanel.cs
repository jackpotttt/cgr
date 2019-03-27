using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedEmployeePanel : MonoBehaviour
{    
    public Text TitleText;
    public Text WorkingText;
    public Text NameText;
    public Text MovesText;
    public Text HealthText;
    public Text SalaryText;
    public InputField RenameField;
    public Image IconImage;
    public GameObject PowerButtonPrefab;


    private Employee emp;
    private List<Button> powerButtonList;

    public void LoadEmployee(Employee emp)
    {
        this.emp = emp;
        UpdateInfo();
    }    

    public void FindEmployee()
    {
        if (CameraControl.instance.worldView)
            CameraControl.instance.transform.position = new Vector3(emp.location.x, emp.location.y, CameraControl.instance.transform.position.z);

        else if (CameraControl.instance.hierarchyView)
            CameraControl.instance.transform.position = new Vector3(emp.card.transform.position.x, emp.card.transform.position.y, CameraControl.instance.transform.position.z);
    }

    public void ShowRenameField()
    {
        RenameField.gameObject.SetActive(true);
        RenameField.text = NameText.text;

        NameText.gameObject.SetActive(false);
        RenameField.Select();
    }

    public void RenameComplete()
    {
        if (!string.IsNullOrEmpty(RenameField.text))
        {
            emp.name = RenameField.text;
            NameText.text = emp.name;
            NetworkPlayer.localPlayer.Cmd_RequestEmployeeNameChange(emp.id, emp.name);
        }
        
        RenameField.text = "";

        RenameField.gameObject.SetActive(false);
        NameText.gameObject.SetActive(true);
    }

    internal void UpdateInfo()
    {
        NameText.text = emp.name;
        WorkingText.text = emp.vacation ? "On Break" : "Working";
        TitleText.text = emp.type.displayName;
        SalaryText.text = "$" + emp.salary;
        IconImage.sprite = emp.type.mapSprite;
        IconImage.color = emp.player.playerColor;
        MovesText.text = $"{emp.movedThisTurn} / {emp.type.movesPerTurn}   (Range {emp.movementSpeed})";

        float i = 0f, k = 0f;
        var j = 0;
        var rect = GetComponent<RectTransform>();
        var growth = new Vector2(130, 0);
        rect.sizeDelta = new Vector2(525, 200);

        if (powerButtonList == null) powerButtonList = new List<Button>();
        foreach (var b in powerButtonList)
            Destroy(b.gameObject);

        powerButtonList.Clear();

        if (emp.friendly)
        {
            foreach (var power in emp.powers)
            {
                var button = Instantiate(PowerButtonPrefab, transform).GetComponent<Button>();
                var buttonRect = button.GetComponent<RectTransform>();
                buttonRect.anchoredPosition += new Vector2(i, k);
                if (j % 2 == 0)
                {
                    rect.sizeDelta += growth;
                    k -= 70f;
                }
                else
                {
                    i += growth.x;
                    k += 70f;
                }

                power.buttonText = button.GetComponentInChildren<Text>();
                power.SetButtonText();

                button.onClick.AddListener(() => emp.UseAbility(power));
                j++;
                powerButtonList.Add(button);
            }
        }
    }
}

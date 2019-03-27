using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hireable : MonoBehaviour, IPointerClickHandler
{
    public EmployeeType employeeType;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer highlightedRenderer;
    public SpriteRenderer plusSign;
    public Color highlightedColor;
    public Color friendlySelectedColor;
    public Color enemySelectedColor;

    internal Action OnClick;
    internal NetworkPlayer hiredByPlayer;
    internal Building building;
    internal int index;
    internal string employeeName;

    private Employee hiredEmployee;

    internal void Init(EmployeeType type, Building b, int i)
    {
        index = i;
        employeeType = type;
        building = b;
        spriteRenderer.sprite = type.mapSprite;
    }

    internal void Highlight()
    {
        highlightedRenderer.enabled = true;
        highlightedRenderer.color = highlightedColor;
    }

    internal void Select()
    {
        highlightedRenderer.enabled = true;
        highlightedRenderer.color = hiredEmployee.friendly ? friendlySelectedColor : enemySelectedColor;
    }

    internal void NetworkHired()
    {           
        NetworkPlayer.localPlayer.Cmd_Hire(building.id, index);
    }

    internal void Hire(NetworkPlayer player, int id, string name)
    {
        if(player.isLocalPlayer)
            GameController.CurrentPowerWasUsed();

        hiredByPlayer = player;
        spriteRenderer.color = player.playerColor;
        plusSign.gameObject.SetActive(false);

        hiredEmployee = new Employee(employeeType, id, name, building, player, this);
        this.employeeName = name;

        if (hiredEmployee.friendly)
        {
            Hierarchy.instance.AddEmployeeCard(hiredEmployee);
        }
        OnClick = null;        

        var playerName = (hiredEmployee.friendly ? "You" : player.buisnessName);
        GameUI.instance.AddTempMessage($"{playerName} hired {hiredEmployee.name} the {hiredEmployee.type.displayName}");
    }

    internal void Departed()
    {
        var color = spriteRenderer.color;
        color.a = 0.75f;
        spriteRenderer.color = color;
    }

    internal void UnDeparted()
    {
        var color = spriteRenderer.color;
        color.a = 1f;
        spriteRenderer.color = color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClick != null)
        {
            OnClick();
        }
        else if(hiredEmployee != null)
        {
            if (hiredEmployee.OnClick != null)
            {
                hiredEmployee.OnClick();
            }
            else
            {
                if (GameController.selectedItem == hiredEmployee)
                    GameController.SelectItem(null);
                else
                    GameController.SelectItem(hiredEmployee);
            }
        }
    }

    internal void RemoveHighlight()
    {
        highlightedRenderer.enabled = false;
    }

    internal void Fire()
    {
        spriteRenderer.color = Color.white;
        plusSign.gameObject.SetActive(false);
        hiredByPlayer = null;
        hiredEmployee = null;
    }
}

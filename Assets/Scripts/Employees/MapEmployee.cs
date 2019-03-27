using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapEmployee : MonoBehaviour, IPointerClickHandler
{
    public Employee employee;

    public SpriteRenderer spriteRenderer;
    public SpriteRenderer selectedRenderer;

    public Color highlightedColor;
    public Color friendlySelectedColor;
    public Color enemySelectedColor;

    public void Init(Employee e)
    {
        employee = e;
        spriteRenderer.sprite = employee.type.mapSprite;
        spriteRenderer.color = employee.player.playerColor;

        if (e.selected) Select();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (employee.OnClick != null)
        {
            employee.OnClick();
        }
        else
        {
            if (employee.selected) GameController.SelectItem(null);
            else GameController.SelectItem(employee);
        }
    }

    internal void Select()
    {
        selectedRenderer.enabled = true;
        selectedRenderer.color = (employee.friendly ? friendlySelectedColor : enemySelectedColor);
    }

    internal void RemoveHighlight()
    {
        selectedRenderer.enabled = false;
    }

    internal void Highlight()
    {
        selectedRenderer.enabled = true;
        selectedRenderer.color = highlightedColor;
    }
}

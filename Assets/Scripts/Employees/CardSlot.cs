using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlot : MonoBehaviour, ISelectable
{
    public RectTransform rectTransform;
    public RectTransform topMiddlePoint;
    public GameObject selectedSprite;

    internal EmployeeCard parent;
    internal EmployeeCard occupied;

    public void Select()
    {
        selectedSprite.SetActive(true);
    }

    public void Deselect()
    {
        selectedSprite.SetActive(false);
    }

    public bool IsSelected()
    {
        return selectedSprite.activeInHierarchy;
    }
}

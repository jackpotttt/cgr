
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EmployeeCard : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler {

    public SpriteRenderer selectedSprite;
    public Employee employee;
    public RectTransform rectTransform;
    public RectTransform topMiddlePoint;
    public RectTransform bottomMiddlePoint;

    public Color highlightedColor;
    public Color friendlySelectedColor;
    public Color enemySelectedColor;

    internal LineRenderer managementLine;
    internal List<CardSlot> slots = new List<CardSlot>();
    internal CardSlot occupyingSlot;

    private Vector2 savedPosition;
    private Vector2 savedMousePosition;
    private CardSlot overSlot;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (employee.type.id != 0 && GameController.phase == GameController.Phase.Restructuring)
        {
            savedPosition = rectTransform.position;
            savedMousePosition = CameraControl.instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);

            Hierarchy.instance.RemoveSlots(this);
        }        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (employee.type.id != 0 && GameController.phase == GameController.Phase.Restructuring)
        {
            Vector2 newMousePosition = CameraControl.instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);
            rectTransform.position = newMousePosition - savedMousePosition + savedPosition;

            var colliders = Physics2D.OverlapCircleAll(rectTransform.position, 4);
            CardSlot best = null;
            float closest = 0f;
            foreach(var c in colliders)
            {
                var s = c.GetComponent<CardSlot>();
                if (s != null && employee.CanOccupySlot(s))
                {
                    if (best == null)
                    {
                        best = s;
                        closest = (best.transform.position - rectTransform.position).magnitude;
                    }
                    else
                    {
                        if ((c.transform.position - rectTransform.position).magnitude < closest) best = s;
                    }
                }
            }

            if(best == null)
            {
                if(overSlot != null)
                {
                    overSlot.Deselect();
                    overSlot = null;
                }
            }
            else
            {
                if(overSlot != null)
                {
                    if (best != overSlot)
                    {
                        overSlot.Deselect();
                        overSlot = best;
                        overSlot.Select();
                    }
                }
                else
                {
                    overSlot = best;
                    overSlot.Select();
                }
            }
        }
    }

    internal void Vacationize()
    {
        Hierarchy.instance.RemoveSlots(this);

        if (occupyingSlot != null)
        {
            occupyingSlot.occupied = null;
            occupyingSlot = null;
        }

        if (rectTransform.position.y < 5)
            rectTransform.position = new Vector3(rectTransform.position.x, 5, rectTransform.position.z);

        while(Physics2D.OverlapCircleAll(rectTransform.position, 2).Length > 2)
        {
            rectTransform.position += Vector3.right * 3.1f;
        }

        employee.vacation = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (employee.type.id != 0 && GameController.phase == GameController.Phase.Restructuring)
        {
            if(overSlot != null)
            {
                if (occupyingSlot != null) occupyingSlot.occupied = null;

                rectTransform.position = overSlot.rectTransform.position;
                occupyingSlot = overSlot;
                occupyingSlot.occupied = this;

                overSlot.Deselect();
                overSlot = null;

                employee.vacation = false;
                Hierarchy.instance.ShowSlots(this);
            }
            else
            {
                Vacationize();
            }
        }
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

    public void Select()
    {
        selectedSprite.gameObject.SetActive(true);
        selectedSprite.color = (employee.friendly ? friendlySelectedColor : enemySelectedColor);
    }

    public void Highlight()
    {
        selectedSprite.gameObject.SetActive(true);
        selectedSprite.color = highlightedColor;
    }

    internal void RemoveHighlight()
    {        
        selectedSprite.gameObject.SetActive(false);        
    }
}

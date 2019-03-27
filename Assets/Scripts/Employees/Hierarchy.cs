using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hierarchy : MonoBehaviour, IPointerClickHandler
{
    public GameObject cardPrefab;
    public GameObject slotPrefab;
    public GameObject linePrefab;

    public static Hierarchy instance;

    private const float minDistHoriz = 3.2f;
    private const float distVert = 5;

    private List<EmployeeCard> vacationQueue;

    public Hierarchy()
    {
        instance = this;
        vacationQueue = new List<EmployeeCard>();
    }

    public void Update()
    {
        foreach(var c in vacationQueue)
        {
            if(c.isActiveAndEnabled)
                c.Vacationize();
        }

        vacationQueue.Clear();
    }

    internal void CreateCeoCard(Employee ceo)
    {
        var card = AddEmployeeCard(ceo);
        card.rectTransform.anchoredPosition = new Vector3(0, -5, 0);

        ShowSlots(card);
    }

    internal void ShowSlots(EmployeeCard card)
    {
        float slots = card.employee.type.managementSlots;

        if (slots > 0)
        {
            if(slots > 1 && card.employee.type.id != 0)
                IncreaseWidth(card, slots);

            var line = Instantiate(linePrefab, transform).GetComponent<LineRenderer>();
            card.managementLine = line;
            line.positionCount = (int)slots * 2;

            card.slots.Clear();
            for (int i = 0; i < slots; i++)
            {
                var x = ((float)i - ((slots - 1) / 2f)) * minDistHoriz;
                var slot = Instantiate(slotPrefab, transform).GetComponent<CardSlot>();
                slot.parent = card;
                card.slots.Add(slot);
                slot.rectTransform.anchoredPosition = new Vector3(card.rectTransform.position.x + x, card.rectTransform.position.y - distVert, 0);

                line.SetPosition(2 * i, card.bottomMiddlePoint.position);
                line.SetPosition((2 * i) + 1, slot.topMiddlePoint.position);
            }
        }
    }

    internal void RemoveSlots(EmployeeCard card)
    {
        if(card.slots.Count > 1)
            DecreaseWidth(card, card.slots.Count);

        if(card.managementLine != null)
            GameObject.Destroy(card.managementLine.gameObject);
        foreach (var slot in card.slots)
        {
            if (slot.occupied != null)
                slot.occupied.Vacationize();
            GameObject.Destroy(slot.gameObject);
        }

        card.slots.Clear();
    }

    private void IncreaseWidth(EmployeeCard card, float slots)
    {
        if (card.occupyingSlot.parent.employee.type.id != 0)
        {
            IncreaseWidth(card.occupyingSlot.parent, slots);
        }

        float width = (slots - 1) * (minDistHoriz / 2);

        int i = 0;
        foreach (var slot in card.occupyingSlot.parent.slots)
        {
            if (slot == card.occupyingSlot) break;
            else i++;
        }

        for (int j = 0; j < i; j++)
        {
            Shift(card.occupyingSlot.parent.slots[j], -width, j);
        }

        for (int j = i + 1; j < card.occupyingSlot.parent.slots.Count; j++)
        {
            Shift(card.occupyingSlot.parent.slots[j], width, j);
        }
    }

    private void Shift(CardSlot cardSlot, float v, int i)
    {
        var offset = new Vector3(v, 0);
        cardSlot.rectTransform.position += offset;

        cardSlot.parent.managementLine.SetPosition((2 * i) + 1, cardSlot.topMiddlePoint.position);

        if(cardSlot.occupied != null)
        {
            cardSlot.occupied.rectTransform.position += offset;
            int j = 0;
            foreach (var s in cardSlot.occupied.slots)
            {
                cardSlot.occupied.managementLine.SetPosition(2 * j, cardSlot.occupied.bottomMiddlePoint.position);
                Shift(s, v, j++);
            }
        }
    }

    private void DecreaseWidth(EmployeeCard card, int slots)
    {
        if (card.occupyingSlot.parent.employee.type.id != 0)
        {
            DecreaseWidth(card.occupyingSlot.parent, slots);
        }

        float width = (slots - 1) * (minDistHoriz / 2);
        width *= -1;

        int i = 0;
        foreach (var slot in card.occupyingSlot.parent.slots)
        {
            if (slot == card.occupyingSlot) break;
            else i++;
        }

        for (int j = 0; j < i; j++)
        {
            Shift(card.occupyingSlot.parent.slots[j], -width, j);
        }

        for (int j = i + 1; j < card.occupyingSlot.parent.slots.Count; j++)
        {
            Shift(card.occupyingSlot.parent.slots[j], width, j);
        }
    }

    internal EmployeeCard AddEmployeeCard(Employee emp)
    {
        var card = Instantiate(cardPrefab, transform).GetComponent<EmployeeCard>();

        card.employee = emp;
        emp.card = card;

        card.rectTransform.anchoredPosition = new Vector3(0, 5, 0);

        card.GetComponent<SpriteRenderer>().sprite = emp.type.cardSprite;

        if (emp.type.id != 0)
        {
            if (isActiveAndEnabled)
                card.Vacationize();
            else
                vacationQueue.Add(card);
        }
        return card;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
            GameController.SelectItem(null);
    }
}

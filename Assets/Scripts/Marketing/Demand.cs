using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demand : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    internal FoodType type;

    public void Setup(FoodType t)
    {
        type = t;
        spriteRenderer.sprite = type.sprite;
    }
}

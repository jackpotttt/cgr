using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EmployeeType {

    public string displayName;

    [HideInInspector]
    public int id;

    public int tier;    
    public List<int> upgrades;

    public int movement;
    public int movesPerTurn = 1;
    public int sightRange;
    public float salary;    
    public int managementSlots;
    public int managementDepth;

    public Sprite cardSprite;
    public Sprite mapSprite;
    public int mapSpawnWeightModifier;

    public List<EmployeePowerDefinition> powers;

    public override string ToString()
    {
        return displayName;
    }
}

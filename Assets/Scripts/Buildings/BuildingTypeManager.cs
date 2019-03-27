using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BuildingTypeManager : MonoBehaviour
{    
    public BuildingType[] allBuildingTypes;

    public static List<BuildingType> RestaurantTypes;
    public static BuildingTypeManager instance;

    public void Awake()
    {
        instance = this;
        RestaurantTypes = new List<BuildingType>();

        for (int i = 0; i < allBuildingTypes.Length; i++)
        {
            var b = allBuildingTypes[i];
            if (b != null)
            {
                switch (b.category)
                {
                    case BuildingCategory.Null:
                        allBuildingTypes[i] = null;
                        break;
                    case BuildingCategory.Restaurant:
                        RestaurantTypes.Add(b);
                        b.Initialize(i);
                        break;
                    default:
                        b.Initialize(i);
                        break;
                }
            }
        }
    }

    public static List<BuildingType> GetGenerationTypes()
    {
        var list = new List<BuildingType>();
        foreach(var type in instance.allBuildingTypes)
        {
            if(type != null && type.category != BuildingCategory.Restaurant && type.category != BuildingCategory.Null)
            {
                list.Add(type);
            }
        }
        return list;
    }
}

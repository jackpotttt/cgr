using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EmployeePowerDefinition
{
    public delegate void EmployeePowerCallback(Employee e, EmployeePower p);

    public string summary;
    public int usesPerRound = 1;
    public int callbackIndex;
    public int range1;
    public int range2;

    public static EmployeePowerCallback[] powerLookup = new EmployeePowerCallback[]
    {
        None, // 0
        Hire, // 1
        Fire, // 2
        PlaceFlier, //3
    };

    public static void None(Employee e, EmployeePower p)
    {

    }

    private static List<Hireable> hireablesToCancel;
    public static void Hire(Employee e, EmployeePower p)
    {        
        GameController.ToggleShowHires(true, true);
        var points = World.GetValidPointsWithin(e.location, p.range1);

        TileSelectorMultipleChoice.Setup(points, null, false);
        var buildings = World.GetBuildingsWithin(points);
        hireablesToCancel = new List<Hireable>();

        foreach(var b in buildings)
        {
            foreach(var h in b.hireables)
            {
                if (h.hiredByPlayer == null)
                {
                    h.Highlight();
                    h.OnClick = h.NetworkHired;
                    hireablesToCancel.Add(h);
                }
            }
        }

        p.OnCancel = CancelHire;
    }    

    public static void CancelHire()
    {
        TileSelectorMultipleChoice.instance.TurnOff();
        foreach (var h in hireablesToCancel)
        {
            h.RemoveHighlight();
            h.OnClick = null;
        }
        hireablesToCancel = null;
    }

    private static List<Employee> employees;
    public static void Fire(Employee e, EmployeePower p)
    {
        var points = World.GetValidPointsWithin(e.location, p.range1);
        TileSelectorMultipleChoice.Setup(points, null, false);
        employees = EmployeeManager.GetEmployeesWithin(points, true, false);

        foreach(var emp in employees)
        {
            if (emp.type.id != 0)
            {
                emp.Highlight();
                emp.OnClick = emp.Fire;
            }
        }

        p.OnCancel = CancelFire;
    }

    public static void CancelFire()
    {
        TileSelectorMultipleChoice.instance.TurnOff();
        foreach (var e in employees)
        {
            if(e.type.id != 0)
            {
                e.RemoveHighlight();
                e.OnClick = null;
            }
        }        
    }
        
    private static void PlaceFlier(Employee e, EmployeePower p)
    {
        var points = World.GetValidPointsWithin(e.location, p.range1, true);
        TileSelectorMultipleChoice.Setup(points, (x, y) => PlaceFlierLocationChosen(e, p, x, y), true);
    }

    private static void PlaceFlierLocationChosen(Employee e, EmployeePower p, int x, int y)
    {
        var points = World.GetValidPointsWithin(new Vector2Int(x, y), p.range2);
        TileSelectorMultipleChoice.Setup(points, null, false);
        GameUI.instance.dialog.Show("Confirm", "Are you sure you want to place a flier here?", "Yes", null, "No", null);
    }
}

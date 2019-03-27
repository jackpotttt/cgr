using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeManager : MonoBehaviour {

    public static EmployeeManager instance;    

    public TextAsset firstNamesText;
    public TextAsset lastNamesText;
    public GameObject mapEmployeePrefab;
    public GameObject hireablePrefab;
    public List<EmployeeType> AllEmployeeTypes;

    internal List<Employee> AllEmployees;

    private string[] firstNames;
    private string[] lastNames;
    private int nextId = 0; // ONLY USE IN SERVER
    private List<int> weightedTypeListForRandomHireableSpawn;

    public void Awake()
    {
        instance = this;
        lastNames = lastNamesText.text.Split(new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries );
        firstNames = firstNamesText.text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        weightedTypeListForRandomHireableSpawn = new List<int>();
        AllEmployees = new List<Employee>();
    }

    public void Start()
    {
        for (int i = 0; i<AllEmployeeTypes.Count; i++)
        {
            AllEmployeeTypes[i].id = i;

            var weight = 12 - AllEmployeeTypes[i].tier + AllEmployeeTypes[i].mapSpawnWeightModifier;
            for (int j = 0; j < weight; j++)
                weightedTypeListForRandomHireableSpawn.Add(i);
        }
    }

    public void AddEmployeeToGlobalCatalog(Employee e)
    {
        while (AllEmployees.Count <= e.id) AllEmployees.Add(null);
        AllEmployees[e.id] = e;
    }

    public int NextEmployeeId() //ONLY CALL THIS IN SERVER
    {
        return nextId++;
    }

    public string GenerateName()
    {
        return firstNames[UnityEngine.Random.Range(0, firstNames.Length - 1)] + " " + NameFix(lastNames[UnityEngine.Random.Range(0, lastNames.Length - 1)]);
    }

    private string NameFix(string v)
    {
        return v.Substring(0, 1).ToUpper() + v.Substring(1).ToLower();
    }

    internal static List<Employee> GetEmployeesWithin(List<Vector2Int> points, bool getFriendly, bool getEnemy)
    {
        var employees = new List<Employee>();
        foreach(var p in points)
        {
            var mo = World.map[p.x, p.y];
            if (mo.employee != null)
            {
                if ((mo.employee.friendly && getFriendly) || (!mo.employee.friendly && getEnemy))
                    employees.Add(mo.employee);
            }
            else
            {
                var b = mo.building as Building;
                if (b != null)
                {
                    foreach (var e in b.employees)
                    {
                        if ((e.friendly && getFriendly) || (!e.friendly && getEnemy))
                            employees.Add(e);
                    }
                }
            }
        }

        return employees;
    }

    internal static void ShowMovementFor(Employee employee)
    {        
        List<Vector2Int> origin, closed;
        if (employee.building != null)
        {
            origin = employee.building.entrances;
            closed = employee.building.tiles;
        }
        else
        {
            origin = new List<Vector2Int>() { employee.location };
            closed = origin;
        }

        var path = PathFinder.GetWithin(origin, employee.movementSpeed, PathFinder.PathFinderRules.WeightedRoad, closed, employee);
        var toRemove = new List<PathFinder.PathNode>();
        foreach(var node in path)        
            if (!employee.AllowedToMoveTo(node.pos.x, node.pos.y))
                toRemove.Add(node);
        foreach (var node in toRemove)
            path.Remove(node);
        
        TileSelectorMultipleChoice.Setup(path, employee.NetworkRequestMoveTo, true);
    }

    internal EmployeeType RandomHireable()
    {
        return AllEmployeeTypes[weightedTypeListForRandomHireableSpawn[UnityEngine.Random.Range(0, weightedTypeListForRandomHireableSpawn.Count)]];
    }

    public void Remove(int employeeId)
    {
        var e = AllEmployees[employeeId];

        if (e.card != null)
        {
            e.card.Vacationize();
            Destroy(e.card.gameObject);
        }
        if (e.mapEmployee != null)
        {
            Destroy(e.mapEmployee);
        }

        AllEmployees[employeeId] = null;
        e.player.employees.Remove(e);

        if (e.building != null) e.building.employees.Remove(e);
        else World.map[e.location.x, e.location.y].employee = null;
    }
}

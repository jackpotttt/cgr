using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PathFinder;

public class Employee : ISelectable, IHighlightable {

    public int id;
    public EmployeeType type;
    public string name;
    public bool vacation;
    public Building building;
    public Vector2Int location;
    public EmployeeCard card;
    public MapEmployee mapEmployee;
    public float salary = 0f;
    public int movementSpeed;
    public NetworkPlayer player;
    public bool friendly = false;
    public Building home;
    public Hireable hireable;
    public int movedThisTurn;
    public bool selected = false;

    public List<EmployeePower> powers;

    public Action OnClick;

    public Employee(EmployeeType type, int id, string name, Building building, NetworkPlayer player, Hireable hireable = null)
    {
        player.employees.Add(this);
        building.employees.Add(this);

        this.player = player;
        this.id = id;
        this.type = type;
        this.building = building;        
        this.hireable = hireable;
        this.name = name;

        if(type.id != 0)
            home = building;

        location = building.origin;
        vacation = true;
        
        if (player == NetworkPlayer.localPlayer) friendly = true;

        movementSpeed = type.movement;
        salary = type.salary;

        EmployeeManager.instance.AddEmployeeToGlobalCatalog(this);

        powers = new List<EmployeePower>();
        foreach (var p in type.powers)        
            powers.Add(new EmployeePower(p));
    }

    public void ResetRound()
    {
        movedThisTurn = 0;

        foreach(var power in powers)
        {
            power.ResetRound();
        }
    }

    public override string ToString()
    {
        return type.displayName + " - " + name;
    }

    internal void NetworkRequestMoveTo(PathNode node)
    {
        var path = node.Flatten();
        NetworkPlayer.localPlayer.Cmd_MoveEmployeeTo(id, path);
    }

    internal bool AllowedToMoveTo(int x, int y)
    {
        return World.map[x,y].employee == null;
    }

    internal void Fire()
    {
        GameController.CurrentPowerWasUsed();
        NetworkPlayer.localPlayer.Cmd_Fire(id);
    }

    internal bool AllowedToMoveThrough(int x, int y)
    {
        var e = World.map[x, y].employee;
        return e == null || e.player == player;
    }

    private void AboutToMove()
    {
        if (building == null)
        {
            World.map[location.x, location.y].employee = null;
        }

        else
        {
            if (home != null && hireable != null && building == home)
            {
                hireable.Departed();
            }
            building.employees.Remove(this);
        }
    }

    internal void MoveTo(Building newBuilding)
    {
        AboutToMove();
        location = newBuilding.origin;
        if (mapEmployee != null)
        {
            GameObject.Destroy(mapEmployee.gameObject);
            mapEmployee = null;
        }
        building = newBuilding;
        newBuilding.employees.Add(this);
        HasMoved();
    }
    
    internal void MoveTo(Vector2Int p)
    {
        AboutToMove();
        location = p;
        if (mapEmployee == null)
        {
            CreateMapEmployee();
        }
        mapEmployee.transform.position = new Vector3(p.x, p.y);
        World.map[p.x, p.y].employee = this;
        building = null;
        HasMoved();
    }

    private void HasMoved()
    {
        movedThisTurn++;
        UpdateVisibility();
        if (GameController.selectedItem == this)
        {
            GameUI.instance.hud.SelectedEmployeePanel.UpdateInfo();
        }
    }

    public void ShowMovement()
    {
        if (CanMove() && !vacation && GameController.phase == GameController.Phase.Operations && friendly)
            EmployeeManager.ShowMovementFor(this);
    }

    internal bool CanOccupySlot(CardSlot s)
    {
        return s.occupied == null;
    }   

    private void CreateMapEmployee()
    {
        mapEmployee = GameObject.Instantiate(EmployeeManager.instance.mapEmployeePrefab, World.instance.transform).GetComponent<MapEmployee>();
        mapEmployee.Init(this);
    }

    private void UpdateVisibility()
    {
        if (player != NetworkPlayer.localPlayer)
        {
            //Check if should be invisible?
        }
        else
        {
            //Run through enemy objs and check vis against this
        }
    }

    internal void GoOnVacation()
    {
        vacation = true;
        MoveTo(home);
        hireable.UnDeparted();
    }

    public void UseAbility(EmployeePower power)
    {
        if (vacation || GameController.phase != GameController.Phase.Operations) return;
            
        if(GameController.selectedPower == power)
        {
            GameController.EndPower();
            Select();
        }

        else if (power.CanUse())
        {
            GameController.EndPower();
            GameController.selectedPower = power;
            power.Use(this);
        }        
    }   

    #region ISelectable

    public void Select()
    {
        selected = true;
        card?.Select();
        mapEmployee?.Select();
        hireable?.Select();
        
        ShowMovement();
    }

    public bool CanMove()
    {
        return movedThisTurn < type.movesPerTurn;
    }

    public void Deselect()
    {
        selected = false;

        card?.RemoveHighlight();
        mapEmployee?.RemoveHighlight();
        hireable?.RemoveHighlight();

        TileSelectorMultipleChoice.instance.TurnOff();

        foreach(var p in powers)
        {
            p.buttonText = null;
        }
    }

    public bool IsSelected()
    {
        return selected;
    }

    #endregion

    #region IHighlightable

    public void Highlight()
    {
        card?.Highlight();
        mapEmployee?.Highlight();
        hireable?.Highlight();
    }

    public void RemoveHighlight()
    {
        card?.RemoveHighlight();
        mapEmployee?.RemoveHighlight();
        hireable?.RemoveHighlight();
    }

    #endregion
}

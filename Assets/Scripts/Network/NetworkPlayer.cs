
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayer : NetworkBehaviour {

    public int restaurantStyleTypeId;
    internal string buisnessName = "Unknown";
    internal Color playerColor = Color.white;
    internal BuildingType RestaurantStyle;
    internal bool ready = false;
    internal List<Building> restaurants;
    internal List<Building> buildings;
    public List<Employee> employees;

    public static NetworkPlayer localPlayer;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        buildings = new List<Building>();
        restaurants = new List<Building>();
        employees = new List<Employee>();
        if (isLocalPlayer)
        {
            localPlayer = this;
            Cmd_SetName();
        }
    }

    [Command]
    internal void Cmd_Hire(int buildingId, int hireableId)
    {
        var hireable = World.allBuildings[buildingId]?.hireables[hireableId];

        if (hireable == null || hireable.hiredByPlayer != null) return;
        hireable.hiredByPlayer = this;

        var name = hireable.employeeName;
        if (string.IsNullOrEmpty(name)) name = EmployeeManager.instance.GenerateName();

        Rpc_Hire(buildingId, hireableId, EmployeeManager.instance.NextEmployeeId(), name);
    }

    [ClientRpc]
    private void Rpc_Hire(int buildingId, int hireableId, int employeeId, string name)
    {
        var hireable = World.allBuildings[buildingId]?.hireables[hireableId];
        hireable.Hire(this, employeeId, name);
    }

    [Command]
    public void Cmd_MoveEmployeeTo(int employeeId, Vector2[] points)
    {
        var emp = EmployeeManager.instance.AllEmployees[employeeId];
        if (emp == null) return;

        Vector2 bestPossible = new Vector2();
        bool canMove = false;

        foreach (var p in points) {
            if (emp.AllowedToMoveThrough((int)p.x, (int)p.y))
            {
                if(emp.AllowedToMoveTo((int)p.x, (int)p.y))
                {
                    bestPossible = p;
                    canMove = true;
                }
            }
            else
            {
                break;
            }
        }
        
        if(canMove)
            Rpc_MoveEmployeeTo(employeeId, bestPossible);
        else
            Rpc_EmployeeCannotMove(employeeId);
    }

    [Command]
    public void Cmd_Fire(int employeeId)
    {
        Rpc_Fire(employeeId);
    }

    [ClientRpc]
    public void Rpc_Fire(int employeeId)
    {        
        var e = EmployeeManager.instance.AllEmployees[employeeId];        
        if (e.hireable != null)
        {
            e.hireable.Fire();
        }
        EmployeeManager.instance.Remove(employeeId);
    }

    [ClientRpc]
    public void Rpc_EmployeeCannotMove(int employeeId)
    {
        if(isLocalPlayer)
            GameUI.instance.AddTempMessage("Employee cannot move there");
    }

    [ClientRpc]
    public void Rpc_MoveEmployeeTo(int employeeId, Vector2 p)
    {
        var intp = new Vector2Int((int)p.x, (int)p.y);
        var emp = EmployeeManager.instance.AllEmployees[employeeId];
        var obj = World.GetMapObject(intp, out bool ignored);

        if (isLocalPlayer)
            TileSelectorMultipleChoice.instance.TurnOff();

        var b = obj.building as Building;
        if (b != null)
            emp.MoveTo(b);
        else
            emp.MoveTo(intp);    
    }

    [Command]
    public void Cmd_RequestEmployeeNameChange(int id, string name)
    {
        Rpc_RequestEmployeeNameChange(id, name);
    }

    [ClientRpc]
    public void Rpc_RequestEmployeeNameChange(int id, string name)
    {
        var e = EmployeeManager.instance.AllEmployees[id];
        e.name = name;

        if (GameController.selectedItem == e) GameUI.instance.hud.SelectedEmployeePanel.UpdateInfo();
    }

    #region SetReady

    public void SetReady(bool ready)
    {
        this.ready = ready;
        Cmd_SetReady(ready);     
    }

    internal void RestructuringDone()
    {
        var vacationList = new List<int>();
        var notVacationList = new List<int>();
        foreach (var e in employees)
        {
            if (e.vacation)
                vacationList.Add(e.id);
            else
                notVacationList.Add(e.id);
        }

        Cmd_UpdateVacationers(vacationList.ToArray(), notVacationList.ToArray());
    }

    [Command]
    private void Cmd_UpdateVacationers(int[] v, int[] nv)
    {
        Rpc_UpdateVacationers(v, nv);
    }

    [ClientRpc]
    private void Rpc_UpdateVacationers(int[] v, int[] nv)
    {
        foreach (var id in v)
        {
            var e = EmployeeManager.instance.AllEmployees[id];            
            e.GoOnVacation();
        }

        foreach (var id in nv)
        {
            EmployeeManager.instance.AllEmployees[id].vacation = false;
        }
    }

    [Command]
    private void Cmd_SetReady(bool ready)
    {
        Rpc_SetReady(ready);
    }
    [ClientRpc]
    private void Rpc_SetReady(bool ready)
    {
        this.ready = ready;

        var allReady = true;
        foreach(var player in GameController.players)
        {
            if (!player.ready)
            {
                allReady = false;
                break;
            }
        }

        if (allReady) GameController.AllPlayersReady();
    }

    #endregion

    #region Startup Communications    

    [Command]
    internal void Cmd_SeedMapHires()
    {
        foreach(var b in World.allBuildings)
        {
            if (b == null) continue;
            for(int i = 0; i<b.type.spawnsHires; i++)
            {
                var type = EmployeeManager.instance.RandomHireable();
                b.AddHireable(type);
                Rpc_SeedMapHires(b.id, type.id);
            }
        }
    }

    [ClientRpc]
    internal void Rpc_SeedMapHires(int buildingId, int employeeTypeId)
    {
        if (isServer) return;

        World.allBuildings[buildingId].AddHireable(EmployeeManager.instance.AllEmployeeTypes[employeeTypeId]);
    }


    [Command]
    private void Cmd_SetName()
    {
        gameObject.name = $"Network Player: {connectionToClient.address}";
        Rpc_SetName(gameObject.name);
    }

    [ClientRpc]
    private void Rpc_SetName(string name)
    {
        gameObject.name = name;
        if (isLocalPlayer) gameObject.name += "(local player)";
    }

    [Server]
    [Command]
    public void Cmd_WaitForInitialMap()
    {
        Debug.Log("Client wants map");
        if (World.ready)
            StartCoroutine(SendInitialWorld());
        else
            StartCoroutine(trySendMap());
    }

    IEnumerator trySendMap()
    {
        while (!World.ready)
        {
            yield return null;
        }
        StartCoroutine(SendInitialWorld());
    }

    IEnumerator SendInitialWorld()
    {
        var size = 1000;
        var map = new int[size];

        Rpc_StartMap(World.width, World.height, World.allBuildings.Count);
         
        int k = 0, starti = 0, startj = 0;
        for (int i = 0; i < World.width; i++)
        {
            for (int j = 0; j < World.height; j++)
            {
                map[k++] = (int)World.map[i, j].type;
                if(k == size)
                {
                    Rpc_ParseMapChunk(starti, startj, map);
                    k = 0;
                    starti = i;
                    startj = j+1;
                    if(startj == World.height)
                    {
                        startj = 0;
                        starti++;
                    }
                    map = new int[size];
                    yield return null;
                }
            }
        }
        Rpc_ParseMapChunk(starti, startj, map);

        foreach (var b in World.allBuildings)
        {
            if(b!=null) Rpc_ParseBuilding(b.ToJson());            
        }
        Rpc_DoneTransmittingWorld();
    }

    [ClientRpc]
    private void Rpc_StartMap(int width, int height, int maxBuildings)
    {
        if (World.ready) return;
        World.Initialize(width, height);
        World.allBuildings = new List<Building>(new Building[maxBuildings]);
    }

    [ClientRpc]
    void Rpc_ParseMapChunk(int starti, int startj, int[] map)
    {
        if (World.ready) return;

        int i = starti;
        int j = startj;
        foreach(var m in map)
        {
            var type = (TileType)m;
            World.map[i, j].type = type;
            switch (type)
            {
                case TileType.empty:
                case TileType.building:
                case TileType.pasture:
                    World.instance.baseTilemap.SetTile(new Vector3Int(i, j, 0), WorldGenerator.Instance.emptyTile);
                    World.map[i, j].type = TileType.empty;
                    break;
                case TileType.tree:
                    World.instance.baseTilemap.SetTile(new Vector3Int(i, j, 0), WorldGenerator.Instance.treeTile);
                    break;
                case TileType.river:
                    World.instance.baseTilemap.SetTile(new Vector3Int(i, j, 0), WorldGenerator.Instance.riverTile);
                    break;
                case TileType.smallRoad:
                case TileType.smallRoadRiver:
                    World.instance.baseTilemap.SetTile(new Vector3Int(i, j, 0), WorldGenerator.Instance.smallRoad);
                    break;
                case TileType.mediumRoad:
                case TileType.mediumRoadRiver:
                    World.instance.baseTilemap.SetTile(new Vector3Int(i, j, 0), WorldGenerator.Instance.mediumRoad);
                    break;
                case TileType.largeRoad:
                case TileType.largeRoadRiver:
                    World.instance.baseTilemap.SetTile(new Vector3Int(i, j, 0), WorldGenerator.Instance.largeRoad);
                    break;
            }
            if (++j >= World.height)
            {
                if (++i >= World.width)
                    return;
                j = 0;
            }
        }
    }

    [Command]
    internal void Cmd_RequestCurrentPlayers()
    {
        foreach (var player in FindObjectsOfType<NetworkPlayer>())
        {
            player.Rpc_SyncSetupProperties(player.name, player.playerColor, player.restaurantStyleTypeId, player.ready);
        }
    }

    #region PlaceFirstRestaurant
    
    public void Call_Cmd_PlaceFirstRestaurant(int x, int y)
    {
        Cmd_PlaceFirstRestaurant(x, y);
    }

    [Command]
    internal void Cmd_PlaceFirstRestaurant(int x, int y)
    {
        var origin = new Vector2Int(x, y);
        Vector2Int errorPos;
        var result = Building.CanPlace(origin, RestaurantStyle, out errorPos);
        if (result == Building.PlacementResult.Success)
        {
            var r = new Building(RestaurantStyle, origin, playerColor);            
            r.Place();
            restaurants.Add(r);
            buildings.Add(r);

            var ceo = new Employee(EmployeeManager.instance.AllEmployeeTypes[0], EmployeeManager.instance.NextEmployeeId(), EmployeeManager.instance.GenerateName(), r, this);
            ceo.vacation = false;

            if (isLocalPlayer)
            {
                Hierarchy.instance.CreateCeoCard(ceo);
                GameUI.instance.StopStaticMessage();
                TileSelectorMouse.instance.TurnOff();
                StartCoroutine(SelectCeoCoroutine());
            }
            
            Rpc_CreateFirstRestaurant(r.ToJson(), ceo.id, ceo.name);
        }
        else
        {
            Rpc_FailedToPlaceRestaurant((int)result, errorPos.x, errorPos.y);
        }
    }

    IEnumerator SelectCeoCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        GameController.SelectItem(employees[0]);
    }

    [ClientRpc]
    private void Rpc_FailedToPlaceRestaurant(int resultInt, int x, int y)
    {
        if (this != localPlayer) return;

        var result = (Building.PlacementResult)resultInt;
        switch (result)
        {
            case Building.PlacementResult.NoRoad:
                GameUI.instance.AddTempMessage("Restaurants must be built touching a road");
                break;
            case Building.PlacementResult.OutOfBounds:
                GameUI.instance.AddTempMessage("Out of bounds");
                break;
            case Building.PlacementResult.SomethingInTheWay:
                GameUI.instance.AddTempMessage("Restaurants must be built in open spaces");
                World.FlashTile(new Vector3Int(x,y,0), new Color(1,0,0,0.7f), 2.5f, 0.1f);
                break;
        }
        TileSelectorMouse.Flash(Color.red, 1f, 0.1f);
        
    }
    [ClientRpc]
    private void Rpc_CreateFirstRestaurant(string bString, int ceoId, string ceoName)
    {        
        if (isServer) return;
        var r = Building.FromJson(bString);
        if (Building.CanPlace(r.origin, r.type, out Vector2Int error) == Building.PlacementResult.Success)
        {
            r.Place();
            restaurants.Add(r);
            buildings.Add(r);

            var ceo = new Employee(EmployeeManager.instance.AllEmployeeTypes[0], ceoId, ceoName, r, this);
            ceo.vacation = false;

            if (isLocalPlayer)
            {
                GameUI.instance.StopStaticMessage();
                TileSelectorMouse.instance.TurnOff();
                Hierarchy.instance.CreateCeoCard(ceo);
                StartCoroutine(SelectCeoCoroutine());
            }            
        }
        else
        {
            Debug.LogError("Incorrect building recieved");
        }
    }

    #endregion

    [Command]
    internal void Cmd_SyncSetupProperties(string text, Color color, int typeId, bool ready)
    {
        Rpc_SyncSetupProperties(text, color, typeId, ready);
    }

    [ClientRpc]
    internal void Rpc_SyncSetupProperties(string name, Color color, int typeId, bool ready)
    {
        if (!string.IsNullOrEmpty(name))
        {
            this.name = name;
            buisnessName = name;
        }
        this.playerColor = color;
        restaurantStyleTypeId = typeId;
        this.ready = ready;
        try
        {
            RestaurantStyle = BuildingTypeManager.instance.allBuildingTypes[typeId];
        }
        catch(NullReferenceException e)
        {
            
        }
    }

    [Command]
    internal void Cmd_SendReadyStartVote(bool ready)
    {
        this.ready = ready;
        var all = true;
        foreach (var player in FindObjectsOfType<NetworkPlayer>())
        {
            if (!player.ready) all = false;
        }
        Rpc_RecieveReadyStartVote(ready, all);
    }

    [ClientRpc]
    internal void Rpc_RecieveReadyStartVote(bool ready, bool all)
    {
        this.ready = ready;
        GameController.StartMatchCountdown(all);
    }    

    [ClientRpc]
    void Rpc_ParseBuilding(string buildingJsonString)
    {
        if (World.ready) return;
        var b = Building.FromJson(buildingJsonString);
        if (Building.CanPlace(b.origin, b.type, out Vector2Int error) == Building.PlacementResult.Success)
        {
            b.Place(b.origin);            
        }
        else
        {
            Debug.LogError("Incorrect building recieved");
        }        
    }

    [ClientRpc]
    void Rpc_DoneTransmittingWorld()
    {
        if (World.ready) return;
        World.SortBuildings();
        GameController.WorldIsReady();
    }

    #endregion
}

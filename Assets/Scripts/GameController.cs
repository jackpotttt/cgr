using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public enum Phase { Restructuring, Operations, Ready, Dinner };

    public static GameController instance;
    public static NetworkPlayer[] players;
    public static Phase phase;

    public static ISelectable selectedItem;    
    public static EmployeePower selectedPower;

    private void Awake()
    {        
        instance = this;
    }

    void Start () {        
        if (WorldGenData.generate) StartCoroutine(WorldGenerator.Instance.Generate());
        else
        {
            StartCoroutine(WorldGenerator.Instance.GetMap());
        }
    }

    #region Startup

    internal static void WorldIsReady()
    {
        World.ready = true;
        World.instance.baseTilemap.RefreshAllTiles();
        World.instance.buildingTilemap.RefreshAllTiles();

        GameUI.instance.StopStaticCenterMessage();
        GameUI.instance.restaurantCustomizationMenu.gameObject.SetActive(true);
    }

    internal static void PlayerIsCustomized()
    {
        GameUI.instance.restaurantCustomizationMenu.gameObject.SetActive(false);
        GameUI.instance.startLobby.gameObject.SetActive(true);
    }

    private static bool countingDown;
    internal static void StartMatchCountdown(bool start)
    {
        countingDown = start;
        CustomNetworkManager.instance.StopAcceptingNewClients(countingDown);
        if (countingDown)
        {
            instance.StartCoroutine(GameStartCountdown(3));
        }
    }
    
    internal static IEnumerator GameStartCountdown(int from)
    {
        for (int i = from; i >= 0; i--)
        {
            GameUI.instance.ShowStaticCenterMessage($"Game Starting in {i}");
            if (!countingDown)
            {
                GameUI.instance.StopStaticCenterMessage();
                yield break;
            }
            yield return new WaitForSeconds(1);
        }
        GameUI.instance.StopStaticCenterMessage();
        MatchStart();
    }

    internal static void MatchStart()
    {
        GameUI.instance.startLobby.gameObject.SetActive(false);
        GameUI.instance.hud.gameObject.SetActive(true);
        GameUI.instance.SetStaticMessage("Place your first restaurant!");        
        
        TileSelectorMouse.Setup(2, 1, NetworkPlayer.localPlayer.Call_Cmd_PlaceFirstRestaurant);
        players = FindObjectsOfType<NetworkPlayer>();

        if(NetworkPlayer.localPlayer.isServer) NetworkPlayer.localPlayer.Cmd_SeedMapHires();


        phase = Phase.Operations;
        UnreadyPlayers();
        GameUI.instance.hud.SetOperationsText(false);
        GameUI.instance.hud.StartTimer(120);
    }

    #endregion

    #region Phases

    public static void RestructuringPhaseStart()
    {
        phase = Phase.Restructuring;
        UnreadyPlayers();
        GameUI.instance.hud.StartTimer(120); //TODO dynamic timer? should be synced between clients of course
        GameUI.instance.hud.SetRestructuringText();

        if(World.instance.gameObject.activeInHierarchy)
        ToggleBetweenHierarchyAndWorld();

        foreach(var e in EmployeeManager.instance.AllEmployees)
        {
            e?.ResetRound();
        }
    }

    public static void RestructuringPhaseEnd()
    {
        NetworkPlayer.localPlayer.RestructuringDone();        
    }

    public static void TransitionFromRestructuringToOperations()
    {
        RestructuringPhaseEnd();
        OperationsPhaseStart();
    }

    public static void OperationsPhaseStart()
    {
        phase = Phase.Operations;
        GameUI.instance.hud.SetOperationsText(false);
        SelectItem(selectedItem);
    }

    public static void OperationsPhaseEnd()
    {

    }

    public static void DinnerPhaseStart()
    {
        phase = Phase.Dinner;
        GameUI.instance.hud.SetDinnerText();

        TileSelectorMouse.instance.TurnOff();
        TileSelectorMultipleChoice.instance.TurnOff();
        SelectItem(null);

        GameUI.instance.hud.EndTimer();
        instance.StartCoroutine(SimulateDinnerPhase(2));
    }

    internal static IEnumerator SimulateDinnerPhase(int seconds)
    {
        for (int i = seconds; i > 0; i--)
        {
            GameUI.instance.ShowStaticCenterMessage($"Simulating dinner phase: {i}");
            yield return new WaitForSeconds(1);
        }
        GameUI.instance.StopStaticCenterMessage();
        RestructuringPhaseStart();
    }

    #endregion

    #region Events

    public static void CurrentPowerWasUsed()
    {
        selectedPower.usesThisRound++;
        selectedPower.SetButtonText();
        EndPower();
    }

    public static bool hiresVisible = true;
    internal static void ToggleShowHires(bool setInsteadOfToggle = false, bool show = false)
    {
        if (!setInsteadOfToggle)        
            show = !hiresVisible;

        if (show != hiresVisible)
        {
            hiresVisible = show;
            foreach (var b in World.allBuildings)
            {
                if (b == null) continue;
                foreach (var h in b.hireables)
                    h.gameObject.SetActive(show);
            }
        }

    }

    internal static void SelectItem(ISelectable p)
    {
        EndPower();

        if (selectedItem != null) selectedItem.Deselect();
        if (p != null) p.Select();

        selectedItem = p;                
        
        GameUI.instance.hud.SelectItem(p);

        if (p == null)
        {
            TileSelectorMultipleChoice.instance.TurnOff();
        }
    }

    public static void EndPower()
    {
        selectedPower?.Cancel();
        selectedPower = null;
        var e = selectedItem as Employee;
        if (e != null)
            e.ShowMovement();
    }

    internal static void ToggleBetweenHierarchyAndWorld(bool overrideToSet = false, bool setToWorld = true)
    {
        CameraControl.instance.SwapBetweenWorldAndHierarchyView();
        if (World.instance.gameObject.activeInHierarchy)
        {
            World.instance.gameObject.SetActive(false);
            Hierarchy.instance.gameObject.SetActive(true);
            GameUI.instance.hud.SetHierarchyButtonImage(false);
        }
        else
        {
            World.instance.gameObject.SetActive(true);
            Hierarchy.instance.gameObject.SetActive(false);
            GameUI.instance.hud.SetHierarchyButtonImage(true);
        }
    }

    internal static void ReadyUp()
    {
        NetworkPlayer.localPlayer.SetReady(true);
        GameUI.instance.hud.SetOperationsText(true);
        phase = Phase.Ready;
    }

    internal static void UndoReadyUp()
    {
        NetworkPlayer.localPlayer.SetReady(false);
        GameUI.instance.hud.SetOperationsText(false);
        phase = Phase.Operations;
    }

    internal static void AllPlayersReady()
    {
        OperationsPhaseEnd();
        DinnerPhaseStart();
    }

    internal static void PlayerOutOfTime()
    {
        if (phase == Phase.Restructuring) RestructuringPhaseEnd();
        else OperationsPhaseEnd();
        DinnerPhaseStart();
    }

    #endregion
    
    #region Utility

    internal static void UnreadyPlayers()
    {        
        foreach (var player in players)
        {
            player.ready = false;
        }
    }

    #endregion
}

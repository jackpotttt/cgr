using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager {

    public NetworkClient currentClient;

    public static CustomNetworkManager instance;
    public static bool AcceptingNewClients = true;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        dontDestroyOnLoad = true;
    }
	
    public void Host(int port)
    {
        networkPort = port;
        currentClient = StartHost();        
    }

    public void Join(string ip, int port)
    {        
        networkAddress = ip;
        networkPort = port;
        currentClient = StartClient();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log($"Client: Successfully connected to server at {conn.address}");
        base.OnClientConnect(conn);
        StartCoroutine(GetCurrentPlayers());
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        Debug.Log($"Server: Client from {conn.address} connected");

        if (AcceptingNewClients)
        {
            base.OnServerConnect(conn);
        }
        else
        {
            conn.Disconnect();
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log($"Server: Client from {conn.address} disconnected");
        base.OnServerDisconnect(conn);
    }

    internal void StopAcceptingNewClients(bool stop)
    {
        AcceptingNewClients = !stop;
        if (stop) maxConnections = numPlayers;
        else maxConnections = 1000;
    }

    public IEnumerator GetCurrentPlayers()
    {
        yield return new WaitUntil(() => NetworkPlayer.localPlayer != null);
        NetworkPlayer.localPlayer.Cmd_RequestCurrentPlayers();
    }
}

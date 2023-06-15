using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.Diagnostics;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkPlayer playerPrefab;
    CharacterInputController characterInputController;
    // Mapping between Token ID and Re-created Players
    Dictionary<int, NetworkPlayer> mapTokenIDWithNetworkPlayer;
    SessionListUIHandler sessionListUIHandler;

    void Awake()
    {
        //Create a new Dictionary
        mapTokenIDWithNetworkPlayer = new Dictionary<int, NetworkPlayer>();
        sessionListUIHandler = FindObjectOfType<SessionListUIHandler>(true);
    }

    public Vector3 GetRandomSpawnPoint()
    {
        return new Vector3(Random.Range(-20, 20), 4, Random.Range(-20, 20));
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Debug.Log("OnPlayerJoined we are server. Spawning player");
            runner.Spawn(playerPrefab, new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 1f, 0), Quaternion.identity, player);
        }
        else
        {
            Debug.Log("OnPlayerJoined");
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (characterInputController == null && NetworkPlayer.Local != null)
        {
            characterInputController = NetworkPlayer.Local.GetComponent<CharacterInputController>();
        }

        if (characterInputController != null)
        {
            input.Set(characterInputController.GetNetworkInput());

        }
    }

    public void SetConnectionTokenMapping(int token, NetworkPlayer networkPlayer)
    {
        mapTokenIDWithNetworkPlayer.Add(token, networkPlayer);
    }

    public void OnHostMigrationCleanUp()
    {
        Debug.Log("Spawner OnHostMigrationCleanUp started");

        foreach (KeyValuePair<int, NetworkPlayer> entry in mapTokenIDWithNetworkPlayer)
        {
            NetworkObject networkObjectInDictionary = entry.Value.GetComponent<NetworkObject>();

            if (networkObjectInDictionary.InputAuthority.IsNone)
            {
                Debug.Log($"{Time.time} Found player that has not reconnected. Despawning {entry.Value.nickName}");

                networkObjectInDictionary.Runner.Despawn(networkObjectInDictionary);
            }
        }

        Debug.Log("Spawner OnHostMigrationCleanUp completed");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        //Only update the list of sessions when the session list UI handler is active
        if (sessionListUIHandler == null)
            return;

        if (sessionList.Count == 0)
        {
            Debug.Log("Joined lobby no sessions found");

            sessionListUIHandler.OnNoSessionsFound();
        }
        else
        {
            sessionListUIHandler.ClearList();

            foreach (SessionInfo sessionInfo in sessionList)
            {
                sessionListUIHandler.AddToList(sessionInfo);

                Debug.Log($"Found session {sessionInfo.Name} playerCount {sessionInfo.PlayerCount}");
            }
        }

    }

    public void OnConnectedToServer(NetworkRunner runner) { Debug.Log("OnConnectedToServer"); }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { Debug.Log("OnShutdown"); }
    public void OnDisconnectedFromServer(NetworkRunner runner) { Debug.Log("OnDisconnectedFromServer"); }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { Debug.Log("OnConnectRequest"); }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { Debug.Log("OnConnectFailed"); }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}

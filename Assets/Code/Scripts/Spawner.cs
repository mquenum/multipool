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
    [SerializeField] private GameObject _playerManagerPrefab;
    [SerializeField] private NetworkPlayer _playerPrefab;
    [SerializeField] private GameObject _ball;
    private CharacterInputController _characterInputController;
    // Mapping between Token ID and Re-created Players
    private Dictionary<int, NetworkPlayer> _mapTokenIDWithNetworkPlayer;
    private SessionListUIHandler _sessionListUIHandler;
    private Dictionary<PlayerRef, NetworkPlayer> _spawnedPlayers = new Dictionary<PlayerRef, NetworkPlayer>();

    void Awake()
    {
        //Create a new Dictionary
        _mapTokenIDWithNetworkPlayer = new Dictionary<int, NetworkPlayer>();
        _sessionListUIHandler = FindObjectOfType<SessionListUIHandler>(true);
    }

    public Vector3 GetRandomSpawnPoint()
    {
        return new Vector3(Random.Range(-20, 20), 4, Random.Range(-20, 20));
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            if (runner.LocalPlayer == player)
            {
                runner.Spawn(_playerManagerPrefab);
            }

            Debug.Log("OnPlayerJoined we are server. Spawning player");
            NetworkPlayer networkPlayer = runner.Spawn(_playerPrefab, new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 1f, 0), Quaternion.identity, player);
            _spawnedPlayers.Add(player, networkPlayer);

            networkPlayer.GetComponent<NetworkPlayer>().NetworkPlayerRef = player;

            Debug.Log($"nb players :{_spawnedPlayers.Count}");

            if (_spawnedPlayers.Count == 2)
            {
                //set game state
                PlayerManager.State.Server_SetState(GameState.EGameState.Game);
                runner.Spawn(_ball, new Vector3(0, 1.6f, -4));
            }
        }
        else
        {
            Debug.Log("OnPlayerJoined");
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("OnPlayerLeft");
        if (runner.IsServer)
        {
            if (_spawnedPlayers.TryGetValue(player, out NetworkPlayer networkPlayer))
            {
                _playerPrefab.PlayerLeft(player);
                _spawnedPlayers.Remove(player);
            }
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (_characterInputController == null && NetworkPlayer.Local != null)
        {
            _characterInputController = NetworkPlayer.Local.GetComponent<CharacterInputController>();
        }

        if (_characterInputController != null)
        {
            input.Set(_characterInputController.GetNetworkInput());
        }
    }

    public void SetConnectionTokenMapping(int token, NetworkPlayer networkPlayer)
    {
        _mapTokenIDWithNetworkPlayer.Add(token, networkPlayer);
    }

    public void OnHostMigrationCleanUp()
    {
        Debug.Log("Spawner OnHostMigrationCleanUp started");

        foreach (KeyValuePair<int, NetworkPlayer> entry in _mapTokenIDWithNetworkPlayer)
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
        if (_sessionListUIHandler == null)
            return;

        if (sessionList.Count == 0)
        {
            Debug.Log("Joined lobby no sessions found");

            _sessionListUIHandler.OnNoSessionsFound();
        }
        else
        {
            _sessionListUIHandler.ClearList();

            foreach (SessionInfo sessionInfo in sessionList)
            {
                _sessionListUIHandler.AddToList(sessionInfo);

                Debug.Log($"Found session {sessionInfo.Name} playerCount {sessionInfo.PlayerCount}");
            }
        }

    }

    public void OnDisconnectedFromServer(NetworkRunner runner) { Debug.Log("OnDisconnectedFromServer"); }
    public void OnConnectedToServer(NetworkRunner runner) { Debug.Log("OnConnectedToServer"); }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { Debug.Log("OnShutdown"); }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { Debug.Log("OnConnectRequest"); }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { Debug.Log("OnConnectFailed"); }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}

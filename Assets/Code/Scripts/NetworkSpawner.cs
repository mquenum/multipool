using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private GameObject _playerManagerPrefab;
    [SerializeField] private NetworkPlayer _playerPrefab;
    [SerializeField] private GameObject _ball;

    private CharacterInputController _characterInputController;
    private Dictionary<PlayerRef, NetworkPlayer> _spawnedPlayers = new Dictionary<PlayerRef, NetworkPlayer>();

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
                PlayerManager.State.Server_SetState(GameState.EGameState.Game);
                runner.Spawn(_ball, new Vector3(0, 1.6f, 0));
            }
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

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        /*//Only update the list of sessions when the session list UI handler is active
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
        }*/
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

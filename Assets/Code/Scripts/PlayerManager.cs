using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : NetworkBehaviour
{
    [Networked(OnChanged = nameof(ActivePlayerChanged))]
    public NetworkPlayer ActivePlayer { get; private set; }
    public UnityEvent<NetworkPlayer> OnPlayerTurnChanged = new();

    public static GameState State => Instance._gameState;
    private GameState _gameState;
    private List<NetworkPlayer> _players = new();
    private List<NetworkPlayer> _playerTurns = new();
    //singleton
    private static PlayerManager _instance;

    public static PlayerManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public override void Spawned()
    {
        Debug.Log("PlayerManager spawned");
        _gameState = GetComponent<GameState>();

        if (_instance == null)
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public bool All(System.Predicate<NetworkPlayer> match)
    {
        return _players.Count(p => !match.Invoke(p)) == 0;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void Rpc_LoadDone(RpcInfo info = default)
    {
        NetworkPlayer player = _players.First(p => p.NetworkPlayerRef == info.Source);
        player.IsLoaded = true;
        Debug.Log($"Player {player.nickName} has loaded.");
    }

    public void RegisterPlayer(NetworkPlayer player)
    {
        _players.Add(player);
    }

    public void UnregisterPlayer(NetworkPlayer player)
    {
        _players.Remove(player);
    }

    public bool IsActivePlayerLocalPlayer()
    {
        if (ActivePlayer == null)
            return false;

        if (ActivePlayer.IsLocalPlayer())
            return true;

        return false;
    }

    public void StartGame()
    {
        Debug.Log("StartGame");
        foreach (var player in _players)
        {
            Debug.Log($"Player {player.NetworkPlayerRef}");
            _playerTurns.Add(player);
        }
        //shuffle ot have a random player start the game
        _playerTurns.Shuffle();

        NextTurn();
    }

    public void EndPlayerTurn(bool fault = false)
    {
        //add player again at the end of the list
        _playerTurns.Add(ActivePlayer);

        //and remove actual turn
        _playerTurns.RemoveAt(0);

        if (fault)
        {
            //add a second turn for next player
            _playerTurns.Insert(0, _playerTurns.First());
        }

        NextTurn();
    }

    private void NextTurn()
    {
        ActivePlayer = _playerTurns.FirstOrDefault();
    }

    /*public void SpawnWhiteBall()
    {
        Runner.Spawn(_whiteBallPrefab);
    }*/

    //network changed callbacks
    static void ActivePlayerChanged(Changed<PlayerManager> p)
    {
        p.Behaviour.OnPlayerTurnChanged?.Invoke(p.Behaviour.ActivePlayer);
    }
}

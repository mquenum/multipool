using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    [Networked]
    public PlayerRef NetworkPlayerRef { get; set; }
    public static NetworkPlayer Local { get; set; }
    // Remote Client Token Hash
    [Networked]
    public int token { get; set; }
    [Networked]
    public bool IsLoaded { get; set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Debug.Log("RegisterPlayer" + this);
        PlayerManager.Instance.RegisterPlayer(this);
    }

    private void OnDestroy()
    {
        PlayerManager.Instance.UnregisterPlayer(this);
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            Debug.Log("Spawned local player");
        }
    }

    /* TODO: error on quit
     NullReferenceException: Object reference not set to an instance of an object
     */
    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    public bool IsLocalPlayer()
    {
        return PlayerManager.Instance.Runner.LocalPlayer == NetworkPlayerRef;
    }
}

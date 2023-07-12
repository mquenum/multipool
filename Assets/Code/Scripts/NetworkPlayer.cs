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
    public TextMeshProUGUI playerNickNameTM;
    [Networked(OnChanged = nameof(OnNickNameChanged))]
    public NetworkString<_16> nickName { get; set; }
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
            RPC_SetNickName(PlayerPrefs.GetString("PlayerNickname"));
        }
        else
        {
            Debug.Log("Spawned local player");
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    static void OnNickNameChanged(Changed<NetworkPlayer> changed)
    {
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.nickName}");

        changed.Behaviour.OnNickNameChanged();
    }

    private void OnNickNameChanged()
    {
        Debug.Log($"Nickname changed for player to {nickName} for player {gameObject.name}");

        playerNickNameTM.text = nickName.ToString();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetNickName(string nickName, RpcInfo info = default)
    {
        Debug.Log($"[RPC] SetNickName {nickName}");
        this.nickName = nickName;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            if ((data.buttons & NetworkInputData.MOUSEBUTTON1) != 0)
            {
                Debug.Log("Oui 1");
            }
            if ((data.buttons & NetworkInputData.MOUSEBUTTON2) != 0)
            {
                Debug.Log("Oui 2");
            }
        }
    }

    public bool IsLocalPlayer()
    {
        return PlayerManager.Instance.Runner.LocalPlayer == NetworkPlayerRef;
    }
}

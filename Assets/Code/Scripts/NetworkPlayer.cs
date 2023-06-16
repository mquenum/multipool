using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }
    public TextMeshProUGUI playerNickNameTM;
    [Networked(OnChanged = nameof(OnNickNameChanged))]
    public NetworkString<_16> nickName { get; set; }
    // Remote Client Token Hash
    [Networked] public int token { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
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
            Debug.Log("ici");
            Debug.Log(Object);
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
}

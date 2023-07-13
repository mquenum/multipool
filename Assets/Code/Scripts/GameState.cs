using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameState : NetworkBehaviour
{
    public enum EGameState { Lobby, Game }
    
    [Networked] public EGameState Current { get; set; }
    [Networked] public EGameState Previous { get; set; }
    
    protected StateMachine<EGameState> StateMachine = new StateMachine<EGameState>();

    public override void Spawned()
    {
        Server_SetState(EGameState.Lobby);
        
        StateMachine[EGameState.Lobby].onExit = next =>
        {
            if (Runner.SessionInfo.IsOpen)
                Runner.SessionInfo.IsOpen = false;
        };
        
        StateMachine[EGameState.Game].onEnter = prev =>
        {
            if (Runner.IsServer)
            {
                PlayerManager.Instance.StartGame();
            }
        };
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.IsForward)
        {
            StateMachine.Update(Current, Previous);
        }
            
    }
    public void Server_SetState(EGameState st)
    {
        if (Runner.IsServer)
        {
            if (Current == st)
                return;
            Previous = Current;
            Current = st;
            Debug.Log($"SetState {Current}");
        }
    }
}

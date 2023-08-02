using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class NetworkPlayer : NetworkBehaviour
{
    public PlayerModel player;

    public static NetworkPlayer Local { get; private set; }

    public static event Action OnReset = delegate { };

    private void Start()
    {
        player = GetComponent<PlayerModel>();
        OnReset += player.ResetLife; 
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            Debug.Log("[Custom Msg] Spawned Own Player");
        }
        else
        {
            Debug.Log("[Custom Msg] Spawned Other Player");
        }
    }
    public void Dead()
    {
        Debug.LogWarning("Player Muerto");
        OnReset();
    }
    public void OnDisconnected()
    {
        Runner.Shutdown();
    }
}

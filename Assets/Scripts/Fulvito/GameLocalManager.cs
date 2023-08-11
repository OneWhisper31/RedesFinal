using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion;
using TMPro;
using Fusion.Sockets;
using System.Linq;

public class GameLocalManager : NetworkBehaviour
{
    /*public static GameLocalManager Instance { get; private set; }

    //key ID, Value isready?
    Dictionary<string, NetworkPlayer> playersDic = new Dictionary<string, NetworkPlayer>();

    private void Awake()
    {
        Instance = GetComponent<GameLocalManager>();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_AddDictionary(NetworkPlayer _player)
    {
        Debug.Log("Replace Dictio");

        if(!playersDic.ContainsKey(_player.Runner.GetPlayerUserId()))
            playersDic.Add(_player.Runner.GetPlayerUserId(), _player);

        foreach (var item in playersDic)
        {
            Debug.Log(item.Key + " " + item.Value.name);
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_RemoveDictionary(NetworkPlayer _player)
    {
        Debug.Log("Replace Dictio");

        if (playersDic.ContainsKey(_player.Runner.GetPlayerUserId()))
            playersDic.Remove(_player.Runner.GetPlayerUserId());

        foreach (var item in playersDic)
        {
            Debug.Log(item.Key + " " + item.Value.name);
        }
    }*/
}

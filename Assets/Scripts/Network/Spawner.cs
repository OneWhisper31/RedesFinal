using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] NetworkPlayer _playerPrefab;

    CharacterInputHandler _characterInputHandler;

    //Callback que se recibe cuando entra un nuevo Cliente a la sala
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Debug.Log("Player Joined, I'm the server/host");
            runner.Spawn(_playerPrefab, new Vector3(-5, 0.8f,0), null, player);
        }
        else
        {
            Debug.Log("Player Joined, I'm not the server/host");
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!NetworkPlayer.Local) return;

        if (!_characterInputHandler)
        {
            _characterInputHandler = NetworkPlayer.Local.GetComponent<CharacterInputHandler>();
        }
        else
        {
            input.Set(_characterInputHandler.GetNetworkInput());
        }
    }


    #region Unused callbacks
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    #endregion
}

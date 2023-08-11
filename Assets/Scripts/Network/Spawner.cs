using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Linq;
using TMPro;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] NetworkPlayer _playerPrefab;
    [SerializeField] TextMeshProUGUI _waitText;

    CharacterInputHandler _characterInputHandler;

    //Callback que se recibe cuando entra un nuevo Cliente a la sala
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        var activePlayers = runner.ActivePlayers;

        switch (runner.ActivePlayers.Count())
        {
            case 1:
                _waitText.gameObject.SetActive(true);
                _waitText.text = "Waiting for players";
                break;
            case 2:
                if (runner.IsServer)
                {
                    _waitText.gameObject.SetActive(false);
                    int id = 0;

                    GameManager.Instance.playersDic =
                        activePlayers.Aggregate(new Dictionary<PlayerRef, int>(), (x, y) =>
                        {
                            id++;

                            runner.Spawn(_playerPrefab, new Vector3(-5, 0.8f, 0), null, y);
                            x.Add(y, 0);

                            return x;
                        });

                }
                else
                {
                    Debug.Log("[Custom Msg] Second Player Joined, I'm not the Host");
                }
                break;
            default:
                break;
        }
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (runner.ActivePlayers.Count() > 3)
            return;
        //NetworkPlayer.Local.player.OnDisconected();
        foreach (var item in FindObjectsOfType<NetworkPlayer>())
        {
            item.OnDisconected();
        }
        _waitText.gameObject.SetActive(true);
        _waitText.text = "Player 2 Disconnected. Waiting for new players.";
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

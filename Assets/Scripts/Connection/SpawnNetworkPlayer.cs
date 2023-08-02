using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Linq;
using TMPro;

public class SpawnNetworkPlayer : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] NetworkPlayer _playerPrefab;

    CharacterInputHandler _characterInputHandler;
    [SerializeField] TextMeshProUGUI _textConnecting;



    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        switch (runner.ActivePlayers.Count())
        {
            case 1:
                Debug.Log("[Custom Msg] On Player Joined (1 player) - Waiting for second player...");
                _textConnecting.gameObject.SetActive(true);
                //GameManager.GM.players = new List<PlayerModel>();
                break;
            case 2:
                _textConnecting.gameObject.SetActive(false);
                Debug.Log("[Custom Msg] On Player Joined (2 player) - Spawning Player as Local...");
                var obj = GameManager.GM;
                runner.Spawn(_playerPrefab, !obj.Object.IsProxy ?obj.StateAuthorityInitialPos.position :
                    obj.PlayerInitialPos.position, Quaternion.identity, runner.LocalPlayer)
                    .gameObject.GetComponent<PlayerModel>();
                break;
            default:
                break;
        }
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player){
        if (runner.ActivePlayers.Count() > 3)
            return;
        NetworkPlayer.Local.player.OnDisconected();
        _textConnecting.gameObject.SetActive(true);
        GameManager.GM.OnQuitEndScreen();
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


    #region Callbacks sin usar
    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnDisconnectedFromServer(NetworkRunner runner) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    #endregion
}

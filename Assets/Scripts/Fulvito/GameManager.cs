using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion;
using TMPro;
using Fusion.Sockets;

public class GameManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    public static GameManager Instance { get; private set; }

    /*[Networked(OnChanged = nameof(OnScoreChange))]
    public int blueScore { get; set; }

    [Networked(OnChanged = nameof(OnScoreChange))]
    public int redScore { get; set; }


    public static Action AddScoreRed = () => Instance.redScore++;
    public static Action AddScoreBlue = () => Instance.blueScore++;*/

    [Networked(OnChanged = nameof(OnScoreChange))]
    public int totalScore { get; set; }


    public static Action AddScore = () => Instance.totalScore++;

    [Networked(OnChanged = nameof(OnTimeChanged))]
    float _timer { get; set; }

    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] public GameObject _gameCanvas;
    [SerializeField] NetworkPlayer _playerPrefab;
    [SerializeField] NetworkObject _carPrefab;

    [Networked]
    public bool _matchStarted { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        Runner.AddCallbacks(this);
        if (!Object.HasStateAuthority) return;
        if (Instance) Destroy(Instance);
        else Instance = this;
    }

    public override void FixedUpdateNetwork()
    {
        //StartMatch();
        if (!Object.HasStateAuthority) return;
        if (!_matchStarted && Runner.IsServer)
        {
            if (Runner.SessionInfo.PlayerCount >= 2)
            {
                StartCoroutine(StartGame());
            }
        }
        if(_timerText.isActiveAndEnabled && _timer >= 0)
            _timer -= Time.fixedDeltaTime;

        if(_timer <= 0 && _matchStarted)
        {
            //RPC_OnRestartRound();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_OnStartMatch()
    {    
        _timer = 500f;
        _gameCanvas.SetActive(true);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_OnRestartRound()
    {
        _timer = 500f;
    }

    static void OnScoreChange(Changed<GameManager> changed)
    {
        changed.Behaviour._scoreText.text = $"{changed.Behaviour.totalScore}";
        //spawn car
        //changed.Behaviour.StartCoroutine(changed.Behaviour.restartCoroutine());
    }

    static void OnTimeChanged(Changed<GameManager> changed)
    {
        changed.Behaviour._timerText.text = $"{TimeSpan.FromSeconds(changed.Behaviour._timer).ToString(@"m\:ss")}";
    }

    IEnumerator StartGame()
    {
        _matchStarted = true;
        yield return new WaitForSeconds(3);
        Runner.Spawn(_carPrefab);
        RPC_OnStartMatch();
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(1);
        Runner.Spawn(_carPrefab);
        RPC_OnRestartRound();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if(_matchStarted)
        {
            Instance = this;
            _gameCanvas.SetActive(true);
        }
    }
    #region Unused
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        //throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        //throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        //throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        //throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        //throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        //throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        //throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        //throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        //throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }
    #endregion
}

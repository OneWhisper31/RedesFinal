using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
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

    //Key: hashcode del Uid y tupla de la ref y los puntos

    public Dictionary<PlayerRef, int> playersDic = new Dictionary<PlayerRef, int>();

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
    [SerializeField] NetworkObject plate;
    [SerializeField] List<PlateInteractor> plateInteractors;
    [SerializeField] List<PlatePlacer> platePlacers;
    [SerializeField] List<GameObject> plateSpawns;
    [SerializeField] List<Plate> plates;

    [SerializeField] LeaderBoardDataSaver textPlayerLeaderboard;
    [SerializeField] Canvas CanvasEnd;
    bool onEnd = false;

[Networked]
    public bool _matchStarted { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        Runner.AddCallbacks(this);

        if (Instance) Destroy(this);
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
            print("Finished");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_OnRestartRound()
    {
        if (Runner.IsServer && !onEnd)
        {
            onEnd = true;
            int i = 0;
            playersDic.Select(x =>
            {
                var y = Runner.Spawn(textPlayerLeaderboard, Vector3.up * -i * 70);
                y.transform.parent = CanvasEnd.transform;

                y.playerRef = x.Key;
                y.score = x.Value;

                var yText = y.GetComponent<TextMeshProUGUI>();

                i++;
                yText.text = x.Key + ": " + x.Value + " Points";


                return y;
            }).ToArray();

            RPC_DecideWinner();
        }
}

[Rpc(RpcSources.All, RpcTargets.All)]
void RPC_DecideWinner()
{
    var datasaver = FindObjectsOfType<LeaderBoardDataSaver>();

    foreach (var item in datasaver)
    {
        TextMeshProUGUI text = item.GetComponent<TextMeshProUGUI>();

        if (item.playerRef == Runner.LocalPlayer)
            text.color = Color.red;
        else
            text.color = Color.white;

    }
}


void SpawnPlates()
    {
        if (plates.Count > 0)
        {
            if(Runner.IsServer)
                RPC_RemovePlates();
        }
        for (int j = 0; j < plateSpawns.Count; j++)
        {
            var newPlate = Runner.Spawn(plate).GetBehaviour<Plate>();
            plates.Add(newPlate);
            RPC_SpawnPlates(newPlate, j);
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_RemovePlates()
    {
        if (plates.Count > 0)
        {
            for (int i = 0; i < plates.Count; i++)
            {
                Runner.Despawn(plates[i].Object, true);
                plates.RemoveAt(i);
            }
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SpawnPlates(Plate newPlate, int interactorNumber)
    {
        newPlate.transform.position = plateSpawns[interactorNumber].transform.position;
        newPlate.myPlatePlacer = platePlacers[interactorNumber].Object;
        plateInteractors[interactorNumber].MyPlate = newPlate;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_OnStartMatch()
    {
        _timer = 180f;
        _gameCanvas.SetActive(true);
    }

    static void OnScoreChange(Changed<GameManager> changed)
    {
        changed.Behaviour._scoreText.text = $"{changed.Behaviour.totalScore}";
    }

    static void OnTimeChanged(Changed<GameManager> changed)
    {
        changed.Behaviour._timerText.text = $"{TimeSpan.FromSeconds(changed.Behaviour._timer).ToString(@"m\:ss")}";
    }


    IEnumerator StartGame()
    {
        _matchStarted = true;
        yield return new WaitForSeconds(3);
        if(Runner.IsServer)
            SpawnPlates();
        Runner.Spawn(_carPrefab);
        RPC_OnStartMatch();
    }

    public IEnumerator NextCar()
    {
        yield return new WaitForSeconds(1);
        SpawnPlates();
        Runner.Spawn(_carPrefab);
        //RPC_OnRestartRound();
    }

    /*public void SetPlatesRecipe(List<Tuple<Ingredient, NetworkBool>> list)
    {
        foreach (var item in plates)
            item.SetPlateIngredients(list);
    }*/

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AddPoints(PlayerRef player, int pointsToAdd)
    {
        if (playersDic.ContainsKey(playersDic[player]))
        {
            playersDic[player]+= pointsToAdd;
            totalScore += pointsToAdd;
            Debug.Log(player + " " + playersDic[player]);
        }
    }


    /*[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_AddDictionary(NetworkPlayer _player)
    {
        Debug.Log("add Dictio");

        var hashP = Runner.GetPlayerUserId(_player.Runner.LocalPlayer);

        Debug.Log(hashP);

        if (!playersDic.ContainsKey(hashP.GetHashCode()))
            playersDic.Add(hashP.GetHashCode(), Tuple.Create(_player.Runner.LocalPlayer,0));
    }
    

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_RemoveDictionary(NetworkPlayer _player)
    {
        Debug.Log("Replace Dictio");

        var hashP = Runner.GetPlayerUserId(_player.Runner.LocalPlayer);

        Debug.Log(hashP);

        if (playersDic.ContainsKey(hashP.GetHashCode()))
            playersDic.Remove(hashP.GetHashCode());

        foreach (var item in playersDic)
        {
            Debug.Log(item.Key + " " + item.Value.Item1.name);
        }
    }*/

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

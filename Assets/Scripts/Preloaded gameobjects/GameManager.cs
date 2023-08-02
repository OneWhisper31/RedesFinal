using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public static GameManager GM { get; private set; }

    [SerializeField] TextMeshProUGUI _textRestarting;

    public Transform StateAuthorityInitialPos;
    public Transform PlayerInitialPos;

    public GameObject canvas,winSing,loseSing;

    //key ID, Value isready?
    public Dictionary<string, bool> OnReplayReady = new Dictionary<string, bool>();

    private void Start()
    {
        GM = GetComponent<GameManager>();
    }

    public override void Spawned()
    {
        Debug.Log("Authority: " + Object.HasStateAuthority + " - Proxy: " + Object.IsProxy);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_OnEnd(string playerDeadID)
    {
        Time.timeScale = 0;
        canvas.SetActive(true);

        loseSing.SetActive(false);
        winSing.SetActive(false);

        string playerID= NetworkPlayer.Local.Runner.GetPlayerUserId();

        if (playerDeadID == playerID)
            loseSing.SetActive(true);
        else
            winSing.SetActive(true);

        if(!OnReplayReady.ContainsKey(NetworkPlayer.Local.Runner.GetPlayerUserId()))
            OnReplayReady.Add(playerID, false);//Seteo el dicionario para poner que ninguno de los dos esta listo para reiniciar

        foreach (var item in OnReplayReady)
        {
            Debug.Log(item.Key);
        }
    }
    public void OnQuitEndScreen()
    {
        Time.timeScale = 1;
        canvas.SetActive(false);
        winSing.SetActive(false);
        loseSing.SetActive(false);
    }
    public bool IsEveryOneReadyToReset() {
        if (OnReplayReady.Count < 2)
            return false;

        foreach (var item in OnReplayReady)
        {
            if (item.Value == false)
                return false;
        }

        return true;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_OnResetLevel(string IDisReady)
    {
        OnReplayReady[IDisReady] = true;

        if (IsEveryOneReadyToReset())
        {
            OnReplayReady = new Dictionary<string, bool>();
            OnQuitEndScreen();
            StartCoroutine(ResetLevel());
        }

    }

    IEnumerator ResetLevel()
    {
        
        _textRestarting.gameObject.SetActive(true);
        NetworkPlayer.Local.player.ResetLife();
        NetworkPlayer.Local.transform.position = !Object.IsProxy ? 
            StateAuthorityInitialPos.position: PlayerInitialPos.position;
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1;
        _textRestarting.gameObject.SetActive(false);
    }
}
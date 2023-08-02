using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLoseHandler : MonoBehaviour
{
    public void OnReplay()
    {
        GameManager.GM.RPC_OnResetLevel(NetworkPlayer.Local.Runner.GetPlayerUserId());
    }

    public void OnQuit() {
        GameManager.GM.OnQuitEndScreen();//los otros se enteran segun el callback OnPlayerLeft
        NetworkPlayer.Local.Runner.Shutdown();
        Application.Quit();
        Debug.Log("App Close");
    }
}

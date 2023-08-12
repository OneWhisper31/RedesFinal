using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseHandler : MonoBehaviour
{
    /*public void OnReplay()
    {
        GameManager.Instance.RPC_OnResetLevel(NetworkPlayer.Local.Runner.GetPlayerUserId());
    }

    public void OnQuit() {
        //GameManager.Instance.OnQuitEndScreen();//los otros se enteran segun el callback OnPlayerLeft
        NetworkPlayer.Local.Runner.Shutdown();
        SceneManager.LoadScene("MainMenu");
        //Application.Quit();
    }*/
    public void OnEnable()
    {
        StartCoroutine(Rutine());
    }
    private IEnumerator Rutine()
    {
        yield return new WaitForSeconds(5);


        foreach (var item in FindObjectsOfType<NetworkPlayer>())
        {
            item.Runner.Disconnect(item.Runner.LocalPlayer);
        }
    }
}

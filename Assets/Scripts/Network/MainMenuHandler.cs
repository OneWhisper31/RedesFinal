using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] NetworkRunnerHandler _networkHandler;

    [Header("Panels")]
    [SerializeField] GameObject _initPanel;
    [SerializeField] GameObject _seshListHandler;
    [SerializeField] GameObject _statusPanel;
    [SerializeField] GameObject _hostGamePanel;

    [Header("Buttons")]
    [SerializeField] Button _joinLobbyBTN;
    [SerializeField] Button _openHostPanelBTN;
    [SerializeField] Button _hostGameBTN;

    [Header("InputField")]
    [SerializeField] TMP_InputField _hostSeshName;

    [Header("Texts")]
    [SerializeField] TextMeshProUGUI _statusText;
    // Start is called before the first frame update
    void Start()
    {

        _joinLobbyBTN.onClick.AddListener(BTN_JoinLobby);

        _openHostPanelBTN.onClick.AddListener(BTN_ShowHostPanel);

        _hostGameBTN.onClick.AddListener(BTN_CreateGameSesh);

        _networkHandler.OnJoinedLobby += () =>
        {
            _statusPanel.SetActive(false);
            _openHostPanelBTN.gameObject.SetActive(true);
        };
    }

    void BTN_CreateGameSesh()
    {
        _networkHandler.CreateGame(_hostSeshName.text, "Game");
    }

    void BTN_JoinLobby()
    {
        _networkHandler.JoinLobby();

        _initPanel.SetActive(false);
        _statusPanel.SetActive(true);
        _seshListHandler.gameObject.SetActive(true);

        _statusText.text = "Searching Games...";
    }

    void BTN_ShowHostPanel()
    {
        _seshListHandler.gameObject.SetActive(false);
        _hostGamePanel.SetActive(true);
    }

    public void BTN_CloseLobbies()
    {
        _seshListHandler?.gameObject.SetActive(false);
        _initPanel?.SetActive(true);
    }

    public void BTN_CloseHost()
    {
        _hostGamePanel?.SetActive(false);
        _seshListHandler?.gameObject.SetActive(true);
    }
    public void BTN_CloseGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

}

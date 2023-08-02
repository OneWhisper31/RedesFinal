using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;
using System;
public class SessionInfoItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _lobbyName;
    [SerializeField] TextMeshProUGUI _playerCount;
    [SerializeField] Button _joinBtn;

    SessionInfo _seshInfo;
    
    public event  Action<SessionInfo> OnJoinSesh;
    public void SetSeshInfo(SessionInfo sesh)
    {
        _seshInfo = sesh;

        _lobbyName.text = sesh.Name;
        _playerCount.text = $"{_seshInfo.PlayerCount}/{_seshInfo.MaxPlayers}";

        _joinBtn.enabled = _seshInfo.PlayerCount < _seshInfo.MaxPlayers;
    }

    public void OnClick()
    {
        OnJoinSesh?.Invoke(_seshInfo);
    }
}

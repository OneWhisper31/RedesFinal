using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
public class SessionListHandler : MonoBehaviour
{
    [SerializeField] NetworkRunnerHandler _networkRunner;

    [SerializeField] TextMeshProUGUI _statusText;

    [SerializeField] SessionInfoItem _seshPrefab;

    [SerializeField] VerticalLayoutGroup _verticalLayoutGroup;
    // Start is called before the first frame update
    void OnEnable()
    {
        _networkRunner.OnSessionListUpdate += ReceiveSeshList;
    }

    private void OnDisable()
    {
        _networkRunner.OnSessionListUpdate -= ReceiveSeshList;
    }

    void ClearList()
    {
        foreach(Transform child in _verticalLayoutGroup.transform)
        {
            Destroy(child.gameObject);
        }

        _statusText.gameObject.SetActive(false);
    }

    void ReceiveSeshList(List<SessionInfo> lobbies)
    {
        ClearList();
        if(lobbies.Count == 0)
        {
            NoSeshFound();
        }
        else
        {
            foreach(SessionInfo item in lobbies)
            {
                AddToList(item);
            }
        }
    }

    void AddToList(SessionInfo session)
    {
        var newSessionInfo = Instantiate(_seshPrefab, _verticalLayoutGroup.transform);
        newSessionInfo.SetSeshInfo(session);
        newSessionInfo.OnJoinSesh += JoinSelectedSesh;
    }

    void JoinSelectedSesh(SessionInfo sesh)
    {
        _networkRunner.JoinGame(sesh);
    }

    void NoSeshFound()
    {
        _statusText.text = "NO LOBBIES FOUND";
        _statusText.gameObject.SetActive(true);
    }
}

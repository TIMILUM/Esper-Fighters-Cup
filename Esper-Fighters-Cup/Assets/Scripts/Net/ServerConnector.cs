using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ServerConnector : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text _statusText;

    private void Awake()
    {
        Debug.Assert(_statusText);
        _statusText.text = string.Empty;
    }

    public override void OnConnectedToMaster()
    {
        _statusText.text = "서버와 연결되었습니다!";
        CoroutineTimer.SetTimerOnce(() => SceneManager.LoadScene("MatchScene"), 0.5f);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

    public void Connect()
    {
        _statusText.text = "서버와 연결을 시도합니다...";
        PhotonNetwork.ConnectUsingSettings();
    }
}

using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ServerConnector : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text _statusText;
    [SerializeField] private Button _connectButton;

    private bool _connected = false;

    private void Awake()
    {
        Debug.Assert(_connectButton);
        Debug.Assert(_statusText);
        _statusText.text = string.Empty;

        // 방에 입장한 플레이어와 씬 동기화
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    /// <summary>
    /// 포톤 서버와 연결이 성공되었을 시 실행되는 콜백 메소드
    /// </summary>
    public override void OnConnectedToMaster()
    {
        if (!_connected)
        {
            return;
        }

        _statusText.text = "서버와 연결되었습니다!";
        CoroutineTimer.SetTimerOnce(() => StartMatch(), 0.5f);
    }

    /// <summary>
    /// 포톤 서버와 연결을 실패했을 시 실행되는 콜백 메소드
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        _statusText.text = "연결에 실패했습니다.";
        _connectButton.interactable = true;
    }

    /// <summary>
    /// 포톤 서버에 연결을 시도합니다.
    /// </summary>
    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            StartMatch();
            return;
        }

        _statusText.text = "서버와 연결을 시도합니다...";
        PhotonNetwork.ConnectUsingSettings();
        _connected = true;
        _connectButton.interactable = false;
    }

    private void StartMatch()
    {
        SceneManager.LoadScene("MatchScene");
    }
}

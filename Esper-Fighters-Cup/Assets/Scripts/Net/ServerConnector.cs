using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ServerConnector : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text _statusText;

    // private bool _connected = false;

    private void Awake()
    {
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
        // 만약 연결이 되어 있는 상태에서 이 스크립트가 있는 씬으로 이동하면 바로 해당 콜백 메소드가 실행되어 매칭 씬으로 이동.
        // 수정 필요
        _statusText.text = "서버와 연결되었습니다!";
        CoroutineTimer.SetTimerOnce(() => SceneManager.LoadScene("MatchScene"), 0.5f);
    }

    /// <summary>
    /// 포톤 서버와 연결을 실패했을 시 실행되는 콜백 메소드
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

    /// <summary>
    /// 포톤 서버에 연결을 시도합니다.
    /// </summary>
    public void Connect()
    {
        _statusText.text = "서버와 연결을 시도합니다...";
        PhotonNetwork.ConnectUsingSettings();
    }
}

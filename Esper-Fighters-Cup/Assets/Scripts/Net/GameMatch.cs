using System.Text;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMatch : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public const byte MaxPlayerCount = 2;

    [SerializeField] private Text _matchStatus;

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("서버와 연결되어 있지 않아 로비로 이동합니다.");
            SceneManager.LoadScene("LobbyScene");
            return;
        }

        // 매칭 씬 로드했는데 이미 룸에 있는 경우 해당 룸 빠져나가기
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        var roomOptions = new RoomOptions { MaxPlayers = MaxPlayerCount, PublishUserId = true };
        var result = PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: roomOptions);
        Debug.Log(result);
        _matchStatus.text = "매칭 중...";
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("On connected to master");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning(cause.ToString());
    }

    public override void OnJoinedRoom()
    {
        _matchStatus.text = "매칭 중... (Joined Room)";

        var room = PhotonNetwork.CurrentRoom;

        var sb = new StringBuilder()
            .AppendLine($"Joined Room info: {room}")
            .AppendLine($"PublishUserId: {room.PublishUserId}")
            .AppendLine($"IsMasterClient: {PhotonNetwork.IsMasterClient}")
            .AppendLine($"Your user id: {PhotonNetwork.LocalPlayer.UserId}")
            .AppendLine($"Your actor number: {PhotonNetwork.LocalPlayer.ActorNumber}");

        var debugText = sb.ToString();
        Debug.Log(debugText);
        _matchStatus.text += $"\n{debugText}";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"New Player: {newPlayer.NickName}#{newPlayer.UserId}");

        var room = PhotonNetwork.CurrentRoom;
        if (PhotonNetwork.IsMasterClient && room.PlayerCount == room.MaxPlayers)
        {
            PhotonEventUtils.BroadcastEvent(GameProtocol.GameMatchEvent, GameProtocol.GameMatch.Success, SendOptions.SendReliable);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case GameProtocol.GameMatchEvent:
                HandleMatchEvent(photonEvent.CustomData);
                break;
        }
    }

    private void HandleMatchEvent(object data)
    {
        var result = (GameProtocol.GameMatch)data;

        switch (result)
        {
            case GameProtocol.GameMatch.Success:
                OnMatched();
                break;

            case GameProtocol.GameMatch.Fail:
                OnmatchFailed();
                break;
        }
    }

    private void OnMatched()
    {
        _matchStatus.text = "매칭 성공";
        if (PhotonNetwork.IsMasterClient)
        {
            // 1.5초 뒤 게임씬으로 이동
            CoroutineTimer.SetTimerOnce(() => PhotonNetwork.LoadLevel("GameScene"), 1.5f);
        }
    }

    private void OnmatchFailed()
    {
    }
}

using System.Text;
using EsperFightersCup.Net;
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

        Debug.Log(PhotonNetwork.CountOfPlayers);

        var roomOptions = new RoomOptions { MaxPlayers = MaxPlayerCount, PublishUserId = true };
        var result = PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: roomOptions);
        _matchStatus.text = result ? "매칭 중..." : "매칭에 실패했습니다.";
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
            var packet = new GameMatchPacket(GameMatchResults.Success);
            PacketSender.Broadcast(GameProtocol.GameMatchEvent, in packet, SendOptions.SendReliable);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == GameProtocol.GameMatchEvent)
        {
            HandleMatchEvent(photonEvent);
        }
    }

    private void HandleMatchEvent(EventData received)
    {
        var buffer = (byte[])received.CustomData;
        var packet = PacketSerializer.Deserialize<GameMatchPacket>(buffer);

        switch (packet.MatchResult)
        {
            case GameMatchResults.Success:
                OnMatched();
                break;

            case GameMatchResults.Fail:
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
        // 매칭이 실패할 원인이 아직은 없음
    }
}

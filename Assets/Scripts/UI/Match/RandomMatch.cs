using EsperFightersCup.Net;
using EsperFightersCup.UI.Popup;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup.UI.Match
{
    [RequireComponent(typeof(GoToScene))]
    public class RandomMatch : PunEventCallbacks
    {
        public const byte MaxPlayers = 2;

        [SerializeField] private Text _matchingText;
        [SerializeField] private BasicPopup _popup;

        private Coroutine _matchFaildTimer;

        private void Start()
        {
            _matchingText.text = "매칭 중...";
#if UNITY_EDITOR
            // 연결되지 않고 인게임 화면이 나온다면 오프라인 모드를 통한 디버깅을 허용
            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogWarning("Enable Offline Mode!");
                PhotonNetwork.OfflineMode = true;
            }
#endif
            CoroutineTimer.SetTimerOnce(StartRandomMatch, 1f);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            var popup = Instantiate(_popup, FindObjectOfType<Canvas>().transform);
            popup.OnYesButtonClicked += () => GetComponent<GoToScene>().LoadScene("TitleScene");
            popup.Open("<color=red>매칭에 실패했습니다.</color>", $"서버와의 연결이 끊겼습니다. {cause}");
        }

        public override void OnJoinedRoom()
        {
            print("Joined Room");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            var popup = Instantiate(_popup, FindObjectOfType<Canvas>().transform);
            popup.OnYesButtonClicked += () => GetComponent<GoToScene>().LoadScene("TitleScene");
            popup.Open("<color=red>매칭에 실패했습니다.</color>", $"[{returnCode}] {message}");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            print($"New Player: {newPlayer.NickName}#{newPlayer.UserId}");

            var room = PhotonNetwork.CurrentRoom;
            if (PhotonNetwork.IsMasterClient && room.PlayerCount == room.MaxPlayers)
            {
                // MasterClient가 제일 오래 기다리므로 플레이어 들어왔는데
                // 타이밍 때문에 실패 메시지가 뜨지 않도록 MasterClient에서 먼저 faildTimer 제거
                CoroutineTimer.Stop(ref _matchFaildTimer);

                var packet = new GameMatchEvent(true);
                PacketSender.Broadcast(in packet, SendOptions.SendReliable);
            }
        }

        protected override void OnGameEventReceived(GameEventArguments args)
        {
            if (args.Code == GameProtocol.Match)
            {
                var data = (GameMatchEvent)args.EventData;
                if (data.IsMatched)
                {
                    OnMatched();
                }
            }
        }

        private void StartRandomMatch()
        {
            if (!PhotonNetwork.IsConnected)
            {
                OnFaild("서버와 연결되어 있지 않습니다.");
                return;
            }

            var roomOptions = new RoomOptions { MaxPlayers = MaxPlayers, PublishUserId = true };
            var result = PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: roomOptions);

            if (!result)
            {
                OnFaild(string.Empty);
                return;
            }

            _matchFaildTimer = CoroutineTimer.SetTimerOnce(OnMatchFailed, 20f);

            void OnFaild(string cause)
            {
                var popup = Instantiate(_popup, FindObjectOfType<Canvas>().transform);
                popup.OnYesButtonClicked += () => GetComponent<GoToScene>().LoadScene("TitleScene");
                popup.Open("<color=red>매칭에 실패했습니다.</color>", cause);
                return;
            }
        }

        private void OnMatched()
        {
            _matchingText.text = "유저를 찾았습니다!";
            if (PhotonNetwork.IsMasterClient)
            {
                // TODO: PhotonNetwork.LoadLevel 대신 따로 씬 로더를 만들어서 씬 전환 애니메이션 구현 대비하기
                // https://doc.photonengine.com/ko-kr/pun/current/gameplay/rpcsandraiseevent#_messageQ
                // 1.5초 뒤 게임씬으로 이동
                CoroutineTimer.SetTimerOnce(() => PhotonNetwork.LoadLevel("GameScene"), 2f);
            }
            else
            {
                CoroutineTimer.Stop(ref _matchFaildTimer);
            }
        }

        private void OnMatchFailed()
        {
            _matchingText.text = "유저를 찾지 못했습니다";

            PhotonNetwork.LeaveRoom();
            CoroutineTimer.SetTimerOnce(() => GetComponent<GoToScene>().LoadScene("LobbyScene"), 2f);
        }
    }
}

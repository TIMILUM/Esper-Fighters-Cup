using EsperFightersCup.Net;
using EsperFightersCup.UI.Popup;
using EsperFightersCup.Util;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EsperFightersCup.UI.Match
{
    public class RandomMatch : PunEventCallbacks
    {
        public const byte MaxPlayers = 2;

        [SerializeField] private Text _matchingText;
        [SerializeField] private BasicPopup _popup;

        private Coroutine _matchFaildTimer;

        private void Start()
        {
            _matchingText.text = "매칭 중...";
            CoroutineTimer.SetTimerOnce(StartRandomMatch, 1f);
        }

        private void StartRandomMatch()
        {
            if (!PhotonNetwork.IsConnected)
            {
                OnFaild("서버와 연결되어 있지 않습니다.");
                return;
            }

            _matchFaildTimer = CoroutineTimer.SetTimerOnce(OnMatchFailed, 20f);
            GameMatchSystem.Instance.OnMatched += OnMatched;

            var roomOptions = new RoomOptions { MaxPlayers = MaxPlayers, PublishUserId = true };
            var result = PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: roomOptions);

            if (!result)
            {
                OnFaild(string.Empty);
                return;
            }

            void OnFaild(string cause)
            {
                var popup = Instantiate(_popup, FindObjectOfType<Canvas>().transform);
                popup.OnYesButtonClicked += () => SceneManager.LoadScene("TitleScene");
                popup.Open("<color=red>매칭에 실패했습니다.</color>", cause);
                return;
            }
        }

        private void OnMatched()
        {
            _matchingText.text = "유저를 찾았습니다!";
            CoroutineTimer.Stop(ref _matchFaildTimer);
        }

        private void OnMatchFailed()
        {
            _matchingText.text = "유저를 찾지 못했습니다";

            PhotonNetwork.LeaveRoom();
            CoroutineTimer.SetTimerOnce(() => GetComponent<GoToScene>().LoadScene("LobbyScene"), 2f);
        }
    }
}

using EsperFightersCup.Net;
using EsperFightersCup.UI.Popup;
using EsperFightersCup.Util;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EsperFightersCup.UI.Match
{
    public class RandomMatch : PunEventCallbacks
    {
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
                PrintRandomMatchFaildMessage("서버와 연결되어 있지 않습니다", "TitleScene");
                return;
            }

            _matchFaildTimer = CoroutineTimer.SetTimerOnce(OnMatchFailed, 20f);
            GameMatchSystem.Instance.OnMatched += OnMatched;

            var result = PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: GameRoom.DefaultRoomOptions);

            if (!result)
            {
                PrintRandomMatchFaildMessage("랜덤 매칭에 실패했습니다", "LobbyScene");
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
            PrintRandomMatchFaildMessage("유저를 찾지 못했습니다", "LobbyScene");
        }

        private void PrintRandomMatchFaildMessage(string cause, string nextScene)
        {
            _matchingText.text = cause;

            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            CoroutineTimer.SetTimerOnce(() => SceneManager.LoadScene(nextScene), 2f);
        }
    }
}

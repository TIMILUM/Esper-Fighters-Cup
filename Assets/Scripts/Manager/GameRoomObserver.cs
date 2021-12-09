using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace EsperFightersCup
{
    public class GameRoomObserver : MonoBehaviourPunCallbacks
    {
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            StopGameAndShowPopup("상대 플레이어가 나갔습니다", "확인을 누르시면 로비로 이동합니다.", LeaveRoom);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            StopGameAndShowPopup("서버와 연결이 끊겼습니다", "확인을 누르시면 타이틀 화면으로 이동합니다.", GoToTitle);
        }

        private void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("LobbyScene");
            Time.timeScale = 1f;
        }

        private void GoToTitle()
        {
            SceneManager.LoadScene("TitleScene");
            Time.timeScale = 1f;
        }

        private void StopGameAndShowPopup(string title, string content, UnityAction afterPopupClicked)
        {
            Time.timeScale = 0f;

            var popup = PopupManager.Instance.CreateNewBasicPopup();
            popup.OnYesButtonClicked += afterPopupClicked;
            popup.Open(title, content);
        }
    }
}

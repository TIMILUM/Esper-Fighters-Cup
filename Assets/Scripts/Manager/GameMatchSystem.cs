using EsperFightersCup.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace EsperFightersCup
{
    public class GameMatchSystem : PunEventSingleton<GameMatchSystem>
    {
        [SerializeField] private BasicPopup _popup;

        public event UnityAction OnMatched;

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            var room = PhotonNetwork.CurrentRoom;
            if (PhotonNetwork.IsMasterClient && room.PlayerCount == room.MaxPlayers)
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { [CustomPropertyKeys.GameStarted] = true });
            }
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.TryGetValue(CustomPropertyKeys.GameStarted, out var value))
            {
                if ((bool)value == false)
                {
                    return;
                }

                OnMatched?.Invoke();
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.CurrentRoom.IsVisible = false;
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    CoroutineTimer.SetTimerOnce(() => PhotonNetwork.LoadLevel("CharacterChoiceScene"), 2f);
                }
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            PopupFaildMessage("매칭에 실패했습니다.", $"서버와의 연결이 끊겼습니다.\n{cause}", "TitleScene");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            MatchFaild(returnCode, message);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            MatchFaild(returnCode, message);
        }

        private void MatchFaild(short returnCode, string message)
        {
            PopupFaildMessage("매칭에 실패했습니다.", $"[{returnCode}] {message}", "TitleScene");
        }

        public void PopupFaildMessage(string title, string cause, string nextScene)
        {
            var popup = Instantiate(_popup, FindObjectOfType<Canvas>().transform);
            popup.OnYesButtonClicked += () => SceneManager.LoadScene(nextScene);
            popup.Open($"<color=red>{title}</color>", cause);
            return;
        }
    }
}

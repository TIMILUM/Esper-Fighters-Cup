using EsperFightersCup.UI.Popup;
using EsperFightersCup.Util;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace EsperFightersCup
{
    public class GameMatchSystem : PunEventSingleton<GameMatchSystem>
    {
        [SerializeField] private BasicPopup _popup;

        public event UnityAction OnMatched;

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.TryGetValue(CustomPropertyKeys.GameStarted, out var value)
                && (bool)value == true)
            {
                OnMatched?.Invoke();
                PhotonNetwork.CurrentRoom.IsVisible = false;
                PhotonNetwork.CurrentRoom.IsOpen = false;

                CoroutineTimer.SetTimerOnce(() => PhotonNetwork.LoadLevel("CharacterChoiceScene"), 2f);
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            var popup = Instantiate(_popup, FindObjectOfType<Canvas>().transform);
            popup.OnYesButtonClicked += () => SceneManager.LoadScene("TitleScene");
            popup.Open("<color=red>매칭에 실패했습니다.</color>", $"서버와의 연결이 끊겼습니다. {cause}");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            var popup = Instantiate(_popup, FindObjectOfType<Canvas>().transform);
            popup.OnYesButtonClicked += () => SceneManager.LoadScene("TitleScene");
            popup.Open("<color=red>매칭에 실패했습니다.</color>", $"[{returnCode}] {message}");
        }
    }
}

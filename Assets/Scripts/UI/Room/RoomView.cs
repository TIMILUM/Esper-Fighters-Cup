using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EsperFightersCup.UI
{
    public class RoomView : MonoBehaviour
    {
        [SerializeField] private Text _roomCodeText;
        [SerializeField] private Text _masterClientNameText;
        [SerializeField] private Text _otherClientNameText;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _startButton;

        private void Start()
        {
            _roomCodeText.text = "대기 중";
            _masterClientNameText.text = string.Empty;
            _otherClientNameText.text = string.Empty;

            _startButton.gameObject.SetActive(false);
            _startButton.interactable = false;

            _startButton.onClick.AddListener(RoomManager.Instance.StartGame);
            _exitButton.onClick.AddListener(RoomManager.Instance.ExitRoom);

            RoomManager.Instance.OnRoomCreateFaild += OnRoomCreateFaild;

            RoomManager.Instance.OnRoomCreated += (roomCode) => ResetRoomInfo();
            RoomManager.Instance.OnRoomJoined += () => ResetRoomInfo();
            RoomManager.Instance.OnPlayerJoined += (player) => ResetRoomInfo();
            RoomManager.Instance.OnPlayerLeft += (player) => ResetRoomInfo();
        }

        private void OnRoomCreateFaild(string message)
        {
            var popup = PopupManager.Instance.CreateNewBasicPopup();
            popup.OnYesButtonClicked += () => SceneManager.LoadScene("LobbyScene");
            popup.Open("<color=red>방 생성에 실패했습니다.</color>", message);
        }

        private void ResetRoomInfo()
        {
            var room = PhotonNetwork.CurrentRoom;

            _roomCodeText.text = room.Name; // RoomCode랑 똑같음

            _otherClientNameText.text = string.Empty;
            foreach (var player in room.Players.Values)
            {
                // var displayText = $"{player.NickName}#{player.UserId.Substring(0, 6)}";
                var displayText = player.NickName;
                if (player.IsMasterClient)
                {
                    _masterClientNameText.text = displayText;
                }
                else
                {
                    _otherClientNameText.text = displayText;
                }
            }

            _startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            _startButton.interactable = room.PlayerCount == room.MaxPlayers;
        }
    }
}

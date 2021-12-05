using System;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup.UI
{
    public class RoomFindView : MonoBehaviour
    {
        [SerializeField] private BasicPopup _popup;
        [SerializeField] private InputField _roomCodeInputField;
        [SerializeField] private Button _roomFindButton;

        private void Start()
        {
            CheckRoomCodeIsValid(string.Empty);
            _roomCodeInputField.onValueChanged.AddListener(CheckRoomCodeIsValid);
            _roomFindButton.onClick.AddListener(FindRoom);
        }

        private void CheckRoomCodeIsValid(string input)
        {
            _roomFindButton.interactable = input.Length == 4;
        }

        private void FindRoom()
        {
            FindRoomAsync().Forget();
        }

        private async UniTaskVoid FindRoomAsync()
        {
            _roomCodeInputField.interactable = false;
            _roomFindButton.interactable = false;

            var timeout = await UniTask.WaitUntil(() => PhotonNetwork.InLobby)
                .Timeout(TimeSpan.FromSeconds(10))
                .SuppressCancellationThrow();

            if (timeout)
            {
                _roomCodeInputField.interactable = true;
                _roomFindButton.interactable = true;
                return;
            }

            var code = _roomCodeInputField.text;
            var result = await RoomFindManager.Instance.FindRoomAsync(code);
            if (!result)
            {
                print("방 입장 실패");
                var popup = Instantiate(_popup, FindObjectOfType<Canvas>().transform);
                popup.Open("<color=red>방 입장에 실패했습니다.</color>", "다시 시도해주세요.");
                _roomCodeInputField.interactable = true;
                _roomFindButton.interactable = true;
            }
        }
    }
}

using EsperFightersCup.UI.Popup;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EsperFightersCup
{
    public class IDMatch : MonoBehaviour
    {
        [SerializeField] private InputField _idField;
        [SerializeField] private Button _createButton;
        [SerializeField] private Button _searchButton;
        [SerializeField] private Text _searchText;
        [SerializeField] private BasicPopup _popup;

        private void Start()
        {
            _searchText.text = string.Empty;
            _createButton.onClick.AddListener(CreateRoom);
            _searchButton.onClick.AddListener(SearchRoomById);
        }

        private void CreateRoom()
        {
            var result = PhotonNetwork.CreateRoom(string.Empty, roomOptions: GameRoomOptions.DefaultRoomOptions);
            if (!result && CheckServerConnected())
            {

            }
        }

        private void SearchRoomById()
        {

        }

        private bool CheckServerConnected()
        {
            if (PhotonNetwork.IsConnected)
            {
                return true;
            }
            OnFaild("서버와 연결되어 있지 않습니다.");
            return false;
        }

        private void OnFaild(string cause)
        {
            var popup = Instantiate(_popup, FindObjectOfType<Canvas>().transform);
            popup.OnYesButtonClicked += () => SceneManager.LoadScene("TitleScene");
            popup.Open("<color=red>매칭에 실패했습니다.</color>", cause);
            return;
        }
    }
}

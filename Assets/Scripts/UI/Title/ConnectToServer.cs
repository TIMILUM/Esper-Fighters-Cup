using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EsperFightersCup.UI
{
    public class ConnectToServer : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Button _connectButton;

        private Text _connectButtonText;

        private string _defaultText;

        private void Awake()
        {
            _connectButtonText = _connectButton.GetComponentInChildren<Text>();
            _defaultText = _connectButtonText.text;

            if (PhotonNetwork.IsConnected)
            {
                _connectButton.interactable = false;
                PhotonNetwork.Disconnect();
            }

            PhotonNetwork.GameVersion = Application.version;
            Debug.Log($"Game Version: {PhotonNetwork.GameVersion}");
        }

        public override void OnConnectedToMaster()
        {
            SceneManager.LoadScene("MainScene");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            if (cause == DisconnectCause.DisconnectByClientLogic)
            {
                _connectButton.interactable = true;
                Debug.Log($"정상적으로 서버와의 접속이 끊어졌습니다.");
                return;
            }

            var popup = PopupManager.Instance.CreateNewBasicPopup();

            popup.OnYesButtonClicked += () => CoroutineTimer.SetTimerOnce(ResetButton, 1f);
            popup.Open("<color=red>연결에 실패했습니다.</color>", cause.ToString());
        }

        public void Connect()
        {
            if (PhotonNetwork.IsConnected)
            {
                OnConnectedToMaster();
                return;
            }

            _connectButton.interactable = false;
            _connectButtonText.text = "연결 중...";
            Debug.Log($"Version: {PhotonNetwork.AppVersion}");
            PhotonNetwork.ConnectUsingSettings();
        }

        private void ResetButton()
        {
            _connectButton.interactable = true;
            _connectButtonText.text = _defaultText;
        }
    }
}

using EsperFightersCup.UI.Popup;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup.UI
{
    [RequireComponent(typeof(Button), typeof(GoToScene))]
    public class ConnectToServer : MonoBehaviourPunCallbacks
    {
        [SerializeField] private BasicPopup _connectFaildPopup;

        private Button _connectButton;
        private Text _connectButtonText;

        private string _defaultText;

        private void Awake()
        {
            _connectButton = GetComponent<Button>();

            _connectButtonText = _connectButton.GetComponentInChildren<Text>();
            _defaultText = _connectButtonText.text;
        }

        public override void OnConnectedToMaster()
        {
            GetComponent<GoToScene>().LoadScene("MainScene");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            var popup = Instantiate(_connectFaildPopup, FindObjectOfType<Canvas>().transform);

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
            PhotonNetwork.ConnectUsingSettings();
        }

        private void ResetButton()
        {
            _connectButton.interactable = true;
            _connectButtonText.text = _defaultText;
        }
    }
}

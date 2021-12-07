using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EsperFightersCup
{
    public class PauseUI : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private CanvasGroup _panel;

        private void Awake()
        {
            _panel.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _panel.gameObject.SetActive(!_panel.gameObject.activeInHierarchy);
            }
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            if (PhotonNetwork.OfflineMode)
            {
                PhotonNetwork.OfflineMode = false;
                SceneManager.LoadScene("TitleScene");
            }

            SceneManager.LoadScene("MainScene");
        }
    }
}

using DG.Tweening;
using EsperFightersCup.Manager;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace EsperFightersCup.UI.GameResult
{
    public class ResultView : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Text _winnerNameText;
        [SerializeField] private Text _loserNameText;
        [SerializeField] private Button _homeButton;
        [SerializeField] private Button _lobbyButton;
        [SerializeField] private Button _rematchButton;

        private Text _rematchButtonText;

        private void Start()
        {
            if (_rematchButton)
            {
                _rematchButtonText = _rematchButton.GetComponentInChildren<Text>();
                _rematchButton.onClick.AddListener(TryRematch);
            }
            if (_lobbyButton)
            {
                _lobbyButton.onClick.AddListener(GoToLobby);
            }
            if (_homeButton)
            {
                _homeButton.onClick.AddListener(GoToHome);
            }

            if (!PhotonNetwork.InRoom)
            {
                _winnerNameText.text = "????";
                _loserNameText.text = "????";
                return;
            }

            var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;

            if (roomProps.TryGetValue("winner", out var winner))
            {
                _winnerNameText.text = winner as string ?? "???";
            }
            else
            {
                _winnerNameText.text = "???";
            }

            if (roomProps.TryGetValue("loser", out var loser))
            {
                _loserNameText.text = loser as string ?? "???";
            }
            else
            {
                _loserNameText.text = "???";
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                _rematchButtonText.text = "상대방 나감";
                _rematchButton.interactable = false;
            }
        }

        public void GoToLobby()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            SceneManager.LoadScene("LobbyScene");
        }

        public void GoToHome()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            SceneManager.LoadScene("MainScene");
        }

        public void TryRematch()
        {
            if (!PhotonNetwork.InRoom)
            {
                return;
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { [GameRematchManager.RematchPropKey] = true });
            _rematchButtonText.text = "대기 중";
            _rematchButton.interactable = false;

            DOTween.Sequence()
                .AppendInterval(10f)
                .AppendCallback(() => ResetRematch())
                .SetLink(gameObject);
        }

        private void ResetRematch()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { [GameRematchManager.RematchPropKey] = false });
                _rematchButtonText.text = "재 대결 신청";
                _rematchButton.interactable = true;
            }
            else
            {
                _rematchButtonText.text = "재 대결 불가";
                _rematchButton.interactable = false;
            }
        }
    }
}

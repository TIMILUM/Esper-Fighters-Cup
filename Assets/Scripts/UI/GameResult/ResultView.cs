using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        private CancellationTokenSource _rematchCancellation;

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

            if (roomProps.TryGetValue(CustomPropertyKeys.GameWinner, out var winner))
            {
                _winnerNameText.text = winner as string ?? "???";
            }
            else
            {
                _winnerNameText.text = "???";
            }

            if (roomProps.TryGetValue(CustomPropertyKeys.GameLooser, out var loser))
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
                _rematchCancellation?.Cancel();
                _rematchButtonText.text = "상대방 나감";
                _rematchButton.interactable = false;
            }
        }

        public void GoToLobby()
        {
            LeaveGame("LobbyScene");
        }

        public void GoToHome()
        {
            LeaveGame("MainScene");
        }

        public void TryRematch()
        {
            _rematchCancellation = new CancellationTokenSource();
            TryRematchAsync(_rematchCancellation.Token).Forget();
        }

        private async UniTask TryRematchAsync(CancellationToken cancellation)
        {
            if (!PhotonNetwork.InRoom)
            {
                return;
            }

            PhotonNetwork.LocalPlayer.SetCustomProperty(CustomPropertyKeys.PlayerGameRematch, true);
            PhotonNetwork.SendAllOutgoingCommands();

            _rematchButtonText.text = "대기 중";
            _rematchButton.interactable = false;

            var canceled = await UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: cancellation).SuppressCancellationThrow();
            if (!canceled)
            {
                ResetRematch();
            }
        }

        private void ResetRematch()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LocalPlayer.SetCustomProperty(CustomPropertyKeys.PlayerGameRematch, false);
                PhotonNetwork.SendAllOutgoingCommands();

                _rematchButtonText.text = "재 대결 신청";
                _rematchButton.interactable = true;
            }
            else
            {
                _rematchButtonText.text = "재 대결 불가";
                _rematchButton.interactable = false;
            }
        }

        private void LeaveGame(string nextScene)
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
                var keys = PhotonNetwork.LocalPlayer.CustomProperties.Keys;
                PhotonNetwork.RemovePlayerCustomProperties(keys.Select(key => key as string).ToArray());
            }
            SceneManager.LoadScene(nextScene);
        }
    }
}

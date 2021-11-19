using Cysharp.Threading.Tasks;
using EsperFightersCup.UI.InGame;
using Photon.Pun;
using UnityEngine;

namespace EsperFightersCup
{
    public class IngameRoundEndState : InGameFSMStateBase
    {
        [SerializeField]
        private GameStateView _gameStateView;

        private int _count;

        protected override void Initialize()
        {
            State = IngameFSMSystem.State.RoundEnd;
        }

        public override void StartState()
        {
            _count = 0;
            base.StartState();
            _gameStateView.Show("K.O.", Vector2.left * 20f);

            NextStateAsync().Forget();
        }

        private async UniTask NextStateAsync()
        {
            await UniTask.Delay(2000);
            await FsmSystem.Curtain.FadeInAsync();

            FsmSystem.photonView.RPC(nameof(RoundEndRPC), RpcTarget.MasterClient);
        }

        [PunRPC]
        private void RoundEndRPC()
        {
            _count++;
            if (_count == InGamePlayerManager.Instance.GamePlayers.Count)
            {
                FsmSystem.photonView.RPC(nameof(RoundEndNextRPC), RpcTarget.All);
            }
        }

        [PunRPC]
        private void RoundEndNextRPC()
        {
            var sawblades = SawBladeSystem.Instance.LocalSpawnedSawBlades;
            Debug.Log($"Destroying {sawblades.Count} sawblades");
            foreach (var sawblade in sawblades.Values)
            {
                PhotonNetwork.Destroy(sawblade.gameObject);
            }

            var winPoint = (int)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyKeys.PlayerWinPoint];
            Debug.Log($"My win point: {winPoint}");

            var winner = (int)PhotonNetwork.CurrentRoom.CustomProperties[CustomPropertyKeys.GameRoundWinner];
            if (winner != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return;
            }

            if (winPoint < 3)
            {
                ChangeState(IngameFSMSystem.State.RoundIntro);
            }
            else
            {
                ChangeState(IngameFSMSystem.State.EndingCut);
            }
        }
    }
}

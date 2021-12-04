using Cysharp.Threading.Tasks;
using EsperFightersCup.UI.InGame;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace EsperFightersCup
{
    public class IngameRoundEndState : InGameFSMStateBase
    {
        [SerializeField] private GameStateView _gameStateView;
        [SerializeField] private UnityEvent<int> _onRoundEnd;

        private int _count;

        /// <summary>
        /// 라운드가 끝나고 페이드인 후 이벤트 발생<para/>
        /// <see cref="int"/> -> 끝난 라운드
        /// </summary>
        public event UnityAction<int> OnRoundEnd
        {
            add => _onRoundEnd.AddListener(value);
            remove => _onRoundEnd.RemoveListener(value);
        }

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
            _onRoundEnd?.Invoke(FsmSystem.Round);
            /*
            var sawblades = SawBladeSystem.Instance.LocalSpawnedSawBlades;
            Debug.Log($"Destroying {sawblades.Count} sawblades");
            foreach (var sawblade in sawblades.Values)
            {
                PhotonNetwork.Destroy(sawblade.gameObject);
            }
            */
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

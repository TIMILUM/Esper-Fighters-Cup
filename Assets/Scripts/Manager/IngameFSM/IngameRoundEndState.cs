using Cysharp.Threading.Tasks;
using EsperFightersCup.UI;
using Photon.Pun;
using Photon.Realtime;
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
            _gameStateView.End();

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
            if (_count == FsmSystem.RoomPlayers.Count)
            {
                FsmSystem.photonView.RPC(nameof(RoundEndNextRPC), RpcTarget.All);
            }
        }

        [PunRPC]
        private void RoundEndNextRPC()
        {
            _onRoundEnd?.Invoke(FsmSystem.Round);

            if (!PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
            if (!roomProps.TryGetValue(CustomPropertyKeys.GameRoundWinner, out var winnerValue))
            {
                return;
            }
            var winner = (int)winnerValue;
            if (PhotonNetwork.LocalPlayer.ActorNumber != winner)
            {
                return;
            }
            var playerProps = PhotonNetwork.LocalPlayer.CustomProperties;
            if (!playerProps.TryGetValue(CustomPropertyKeys.PlayerWinPoint, out var winVal))
            {
                return;
            }
            var winPoint = (int)winVal;
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

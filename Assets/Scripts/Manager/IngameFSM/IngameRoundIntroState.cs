using Cysharp.Threading.Tasks;
using EsperFightersCup.UI;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace EsperFightersCup
{
    public class IngameRoundIntroState : InGameFSMStateBase
    {
        [SerializeField] private GameStateView _gameStateView;
        [SerializeField] private UnityEvent<int> _onRoundStart;

        private int _count;

        /// <summary>
        /// 페이드아웃 후 라운드 시작 텍스트 출력 후 이벤트 발생<para/>
        /// <see cref="int"/> -> 시작한 라운드
        /// </summary>
        public event UnityAction<int> OnRoundStart
        {
            add => _onRoundStart.AddListener(value);
            remove => _onRoundStart.RemoveListener(value);
        }

        protected override void Initialize()
        {
            State = IngameFSMSystem.State.RoundIntro;
        }

        public override void StartState()
        {
            base.StartState();
            _count = 0;

            if (PhotonNetwork.IsMasterClient)
            {
                var round = FsmSystem.Round;

                var props = new Hashtable
                {
                    [CustomPropertyKeys.GameRound] = ++round,
                    [CustomPropertyKeys.GameRoundWinner] = 0
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }

            /*
            // 로컬플레이어 설정
            var localplayer = InGamePlayerManager.Instance.LocalPlayer;
            localplayer.ResetPositionAndRotation();
            localplayer.HP = 100;
            */
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (!propertiesThatChanged.ContainsKey(CustomPropertyKeys.GameRound))
            {
                return;
            }
            FsmSystem.photonView.RPC(nameof(RoundSetCompleteRPC), RpcTarget.MasterClient);
        }

        [PunRPC]
        private void RoundSetCompleteRPC()
        {
            _count++;
            if (_count == FsmSystem.RoomPlayers.Count)
            {
                FsmSystem.photonView.RPC(nameof(RoundIntroRPC), RpcTarget.All);
            }
        }

        [PunRPC]
        private void RoundIntroRPC()
        {
            _count = 0;
            RoundIntroAsync(FsmSystem.Round).Forget();
        }

        private async UniTask RoundIntroAsync(int round)
        {
            await UniTask.NextFrame();

            IngameBGMManager.Instance.IngameBGMUpdate(round);
            _onRoundStart?.Invoke(round);
            await UniTask.WaitUntil(() => InGamePlayerManager.Instance.GamePlayers.Count == PhotonNetwork.CurrentRoom.PlayerCount);

            await FsmSystem.Curtain.FadeOutAsync();

            await UniTask.Delay(2000);
            _gameStateView.Ready();

            await UniTask.Delay(1500);
            _gameStateView.Fight();

            FsmSystem.photonView.RPC(nameof(RoundIntroEndRPC), RpcTarget.MasterClient);
        }

        [PunRPC]
        private void RoundIntroEndRPC()
        {
            _count++;
            if (_count == InGamePlayerManager.Instance.GamePlayers.Count)
            {
                ChangeState(IngameFSMSystem.State.InBattle);
            }
        }
    }
}

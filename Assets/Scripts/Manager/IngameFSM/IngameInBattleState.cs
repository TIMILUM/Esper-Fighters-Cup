using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace EsperFightersCup
{
    public class IngameInBattleState : InGameFSMStateBase
    {
        [SerializeField] private UnityEvent _onBattleStart;
        [SerializeField] private UnityEvent _onBattleEnd;

        private CancellationTokenSource _checkCanllation;

        public event UnityAction OnBattleStart
        {
            add => _onBattleStart.AddListener(value);
            remove => _onBattleStart.RemoveListener(value);
        }

        public event UnityAction OnBattleEnd
        {
            add => _onBattleEnd.AddListener(value);
            remove => _onBattleEnd.RemoveListener(value);
        }

        protected override void Initialize()
        {
            State = IngameFSMSystem.State.InBattle;
        }

        public override void StartState()
        {
            base.StartState();

            _onBattleStart?.Invoke();

            _checkCanllation = new CancellationTokenSource();
            CheckLocalPlayerHPAsync(_checkCanllation.Token).Forget();
            GenerateSawBladeAsync(_checkCanllation.Token).Forget();
        }

        public override void EndState()
        {
            base.EndState();
        }

        private async UniTask GenerateSawBladeAsync(CancellationToken cancellation)
        {
            var start = PhotonNetwork.ServerTimestamp;
            var localplayer = InGamePlayerManager.Instance.LocalPlayer;

            while (!cancellation.IsCancellationRequested)
            {
                if (!localplayer)
                {
                    return;
                }

                if (localplayer.HP <= 30)
                {
                    var time = PhotonNetwork.ServerTimestamp - start;
                    if (time > 5000)
                    {
                        start = PhotonNetwork.ServerTimestamp;
                        FsmSystem.SawBladeSystem.GenerateSawBlade();
                    }
                }

                await UniTask.NextFrame();
            }
        }

        private async UniTask CheckLocalPlayerHPAsync(CancellationToken cancellation)
        {
            // 상대방이 먼저 피가 0이 되어서 RPC를 보내면 WaitUntil 종료 및 false 반환
            var isDead = !await UniTask.WaitUntil(CheckLocalPlayerIsDead, cancellationToken: cancellation)
                .SuppressCancellationThrow();

            // 본인이 먼저 피가 0이 된 경우에 RPC 호출
            if (isDead)
            {
                Debug.Log($"LocalPlayer is dead!");
                var actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                int winner = InGamePlayerManager.Instance.GamePlayers.Keys.First(x => x != actorNumber);
                PhotonNetwork.CurrentRoom.SetCustomPropertyBySafe(CustomPropertyKeys.GameRoundWinner, winner);
            }

            static bool CheckLocalPlayerIsDead()
            {
                if (InGamePlayerManager.Instance is null)
                {
                    return false;
                }
                var localplayer = InGamePlayerManager.Instance.LocalPlayer;
                return localplayer.HP <= 0;
            }
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (!propertiesThatChanged.TryGetValue(CustomPropertyKeys.GameRoundWinner, out var value))
            {
                return;
            }

            print("IngameInBattleState - OnRoomPropertiesUpdate");

            _checkCanllation.Cancel();

            _onBattleEnd?.Invoke();

            var winner = (int)value;
            Debug.Log($"Round winner is {winner}");
            if (winner == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                // 라운드 우승자는 WinPoint에 1을 더하고 RoundEnd로 GameState 변경
                var winPoint = (int)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyKeys.PlayerWinPoint];
                PhotonNetwork.LocalPlayer.SetCustomProperty(CustomPropertyKeys.PlayerWinPoint, ++winPoint);
                Debug.Log($"Add WinPoint to LocalPlayer - {winPoint}");

                PhotonNetwork.SendAllOutgoingCommands();
                ChangeState(IngameFSMSystem.State.RoundEnd);
            }
        }
    }
}

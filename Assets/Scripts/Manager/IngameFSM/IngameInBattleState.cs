using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
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
            if (PhotonNetwork.OfflineMode)
            {
                CheckDummyPlayerHPAsync(_checkCanllation.Token).Forget();
            }

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
            APlayer dummyPlayer = null;
            if (PhotonNetwork.OfflineMode)
            {
                dummyPlayer = InGamePlayerManager.Instance.GamePlayers[PhotonNetwork.LocalPlayer.ActorNumber + 1];
            }

            while (!cancellation.IsCancellationRequested)
            {
                if (!localplayer)
                {
                    return;
                }

                if (localplayer.HP <= 30 || (!(dummyPlayer is null) && dummyPlayer.HP <= 30))
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
                int winner;
                try
                {
                    winner = InGamePlayerManager.Instance.GamePlayers.Keys.First(x => x != actorNumber);
                }
                catch (System.InvalidOperationException)
                {
                    winner = actorNumber;
                }

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

        private async UniTask CheckDummyPlayerHPAsync(CancellationToken cancellation)
        {
            var dummyActorNr = PhotonNetwork.LocalPlayer.ActorNumber + 1;
            var dummyPlayer = InGamePlayerManager.Instance.GamePlayers.First(x => x.Key == dummyActorNr).Value;

            // 상대방이 먼저 피가 0이 되어서 RPC를 보내면 WaitUntil 종료 및 false 반환
            var isDead = !await UniTask.WaitUntil(CheckDummyPlayerIsDead, cancellationToken: cancellation)
                .SuppressCancellationThrow();

            // 본인이 먼저 피가 0이 된 경우에 RPC 호출
            if (isDead)
            {
                Debug.Log($"DummyPlayer is dead!");

                int winner;
                try
                {
                    winner = InGamePlayerManager.Instance.GamePlayers.Keys.First(x => x != dummyActorNr);
                }
                catch (System.InvalidOperationException)
                {
                    // NOTE: 오프라인 디버깅용 코드임. 본인이 죽어도 본인이 이긴걸로 판단함
                    winner = dummyActorNr;
                }

                PhotonNetwork.CurrentRoom.SetCustomPropertyBySafe(CustomPropertyKeys.GameRoundWinner, winner);
            }

            bool CheckDummyPlayerIsDead()
            {
                if (InGamePlayerManager.Instance is null)
                {
                    return false;
                }
                return dummyPlayer.HP <= 0;
            }
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (!propertiesThatChanged.TryGetValue(CustomPropertyKeys.GameRoundWinner, out var winnerValue))
            {
                return;
            }

            _checkCanllation.Cancel();

            _onBattleEnd?.Invoke();

            var winner = (int)winnerValue;
            Debug.Log($"Round winner is {winner}");
            if (winner == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                // 라운드 우승자는 WinPoint에 1을 더하고 RoundEnd로 GameState 변경
                int winPoint = 0;
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(CustomPropertyKeys.PlayerWinPoint, out var winPointValue))
                {
                    winPoint = (int)winPointValue;
                }
                PhotonNetwork.LocalPlayer.SetCustomProperty(CustomPropertyKeys.PlayerWinPoint, ++winPoint);
            }
            else if (PhotonNetwork.OfflineMode)
            {
                int winPoint = 0;
                if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("dummyWin", out var winPointValue))
                {
                    winPoint = (int)winPointValue;
                }
                PhotonNetwork.LocalPlayer.SetCustomProperty("dummyWin", ++winPoint);
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.TryGetValue("dummyWin", out var _))
            {
                ChangeState(IngameFSMSystem.State.RoundEnd);
                return;
            }

            if (!changedProps.TryGetValue(CustomPropertyKeys.PlayerWinPoint, out var value))
            {
                return;
            }
            var winPoint = (int)value;
            Debug.Log($"Added WinPoint to [{targetPlayer.ActorNumber}]{targetPlayer.NickName} - {winPoint}");

            // 상대방 플레이어가 WinPoint 변경을 확인했을 때 RoundEnd로 넘어감
            if (targetPlayer != PhotonNetwork.LocalPlayer || PhotonNetwork.OfflineMode)
            {
                ChangeState(IngameFSMSystem.State.RoundEnd);
            }
        }
    }
}

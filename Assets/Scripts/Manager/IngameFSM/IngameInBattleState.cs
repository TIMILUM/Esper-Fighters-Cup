using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

namespace EsperFightersCup
{
    public class IngameInBattleState : InGameFSMStateBase
    {
        private CancellationTokenSource _checkCanllation;

        protected override void Initialize()
        {
            State = IngameFSMSystem.State.InBattle;
        }

        public override void StartState()
        {
            base.StartState();
            _checkCanllation = new CancellationTokenSource();
            CheckLocalPlayerHPAsync(_checkCanllation.Token).Forget();
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
                var winner = InGamePlayerManager.Instance.GamePlayers.Keys.First(x => x != actorNumber);
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

            var winner = (int)value;
            Debug.Log($"Round winner is {winner}");
            if (winner == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                _checkCanllation.Cancel();

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

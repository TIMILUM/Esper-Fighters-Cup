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
            GenerateSawBladeAsync(_checkCanllation.Token).Forget();
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

                await UniTask.Yield();
            }
        }

        private async UniTask CheckLocalPlayerHPAsync(CancellationToken cancellation)
        {
            // ?곷?諛⑹씠 癒쇱? ?쇨? 0???섏뼱??RPC瑜?蹂대궡硫?WaitUntil 醫낅즺 諛?false 諛섑솚
            var isDead = !await UniTask.WaitUntil(CheckLocalPlayerIsDead, cancellationToken: cancellation)
                .SuppressCancellationThrow();

            // 蹂몄씤??癒쇱? ?쇨? 0????寃쎌슦??RPC ?몄텧
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

            _checkCanllation.Cancel();

            var winner = (int)value;
            Debug.Log($"Round winner is {winner}");
            if (winner == PhotonNetwork.LocalPlayer.ActorNumber)
            {

                // ?쇱슫???곗듅?먮뒗 WinPoint??1???뷀븯怨?RoundEnd濡?GameState 蹂寃?
                var winPoint = (int)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyKeys.PlayerWinPoint];
                PhotonNetwork.LocalPlayer.SetCustomProperty(CustomPropertyKeys.PlayerWinPoint, ++winPoint);
                Debug.Log($"Add WinPoint to LocalPlayer - {winPoint}");

                PhotonNetwork.SendAllOutgoingCommands();
                ChangeState(IngameFSMSystem.State.RoundEnd);
            }
        }
    }
}

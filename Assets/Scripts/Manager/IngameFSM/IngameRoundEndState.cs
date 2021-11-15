using System.Linq;
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

        protected override void Initialize()
        {
            State = IngameFSMSystem.State.RoundEnd;
        }

        public override void StartState()
        {
            base.StartState();
            _gameStateView.Show("K.O.", Vector2.left * 20f);

            NextStateAsync().Forget();
        }

        private async UniTask NextStateAsync()
        {
            await UniTask.Delay(2000);
            await FsmSystem.Curtain.FadeInAsync();

            var players = InGamePlayerManager.Instance.GamePlayers;

            foreach (var player in players)
            {
                Debug.Log($"WinPoint - [{player.Key}] = {player.Value.WinPoint}");
            }

            var higher = players.OrderBy(x => x.Value.WinPoint).First();
            Debug.Log($"Higher is {higher.Key}");

            // WinPoint가 제일 높은 사람이 다음 State를 정함
            if (higher.Key != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return;
            }

            if (higher.Value.WinPoint < 3)
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

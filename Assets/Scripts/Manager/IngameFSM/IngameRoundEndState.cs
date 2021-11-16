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

            var winPoint = (int)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyKeys.PlayerWinPoint];
            Debug.Log($"My win point: {winPoint}");

            // 이번 라운드 우승자의 WinPoint 체크 후 다음 State 결정
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

using System.Collections;
using System.Collections.Generic;
using EsperFightersCup.UI.InGame;
using UnityEngine;

namespace EsperFightersCup
{
    public class IngameRoundEndState : InGameFSMStateBase
    {
        [SerializeField]
        private GameStateView _gameStateView;

        private (short, short) _winPoints;
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        protected override void Initialize()
        {
            State = IngameFSMSystem.State.RoundEnd;
        }

        public override void StartState()
        {
            base.StartState();
            _gameStateView.Show("K.O.", Vector2.left * 20f);
            _winPoints = FsmSystem.GetWinPoint();
            var isGameOver = (_winPoints.Item1 >= 2 || _winPoints.Item2 >= 2);

            CoroutineTimer.SetTimerOnce(() =>
            {
                ChangeState(isGameOver ? IngameFSMSystem.State.Result : IngameFSMSystem.State.RoundIntro);
            }, 4f);
        }

        public override void EndState()
        {
            base.EndState();
        }
    }
}

namespace EsperFightersCup
{
    public class IngameInBattleState : InGameFSMStateBase
    {
        // Update is called once per frame
        private void Update()
        {
            /*
            var playerList = FsmSystem.PlayerList;
            for (var i = 0; i < playerList.Count; i++)
            {
                var player = playerList[i];
                if (!(player.Hp <= 0))
                {
                    continue;
                }

                FsmSystem.AddWinPoint(i);
                ChangeState(IngameFSMSystem.State.RoundEnd);
            }
            */
        }

        protected override void Initialize()
        {
            State = IngameFSMSystem.State.InBattle;
        }

        public override void StartState()
        {
            base.StartState();
        }

        public override void EndState()
        {
            base.EndState();
        }
    }
}

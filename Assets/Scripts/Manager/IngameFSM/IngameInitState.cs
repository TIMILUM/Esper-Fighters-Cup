namespace EsperFightersCup
{
    public class IngameInitState : InGameFSMStateBase
    {
        protected override void Initialize()
        {
            State = IngameFSMSystem.State.Init;
        }

        public override void EndState()
        {
            base.EndState();
        }
    }
}
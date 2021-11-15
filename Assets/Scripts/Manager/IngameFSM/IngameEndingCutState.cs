namespace EsperFightersCup
{
    public class IngameEndingCutState : InGameFSMStateBase
    {
        protected override void Initialize()
        {
            State = IngameFSMSystem.State.EndingCut;
        }
    }
}

namespace EsperFightersCup
{
    public class IngameIntroCutState : InGameFSMStateBase
    {
        protected override void Initialize()
        {
            State = IngameFSMSystem.State.IntroCut;
        }
    }
}

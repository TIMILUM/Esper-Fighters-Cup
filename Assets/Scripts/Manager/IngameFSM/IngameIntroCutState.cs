using Cysharp.Threading.Tasks;

namespace EsperFightersCup
{
    public class IngameIntroCutState : InGameFSMStateBase
    {
        protected override void Initialize()
        {
            State = IngameFSMSystem.State.IntroCut;
        }

        public override void StartState()
        {
            base.StartState();
            UniTask.Run(FsmSystem.Curtain.FadeOut);
        }
    }
}

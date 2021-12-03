using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Playables;

namespace EsperFightersCup
{
    public class IngameEndingCutState : InGameFSMStateBase
    {
        [SerializeField] private PlayableDirector _outro;

        private int _count;

        protected override void Initialize()
        {
            State = IngameFSMSystem.State.EndingCut;
            _outro.gameObject.SetActive(false);
        }

        public override void StartState()
        {
            base.StartState();

            // outro 새로 만들어서 등록 필요
            _outro.gameObject.SetActive(true);
            FsmSystem.Curtain.FadeOutAsync();
            _outro.stopped += OnOutroEnd;
            _outro.Play();
        }

        private void OnOutroEnd(PlayableDirector director)
        {
            director.stopped -= OnOutroEnd;
            EndEndingCutAsync().Forget();
        }

        private async UniTask EndEndingCutAsync()
        {
            await FsmSystem.Curtain.FadeInAsync();
            _outro.gameObject.SetActive(false);
            FsmSystem.photonView.RPC(nameof(OutroEndRPC), RpcTarget.MasterClient);
        }

        [PunRPC]
        private void OutroEndRPC()
        {
            _count++;
            if (_count == InGamePlayerManager.Instance.GamePlayers.Count)
            {
                ChangeState(IngameFSMSystem.State.Result);
            }
        }
    }
}

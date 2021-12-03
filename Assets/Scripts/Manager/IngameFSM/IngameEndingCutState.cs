using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace EsperFightersCup
{
    public class IngameEndingCutState : InGameFSMStateBase
    {
        [SerializeField] private PlayableDirector _outro;
        [SerializeField] private UnityEvent _onOutroStart;
        [SerializeField] private UnityEvent _onOutroEnd;

        private int _count;

        public event UnityAction OnOutroStart
        {
            add => _onOutroStart.AddListener(value);
            remove => _onOutroStart.RemoveListener(value);
        }

        public event UnityAction OnOutroEnd
        {
            add => _onOutroEnd.AddListener(value);
            remove => _onOutroEnd.RemoveListener(value);
        }

        protected override void Initialize()
        {
            State = IngameFSMSystem.State.EndingCut;
            _outro.gameObject.SetActive(false);
        }

        public override void StartState()
        {
            base.StartState();

            _onOutroStart?.Invoke();
            // outro 새로 만들어서 등록 필요
            _outro.gameObject.SetActive(true);
            FsmSystem.Curtain.FadeOutAsync();
            _outro.stopped += HandleOutroStopped;
            _outro.Play();
        }

        private void HandleOutroStopped(PlayableDirector director)
        {
            director.stopped -= HandleOutroStopped;
            EndEndingCutAsync().Forget();
        }

        private async UniTask EndEndingCutAsync()
        {
            await FsmSystem.Curtain.FadeInAsync();
            _outro.gameObject.SetActive(false);

            _onOutroEnd?.Invoke();

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

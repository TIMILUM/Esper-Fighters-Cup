using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace EsperFightersCup
{
    public class IngameIntroCutState : InGameFSMStateBase
    {
        [SerializeField] private PlayableDirector _intro;
        [SerializeField] private UnityEvent _onIntroStart;
        [SerializeField] private UnityEvent _onIntroEnd;

        private int _count;

        public event UnityAction OnIntroStart
        {
            add => _onIntroStart.AddListener(value);
            remove => _onIntroStart.RemoveListener(value);
        }

        public event UnityAction OnIntroEnd
        {
            add => _onIntroEnd.AddListener(value);
            remove => _onIntroEnd.RemoveListener(value);
        }

        protected override void Initialize()
        {
            State = IngameFSMSystem.State.IntroCut;
            _intro.gameObject.SetActive(false);
        }

        public override void StartState()
        {
            base.StartState();

            UniTask.Delay(1000).ContinueWith(() =>
            {
                _onIntroStart?.Invoke();

                _intro.gameObject.SetActive(true);
                FsmSystem.Curtain.FadeOutAsync();
                _intro.stopped += HandleIntroStopped;
                _intro.Play();
            }).Forget();
        }

        // 컷씬 끝났을 때 실행
        private void HandleIntroStopped(PlayableDirector director)
        {
            director.stopped -= HandleIntroStopped;
            EndIntroCutAsync().Forget();
        }

        private async UniTask EndIntroCutAsync()
        {
            // 컷씬이 비활성화되면 바로 게임카메라가 보이기 때문에 페이드아웃 후 비활성화해야 함
            await FsmSystem.Curtain.FadeInAsync();

            // 컷씬 비활성화
            _intro.gameObject.SetActive(false);

            _onIntroEnd?.Invoke();

            // 마스터클라이언트로 RPC보내서 컷씬 완료 신호
            FsmSystem.photonView.RPC(nameof(IntroEndRPC), RpcTarget.MasterClient);
        }

        [PunRPC]
        private void IntroEndRPC()
        {
            // 이 RPC는 MasterClient만 받기 때문에 MasterClient가 체크 후 GameState 변경
            _count++;
            if (_count == FsmSystem.RoomPlayers.Count)
            {
                ChangeState(IngameFSMSystem.State.RoundIntro);
            }
        }
    }
}

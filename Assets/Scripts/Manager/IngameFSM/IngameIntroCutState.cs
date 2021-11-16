using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Playables;

namespace EsperFightersCup
{
    public class IngameIntroCutState : InGameFSMStateBase
    {
        [SerializeField] private PlayableDirector _intro;

        private int _count;

        protected override void Initialize()
        {
            State = IngameFSMSystem.State.IntroCut;
        }

        public override void StartState()
        {
            base.StartState();
            PhotonNetwork.LocalPlayer.SetCustomProperties(CustomPropertyKeys.PlayerWinPoint, 0);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomPropertyKeys.GameRound, 0);

            _intro.gameObject.SetActive(true);
            FsmSystem.Curtain.FadeOutAsync();
            _intro.stopped += OnIntroEnd;
            _intro.Play();
        }

        // 컷씬 끝났을 때 실행
        private void OnIntroEnd(PlayableDirector director)
        {
            director.stopped -= OnIntroEnd;
            EndIntroCutAsync().Forget();
        }

        private async UniTask EndIntroCutAsync()
        {
            // 컷씬이 비활성화되면 바로 게임카메라가 보이기 때문에 페이드아웃 후 비활성화해야 함
            await FsmSystem.Curtain.FadeInAsync();

            // 컷씬 비활성화
            _intro.gameObject.SetActive(false);

            // 마스터클라이언트로 RPC보내서 컷씬 완료 신호
            FsmSystem.photonView.RPC(nameof(IntroEndRPC), RpcTarget.MasterClient);
        }

        [PunRPC]
        private void IntroEndRPC()
        {
            // 이 RPC는 MasterClient만 받기 때문에 MasterClient가 체크 후 GameState 변경
            _count++;
            if (_count == InGamePlayerManager.Instance.GamePlayers.Count)
            {
                ChangeState(IngameFSMSystem.State.RoundIntro);
            }
        }
    }
}

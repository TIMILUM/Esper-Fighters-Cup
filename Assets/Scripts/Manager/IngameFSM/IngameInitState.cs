using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace EsperFightersCup
{
    public class IngameInitState : InGameFSMStateBase
    {
        [SerializeField] private UnityEvent _onReadyToPlay;

        public event UnityAction OnReadyToPlay
        {
            add => _onReadyToPlay.AddListener(value);
            remove => _onReadyToPlay.RemoveListener(value);
        }

        protected override void Initialize()
        {
            State = IngameFSMSystem.State.Init;
        }

        public override void StartState()
        {
            base.StartState();
            if (PhotonNetwork.IsMasterClient)
            {
                WaitForAllPlayersReadyAsync().Forget();
            }
        }

        public override void EndState()
        {
            base.EndState();
            _onReadyToPlay?.Invoke();
        }

        private async UniTaskVoid WaitForAllPlayersReadyAsync()
        {
            // TODO: 타임아웃 기능 추가
            await UniTask.WaitUntil(
                () => InGamePlayerManager.Instance.GamePlayers.Count == PhotonNetwork.CurrentRoom.PlayerCount);

            await UniTask.Delay(500);
            FsmSystem.ChangeState(IngameFSMSystem.State.IntroCut);
        }
    }
}

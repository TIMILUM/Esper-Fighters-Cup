using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace EsperFightersCup
{
    public class IngameResultState : InGameFSMStateBase
    {
        [SerializeField] private UnityEvent _onGameEnd;

        private int _count;

        public event UnityAction OnGameEnd
        {
            add => _onGameEnd.AddListener(value);
            remove => _onGameEnd.RemoveListener(value);
        }

        protected override void Initialize()
        {
            State = IngameFSMSystem.State.Result;
        }

        public override void StartState()
        {
            base.StartState();

            var myWinPoint = (int)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyKeys.PlayerWinPoint];
            if (myWinPoint == 3)
            {
                PhotonNetwork.CurrentRoom.SetCustomProperty(CustomPropertyKeys.GameWinner, PhotonNetwork.LocalPlayer.NickName);
            }
            else
            {
                PhotonNetwork.CurrentRoom.SetCustomProperty(CustomPropertyKeys.GameLooser, PhotonNetwork.LocalPlayer.NickName);
            }

            ResultEndAsync().Forget();
            _onGameEnd?.Invoke();
        }

        private async UniTask ResultEndAsync()
        {
            await UniTask.Delay(1000);
            FsmSystem.photonView.RPC(nameof(ResultEndRPC), RpcTarget.MasterClient);
        }

        [PunRPC]
        private void ResultEndRPC()
        {
            _count++;
            if (_count == FsmSystem.RoomPlayers.Count)
            {
                PhotonNetwork.LoadLevel("ResultScene");
            }
        }
    }
}

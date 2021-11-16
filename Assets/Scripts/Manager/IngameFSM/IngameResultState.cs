using Cysharp.Threading.Tasks;
using Photon.Pun;

namespace EsperFightersCup
{
    public class IngameResultState : InGameFSMStateBase
    {
        private int _count;

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
        }

        private async UniTask ResultEndAsync()
        {
            PhotonNetwork.Destroy(InGamePlayerManager.Instance.LocalPlayer.gameObject);

            await UniTask.Delay(1000);
            FsmSystem.photonView.RPC(nameof(ResultEndRPC), RpcTarget.MasterClient);
        }

        [PunRPC]
        private void ResultEndRPC()
        {
            _count++;
            if (_count == InGamePlayerManager.Instance.GamePlayers.Count)
            {
                PhotonNetwork.LoadLevel("ResultScene");
            }
        }
    }
}

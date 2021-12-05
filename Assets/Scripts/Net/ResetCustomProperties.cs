using Photon.Pun;
using UnityEngine;

namespace EsperFightersCup
{
    public class ResetCustomProperties : MonoBehaviour
    {
        private void Start()
        {
            if (!PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
            {
                return;
            }

            PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonOptions.DefaultCustomPlayerProperties);
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonOptions.DefaultCustomRoomProperties);
            }
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }
}

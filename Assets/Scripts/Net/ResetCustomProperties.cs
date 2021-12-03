using Photon.Pun;
using UnityEngine;

namespace EsperFightersCup
{
    public class ResetCustomProperties : MonoBehaviour
    {
        private void Start()
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonOptions.DefaultCustomPlayerProperties);
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonOptions.DefaultCustomRoomProperties);
            }
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }
}

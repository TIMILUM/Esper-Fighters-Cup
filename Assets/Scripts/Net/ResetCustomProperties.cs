using Photon.Pun;
using UnityEngine;

namespace EsperFightersCup
{
    public class ResetCustomProperties : MonoBehaviour
    {
        private void Start()
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(GameRoomOptions.DefaultPlayerCustomProperties);
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(GameRoomOptions.DefaultRoomCustomProperties);
            }
            PhotonNetwork.SendAllOutgoingCommands();
        }
    }
}

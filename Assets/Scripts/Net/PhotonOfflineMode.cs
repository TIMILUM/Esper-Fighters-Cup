using Photon.Pun;
using UnityEngine;

namespace EsperFightersCup
{
    public class PhotonOfflineMode : MonoBehaviour
    {
        private void Awake()
        {
            // 연결되지 않고 인게임 화면이 나온다면 오프라인 모드를 통한 디버깅을 허용
            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogWarning("Enable Offline Mode!");
                PhotonNetwork.OfflineMode = true;
                PhotonNetwork.NickName = "OfflinePlayer";
                PhotonNetwork.CreateRoom("OfflineRoom");
            }
        }
    }
}

using Photon.Pun;
using UnityEngine;

public class PhotonStartup : MonoBehaviourPunCallbacks
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitPhoton()
    {
        // 포톤의 게임버전은 프로젝트의 버전을 따라갑니다.
        PhotonNetwork.GameVersion = Application.version;
        Debug.Log($"Game Version: {PhotonNetwork.GameVersion}");

        // 방에 입장한 플레이어와 씬 동기화
        PhotonNetwork.AutomaticallySyncScene = true;
    }
}

using Photon.Pun;
using UnityEngine;

#if UNITY_EDITOR
using Photon.Pun.UtilityScripts;
#endif

public class PhotonStartup
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitPhoton()
    {
        // 포톤의 게임버전은 프로젝트의 버전을 따라갑니다.
        PhotonNetwork.GameVersion = Application.version;
        Debug.Log($"Game Version: {PhotonNetwork.GameVersion}");

        // 방에 입장한 플레이어와 씬 동기화
        PhotonNetwork.AutomaticallySyncScene = true;

#if UNITY_EDITOR
        // 포톤 연결 상태 확인용 테스트 UI
        var statusUi = new GameObject { name = "Photon Status" };
        statusUi.AddComponent<PhotonStatsGui>();
        Object.DontDestroyOnLoad(statusUi);
#endif
    }
}

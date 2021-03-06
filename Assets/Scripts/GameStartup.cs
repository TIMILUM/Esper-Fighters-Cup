using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class GameStartup : MonoBehaviour
{
    // private const string PhotonAppIdFilePath = "photon-cloud";

    /// <summary>
    /// 최초로 게임이 실행될 때 씬의 오브젝트들이 초기화되기 전 호출됩니다.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitGame()
    {
        InitGameVersion();
        // InitAppId();
        PhotonNetworkSettings();
        CreateStartupObject();
        CreatePhotonStatus();
    }

    private static void InitGameVersion()
    {
        // 포톤의 게임버전은 프로젝트의 버전을 따라갑니다.
        // 같은 게임버전인 플레이어끼리만 매칭이 가능합니다.
        // PhotonNetwork.GameVersion = Application.version;
        // Debug.Log($"Game Version: {PhotonNetwork.GameVersion}");
    }

    /*
    private static void InitAppId()
    {
        var appIdFile = Resources.Load<TextAsset>(PhotonAppIdFilePath);
        if (appIdFile is null)
        {
            Debug.LogError($"Resources/{PhotonAppIdFilePath}.txt 파일이 존재하지 않습니다.");
            return;
        }

        var appId = appIdFile.text.Replace(" ", string.Empty);

        // 빈 파일 여부 체크
        if (string.IsNullOrEmpty(appId))
        {
            Debug.LogError($"Resources /{ PhotonAppIdFilePath}.txt 파일이 비어 있습니다.");
            return;
        }

        // Id 패턴 정규식 체크
        if (!Regex.IsMatch(appId, @"^[A-Za-z0-9]{8}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{12}$"))
        {
            Debug.LogError($"Photon Cloud App Id가 올바르지 않습니다.");
            return;
        }

        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = appId;
        Debug.Log($"Photon Cloud App Id를 성공적으로 로드했습니다.");
        // Debug.Log($"Photon Cloud App Id: {appId}");
    }
    */

    private static void PhotonNetworkSettings()
    {
        // 방에 입장한 플레이어와 씬 동기화
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NetworkingClient.LoadBalancingPeer.ReuseEventInstance = true;
    }

    private static void CreateStartupObject()
    {
        var instance = new GameObject("Game Startup");
        instance.AddComponent<GameStartup>();
    }

    private static void CreatePhotonStatus()
    {
        // Development Build에서만 생성
        if (!Debug.isDebugBuild)
        {
            return;
        }

        // 포톤 연결 상태 확인용 테스트 UI
        var statusUi = new GameObject { name = "Photon Status" };
        statusUi.AddComponent<PhotonStatsGui>();
        DontDestroyOnLoad(statusUi);
    }

    private void Awake()
    {
        var objects = Resources.LoadAll<GameObject>("Prefab/InitializeOnStart");

        foreach (var obj in objects)
        {
            var newInstace = Instantiate(obj);
            newInstace.name = obj.name;
        }
        Destroy(gameObject);
    }
}

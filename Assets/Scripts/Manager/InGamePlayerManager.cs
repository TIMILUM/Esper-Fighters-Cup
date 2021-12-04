using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EsperFightersCup;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class InGamePlayerManager : PunEventSingleton<InGamePlayerManager>
{
    private const string CharacterPrefabLocation = "Prefab/Character/{0}";

    [Header("[Player Generate]")]
    [SerializeField] private List<ACharacter> _characterPrefabs;
    [SerializeField] private ACharacter.Type _defaultCharacterType = ACharacter.Type.Telekinesis;

    [SerializeField] private List<Transform> _startLocations;
    [SerializeField] private IngameFSMSystem _ingameFsmSystem;

    [Header("[Player's Camera]")]
    [SerializeField] private CameraMovement _cameraMovement;

    /// <summary>
    /// 현재 씬의 로컬 플레이어 인스턴스를 가져옵니다.
    /// </summary>
    public APlayer LocalPlayer { get; private set; }

    /// <summary>
    /// 키가 ActorNumber, 값이 플레이어 인스턴스인 딕셔너리를 제공합니다.
    /// </summary>
    public Dictionary<int, APlayer> GamePlayers { get; } = new Dictionary<int, APlayer>();

    /// <summary>
    /// 인게임의 플레이어 시작 위치를 담고 있습니다.
    /// </summary>
    public List<Transform> StartLocations => _startLocations;

    /// <summary>
    /// 액터 번호로 정렬된 플레이어 목록 중 해당 플레이어가 몇 번째에 존재하는지 검색합니다.
    /// </summary>
    /// <param name="searchPlayer"></param>
    /// <returns></returns>
    public static int FindPlayerIndex(Player searchPlayer)
    {
        return Array.FindIndex(PhotonNetwork.PlayerList, p => p.ActorNumber == searchPlayer.ActorNumber);
    }

    private void Start()
    {
        // TODO: 플레이어를 MasterClient에서 생성하고 소유권을 각자에게 나눠주도록 구현해보기
        LocalPlayer = SpawnLocalPlayer();
        var pvID = LocalPlayer.photonView.ViewID;

        Debug.Log($"New local player instance = {pvID}-{LocalPlayer}");
        // Debug.Log($"GamePlayers count: {GamePlayers.Count}");

        PhotonNetwork.LocalPlayer.SetCustomProperty(CustomPropertyKeys.PlayerPhotonView, pvID);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!changedProps.TryGetValue(CustomPropertyKeys.PlayerPhotonView, out var value))
        {
            return;
        }

        var targetPV = (int)value;
        var playerInstance = PhotonNetwork.GetPhotonView(targetPV).gameObject.GetComponent<APlayer>();
        GamePlayers[targetPlayer.ActorNumber] = playerInstance;
        Debug.Log($"New player instance: [{targetPlayer.ActorNumber}] = {targetPV}-{playerInstance}");
        // Debug.Log($"GamePlayers count: {GamePlayers.Count}");

        if (PhotonNetwork.IsMasterClient && GamePlayers.Count == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            NextStateAsync().Forget();
        }
    }

    private APlayer SpawnLocalPlayer()
    {
        ACharacter.Type characterType;
        var props = PhotonNetwork.LocalPlayer.CustomProperties;

        if (props.TryGetValue(CustomPropertyKeys.PlayerCharacterType, out var characterTypeRaw))
        {
            characterType = (ACharacter.Type)(int)characterTypeRaw;
        }
        else
        {
            Debug.LogWarning($"Can not found local player's character type.");
            characterType = _defaultCharacterType;
        }

        var prefab = _characterPrefabs.Find(x => x.CharacterType == characterType);
        if (prefab == null)
        {
            throw new Exception("생성할 캐릭터의 타입을 찾을 수 없습니다.");
        }

        var player = PhotonNetwork.Instantiate(string.Format(CharacterPrefabLocation, prefab.name),
            transform.position + (Vector3.up * 5f), Quaternion.identity);

        var localplayer = player.GetComponent<APlayer>();
        localplayer.ResetPositionAndRotation();

        Camera.main.GetComponent<FMODUnity.StudioListener>().attenuationObject = gameObject;
        return localplayer;
    }

    private async UniTask NextStateAsync()
    {
        await UniTask.Delay(1000);
        IngameFSMSystem.Instance.ChangeState(IngameFSMSystem.State.IntroCut);
    }
}

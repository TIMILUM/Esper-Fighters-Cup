using System;
using System.Collections.Generic;
using EsperFightersCup;
using EsperFightersCup.Util;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class InGamePlayerManager : PunEventSingleton<InGamePlayerManager>
{
    private const string CharacterPrefabLocation = "Prefab/Character/{0}";

    [Header("[Player Generate]")]
    [SerializeField] private List<ACharacter> _characterPrefabs;

    [SerializeField] private Transform _spawnTransform;
    [SerializeField] private IngameFSMSystem _ingameFsmSystem;

    [Header("[Player's Camera]")]
    [SerializeField] private CameraMovement _cameraMovement;

    /// <summary>
    /// 현재 씬의 로컬 플레이어 인스턴스를 가져옵니다.
    /// </summary>
    public APlayer LocalPlayer { get; private set; }

    /// <summary>
    /// 키가 ActorNumber, 값이 플레이어 인스턴스인 딕셔너리를 제공합니다ㅏ.
    /// </summary>
    public Dictionary<int, APlayer> GamePlayers { get; } = new Dictionary<int, APlayer>();

    private void Start()
    {
        LocalPlayer = SpawnLocalPlayer();
        var pvID = LocalPlayer.photonView.ViewID;

        // 로컬 플레이어는 여기서 바로 저장
        GamePlayers[PhotonNetwork.LocalPlayer.ActorNumber] = LocalPlayer;
        PhotonNetwork.SetPlayerCustomProperties(new Hashtable { [CustomPropertyKeys.PlayerPhotonView] = pvID });

        Debug.Log($"New local player instance = {pvID}-{LocalPlayer}");
        Debug.Log($"GamePlayers count: {GamePlayers.Count}");
        if (GamePlayers.Count == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            IngameFSMSystem.Instance.ChangeState(IngameFSMSystem.State.IntroCut);
            Debug.Log($"Change state to round intro");
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // 로컬 플레이어가 아닐 때만 여기 통해서 저장
        if (targetPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }

        if (!changedProps.TryGetValue(CustomPropertyKeys.PlayerPhotonView, out var value))
        {
            return;
        }

        var targetPV = (int)value;
        var playerInstance = PhotonNetwork.GetPhotonView(targetPV).gameObject.GetComponent<APlayer>();
        GamePlayers[targetPlayer.ActorNumber] = playerInstance;
        Debug.Log($"New player instance: [{targetPlayer.ActorNumber}] = {targetPV}-{playerInstance}");
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
            characterType = ACharacter.Type.Telekinesis;
        }

        var prefab = _characterPrefabs.Find(x => x.CharacterType == characterType);
        if (prefab == null)
        {
            throw new Exception("생성할 캐릭터의 타입을 찾을 수 없습니다.");
        }

        var player = PhotonNetwork.Instantiate(string.Format(CharacterPrefabLocation, prefab.name),
            _spawnTransform.position + Vector3.up, Quaternion.identity);

        return player.GetComponent<APlayer>();
    }
}

using System;
using System.Collections.Generic;
using EsperFightersCup;
using EsperFightersCup.Util;
using Photon.Pun;
using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class InGamePlayerManager : PunEventSingleton<InGamePlayerManager>
{
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

    private void Start()
    {
#if UNITY_EDITOR
        // 연결되지 않고 인게임 화면이 나온다면 오프라인 모드를 통한 디버깅을 허용
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("Enable Offline Mode!");
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.JoinRandomRoom();
        }
#endif
        SpawnLocalPlayer();
    }

    private void SpawnLocalPlayer()
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

        var player = PhotonNetwork.Instantiate("Prefabs/Characters/" + prefab.name,
            _spawnTransform.position + Vector3.up, Quaternion.identity);

        LocalPlayer = player.GetComponent<APlayer>();

        var pvID = LocalPlayer.photonView.ViewID;
        PhotonNetwork.SetPlayerCustomProperties(new Hashtable { [CustomPropertyKeys.PlayerPhotonView] = pvID });
    }
}

using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private List<ACharacter> _characterPrefabs;

    // TODO: 나중에 캐릭터 선택 구현 시 임시로 작성한 PlayerSpawnManager 해당 코드의 수정이 필요함
    [SerializeField]
    private ACharacter.Type _currentCharacterType = ACharacter.Type.Telekinesis;

    [Header("etc")]
    [SerializeField]
    private Transform _spawnTransform;

    [SerializeField]
    private CameraMovement _cameraMovement;

    // Start is called before the first frame update
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
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        var prefab = _characterPrefabs.Find(x => x.CharacterType == _currentCharacterType);
        if (prefab == null)
        {
            throw new Exception("생성할 캐릭터의 타입을 찾을 수 없습니다.");
        }

        var player = PhotonNetwork.Instantiate("Prefabs/Characters/" + prefab.name,
            _spawnTransform.position + Vector3.up, Quaternion.identity);
        // player.transform.SetParent(_spawnTransform);
        _cameraMovement.AddTarget(player.transform);
    }
}

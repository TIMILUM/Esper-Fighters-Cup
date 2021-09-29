using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class InGamePlayerManager : MonoBehaviourPunCallbacks
{
    [Header("Player Generate")]
    
    [SerializeField]
    private List<ACharacter> _characterPrefabs;

    // TODO: 나중에 캐릭터 선택 구현 시 임시로 작성한 PlayerSpawnManager 해당 코드의 수정이 필요함
    [SerializeField]
    private ACharacter.Type _currentCharacterType = ACharacter.Type.Telekinesis;
    [SerializeField]
    private Transform _spawnTransform;

    
    private static ACharacter s_myCharacter = null;
    private static ACharacter s_enemyCharacter = null;

    [Header("Player's Camera")]
    
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
    }

    public static void SetMyPlayer(ACharacter character)
    {
        s_myCharacter = character;
    }
    
    public static void SetEnemyPlayer(ACharacter character)
    {
        s_enemyCharacter = character;
    }
}

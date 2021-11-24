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

    [SerializeField] private List<Transform> _startLocations;
    [SerializeField] private IngameFSMSystem _ingameFsmSystem;

    [Header("[Player's Camera]")]
    [SerializeField] private CameraMovement _cameraMovement;

    /// <summary>
    /// ?꾩옱 ?ъ쓽 濡쒖뺄 ?뚮젅?댁뼱 ?몄뒪?댁뒪瑜?媛?몄샃?덈떎.
    /// </summary>
    public APlayer LocalPlayer { get; private set; }

    /// <summary>
    /// ?ㅺ? ActorNumber, 媛믪씠 ?뚮젅?댁뼱 ?몄뒪?댁뒪???뺤뀛?덈━瑜??쒓났?⑸땲?ㅳ뀖.
    /// </summary>
    public Dictionary<int, APlayer> GamePlayers { get; } = new Dictionary<int, APlayer>();

    /// <summary>
    /// ?멸쾶?꾩쓽 ?뚮젅?댁뼱 ?쒖옉 ?꾩튂瑜??닿퀬 ?덉뒿?덈떎.
    /// </summary>
    public List<Transform> StartLocations => _startLocations;

    private void Start()
    {
        LocalPlayer = SpawnLocalPlayer();
        var pvID = LocalPlayer.photonView.ViewID;
        PhotonNetwork.LocalPlayer.SetCustomProperty(CustomPropertyKeys.PlayerPhotonView, pvID);

        Debug.Log($"New local player instance = {pvID}-{LocalPlayer}");
        Debug.Log($"GamePlayers count: {GamePlayers.Count}");
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
        Debug.Log($"GamePlayers count: {GamePlayers.Count}");

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
            characterType = ACharacter.Type.Telekinesis;
        }

        var prefab = _characterPrefabs.Find(x => x.CharacterType == characterType);
        if (prefab == null)
        {
            throw new Exception("?앹꽦??罹먮┃?곗쓽 ??낆쓣 李얠쓣 ???놁뒿?덈떎.");
        }

        var player = PhotonNetwork.Instantiate(string.Format(CharacterPrefabLocation, prefab.name),
            transform.position + (Vector3.up * 5f), Quaternion.identity);

        var localplayer = player.GetComponent<APlayer>();
        localplayer.ResetPositionAndRotation();

        return localplayer;
    }

    private async UniTask NextStateAsync()
    {
        Debug.Log($"Change state to IntroCut");
        await UniTask.Delay(1000);
        IngameFSMSystem.Instance.ChangeState(IngameFSMSystem.State.IntroCut);
    }
}

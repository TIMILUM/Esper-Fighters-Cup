using System.Collections.Generic;
using EsperFightersCup;
using Photon.Pun;
using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;

[RequireComponent(typeof(PhotonView))]
public class IngameFSMSystem : InspectorFSMSystem<IngameFSMSystem.State, InGameFSMStateBase>
{
    public enum State
    {
        Init,
        IntroCut,
        RoundIntro,
        InBattle,
        RoundEnd,
        EndingCut,
        Result,
    }

    [SerializeField] private CurtainAnimation _curtain;
    [SerializeField] private SawBladeSystem _sawBladeSystem;
    // private DateTime _sawUsingStartTime = DateTime.MinValue;

    public static IngameFSMSystem Instance { get; private set; }

    // 게임 시작할 때 각 플레이어의 PhotonViewID를 가져와서 캐싱
    public Dictionary<int, Photon.Realtime.Player> GamePlayers => PhotonNetwork.CurrentRoom.Players;

    /// <summary>
    /// 현재 게임의 톱날 시스템을 가져옵니다.
    /// </summary>
    public SawBladeSystem SawBladeSystem => _sawBladeSystem;
    public CurtainAnimation Curtain => _curtain;

    /// <summary>
    /// 현재 게임의 라운드 수를 가져오거나 설정합니다.<para/>
    /// 가져오는데 실패했을 경우 0을 반환합니다. 라운드 설정은 MasterClient만 가능합니다.
    /// </summary>
    public int RoundCount
    {
        get
        {
            var room = PhotonNetwork.CurrentRoom;
            if (room is null)
            {
                return 0;
            }
            return (int)(room.CustomProperties[CustomPropertyKeys.GameRound] ?? 0);
        }
        set
        {
            var room = PhotonNetwork.CurrentRoom;
            if (!PhotonNetwork.IsMasterClient || room is null)
            {
                return;
            }

            int round = value < 1 ? 1 : value;
            room.SetCustomProperties(new Hashtable { [CustomPropertyKeys.GameRound] = round });
        }
    }

    /* SawbladeSystem으로 이동 필요
    private void Update()
    {
        foreach (var player in PlayerList)
        {
            if (player.Hp < 30)
            {
                var currentTime = (DateTime.Now - _sawUsingStartTime).TotalSeconds;
                if (currentTime > 5)
                {
                    _sawUsingStartTime = DateTime.Now;
                    _sawBladeSystem.GenerateSawBlade();
                }
            }
        }
    }
    */

    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
        }

#if UNITY_EDITOR
        // 연결되지 않고 인게임 화면이 나온다면 오프라인 모드를 통한 디버깅을 허용
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogWarning("Enable Offline Mode!");
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.JoinRandomRoom();
        }
#endif
    }

    public override void ChangeState(State state)
    {
        if (state == CurrentState)
        {
            return;
        }

        var customProperties = PhotonNetwork.CurrentRoom.CustomProperties;
        var changeProp = new Hashtable
        {
            [CustomPropertyKeys.GameState] = (int)state
        };

        // Check And Swap
        Hashtable oldProp = null;
        if (customProperties.TryGetValue(CustomPropertyKeys.GameState, out var value))
        {
            oldProp = new Hashtable
            {
                [CustomPropertyKeys.GameState] = value
            };
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(changeProp, oldProp);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (!propertiesThatChanged.TryGetValue(CustomPropertyKeys.GameState, out var value))
        {
            return;
        }

        var nextState = (int)value;
        base.ChangeState((State)nextState);
    }
}

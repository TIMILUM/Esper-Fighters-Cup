using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

    public CurtainAnimation Curtain => _curtain;

    /// <summary>
    /// 현재 게임의 톱날 시스템을 가져옵니다.
    /// </summary>
    public SawBladeSystem SawBladeSystem => _sawBladeSystem;

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
            if (room is null)
            {
                return;
            }

            int round = Mathf.Max(value, 0);
            Debug.Log($"Set RoundCount = {round}");
            room.SetCustomProperties(CustomPropertyKeys.GameRound, round);
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

        PhotonNetwork.CurrentRoom.SetCustomPropertiesBySafe(CustomPropertyKeys.GameState, (int)state);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (!propertiesThatChanged.TryGetValue(CustomPropertyKeys.GameState, out var value))
        {
            return;
        }

        ChangeStateAsync((State)(int)value).Forget();
    }

    private async UniTask ChangeStateAsync(State state)
    {
        // Enumerator가 중간에 제거되는 불상사를 막기 위해
        // OnRoomPropertiesUpdate가 모두 끝날 때까지 기다림
        await UniTask.Yield();

        Debug.Log($"Next GameState: {state}");
        base.ChangeState(state);
    }
}

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EsperFightersCup;
using EsperFightersCup.UI;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[RequireComponent(typeof(PhotonView))]
public class IngameFSMSystem : InspectorFSMSystem<IngameFSMSystem.State, InGameFSMStateBase>
{
    public enum State
    {
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

    /// <summary>
    /// 게임 라운드
    /// </summary>
    public int Round => (int)(PhotonNetwork.CurrentRoom.CustomProperties[CustomPropertyKeys.GameRound] ?? 0);

    public IReadOnlyDictionary<int, Photon.Realtime.Player> RoomPlayers => PhotonNetwork.CurrentRoom.Players;

    public CurtainAnimation Curtain => _curtain;

    /// <summary>
    /// 현재 게임의 톱날 시스템을 가져옵니다.
    /// </summary>
    public SawBladeSystem SawBladeSystem => _sawBladeSystem;

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
            PhotonNetwork.NickName = "OfflinePlayer";
            PhotonNetwork.CreateRoom("OfflineRoom");
        }
#endif
    }

    /// <summary>
    /// 해당 FSMSystem의 State를 변경합니다. CustomRoomProperty를 이용하여 모든 클라이언트에 동기화됩니다.
    /// </summary>
    /// <param name="state"></param>
    public override void ChangeState(State state)
    {
        if (state == CurrentState)
        {
            return;
        }
        PhotonNetwork.CurrentRoom.SetCustomPropertyBySafe(CustomPropertyKeys.GameState, (int)state);
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
        await UniTask.NextFrame();
        base.ChangeState(state);
    }
}

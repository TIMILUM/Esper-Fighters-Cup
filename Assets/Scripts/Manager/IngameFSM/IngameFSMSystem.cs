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

        PhotonNetwork.CurrentRoom.SetCustomPropertyBySafe(CustomPropertyKeys.GameState, (int)state);
        PhotonNetwork.SendAllOutgoingCommands();
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

using System;
using System.Collections.Generic;
using EsperFightersCup;
using Photon.Pun;
using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;

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

    [SerializeField] private SawBladeSystem _sawBladeSystem;
    [SerializeField] private IngameTopUI _ingameTopUI;

    private DateTime _sawUsingStartTime = DateTime.MinValue;

    public IngameTopUI IngameTopUIObject => _ingameTopUI;

    /// <summary>
    /// 키가 Actor Number, 값이 PhotonView인 플레이어 목록입니다.
    /// </summary>
    public Dictionary<int, PhotonView> GamePlayers { get; } = new Dictionary<int, PhotonView>();

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

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    private void Update()
    {
        foreach (var player in GamePlayers)
        {

        }

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

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.TryGetValue(CustomPropertyKeys.PlayerPhotonView, out var value) && value is int viewID)
        {
            // 이미 존재하는 경우 덮어 씀
            GamePlayers[targetPlayer.ActorNumber] = PhotonNetwork.GetPhotonView(viewID);
        }
    }

    private void SetPlayerFunction(APlayer player)
    {
        PlayerList.Add(player);
        _ingameTopUI.SetPlayer(player);
    }

    public (short, short) GetWinPoint()
    {
        return (_winPoint[0], _winPoint[1]);
    }

    public short AddWinPoint(int index)
    {
        return ++_winPoint[index];
    }

    public override void ChangeState(State state)
    {
        base.ChangeState(state);
        CurrentState = state;
    }
}

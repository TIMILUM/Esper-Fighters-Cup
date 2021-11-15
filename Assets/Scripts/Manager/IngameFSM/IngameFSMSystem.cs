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

    public static State CurrentState;

    [SerializeField] private SawBladeSystem _sawBladeSystem;
    // private DateTime _sawUsingStartTime = DateTime.MinValue;

    // 게임 시작할 때 각 플레이어의 PhotonViewID를 가져와서 캐싱
    public Dictionary<int, Photon.Realtime.Player> GamePlayers => PhotonNetwork.CurrentRoom.Players;

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

    public override void ChangeState(State state)
    {
        base.ChangeState(state);
        CurrentState = state;
    }
}

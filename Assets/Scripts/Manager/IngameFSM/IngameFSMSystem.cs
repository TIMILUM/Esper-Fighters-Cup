using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    private static UnityAction<APlayer> s_setPlayer = null;

    [SerializeField]
    private short[] _winPoint = new short[2];

    [SerializeField]
    private IngameTopUI _ingameTopUI;

    public IngameTopUI IngameTopUIObject => _ingameTopUI;

    public List<APlayer> PlayerList { get; } = new List<APlayer>();

    [SerializeField]
    private SawBladeSystem _sawBladeSystem;

    private DateTime _sawUsingStartTime = DateTime.MinValue;

    public int RoundCount { get; set; }

    public static void SetPlayer(APlayer player)
    {
        s_setPlayer?.Invoke(player);
    }

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

    protected override void Awake()
    {
        base.Awake();
        s_setPlayer += SetPlayerFunction;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        s_setPlayer = null;
    }
}

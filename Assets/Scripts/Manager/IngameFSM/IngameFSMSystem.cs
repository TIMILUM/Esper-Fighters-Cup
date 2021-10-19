using System;
using System.Collections;
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
    private static UnityAction<APlayer> _setPlayer = null;

    [SerializeField]
    private short[] _winPoint = new short[2];

    [SerializeField]
    private IngameTopUI _ingameTopUI;

    public IngameTopUI IngameTopUIObject => _ingameTopUI;
    
    private List<APlayer> _playerList = new List<APlayer>();
    public List<APlayer> PlayerList => _playerList;

    public int RoundCount { get; set; }

    public static void SetPlayer(APlayer player)
    {
        _setPlayer?.Invoke(player);
    }
    
    private void SetPlayerFunction(APlayer player)
    {
        _playerList.Add(player);
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
        _setPlayer += SetPlayerFunction;
    }

    private void OnDestroy()
    {
        _setPlayer = null;
    }
}

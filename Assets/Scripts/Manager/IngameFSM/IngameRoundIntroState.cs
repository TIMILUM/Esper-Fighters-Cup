using System.Collections.Generic;
using EsperFightersCup.UI.InGame;
using EsperFightersCup.Util;
using UnityEngine;

public class IngameRoundIntroState : InGameFSMStateBase
{
    [SerializeField]
    private GameStateView _gameStateView;

    [SerializeField]
    private List<Transform> _startPosition;

    protected override void Initialize()
    {
        State = IngameFSMSystem.State.RoundIntro;
    }

    private void Start()
    {

    }

    public override void StartState()
    {
        base.StartState();
        var round = ++FsmSystem.RoundCount;
        FsmSystem.IngameTopUIObject.SetRoundCount(round);

        for (var i = 0; i < FsmSystem.PlayerList.Count; i++)
        {
            var player = FsmSystem.PlayerList[i];
            var startPosition = _startPosition[i];
            player.Hp = 100;
            player.transform.position = startPosition.position;
        }

        // 몇초 뒤에 보이게 해야 잘 보임.
        _gameStateView.Show($"Round {round}", Vector2.left * 20f);
        CoroutineTimer.SetTimerOnce(() =>
        {
            _gameStateView.Show("Fight!", Vector2.left * 20f);
            ChangeState(IngameFSMSystem.State.InBattle);
        }, 2f);
    }

    public override void EndState()
    {
        base.EndState();
    }
}

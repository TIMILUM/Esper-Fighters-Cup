using System.Collections.Generic;
using DG.Tweening;
using EsperFightersCup.UI.InGame;
using Photon.Pun;
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

        Debug.Log($"GamePlayers: {FsmSystem.GamePlayers.Count}");
        var localplayer = InGamePlayerManager.Instance.LocalPlayer;

        localplayer.Hp = 100f;
        localplayer.transform.position = _startPosition[PhotonNetwork.IsMasterClient ? 0 : 1].position;

        // 몇초 뒤에 보이게 해야 잘 보임.
        DOTween.Sequence()
            .AppendInterval(3.0f)
            .AppendCallback(() => _gameStateView.Show($"Round {round}", Vector2.left * 20f))
            .AppendInterval(1.5f)
            .AppendCallback(() =>
            {
                _gameStateView.Show("Fight!", Vector2.left * 20f);
                ChangeState(IngameFSMSystem.State.InBattle);
            });
    }

    public override void EndState()
    {
        base.EndState();
    }
}

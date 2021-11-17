using Cysharp.Threading.Tasks;
using EsperFightersCup;
using EsperFightersCup.UI.InGame;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class IngameRoundIntroState : InGameFSMStateBase
{
    [SerializeField] private GameStateView _gameStateView;

    private int _count;
    private int _currentRound;

    protected override void Initialize()
    {
        State = IngameFSMSystem.State.RoundIntro;
    }

    public override void StartState()
    {
        base.StartState();
        _count = 0;

        if (PhotonNetwork.IsMasterClient)
        {
            var roundValue = PhotonNetwork.CurrentRoom.CustomProperties[CustomPropertyKeys.GameRound] ?? 0;
            var round = (int)roundValue;
            _currentRound = ++round;

            var props = new Hashtable
            {
                [CustomPropertyKeys.GameRound] = round,
                [CustomPropertyKeys.GameRoundWinner] = 0
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

        // 로컬플레이어 설정
        var localplayer = InGamePlayerManager.Instance.LocalPlayer;
        localplayer.ResetPositionAndRotation();
        localplayer.HP = 100;

        // 설정 완료 후 MasterClient에게 신호
        FsmSystem.photonView.RPC(nameof(RoundSetCompleteRPC), RpcTarget.MasterClient);
    }

    [PunRPC]
    private void RoundSetCompleteRPC()
    {
        _count++;
        if (_count == InGamePlayerManager.Instance.GamePlayers.Count)
        {
            FsmSystem.photonView.RPC(nameof(RoundIntroRPC), RpcTarget.All, _currentRound);
        }
    }

    [PunRPC]
    private void RoundIntroRPC(int round)
    {
        _count = 0;
        RoundIntroAsync(round).Forget();
    }

    private async UniTask RoundIntroAsync(int round)
    {
        // var round = PhotonNetwork.CurrentRoom.CustomProperties[CustomPropertyKeys.GameRound];
        await FsmSystem.Curtain.FadeOutAsync();

        await UniTask.Delay(2000);
        _gameStateView.Show($"Round {round}", Vector2.left * 20f);

        await UniTask.Delay(1500);
        _gameStateView.Show("Fight!", Vector2.left * 20f);

        FsmSystem.photonView.RPC(nameof(RoundIntroEndRPC), RpcTarget.MasterClient);
    }

    [PunRPC]
    private void RoundIntroEndRPC()
    {
        _count++;
        if (_count == InGamePlayerManager.Instance.GamePlayers.Count)
        {
            ChangeState(IngameFSMSystem.State.InBattle);
        }
    }
}

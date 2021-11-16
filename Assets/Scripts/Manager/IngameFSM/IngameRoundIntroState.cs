using Cysharp.Threading.Tasks;
using EsperFightersCup;
using EsperFightersCup.UI.InGame;
using Photon.Pun;
using UnityEngine;

public class IngameRoundIntroState : InGameFSMStateBase
{
    [SerializeField] private GameStateView _gameStateView;

    private int _count;

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
            var round = (int)PhotonNetwork.CurrentRoom.CustomProperties[CustomPropertyKeys.GameRound];
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomPropertyKeys.GameRound, round + 1);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomPropertyKeys.GameRoundWinner, 0);
        }

        // 로컬플레이어 설정
        var localplayer = InGamePlayerManager.Instance.LocalPlayer;
        localplayer.ResetPositionAndRotation();
        localplayer.HP = 100;

        // 설정 완료 후 MasterClient에게 신호
        FsmSystem.photonView.RPC(nameof(RoundSetCompleteRPC), RpcTarget.MasterClient);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    private void RoundSetCompleteRPC()
    {
        _count++;
        if (_count == InGamePlayerManager.Instance.GamePlayers.Count)
        {
            FsmSystem.photonView.RPC(nameof(RoundIntroRPC), RpcTarget.All);
        }
    }

    [PunRPC]
    private void RoundIntroRPC()
    {
        _count = 0;
        RoundIntroAsync().Forget();
    }

    private async UniTask RoundIntroAsync()
    {
        var round = FsmSystem.RoundCount;
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

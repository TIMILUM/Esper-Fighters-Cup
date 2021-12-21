using System;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace EsperFightersCup
{
    public class IngameEndingCutState : InGameFSMStateBase
    {
        [SerializeField] private PaletteSwapItem<PlayableDirector>[] _outroCutScenes;
        [SerializeField] private UnityEvent _onOutroStart;
        [SerializeField] private UnityEvent _onOutroEnd;

        private int _count;

        public event UnityAction OnOutroStart
        {
            add => _onOutroStart.AddListener(value);
            remove => _onOutroStart.RemoveListener(value);
        }

        public event UnityAction OnOutroEnd
        {
            add => _onOutroEnd.AddListener(value);
            remove => _onOutroEnd.RemoveListener(value);
        }

        protected override void Initialize()
        {
            State = IngameFSMSystem.State.EndingCut;
        }

        public override void StartState()
        {
            base.StartState();
            RunOutroCutAsync().Forget();
        }

        private async UniTask RunOutroCutAsync()
        {
            await UniTask.NextFrame();
            var outro = await GetVictoryCutAsync();

            if (outro == null)
            {
                Debug.LogError("우승자 아웃트로를 찾지 못함!");
                return;
            }

            outro.gameObject.SetActive(true);

            var uniTaskCompletion = new UniTaskCompletionSource();
            outro.stopped += (director) => uniTaskCompletion.TrySetResult();
            outro.Play();
            await FsmSystem.Curtain.FadeOutAsync();

            await uniTaskCompletion.Task;

            await FsmSystem.Curtain.FadeInAsync();
            outro.gameObject.SetActive(false);

            _onOutroEnd?.Invoke();
            FsmSystem.photonView.RPC(nameof(OutroEndRPC), RpcTarget.MasterClient);
        }

        private async UniTask<PlayableDirector> GetVictoryCutAsync()
        {
            await UniTask.Delay(2000);

            if (PhotonNetwork.OfflineMode)
            {
                var player = PhotonNetwork.LocalPlayer;
                var localPlayerWinPoint = (int)(PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyKeys.PlayerWinPoint] ?? 0);
                var dummyWinPoint = (int)(PhotonNetwork.LocalPlayer.CustomProperties["dummyWin"] ?? 0);

                if (localPlayerWinPoint > dummyWinPoint)
                {
                    var value = player.CustomProperties[CustomPropertyKeys.PlayerCharacterType];

                    var type = value is null ? InGamePlayerManager.Instance.DefaultCharacter : (ACharacter.Type)(int)value;
                    var paletteIndex = (int)player.CustomProperties[CustomPropertyKeys.PlayerPalette];
                    var characterPalette = Array.Find(_outroCutScenes, x => x.Character == type);
                    return characterPalette.Palettes[paletteIndex];
                }
                else
                {
                    var type = (ACharacter.Type)(int)player.CustomProperties[CustomPropertyKeys.PlayerCharacterType];

                    var paletteIndex = 0;
                    var characterPalette = Array.Find(_outroCutScenes, x => x.Character != type);
                    return characterPalette.Palettes[paletteIndex];
                }
            }

            foreach (var player in PhotonNetwork.PlayerList)
            {
                var winPoint = (int)(player.CustomProperties[CustomPropertyKeys.PlayerWinPoint] ?? 0);
                if (winPoint == 3)
                {
                    var type = (ACharacter.Type)(int)player.CustomProperties[CustomPropertyKeys.PlayerCharacterType];
                    var paletteIndex = (int)player.CustomProperties[CustomPropertyKeys.PlayerPalette];
                    var characterPalette = Array.Find(_outroCutScenes, x => x.Character == type);
                    return characterPalette.Palettes[paletteIndex];
                }
            }
            return null;
        }

        [PunRPC]
        private void OutroEndRPC()
        {
            _count++;
            if (_count == FsmSystem.RoomPlayers.Count)
            {
                ChangeState(IngameFSMSystem.State.Result);
            }
        }
    }
}

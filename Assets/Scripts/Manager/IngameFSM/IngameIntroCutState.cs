using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace EsperFightersCup
{
    public class IngameIntroCutState : InGameFSMStateBase
    {
        [SerializeField] private PaletteSwapItem<PlayableDirector>[] _introCutScenes;
        [SerializeField] private UnityEvent _onIntroStart;
        [SerializeField] private UnityEvent _onIntroEnd;

        private int _count;

        public event UnityAction OnIntroStart
        {
            add => _onIntroStart.AddListener(value);
            remove => _onIntroStart.RemoveListener(value);
        }

        public event UnityAction OnIntroEnd
        {
            add => _onIntroEnd.AddListener(value);
            remove => _onIntroEnd.RemoveListener(value);
        }

        protected override void Initialize()
        {
            State = IngameFSMSystem.State.IntroCut;
        }

        public override void StartState()
        {
            base.StartState();

            RunIntroCutAsync().Forget();
        }

        private async UniTask RunIntroCutAsync()
        {
            await UniTask.NextFrame();

            var introQueue = new Queue<PlayableDirector>();
            await SetupIntroCutAsync(introQueue);
            _onIntroStart?.Invoke();

            // BUG: 컷씬 진행 중에 플레이어 나가면 로비로 이동하면서 삭제된 오브젝트라는 에러가 뜸
            while (introQueue.Count > 0)
            {
                var intro = introQueue.Dequeue();
                intro.gameObject.SetActive(true);

                var uniTaskCompletion = new UniTaskCompletionSource();
                intro.stopped += (director) => uniTaskCompletion.TrySetResult();
                intro.Play();
                await FsmSystem.Curtain.FadeOutAsync();

                await uniTaskCompletion.Task;

                await FsmSystem.Curtain.FadeInAsync();
                intro.gameObject.SetActive(false);
            }

            _onIntroEnd?.Invoke();
            // 마스터클라이언트로 RPC보내서 컷씬 완료 신호
            FsmSystem.photonView.RPC(nameof(IntroEndRPC), RpcTarget.MasterClient);
        }

        private async UniTask SetupIntroCutAsync(Queue<PlayableDirector> queue)
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                var properties = player.CustomProperties;
                var characterType = ACharacter.Type.None;
                var paletteIndex = 0;

                await UniTask.WaitUntil(() =>
                {
                    if (PhotonNetwork.OfflineMode)
                    {
                        var typeRaw = properties[CustomPropertyKeys.PlayerCharacterType];
                        characterType = typeRaw is null ? ACharacter.Type.Telekinesis : (ACharacter.Type)typeRaw;
                        return true;
                    }

                    if (properties.TryGetValue(CustomPropertyKeys.PlayerCharacterType, out var value) && value is int type)
                    {
                        characterType = (ACharacter.Type)type;
                        return true;
                    }
                    return false;
                });

                await UniTask.WaitUntil(() =>
                {
                    if (PhotonNetwork.OfflineMode)
                    {
                        paletteIndex = 0;
                        return true;
                    }

                    if (properties.TryGetValue(CustomPropertyKeys.PlayerPalette, out var value) && value is int index)
                    {
                        paletteIndex = index;
                        return true;
                    }
                    return false;
                });

                var characterPalette = Array.Find(_introCutScenes, x => x.Character == characterType);
                if (characterPalette == null || paletteIndex >= characterPalette.Palettes.Length)
                {
                    continue;
                }

                queue.Enqueue(characterPalette.Palettes[paletteIndex]);

                if (PhotonNetwork.OfflineMode)
                {
                    var dummyPalette = Array.Find(_introCutScenes, x => x.Character != characterType);
                    queue.Enqueue(dummyPalette.Palettes[0]);
                }
            }
        }

        [PunRPC]
        private void IntroEndRPC()
        {
            // 이 RPC는 MasterClient만 받기 때문에 MasterClient가 체크 후 GameState 변경
            _count++;
            if (_count == FsmSystem.RoomPlayers.Count)
            {
                ChangeState(IngameFSMSystem.State.RoundIntro);
            }
        }
    }
}

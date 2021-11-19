using DG.Tweening;
using EsperFightersCup.Manager;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace EsperFightersCup.UI.CharacterChoice
{
    public class CharacterChoiceView : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Button _localPlayerSubmitButton;
        [SerializeField] private Text _otherPlayerStateText;

        [SerializeField] private Text _localPlayerCharacterDummyText;
        [SerializeField] private Text _otherPlayerCharacterDummyText;

        private Text _localPlayerStateText;
        private Sequence _localPlayerSubmitTimeout;

        private void Awake()
        {
            if (_localPlayerSubmitButton)
            {
                _localPlayerStateText = _localPlayerSubmitButton.GetComponentInChildren<Text>();
                _localPlayerStateText.text = "준비";

                _localPlayerSubmitButton.onClick.AddListener(Submit);
            }

            if (_otherPlayerStateText)
            {
                _otherPlayerStateText.text = "대기 중";
            }
        }

        private void Submit()
        {
            _localPlayerSubmitButton.interactable = false;
            if (CharacterChoiceSystem.Instance.Submit())
            {
                _localPlayerSubmitTimeout = DOTween.Sequence()
                    .SetLink(gameObject)
                    .AppendInterval(3.0f)
                    .AppendCallback(() => _localPlayerSubmitButton.interactable = true);
            }
            else
            {
                DOTween.Sequence()
                    .SetLink(gameObject)
                    .AppendInterval(1.0f)
                    .AppendCallback(() => _localPlayerSubmitButton.interactable = true);
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!changedProps.TryGetValue(CustomPropertyKeys.PlayerCharacterType, out var value) || value is null)
            {
                return;
            }

            var playerChoose = (int)value;

            if (targetPlayer == PhotonNetwork.LocalPlayer)
            {
                _localPlayerSubmitTimeout?.Kill();
                _localPlayerStateText.text = "준비 완료";

                var characterType = (ACharacter.Type)playerChoose;
                _localPlayerCharacterDummyText.text = characterType.ToString();
            }
            else
            {
                if (_otherPlayerStateText)
                {
                    _otherPlayerStateText.text = "준비 완료";
                }

                var characterType = (ACharacter.Type)playerChoose;
                _otherPlayerCharacterDummyText.text = characterType.ToString();
            }
        }
    }
}

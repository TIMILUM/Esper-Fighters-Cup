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

        //상대 플레이어가 준비 완료
        [SerializeField] private GameObject _otherPlayerCharacterImagePanal;

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
                _localPlayerCharacterDummyText.text = "READY"; //캐릭터 타입대신 준비 완료 텍스트가 뜨게 했습니다.
            }
            else
            {
                if (_otherPlayerStateText)
                {
                    _otherPlayerStateText.text = "준비 완료";
                }

                var characterType = (ACharacter.Type)playerChoose;
                _otherPlayerCharacterDummyText.text = "READY";  //캐릭터 타입대신 준비 완료 텍스트가 뜨게 했습니다.

                //상대 플레이어가 준비 완료 시에 랜더 텍스처를 출력합니다.
                if(characterType.ToString()== "Telekinesis")
                {
                    _otherPlayerCharacterImagePanal.GetComponent<Ch_Select_RenderTexcure_Con>().ChangeToElenaImage();   //엘레나의 랜더 텍스처로 변경하는 함수를 호출합니다.
                }
                else if(characterType.ToString() == "Plank")
                {
                    _otherPlayerCharacterImagePanal.GetComponent<Ch_Select_RenderTexcure_Con>().ChangeToPlankImage();   //플랭크의 랜더 텍스처로 변경하는 함수를 호출합니다.
                }
            }
        }
    }
}

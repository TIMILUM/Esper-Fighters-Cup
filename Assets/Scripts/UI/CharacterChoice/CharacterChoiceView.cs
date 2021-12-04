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

        //캐릭터 패널에 raw 이미지를 컨트롤 하기 위해(해당 컴포넌트 접근을 위해) 인스펙터에서 참조했습니다.
        [SerializeField] private GameObject _localPlayerCharacterImagePanal;
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

                //캐릭터 UI 패널에 들어있는 컴포넌트(스크립트)의 함수를 호출해 랜더 텍스처를 수정했습니다.
                if(characterType.ToString()== "Telekinesis")
                {
                    _localPlayerCharacterImagePanal.GetComponent<Ch_Select_RenderTexcure_Con>().ChangeToElenaImage();   //엘레나가 출력되는 렌더 텍스처로 변경해주는 함수입니다.
                }
                else if(characterType.ToString() == "Plank")
                {
                    _localPlayerCharacterImagePanal.GetComponent<Ch_Select_RenderTexcure_Con>().ChangeToPlankImage();   //플랭크가 출력되는 렌더 텍스처로 변경해주는 함수입니다.
                }
            }
            else
            {
                if (_otherPlayerStateText)
                {
                    _otherPlayerStateText.text = "준비 완료";
                }

                var characterType = (ACharacter.Type)playerChoose;
                _otherPlayerCharacterDummyText.text = "READY";  //캐릭터 타입대신 준비 완료 텍스트가 뜨게 했습니다.

                //캐릭터 UI 패널에 들어있는 컴포넌트(스크립트)의 함수를 호출해 랜더 텍스처를 수정했습니다.
                if (characterType.ToString() == "Telekinesis")
                {
                    _otherPlayerCharacterImagePanal.GetComponent<Ch_Select_RenderTexcure_Con>().ChangeToElenaImage();   //엘레나가 출력되는 렌더 텍스처로 변경해주는 함수입니다.
                }
                else if (characterType.ToString() == "Plank")
                {
                    _otherPlayerCharacterImagePanal.GetComponent<Ch_Select_RenderTexcure_Con>().ChangeToPlankImage();   //플랭크가 출력되는 렌더 텍스처로 변경해주는 함수입니다.
                }
            }
        }
    }
}

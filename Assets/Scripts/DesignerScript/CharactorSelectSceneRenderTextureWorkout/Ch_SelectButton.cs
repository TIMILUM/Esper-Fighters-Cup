using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup
{
    public class Ch_SelectButton : MonoBehaviour
    {
        //캐릭터 선택 씬 이미지 패널을 오브젝트로 받음
        [SerializeField] private GameObject _localPlayerCharacterImagePanal;

        public void ElenaImage()
        {
            _localPlayerCharacterImagePanal.GetComponent<Ch_Select_RenderTexcure_Con>().ChangeToElenaImage();   //엘레나가 출력되는 렌더 텍스처로 변경해주는 함수입니다.
        }

        public void PlankImage()
        {
            _localPlayerCharacterImagePanal.GetComponent<Ch_Select_RenderTexcure_Con>().ChangeToPlankImage();   //플랭크가 출력되는 렌더 텍스처로 변경해주는 함수입니다.
        }

        public void ElenaSwapImage()
        {
            _localPlayerCharacterImagePanal.GetComponent<Ch_Select_RenderTexcure_Con>().ChangeToElenaSwapImage();  //엘레나(팔래트스왑)가 출력되는 렌더 텍스처로 변경해주는 함수입니다.
        }

        public void PlankSwapImage()
        {
            _localPlayerCharacterImagePanal.GetComponent<Ch_Select_RenderTexcure_Con>().ChangeToPlankSwapImage();   //플랭크(팔래트스왑)가 출력되는 렌더 텍스처로 변경해주는 함수입니다.
        }

    }
}

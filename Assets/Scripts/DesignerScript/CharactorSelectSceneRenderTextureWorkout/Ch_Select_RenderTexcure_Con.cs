using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup
{
    public class Ch_Select_RenderTexcure_Con : MonoBehaviour
    {

        //4종의 랜더 텍스처를 인스펙터에 저장(캐릭터 및 팔래트 스왑 별로 존재)
        [SerializeField]
        private RenderTexture ElenaRenderTexture;

        [SerializeField]
        private RenderTexture ElenaSwapRenderTexture;

        [SerializeField]
        private RenderTexture PlankRenderTexture;

        [SerializeField]
        private RenderTexture PlankSwapRenderTexture;


        public void ChangeToElenaImage()
        {
            this.gameObject.GetComponent<RawImage>().texture = ElenaRenderTexture;
        }

        public void ChangeToElenaSwapImage()
        {
            this.gameObject.GetComponent<RawImage>().texture = ElenaSwapRenderTexture;
        }

        public void ChangeToPlankImage()
        {
            this.gameObject.GetComponent<RawImage>().texture = PlankRenderTexture;
        }

        public void ChangeToPlankSwapImage()
        {
            this.gameObject.GetComponent<RawImage>().texture = PlankSwapRenderTexture;
        }

    }
}

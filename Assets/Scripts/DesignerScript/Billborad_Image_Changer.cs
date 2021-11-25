using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EsperFightersCup
{
    public class Billborad_Image_Changer : MonoBehaviour
    {

        //유니티 인스펙터에서 이 스크립트에 총 5개의 머테리얼을 받음. (이미지와 카메라 렌더 텍스처 포함)
        [SerializeField]
        private Material matNumber01;

        [SerializeField]
        private Material matNumber02;

        [SerializeField]
        private Material matNumber03;

        [SerializeField]
        private Material matNumber04;

        [SerializeField]
        private Material warningMaterial;   //기믹 등장 시 경고 표시 머테리얼은 여기에


        //기능 처리용 변수들
        private int changeLevel;    //어떤 이미지로 변환시킬지 결정하는 용도
        float OKSignTime;           //이 값(시간) 마다 이미지 바꿈
        float delta;                //시간값 확인하는 용도




        //조건에 따라 이 스크립트를 호출하여 머테리얼 업데이트. change 레벨에 따라 알맞은 머테리얼로 바뀜.
        private void ImageChange(int changeLevel)
        {

            switch (changeLevel)
            {
                case 1:
                    this.gameObject.GetComponent<MeshRenderer>().material = matNumber01;
                    break;

                case 2:
                    this.gameObject.GetComponent<MeshRenderer>().material = matNumber02;
                    break;

                case 3:
                    this.gameObject.GetComponent<MeshRenderer>().material = matNumber03;
                    break;

                case 4:
                    this.gameObject.GetComponent<MeshRenderer>().material = matNumber04;
                    break;

                case 5:
                    this.delta = 0;
                    this.gameObject.GetComponent<MeshRenderer>().material = warningMaterial;
                    break;
            }

        }

        

        void Start()
        {
            //최초 머테리얼 초기값 설정.
            this.gameObject.GetComponent<MeshRenderer>().material = matNumber01;
            //변수값 초기화
            this.changeLevel = 2;
            this.OKSignTime = 3.0f;
            this.delta = 0;

        }


        void Update()
        {
            this.delta += Time.deltaTime;   //델타타임으로 시간 흐름 업데이트
            if(this.delta>this.OKSignTime)  //변경할 시간이 되면
            {
                this.delta = 0;             //시간값 초기화

                this.ImageChange(this.changeLevel); //이미지 변경 함수 호출

                //이미지 체인지 업데이트
                if (this.changeLevel == 4) { this.changeLevel = 1; }
                else { this.changeLevel += 1; }
                    //1~4가 순서대로 출력되도록 함.
            }
        }

        //이 함수를 호출하시면 경고 이미지로 바뀝니다.
        public void BillboradWorning()
        {
            this.ImageChange(5);
        }

    }
}

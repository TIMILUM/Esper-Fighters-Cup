using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EsperFightersCup
{
    public class Billborad_ImageChange_Manager : MonoBehaviour
    {

        
        //인스펙터를 통해서 맵에 배치된 5개의 스크린 오브젝트(소형4개, 대형 1개)에 접근
        [SerializeField]
        private GameObject Screen01;

        [SerializeField]
        private GameObject Screen02;

        [SerializeField]
        private GameObject Screen03;

        [SerializeField]
        private GameObject Screen04;

        [SerializeField]
        private GameObject Screen05;

        //기믹이 등장할 때 아래 함수를 호출해주세요.
        public void Billborad_Gimmick_Alarm()
        {
            this.Screen01.GetComponent<Billborad_Image_Changer>().BillboradWorning();
            this.Screen02.GetComponent<Billborad_Image_Changer>().BillboradWorning();
            this.Screen03.GetComponent<Billborad_Image_Changer>().BillboradWorning();
            this.Screen04.GetComponent<Billborad_Image_Changer>().BillboradWorning();
            this.Screen05.GetComponent<Billborad_Image_Changer>().BillboradWorning();
        }
        //각 오브젝트마다 배치된 "Billborad_Image_Changer" 스크립트 컴포넌트의 퍼블릭 함수 호출
    }
}

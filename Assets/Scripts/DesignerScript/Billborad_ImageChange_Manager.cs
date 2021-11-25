using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EsperFightersCup
{
    public class Billborad_ImageChange_Manager : MonoBehaviour
    {

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

        public void Billborad_Gimmick_Alarm()
        {
            this.Screen01.GetComponent<Billborad_Image_Changer>().BillboradWorning();
            this.Screen02.GetComponent<Billborad_Image_Changer>().BillboradWorning();
            this.Screen03.GetComponent<Billborad_Image_Changer>().BillboradWorning();
            this.Screen04.GetComponent<Billborad_Image_Changer>().BillboradWorning();
            this.Screen05.GetComponent<Billborad_Image_Changer>().BillboradWorning();
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

namespace EsperFightersCup
{
    public class IngameBGMManager : MonoBehaviour
    {

        private FMOD.Studio.EventInstance InGameBGM;    //인게임 배경음악 인스턴스 생성

        void Start()
        {
            InGameBGM = FMODUnity.RuntimeManager.CreateInstance("event:/BGM/InGameBGM");    //인게임 배경음악 인스턴스 초기화
            InGameBGM.start();  //배경음악 이벤트 재생: 캐릭터 연출 씬 음악 출력됨(루프)
        }

        //Round UI 출력 딜레이 전에 이 함수 호출. 함수 인자로 변경될 라운드 값 입력(캐릭터 등장연출 끝나고 1라운드 될 때 1. 1라 끝나고 2라 될 때 2)
        public void IngameBGMUpdate(int round)
        {
            switch(round)
            {
                case 1:
                    InGameBGM.setParameterByName("EntryFinish", 1);
                    break;

                case 2:
                    InGameBGM.setParameterByName("Round2Start", 1);
                    InGameBGM.setParameterByName("EntryFinish", 0);
                    break;

                case 3:
                    InGameBGM.setParameterByName("Round3Start", 1);
                    InGameBGM.setParameterByName("Round2Start", 0);
                    break;

                case 4:
                    InGameBGM.setParameterByName("Round3Finish", 1);
                    InGameBGM.setParameterByName("Round3Start", 0);
                    break;

                case 5:
                    InGameBGM.setParameterByName("FinalRoundStart", 1);
                    InGameBGM.setParameterByName("Round3Finish", 0);
                    break;
            }
        }

        //다른 스크립트에서 배경음악 인스턴스 끌 수 있게 퍼블릭 함수화.ㄴ
        public void InGameBGMOff()
        {
            InGameBGM.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        /*   
        //테스트용 변수
        private int testnumber = 1;
        */

        void Update()
        {
            /*
            //테스트용
            if (Input.GetKeyDown(KeyCode.L))
            {
                IngameBGMUpdate(testnumber);
                testnumber += 1;
            }
            */

        }
    }
}

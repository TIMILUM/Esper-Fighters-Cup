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
            }
        }

        void Update()
        {
        
        }
    }
}

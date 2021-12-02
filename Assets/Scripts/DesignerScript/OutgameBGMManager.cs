using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using UnityEngine.SceneManagement;

namespace EsperFightersCup
{
    public class OutgameBGMManager : MonoBehaviour
    {

        private FMOD.Studio.EventInstance OutGameBGM;   //아웃게임 배경음악 인스턴스 생성

        private void Awake()
        {
            var sameOBJ = FindObjectsOfType<OutgameBGMManager>();
            if(sameOBJ.Length == 1)
            {
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        void Start()
        {
            OutGameBGM = FMODUnity.RuntimeManager.CreateInstance("event:/BGM/OutGameBGM");  //인스턴스 초기화
            OutGameBGM.start();     //배경음악 재생
        }

        // Update is called once per frame
        void Update()
        {
            if(SceneManager.GetActiveScene().name == "GameScene")
            {
                OutGameBGM.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                Destroy(this.gameObject,15);
            }
        }
    }
}

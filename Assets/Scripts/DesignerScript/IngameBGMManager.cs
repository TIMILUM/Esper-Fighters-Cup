using UnityEngine;

namespace EsperFightersCup
{
    [RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
    public class IngameBGMManager : Singleton<IngameBGMManager>
    {
        private FMODUnity.StudioEventEmitter _emitter;

        protected override void Awake()
        {
            base.Awake();
            _emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        }

        private void Start()
        {
            _emitter.Play(); //배경음악 이벤트 재생: 캐릭터 연출 씬 음악 출력됨(루프)
        }

        protected override void OnDestroy()
        {
            _emitter.Stop();
            base.OnDestroy();
        }

        //Round UI 출력 딜레이 전에 이 함수 호출. 함수 인자로 변경될 라운드 값 입력(캐릭터 등장연출 끝나고 1라운드 될 때 1. 1라 끝나고 2라 될 때 2)
        public void IngameBGMUpdate(int round)
        {
            switch (round)
            {
                case 1:
                    _emitter.SetParameter("EntryFinish", 1);
                    break;

                case 2:
                    _emitter.SetParameter("Round2Start", 1);
                    _emitter.SetParameter("EntryFinish", 0);
                    break;

                case 3:
                    _emitter.SetParameter("Round3Start", 1);
                    _emitter.SetParameter("Round2Start", 0);
                    break;

                case 4:
                    _emitter.SetParameter("Round3Finish", 1);
                    _emitter.SetParameter("Round3Start", 0);
                    break;

                case 5:
                    _emitter.SetParameter("FinalRoundStart", 1);
                    _emitter.SetParameter("Round3Finish", 0);
                    break;
            }
        }
    }
}

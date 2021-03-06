using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace EsperFightersCup
{
    /// <summary>
    /// 코루틴으로 동작하는 타이머입니다.
    /// </summary>
    public class CoroutineTimer : Singleton<CoroutineTimer>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitInstance()
        {
            CreateNewSingletonObject();
        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 단 한번만 동작하는 타이머
        /// </summary>
        /// <param name="timerAction">실행할 메소드</param>
        /// <param name="delay">메소드 실행 전 딜레이 시간</param>
        public static Coroutine SetTimerOnce(UnityAction timerAction, float delay)
        {
            return Instance.StartCoroutine(SetTimerOnceCoroutine(timerAction, delay));
        }

        /// <summary>
        /// 반복적으로 동작하는 타이머
        /// </summary>
        /// <param name="timerAction">실행할 메소드</param>
        /// <param name="interval">메소드 실행 시간 간격</param>
        /// <param name="startDelay">최초 실행 전 딜레이 시간</param>
        /// <returns></returns>
        public static Coroutine SetTimerRepeat(UnityAction timerAction, float interval, float startDelay)
        {
            return Instance.StartCoroutine(SetTimerRepeatCoroutine(timerAction, interval, startDelay));
        }

        /// <summary>
        /// CoroutineTimer에서 생성한 타이머 코루틴을 정지합니다. 정지된 타이머는 null을 참조합니다.
        /// </summary>
        /// <param name="timer"><see cref="CoroutineTimer"/>에서 생성한 타이머</param>
        public static void Stop(ref Coroutine timer)
        {
            if (timer is null)
            {
                return;
            }

            Instance.StopCoroutine(timer);
            timer = null;
        }

        private static IEnumerator SetTimerOnceCoroutine(UnityAction timerAction, float delay)
        {
            if (timerAction is null)
            {
                yield break;
            }

            yield return new WaitForSeconds(delay);
            timerAction();
        }

        private static IEnumerator SetTimerRepeatCoroutine(UnityAction timerAction, float interval, float startDelay)
        {
            if (timerAction is null)
            {
                yield break;
            }

            var wait = new WaitForSeconds(interval);

            yield return new WaitForSeconds(startDelay);
            while (true)
            {
                timerAction();
                yield return wait;
            }
        }
    }
}

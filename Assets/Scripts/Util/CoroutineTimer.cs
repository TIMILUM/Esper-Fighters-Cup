using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 코루틴으로 동작하는 타이머입니다.
/// </summary>
public class CoroutineTimer : MonoBehaviour
{
    private static CoroutineTimer Instance
    {
        get
        {
            if (s_instance == null)
            {
                Init();
            }

            return s_instance;
        }
    }
    private static CoroutineTimer s_instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        if (s_instance)
        {
            return;
        }

        var timerObject = new GameObject { name = "Coroutine Timer" };
        s_instance = timerObject.AddComponent<CoroutineTimer>();
        DontDestroyOnLoad(timerObject);
    }

    /// <summary>
    /// 단 한번만 동작하는 타이머
    /// </summary>
    /// <param name="timerAction">실행할 메소드</param>
    /// <param name="delay">메소드 실행 전 딜레이 시간</param>
    public static void SetTimerOnce(UnityAction timerAction, float delay)
    {
        Instance.StartCoroutine(SetTimerOnceCoroutine(timerAction, delay));
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

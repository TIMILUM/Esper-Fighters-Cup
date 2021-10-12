using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 스킬에서 만들어내는 오브젝트를 관리하기 위해서
/// 싱글톤으로 만들었습니다.
/// </summary>
public class InGameSkillManager : MonoBehaviour
{
    private static InGameSkillManager s_instance;

    [SerializeField]
    private FragmentAreaManager _fragmentArea;

    [SerializeField]
    private SkillObjectFactory _skillObjectfactory;


    public static InGameSkillManager Instance
    {
        get
        {
            if (s_instance == null)
                return null;

            return s_instance;
        }
    }

    // Start is called before the first frame update
    private void Awake()
    {
        if (s_instance == null)
            s_instance = this;


    }
    /// <summary>
    /// 파편지대 생성
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="range"></param>
    public void AddFragmentArea(Transform trans, float range)
    {
        _fragmentArea.AddFragmentList(trans, range);
    }
    /// <summary>
    /// 파편지대 시작 event 띄움
    /// </summary>
    public void FragmentEventStart()
    {
        _fragmentArea.EventStart();
    }
    /// <summary>
    /// 지금 설치한 파편지대 수
    /// </summary>
    /// <returns></returns>
    public int FragmentCount()
    {
        return _fragmentArea.FragmentCount();
    }
    /// <summary>
    /// 파편지대 모두 활성화
    /// </summary>
    public void FragmentAllActive(Vector3 pos, float range)
    {
        _fragmentArea.SetFragmentAreaActive(pos, range);
    }
    /// <summary>
    /// 파편지대 위치 설정한 파편지대 삭제
    /// </summary>
    public void FragmentAllDestroy()
    {
        _fragmentArea.AllDestory();
    }
    /// <summary>
    /// 연속 설치 방지하기 위해서 체크하는 함수
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool CreateFragmentCheck(Vector3 pos)
    {
        return _fragmentArea.CreateFragmentCheck(pos);
    }

    /// <summary>
    /// 오브젝트 던지기는 함수
    /// </summary>
    /// <param name="buffstruct"></param>
    /// <param name="target"></param>
    public void FragmentAreaThrowObject(BuffObject.BuffStruct buffstruct, Vector3 target)
    {
        _fragmentArea.ThrowObject(buffstruct, target);
    }
    /// <summary>
    /// 취소함수
    /// </summary>
    public void CancelFragment()
    {
        _fragmentArea.CancelFragment();
    }

    /// <summary>
    /// 방향을 나타내는 함수
    /// </summary>
    /// <param name="pos"></param>
    public void FragmentDirection(Vector3 pos)
    {
        _fragmentArea.FragmentDirection(pos);
    }
    public void FragmentClear()
    {
        _fragmentArea.CurrentFragmentAreaClear();
    }




    /// <summary>
    /// 오브젝트 생성하는 함수
    /// </summary>
    /// <param name="objectname"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public GameObject CreateSkillObject(string objectname, Vector3 pos)
    {
        return _skillObjectfactory.CreateSkillObject(objectname, pos);
    }


}

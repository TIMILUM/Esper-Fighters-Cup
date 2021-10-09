using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 스킬에서 만들어내는 오브젝트를 관리하기 위해서 
/// 싱글톤으로 만들었습니다.
/// </summary>
public class InGameSkillManager : MonoBehaviour
{
    private static InGameSkillManager s_Instance;

    [SerializeField]
    private FragmentAreaManager _FragmentArea;

    [SerializeField]
    private SkillObjectFactory _skillObjectfactory;


    public static InGameSkillManager Instance
    {
        get
        {
            if (s_Instance == null)
                return null;

            return s_Instance;
        }
    }

    // Start is called before the first frame update
    private void Awake()
    {
        if (s_Instance == null)
            s_Instance = this;


    }
    /// <summary>
    /// 파편지대 생성
    /// </summary>
    /// <param name="trans"></param> 
    /// <param name="range"></param>
    public void AddFragmentArea(Transform trans, float range)
    {
        _FragmentArea.AddFragmentList(trans, range);
    }
    /// <summary>
    /// 파편지대 시작 event 띄움
    /// </summary>
    public void FragmentEventStart()
    {
        _FragmentArea.EventStart();
    }
    /// <summary>
    /// 지금 설치한 파편지대 수
    /// </summary>
    /// <returns></returns>
    public int FragmentCount()
    {
        return _FragmentArea.FragmentCount();
    }
    /// <summary>
    /// 파편지대 모두 활성화
    /// </summary>
    public void FragmentAllActive(Vector3 pos, float range)
    {
        _FragmentArea.SetFragmentAreaActive(pos, range);
    }
    /// <summary>
    /// 파편지대 위치 설정한 파편지대 삭제
    /// </summary>
    public void FragmentAllDestroy()
    {
        _FragmentArea.AllDestory();
    }
    /// <summary>
    /// 연속 설치 방지하기 위해서 체크하는 함수
    /// </summary>
    /// <param name="Pos"></param>
    /// <returns></returns>
    public bool CreateFragmentCheck(Vector3 Pos)
    {
        return _FragmentArea.CreateFragmentCheck(Pos);
    }

    /// <summary>
    /// 오브젝트 던지기는 함수
    /// </summary>
    /// <param name="Buffstruct"></param>
    /// <param name="Target"></param>
    public void FragmentAreaThrowObject(BuffObject.BuffStruct Buffstruct, Vector3 Target)
    {
        _FragmentArea.ThrowObject(Buffstruct, Target);
    }
    /// <summary>
    /// 취소함수
    /// </summary>
    public void CancelFragment()
    {
        _FragmentArea.CancelFragment();
    }

    /// <summary>
    /// 방향을 나타내는 함수
    /// </summary>
    /// <param name="pos"></param>
    public void FragmentDirection(Vector3 pos)
    {
        _FragmentArea.FragmentDirection(pos);
    }





    public GameObject CreateSkillObject(string objectname, Vector3 pos)
    {
        return _skillObjectfactory.CreateSkillObject(objectname, pos);
    }
    public List<GameObject> CompareSkillObject(Vector3 pos, float range)
    {
        return _skillObjectfactory.CompareSkillObject(pos, range);
    }
    public void RemoveSkillObject(GameObject removeObject)
    {
        _skillObjectfactory.RemoveSkillObject(removeObject);
    }

}

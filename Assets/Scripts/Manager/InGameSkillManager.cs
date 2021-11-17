using EsperFightersCup.Util;
using UnityEngine;

/// <summary>
/// 스킬에서 만들어내는 오브젝트를 관리하기 위해서
/// 싱글톤으로 만들었습니다.
/// </summary>
public class InGameSkillManager : Singleton<InGameSkillManager>
{

    [SerializeField]
    private SkillObjectFactory _skillObjectfactory;

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

    public GameObject CreateSkillUI(string objectname, Vector3 pos)
    {
        return _skillObjectfactory.CreateSkillUI(objectname, pos);
    }

    public void DestroySkillObj(GameObject obj)
    {
        _skillObjectfactory.DestroyObj(obj);
    }

}

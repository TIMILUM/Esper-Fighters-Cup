using EsperFightersCup;
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
    public GameObject CreateSkillObject(string objectname, Vector3 pos, Quaternion rot)
    {
        return _skillObjectfactory.CreateSkillObject(objectname, pos, rot);
    }
    public GameObject CreateSkillObject(string objectname, Vector3 pos, Quaternion rot, Vector3 Scale)
    {
        return _skillObjectfactory.CreateSkillObject(objectname, pos, rot);
    }





    /// <summary>
    ///     오브젝트의 ID 값을 가지고 오브젝트를 생성하는 함수입니다. (CSV의 ID값을 통해 오브젝트를 불러오기 때문에 생성되었습니다.)
    ///     수정이 필요하면 함수를 만든 사람에게 물어볼 필요 없이 바로 수정하셔도 됩니다.
    /// </summary>
    /// <param name="objectId">오브젝트 ID</param>
    /// <param name="pos">생성 포지션</param>
    /// <returns>생성된 게임 오브젝트</returns>
    public GameObject CreateSkillObject(int objectId, Vector3 pos)
    {
        return _skillObjectfactory.CreateSkillObject(objectId, pos);
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

public interface IReadonlySkillCollection : IReadOnlyCollection<KeyValuePair<int, SkillObject>>
{
    /// <summary>
    /// 스킬 콜렉션에 존재하는 스킬 아이디 목록
    /// </summary>
    IEnumerable<int> Ids { get; }

    /// <summary>
    /// 스킬 콜렉션에 존재하는 스킬 오브젝트 목록
    /// </summary>
    IEnumerable<SkillObject> Skills { get; }

    /// <summary>
    /// ID와 일치하는 스킬을 가져옵니다.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    SkillObject this[int id] { get; }

    /// <summary>
    /// ID와 일치하는 스킬이 있는지 확인합니다.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    bool Exist(int id);

    /// <summary>
    /// ID와 일치하는 스킬 오브젝트를 가져옵니다.
    /// </summary>
    /// <param name="id">가져올 스킬의 ID</param>
    /// <param name="skill">ID와 일치하는 스킬 오브젝트. 존재하지 않으면 <see langword="null"/>을 반환합니다.</param>
    /// <returns>ID와 일치하는 스킬 오브젝트의 존재 여부</returns>
    bool TryGetSkill(int id, out SkillObject skill);

    /// <summary>
    /// 이름과 일치하는 스킬 오브젝트를 검색합니다.
    /// </summary>
    /// <param name="skill">검색된 스킬 오브젝트</param>
    /// <returns>검색 여부</returns>
    bool TryFindSkill(string name, out SkillObject skill);
}

public interface ISkillCollection
{
    /// <summary>
    /// 스킬 콜렉션에 스킬을 추가합니다.
    /// </summary>
    /// <param name="skill">추가할 스킬</param>
    /// <returns>ID가 일치하는 스킬이 이미 존재할 경우 추가하지 않고 <see langword="false"/>를 반환합니다.</returns>
    bool Add(SkillObject skill);

    /// <summary>
    /// ID와 일치하는 스킬 오브젝트를 목록에서 제거합니다.
    /// </summary>
    /// <param name="id">제거하고 싶은 스킬의 ID</param>
    /// <param name="removedSkill">목록에서 제거된 스킬 오브젝트</param>
    /// <returns>제거 여부</returns>
    bool Remove(int id, out SkillObject removedSkill);

    /// <summary>
    /// 모든 스킬을 스킬 콜렉션에서 제거합니다.
    /// </summary>
    /// <returns>제거된 스킬 목록</returns>
    IReadOnlyList<SkillObject> Clear();
}

public class SkillCollection : IReadonlySkillCollection, ISkillCollection
{
    private readonly Dictionary<int, SkillObject> _skills = new Dictionary<int, SkillObject>();

    public SkillObject this[int id] => _skills[id];

    public IEnumerable<int> Ids => _skills.Keys;

    public IEnumerable<SkillObject> Skills => _skills.Values;

    /// <summary>
    /// 스킬 콜렉션에 있는 스킬의 갯수를 가져옵니다.
    /// </summary>
    public int Count => _skills.Count;

    public bool Add(SkillObject skill)
    {
        if (!skill || _skills.ContainsKey(skill.ID))
        {
            return false;
        }
        _skills.Add(skill.ID, skill);
        return true;
    }

    public bool TryFindSkill(string name, out SkillObject skill)
    {
        skill = Skills.FirstOrDefault(skill => skill.Name == name);
        return skill != null;
    }

    public bool TryGetSkill(int id, out SkillObject skill)
    {
        return _skills.TryGetValue(id, out skill);
    }

    public bool Remove(int id, out SkillObject removedSkill)
    {
        if (!_skills.TryGetValue(id, out removedSkill))
        {
            return false;
        }

        _skills.Remove(id);
        return true;
    }

    public IReadOnlyList<SkillObject> Clear()
    {
        var removedSkills = _skills.Values.ToList();
        _skills.Clear();

        return removedSkills;
    }

    public bool Exist(int id)
    {
        return _skills.TryGetValue(id, out var _);
    }

    public IEnumerator<KeyValuePair<int, SkillObject>> GetEnumerator()
    {
        return _skills.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

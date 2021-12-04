using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillController : ControllerBase
{
    private readonly SkillCollection _activeSkills = new SkillCollection();
    private readonly object _skillReleaseLock = new object();

    private Dictionary<int, SkillObject> _skillTemplates;
    private BuffController _buffController;

    /// <summary>
    /// 현재 플레이어에 활성화된 스킬 목록
    /// </summary>
    public IReadonlySkillCollection ActiveSkills => _activeSkills;

    protected override void Reset()
    {
        base.Reset();

        // 컨트롤러 타입 지정을 위해 Reset 함수로 이렇게 선언을 해줘야 합니다.
        // 리플렉션으로 전환할 예정 (IL2CPP 모듈 추가가 필요하기 때문에 나중에 전환할 예정)
        SetControllerType(ControllerManager.Type.SkillController);
    }

    protected override void Start()
    {
        base.Start();
        if (!ControllerManager.Author.photonView.IsMine)
        {
            return;
        }

        var skills = new List<SkillObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.TryGetComponent<SkillObject>(out var skill))
            {
                skills.Add(skill);
            }
        }
        _skillTemplates = skills.ToDictionary(skill => skill.ID);

        _buffController = ControllerManager.GetController<BuffController>(ControllerManager.Type.BuffController);
    }

    protected override void Update()
    {
        base.Update();
        if (!ControllerManager.Author.photonView.IsMine)
        {
            return;
        }

        if (IngameFSMSystem.Instance.CurrentState != IngameFSMSystem.State.InBattle)
        {
            if (_activeSkills.Count > 0)
            {
                ReleaseAllSkills();
            }
            return;
        }

        // 스턴 확인 시 스킬 사용을 멈춥니다.
        if (_buffController.ActiveBuffs.Exists(BuffObject.Type.Stun))
        {
            if (_activeSkills.Count > 0)
            {
                ReleaseAllSkills();
            }
            return;
        }

        if (ActiveSkills.Count > 0)
        {
            return;
        }

        foreach (var skillTemplate in _skillTemplates.Values)
        {
            if (Input.GetKeyDown(skillTemplate.InputKey) && !_activeSkills.Exist(skillTemplate.ID))
            {
                // Debug.Log($"UseSkill({skillTemplate.ID})");
                // TODO: GenerateSkill 이후 다음 프레임에 바로 GetSkill에서 확인이 되는지 체크
                UseSkill(skillTemplate.ID);
            }
        }
    }

    /// <summary>
    /// 스킬을 생성합니다.
    /// </summary>
    /// <param name="id">생성할 스킬의 ID</param>
    public void UseSkill(int id)
    {
        if (_activeSkills.Exist(id))
        {
            Debug.LogWarning($"이미 아이디가 {id}인 스킬이 플레이어에게 존재합니다.", gameObject);
            return;
        }

        if (!_skillTemplates.TryGetValue(id, out var skill))
        {
            Debug.LogWarning($"스킬 목록에 {id} 와 일치하는 ID를 가진 스킬이 없습니다.");
            return;
        }

        if (_activeSkills.Exist(id))
        {
            Debug.LogWarning($"아이디가 {id}인 스킬이 이미 존재합니다.", gameObject);
            return;
        }

        if (skill.Register(this, () => RemoveAfterReleased(skill)))
        {
            _activeSkills.Add(skill);
        }
        else
        {
            Debug.Log("Register 실패");
        }
    }

    /// <summary>
    /// 현재 활성화된 모든 스킬을 해제합니다.
    /// </summary>
    public void ReleaseAllSkills()
    {
        foreach (var skill in ActiveSkills.Skills.ToList())
        {
            ReleaseSkill(skill);
        }
    }

    /// <summary>
    /// 스킬을 해제합니다.
    /// </summary>
    /// <param name="skillObject">해제할 스킬 오브젝트</param>
    public void ReleaseSkill(SkillObject skillObject)
    {
        lock (_skillReleaseLock)
        {
            if (skillObject.CurrentState != SkillObject.State.Release)
            {
                skillObject.Release();
            }
        }
    }

    private void RemoveAfterReleased(SkillObject skill)
    {
        _activeSkills.Remove(skill.ID, out var _);
    }
}

using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// TODO: SkillCollection 구현 및 적용
// public class SkillCollection { }

[RequireComponent(typeof(PhotonView))]
public class SkillController : ControllerBase
{
    [SerializeField] private Skill[] _skills;

    private readonly Dictionary<int, SkillObject> _activeSkills = new Dictionary<int, SkillObject>();
    private readonly Dictionary<int, Skill> _skillTable = new Dictionary<int, Skill>();
    private readonly object _skillReleaseLock = new object();

    private BuffController _buffController;

    /// <summary>
    /// 현재 플레이어에 활성화된 키가 스킬 아이디, 값이 스킬 오브젝트인 스킬 목록
    /// </summary>
    public IReadOnlyDictionary<int, SkillObject> ActiveSkills => _activeSkills;

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

        _buffController = ControllerManager.GetController<BuffController>(ControllerManager.Type.BuffController);

        foreach (var skill in _skills)
        {
            _skillTable.Add(skill.SkillPrefab.ID, skill);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (photonView.IsMine)
        {
            UpdateMine();
        }
    }

    private void UpdateMine()
    {
        if (IngameFSMSystem.Instance.CurrentState != IngameFSMSystem.State.InBattle)
        {
            ReleaseAllSkills();
            return;
        }

        // 스턴 확인 시 스킬 사용을 멈춥니다.
        if (_buffController.ActiveBuffs.Exists(BuffObject.Type.Stun))
        {
            ReleaseAllSkills();
            return;
        }

        foreach (var skill in _skills)
        {
            if (!Input.GetKeyDown(skill.Key))
            {
                continue;
            }

            if (!TryGetActiveSkill(skill.SkillPrefab.ID, out var _))
            {
                // TODO: GenerateSkill 이후 다음 프레임에 바로 GetSkill에서 확인이 되는지 체크
                GenerateSkill(skill.SkillPrefab.ID);
            }
        }
    }

    /// <summary>
    /// 현재 활성화된 스킬 중 이름과 일치하는 스킬을 가져옵니다.
    /// </summary>
    /// <param name="skillName">검색할 스킬 이름</param>
    /// <returns>검색된 스킬 오브젝트를 반환합니다. 스킬을 찾지 못한 경우 null을 반환합니다.</returns>
    public SkillObject GetActiveSkill(string skillName)
    {
        for (var i = _activeSkills.Count - 1; i >= 0; --i)
        {
            var skill = _activeSkills[i];
            if (skill.Name.Equals(skillName))
            {
                return skill;
            }
        }

        return null;
    }

    /// <summary>
    /// ID와 일치하는 활성화 스킬을 검색합니다. <see cref="GetActiveSkill(string)"/>보다 빠릅니다.
    /// </summary>
    /// <param name="id">검색할 스킬 ID</param>
    /// <param name="skill">검색된 스킬 오브젝트</param>
    /// <returns>스킬의 존재 여부를 반환합니다.</returns>
    public bool TryGetActiveSkill(int id, out SkillObject skill)
    {
        return _activeSkills.TryGetValue(id, out skill);
    }

    /// <summary>
    /// 스킬을 생성합니다. RPC를 통해 동기화됩니다.
    /// </summary>
    /// <param name="id">생성할 스킬의 ID</param>
    public void GenerateSkill(int id)
    {
        photonView.RPC(nameof(GenerateSkillRPC), RpcTarget.All, id);
    }

    [PunRPC]
    private void GenerateSkillRPC(int id)
    {
        if (!_skillTable.TryGetValue(id, out var skill))
        {
            return;
        }

        var skillObject = Instantiate(skill.SkillPrefab, transform);
        skillObject.Register(this);

        _activeSkills.Add(skillObject.ID, skillObject);
    }

    /// <summary>
    /// 스킬을 해제합니다. RPC를 통해 동기화됩니다.
    /// </summary>
    /// <param name="skillObject">해제할 스킬 오브젝트</param>
    public void ReleaseSkill(SkillObject skillObject)
    {
        photonView.RPC(nameof(ReleaseSkillRPC), RpcTarget.All, skillObject.ID);
    }

    [PunRPC]
    private void ReleaseSkillRPC(int id)
    {
        SkillObject targetSkill;
        lock (_skillReleaseLock)
        {
            if (!_activeSkills.TryGetValue(id, out targetSkill))
            {
                return;
            }
            _activeSkills.Remove(id);
        }

        if (targetSkill.CurrentState != SkillObject.State.Release)
        {
            targetSkill.SetState(SkillObject.State.Canceled);
        }
    }

    /// <summary>
    /// 현재 활성화된 모든 스킬을 해제합니다. RPC를 통해 동기화됩니다.
    /// </summary>
    public void ReleaseAllSkills()
    {
        photonView.RPC(nameof(ReleaseAllSkillsRPC), RpcTarget.All);
    }

    [PunRPC]
    private void ReleaseAllSkillsRPC()
    {
        lock (_skillReleaseLock)
        {
            foreach (var targetSkill in _activeSkills.Values)
            {
                if (targetSkill.CurrentState != SkillObject.State.Release)
                {
                    targetSkill.SetState(SkillObject.State.Canceled);
                }
            }
            _activeSkills.Clear();
        }
    }

    [Serializable]
    private class Skill
    {
        [Tooltip("스킬 이름")]
        [SerializeField] private string _name;

        [Tooltip("스킬 사용 키")]
        [SerializeField] private KeyCode _key;

        [Tooltip("스킬 프리팹")]
        [SerializeField] private SkillObject _skillPrefab;

        public string Name => _name;
        public KeyCode Key => _key;
        public SkillObject SkillPrefab => _skillPrefab;
    }
}

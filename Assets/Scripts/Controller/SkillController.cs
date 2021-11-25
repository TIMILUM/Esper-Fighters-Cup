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

    private readonly SkillCollection _activeSkills = new SkillCollection();
    private readonly object _skillReleaseLock = new object();

    private IReadOnlyDictionary<int, SkillObject> _skillTable;
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

        var table = new Dictionary<int, SkillObject>();
        foreach (var skill in _skills)
        {
            table.Add(skill.SkillPrefab.ID, skill.SkillPrefab);
        }
        _skillTable = table;

        _buffController = ControllerManager.GetController<BuffController>(ControllerManager.Type.BuffController);
    }

    protected override void Update()
    {
        base.Update();

        if (!photonView.IsMine)
        {
            return;
        }

        if (IngameFSMSystem.Instance.CurrentState != IngameFSMSystem.State.InBattle && _activeSkills.Count > 0)
        {
            ReleaseAllSkills();
            return;
        }

        // 스턴 확인 시 스킬 사용을 멈춥니다.
        if (_buffController.ActiveBuffs.Exists(BuffObject.Type.Stun) && _activeSkills.Count > 0)
        {
            ReleaseAllSkills();
            return;
        }

        foreach (var skill in _skills)
        {
            if (Input.GetKeyDown(skill.Key) && !_activeSkills.Exist(skill.SkillPrefab.ID))
            {
                // TODO: GenerateSkill 이후 다음 프레임에 바로 GetSkill에서 확인이 되는지 체크
                GenerateSkill(skill.SkillPrefab.ID);
            }
        }
    }

    /// <summary>
    /// 스킬을 생성합니다. RPC를 통해 동기화됩니다.
    /// </summary>
    /// <param name="id">생성할 스킬의 ID</param>
    public void GenerateSkill(int id)
    {
        if (_activeSkills.Exist(id))
        {
            Debug.LogWarning($"이미 아이디가 {id}인 스킬이 플레이어에게 존재합니다.", gameObject);
            return;
        }

        photonView.RPC(nameof(GenerateSkillRPC), RpcTarget.All, id);
    }

    /// <summary>
    /// 현재 활성화된 모든 스킬을 해제합니다. RPC를 통해 동기화됩니다.
    /// </summary>
    public void ReleaseAllSkills()
    {
        // RPC를 받기 전에 Update에서 계속 체크하는 이슈때문에 로컬에서 먼저 삭제해야 함
        ReleaseAllSkillsRPC();
        photonView.RPC(nameof(ReleaseAllSkillsRPC), RpcTarget.Others);
    }

    /// <summary>
    /// 스킬을 해제합니다. RPC를 통해 동기화됩니다.
    /// </summary>
    /// <param name="skillObject">해제할 스킬 오브젝트</param>
    public void ReleaseSkill(SkillObject skillObject)
    {
        photonView.RPC(nameof(ReleaseSkillRPC), RpcTarget.All, skillObject.ID);
    }

    /// <summary>
    /// 스킬의 상태를 변경합니다. RPC를 통해 동기화됩니다.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="state"></param>
    public void ChangeState(int id, SkillObject.State state)
    {
        // 혹시 스킬에서 계속 확인하는 로직이 있을 수 있어서 로컬에서 먼저 수행 후 RPC를 보내는 방식으로 작성
        ChangeSkillStateRPC(id, (int)state);
        photonView.RPC(nameof(ChangeSkillStateRPC), RpcTarget.Others, id, (int)state);
    }

    [PunRPC]
    private void GenerateSkillRPC(int id)
    {
        if (!_skillTable.TryGetValue(id, out var skillTemplate))
        {
            return;
        }

        var skillObject = Instantiate(skillTemplate, transform);
        if (_activeSkills.TryAdd(skillObject))
        {
            skillObject.Register(this);
        }
        else
        {
            Debug.LogWarning($"아이디가 {id}인 스킬이 이미 존재합니다.", gameObject);
            Destroy(skillObject);
        }
    }

    [PunRPC]
    private void ReleaseAllSkillsRPC()
    {
        lock (_skillReleaseLock)
        {
            var removed = _activeSkills.Clear();
            foreach (var skill in removed)
            {
                if (skill.CurrentState != SkillObject.State.Release)
                {
                    // SetState는 RPC로 동기화되기 때문에 이미 ReleaseAllSkills 메소드가 RPC로 호출된 시점에서
                    // 또 State를 동기화할 필요가 없으므로 SyncState 호출
                    skill.SetStateToLocal(SkillObject.State.Canceled);
                }
            }
        }
    }

    [PunRPC]
    private void ReleaseSkillRPC(int id)
    {
        lock (_skillReleaseLock)
        {
            if (_activeSkills.TryRemove(id, out var removed) && removed.CurrentState != SkillObject.State.Release)
            {
                removed.SetStateToLocal(SkillObject.State.Canceled);
            }
        }
    }

    [PunRPC]
    private void ChangeSkillStateRPC(int id, int state)
    {
        var skillState = (SkillObject.State)state;
        if (!_activeSkills.TryGetSkill(id, out var skill))
        {
            Debug.LogError($"{id}와 일치하는 ID의 스킬 오브젝트를 찾지 못했습니다.");
            return;
        }

        skill.SetStateToLocal(skillState);
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

using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : ControllerBase
{
    [SerializeField]
    private Skill[] _skills;

    private readonly List<ControllerObject> _skillObjects = new List<ControllerObject>();
    private BuffController _buffController;

    private void Reset()
    {
        // 컨트롤러 타입 지정을 위해 Reset 함수로 이렇게 선언을 해줘야 합니다.
        // 리플렉션으로 전환할 예정 (IL2CPP 모듈 추가가 필요하기 때문에 나중에 전환할 예정)
        SetControllerType(ControllerManager.Type.SkillController);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _buffController = _controllerManager.GetController<BuffController>(ControllerManager.Type.BuffController);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!photonView.IsMine)
        {
            return;
        }

        UpdateMine();
    }

    private void UpdateMine()
    {
        if (IngameFSMSystem.CurrentState != IngameFSMSystem.State.InBattle)
        {
            ReleaseAllSkills();
            return;
        }
        
        // 스턴 확인 시 스킬 사용을 멈춥니다.
        if (_buffController.GetBuff(BuffObject.Type.Stun) != null)
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

            if (GetSkill(skill.SkillPrefab.ID) != null)
            {
                continue;
            }

            var skillObject = GenerateSkill(skill.SkillPrefab);
            skillObject.Name = skill.Name;
        }
    }

    public SkillObject GetSkill(string skillName)
    {
        for (var i = _skillObjects.Count - 1; i >= 0; --i)
        {
            var skill = (SkillObject)_skillObjects[i];
            if (skill.Name.Equals(skillName))
            {
                return skill;
            }
        }

        return null;
    }

    public SkillObject GetSkill(int id)
    {
        for (var i = _skillObjects.Count - 1; i >= 0; --i)
        {
            var skill = (SkillObject)_skillObjects[i];
            if (skill.ID == id)
            {
                return skill;
            }
        }

        return null;
    }

    public SkillObject GenerateSkill(SkillObject skillPrefab)
    {
        var skillObject = Instantiate(skillPrefab, transform);
        skillObject.Register(this);

        _skillObjects.Add(skillObject);
        return skillObject;
    }

    public void ReleaseSkill(SkillObject skillObject)
    {
        var obj = _skillObjects.Find(x => x == skillObject);
        if (obj == null)
        {
            return;
        }

        if (skillObject.CurrentState != SkillObject.State.Release)
        {
            skillObject.SetState(SkillObject.State.Canceled);
        }
        _skillObjects.Remove(skillObject);
    }

    public void ReleaseAllSkills()
    {
        for (var i = _skillObjects.Count - 1; i >= 0; --i)
        {
            var skillObject = (SkillObject)_skillObjects[i];
            ReleaseSkill(skillObject);
        }
    }

    [Serializable]
    private class Skill
    {
        [SerializeField] private string _name;
        [SerializeField] private KeyCode _key;
        [SerializeField] private SkillObject _skillPrefab;

        public string Name { get => _name; set => _name = value; }
        public KeyCode Key { get => _key; set => _key = value; }
        public SkillObject SkillPrefab { get => _skillPrefab; set => _skillPrefab = value; }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : ControllerBase
{
    [SerializeField]
    private Skill[] _skills;

    private readonly List<ControllerObject> _skillObjects = new List<ControllerObject>();
    private BuffController _buffController;
    private APlayer _player;

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
        _player = _controllerManager.GetActor() as APlayer;
        _buffController =
            _controllerManager.GetController<BuffController>(ControllerManager.Type.BuffController);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (photonView != null && !photonView.IsMine)
        {
            return;
        }

        UpdateMine();
    }

    public override void OnPlayerHitEnter(GameObject other)
    {
        for (var i = _skillObjects.Count - 1; i >= 0; i--)
        {
            _skillObjects[i].OnPlayerHitEnter(other);
        }
    }

    private void UpdateMine()
    {
        // 스턴 확인 시 스킬 사용을 멈춥니다.
        if (_buffController.GetBuff(BuffObject.Type.Stun) != null)
        {
            ReleaseAllSkills();
            return;
        }

        foreach (var skill in _skills)
        {
            if (Input.GetKeyDown(skill.Key))
            {
                if (GetSkill(skill.Name) != null)
                {
                    continue;
                }

                var skillObject = GenerateSkill(skill.SkillPrefab);
                skillObject.Name = skill.Name;
            }
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

        skillObject.SetState(SkillObject.State.Canceled);
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
        public string Name;
        public KeyCode Key;
        public SkillObject SkillPrefab;
    }
}
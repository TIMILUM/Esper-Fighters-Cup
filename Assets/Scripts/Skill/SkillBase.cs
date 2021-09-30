using UnityEngine;

public enum SKILLKEYCODE
{
    LEFTMOUSEBUTTONDOWN, RIGHTMOUSEBUTTONDOWN, CONTROL
}

public abstract class SkillBase : MonoBehaviour
{
    [SerializeField, TextArea] private string _SkillDesc; // 스킬 설명
    [SerializeField] private string _SkillName; // 스킬 이름
    [SerializeField] private string _SkillAnimation; // 스킬에 적용할 애니메이션
    [SerializeField] protected float _SKillRange; // 스킬 사거리
    [SerializeField] protected SKILLKEYCODE _SkillKeyCode; // 스킬 키 코드
    public bool isSkillUsing { get; set; } // 스킬 사용중인지 아닌지 판별


    public abstract void SkillExecute();
    public abstract bool EndSkill();
    public bool TrySkill()
    {

        if (_SkillKeyCode == SKILLKEYCODE.CONTROL)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                isSkillUsing = true;
                return true;
            }

        }
        else if (_SkillKeyCode == SKILLKEYCODE.LEFTMOUSEBUTTONDOWN)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                isSkillUsing = true;
                return true;
            }
        }
        else if (_SkillKeyCode == SKILLKEYCODE.RIGHTMOUSEBUTTONDOWN)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                isSkillUsing = true;
                return true;

            }
        }

        return false;
    }
}

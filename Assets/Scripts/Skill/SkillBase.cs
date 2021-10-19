using UnityEngine;

public enum SKILLKEYCODE
{
    LEFTMOUSEBUTTONDOWN, RIGHTMOUSEBUTTONDOWN, CONTROL
}

public abstract class SkillBase : MonoBehaviour
{
    [SerializeField, TextArea] private string _skillDesc; // 스킬 설명
    [SerializeField] private string _skillName; // 스킬 이름
    [SerializeField] private string _skillAnimation; // 스킬에 적용할 애니메이션
    [SerializeField] protected float _sKillRange; // 스킬 사거리
    [SerializeField] protected SKILLKEYCODE _skillKeyCode; // 스킬 키 코드
    public bool IsSkillUsing { get; set; } // 스킬 사용중인지 아닌지 판별


    public abstract void SkillExecute();
    public abstract bool EndSkill();
    public bool TrySkill()
    {

        if (_skillKeyCode == SKILLKEYCODE.CONTROL)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                IsSkillUsing = true;
                return true;
            }

        }
        else if (_skillKeyCode == SKILLKEYCODE.LEFTMOUSEBUTTONDOWN)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                IsSkillUsing = true;
                return true;
            }
        }
        else if (_skillKeyCode == SKILLKEYCODE.RIGHTMOUSEBUTTONDOWN)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                IsSkillUsing = true;
                return true;

            }
        }

        return false;
    }
}

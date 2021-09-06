using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum SKILLKEYCODE
{
    LEFTMOUSEBUTTONDOWN , RIGHTMOUSEBUTTONDOWN , CONTROL
}

public abstract class SkillBase : MonoBehaviour
{
    [SerializeField , TextArea] private string _SkillDesc; // 스킬 설명
    [SerializeField] private string _SkillName; // 스킬 이름
    [SerializeField] private string _SkillAnimation;
    [SerializeField] protected float _SKillRange;
    [SerializeField] protected SKILLKEYCODE _SkillKeyCode;
    public bool isSkillUsing { get; set; }
   

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

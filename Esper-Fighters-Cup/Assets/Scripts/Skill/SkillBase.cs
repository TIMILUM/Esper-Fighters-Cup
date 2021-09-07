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
    [SerializeField , TextArea] private string _SkillDesc; // ��ų ����
    [SerializeField] private string _SkillName; // ��ų �̸�
    [SerializeField] private string _SkillAnimation; // ��ų�� ������ �ִϸ��̼�
    [SerializeField] protected float _SKillRange; // ��ų ��Ÿ�
    [SerializeField] protected SKILLKEYCODE _SkillKeyCode; // ��ų Ű �ڵ�
    public bool isSkillUsing { get; set; } // ��ų ��������� �ƴ��� �Ǻ�
   

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

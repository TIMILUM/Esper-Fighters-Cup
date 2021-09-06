using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CHARACTER
{ 
    IDLE ,  MOVE , SPASTICITY , SKILL
}


public class CharacterContorller : MonoBehaviour
{

    [SerializeField] private float _MoveSpeed;
    [SerializeField] private float _IncreaseSpeedTime;
    [SerializeField] private float _DecreaseSpeedTime;

    [SerializeField] private float _Spastic;
    [SerializeField] private float _RotateSmooth;

    

    private SkillBase[] Skills;

    private float _MoveSpeedTime;

    private CHARACTER _CharacterStateType;
    
    private float _DirX;
    private float _DirZ;

    private float _CurrentDirX;
    private float _CurrentDirZ;



    private void Start()
    {
        Skills = GetComponents<SkillBase>();
    }


    private void Update()
    {
        StateUpdate();
        Getinput();
    }

    // ĳ���� ������
    private void CharacterMove()
    {
        CharacterLookAt();

        if (_CharacterStateType == CHARACTER.MOVE)
        {
            _CurrentDirX = _DirX;
            _CurrentDirZ = _DirZ;
        }


        _DirX = Input.GetAxisRaw("Horizontal");
        _DirZ = Input.GetAxisRaw("Vertical");
    }

    private void CharacterLookAt()
    {

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] Hitinfos = Physics.RaycastAll(ray);

        foreach (var Hitinfo in Hitinfos)
        {
            if (Hitinfo.transform.tag == "Floor")
            {
                Vector3 Dir = Hitinfo.point + new Vector3(0.0f, GetComponent<Collider>().bounds.extents.y, 0.0f)
                    - transform.position;
                transform.rotation = Quaternion.Lerp(transform.rotation,
                    Quaternion.LookRotation(Dir), _RotateSmooth);
            }
        }

    }

    // �� ���¿� ���� �̺�Ʈ ����
    private void StateUpdate()
    {
        switch (_CharacterStateType)
        {
            case CHARACTER.IDLE:
                ExecuteIdle();
                IdleStateChange();
                break;

            case CHARACTER.MOVE:
                ExecuteMove();
                MoveStateChange();
                break;

            case CHARACTER.SPASTICITY:
                StartCoroutine(SpasticityCoroutine());
                break;

            case CHARACTER.SKILL:
                CharacterMove();
                ExecuteSkill();
                SkillStateChange();
                break;
            default:
                break;
        }
    }

    


    // ��ų ���
    private void TrySkill()
    {
        foreach (var Skill in Skills)
        {
            if (Skill.TrySkill())
            {
                _CharacterStateType = CHARACTER.SKILL;
            }
        }
    }

    // ��ų ����
    private void ExecuteSkill()
    {
        foreach (var Skill in Skills)
        {
            if (Skill.isSkillUsing)
                Skill.SkillExecute();
        }
    }


    // �̵� �����϶� �� �����Ӹ��� ����
    private void ExecuteMove()
    {
        CharacterMove();
        IncreaseTime();
    }

    // ��� �����϶� �� �����Ӹ��� ����
    private void ExecuteIdle()
    {
        CharacterMove();
        DecreaseTime();
    }

    //���ǵ� ����
    private void IncreaseTime()
    {
        if (_IncreaseSpeedTime == 0)
            _IncreaseSpeedTime = 1;

        _MoveSpeedTime += Time.deltaTime / _IncreaseSpeedTime;
        _MoveSpeedTime = Mathf.Clamp(_MoveSpeedTime, 0, 1);

        var Dir = new Vector3(_CurrentDirX, 0.0f, _CurrentDirZ);
        Dir.Normalize();

        var CurrentDir = Vector3.Lerp(Vector3.zero, Dir, _MoveSpeedTime);

        transform.position += CurrentDir * _MoveSpeed * Time.deltaTime;


    }

    //���ǵ� ����
    private void DecreaseTime()
    {

        if (_DecreaseSpeedTime == 0)
            _DecreaseSpeedTime = 1;

        _MoveSpeedTime -= Time.deltaTime / _DecreaseSpeedTime;
        _MoveSpeedTime = Mathf.Clamp(_MoveSpeedTime, 0, 1);

        var Dir = new Vector3(_CurrentDirX, 0.0f, _CurrentDirZ);
        Dir.Normalize();

        var CurrentDir = Vector3.Lerp(Dir, Vector3.zero, 1 - _MoveSpeedTime);
        transform.position += CurrentDir * _MoveSpeed * Time.deltaTime;
    }




    //����϶� ���� ����
    private void IdleStateChange()
    {
   

        if (_DirX != 0 || _DirZ != 0)
        {
            _CharacterStateType = CHARACTER.MOVE;
            return;
        }
        TrySkill();
    }

    // ��ų�� ���� ������ ���� ����
    private void SkillStateChange()
    {
        foreach (var Skill in Skills)
        {
            if (!Skill.isSkillUsing)
                continue;
            if (Skill.EndSkill() )
            {
                _CharacterStateType = CHARACTER.IDLE;
                return;
            }
        }
    }

    //������ ���� ���� 
    private void MoveStateChange()
    {
        if (_DirX == 0 && _DirZ == 0)
        {
            _CharacterStateType = CHARACTER.IDLE;
        }
        TrySkill();
    } 


    //�ӽ÷� ����
    private void Getinput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _CharacterStateType = CHARACTER.SPASTICITY;
        }
    }

    //�ӽ� ���� �ð�
    private IEnumerator SpasticityCoroutine()
    {
        
        yield return new WaitForSeconds(_Spastic);

        _CharacterStateType = CHARACTER.IDLE;
    }
        
}

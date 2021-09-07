using System.Collections;
using UnityEngine;

public enum CHARACTER
{
    IDLE, MOVE, SPASTICITY, SKILL
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

    // 캐릭터 움직임
    private void CharacterMove()
    {
        CharacterLookAt();

        if (_DirX != 0 || _DirZ != 0)
        {
            _CurrentDirX = _DirX;
            _CurrentDirZ = _DirZ;
        }


        _DirX = Input.GetAxisRaw("Horizontal");
        _DirZ = Input.GetAxisRaw("Vertical");
    }


    // 마우스를 바라보는 로직
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

    // 각 상태에 따른 이벤트 적용
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
                PlayerSkillMove();
                ExecuteSkill();
                SkillStateChange();
                break;
            default:
                break;
        }
    }




    // 스킬 사용
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

    // 스킬 적용
    private void ExecuteSkill()
    {
        foreach (var Skill in Skills)
        {
            if (Skill.isSkillUsing)
                Skill.SkillExecute();
        }
    }


    // 이동 상태일때 매 프레임마다 적용
    private void ExecuteMove()
    {
        CharacterMove();
        IncreaseTime();
    }
    // 대기 상태일때 매 프레임마다 적용
    private void ExecuteIdle()
    {
        CharacterMove();
        DecreaseTime();
    }
    //스피드 증가
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
    //스피드 감소
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

    private void PlayerSkillMove()
    {
        if (_DirX == 0 && _DirZ == 0)
        {
            DecreaseTime();
            return;
        }

        IncreaseTime();
    }


    //대기일때 상태 변이
    private void IdleStateChange()
    {
        if (_DirX != 0 || _DirZ != 0)
        {
            _CharacterStateType = CHARACTER.MOVE;
            return;
        }
        TrySkill();
    }

    // 스킬이 끝난 다음의 상태 변이
    private void SkillStateChange()
    {
        foreach (var Skill in Skills)
        {
            if (!Skill.isSkillUsing)
                continue;
            if (Skill.EndSkill())
            {
                _CharacterStateType = CHARACTER.IDLE;
                return;
            }
        }
    }

    //움직임 상태 변이 
    private void MoveStateChange()
    {
        if (_DirX == 0 && _DirZ == 0)
        {
            _CharacterStateType = CHARACTER.IDLE;
        }
        TrySkill();
    }

    //임시로 경직
    private void Getinput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _CharacterStateType = CHARACTER.SPASTICITY;
        }
    }

    //임시 경직 시간
    private IEnumerator SpasticityCoroutine()
    {

        yield return new WaitForSeconds(_Spastic);

        _CharacterStateType = CHARACTER.IDLE;
    }

}

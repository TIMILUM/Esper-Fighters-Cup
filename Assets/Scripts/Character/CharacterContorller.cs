using System.Collections;
using UnityEngine;

public enum CHARACTER
{
    IDLE, MOVE, SPASTICITY, SKILL
}

public class CharacterContorller : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _increaseSpeedTime;
    [SerializeField] private float _decreaseSpeedTime;

    [SerializeField] private float _spastic;
    [SerializeField] private float _rotateSmooth;

    private SkillBase[] _skills;

    private float _moveSpeedTime;

    private CHARACTER _characterStateType;

    private float _dirX;
    private float _dirZ;

    private float _currentDirX;
    private float _currentDirZ;

    private void Start()
    {
        _skills = GetComponents<SkillBase>();
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

        if (_dirX != 0 || _dirZ != 0)
        {
            _currentDirX = _dirX;
            _currentDirZ = _dirZ;
        }


        _dirX = Input.GetAxisRaw("Horizontal");
        _dirZ = Input.GetAxisRaw("Vertical");
    }


    // 마우스를 바라보는 로직
    private void CharacterLookAt()
    {

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hitinfos = Physics.RaycastAll(ray);

        foreach (var hitinfo in hitinfos)
        {
            if (hitinfo.transform.CompareTag("Floor"))
            {
                Vector3 dir = hitinfo.point + new Vector3(0.0f, GetComponent<Collider>().bounds.extents.y, 0.0f)
                    - transform.position;
                transform.rotation = Quaternion.Lerp(transform.rotation,
                    Quaternion.LookRotation(dir), _rotateSmooth);
            }
        }

    }

    // 각 상태에 따른 이벤트 적용
    private void StateUpdate()
    {
        switch (_characterStateType)
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
        foreach (var skill in _skills)
        {
            if (skill.TrySkill())
            {
                _characterStateType = CHARACTER.SKILL;
            }
        }
    }

    // 스킬 적용
    private void ExecuteSkill()
    {
        foreach (var skill in _skills)
        {
            if (skill.isSkillUsing)
            {
                skill.SkillExecute();
            }
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
        if (_increaseSpeedTime == 0)
        {
            _increaseSpeedTime = 1;
        }

        _moveSpeedTime += Time.deltaTime / _increaseSpeedTime;
        _moveSpeedTime = Mathf.Clamp(_moveSpeedTime, 0, 1);

        var dir = new Vector3(_currentDirX, 0.0f, _currentDirZ);
        dir.Normalize();

        var currentDir = Vector3.Lerp(Vector3.zero, dir, _moveSpeedTime);

        transform.position += _moveSpeed * Time.deltaTime * currentDir;
    }

    //스피드 감소
    private void DecreaseTime()
    {

        if (_decreaseSpeedTime == 0)
        {
            _decreaseSpeedTime = 1;
        }

        _moveSpeedTime -= Time.deltaTime / _decreaseSpeedTime;
        _moveSpeedTime = Mathf.Clamp(_moveSpeedTime, 0, 1);

        var dir = new Vector3(_currentDirX, 0.0f, _currentDirZ);
        dir.Normalize();

        var currentDir = Vector3.Lerp(dir, Vector3.zero, 1 - _moveSpeedTime);
        transform.position += _moveSpeed * Time.deltaTime * currentDir;
    }

    private void PlayerSkillMove()
    {
        if (_dirX == 0 && _dirZ == 0)
        {
            DecreaseTime();
            return;
        }

        IncreaseTime();
    }

    //대기일때 상태 변이
    private void IdleStateChange()
    {
        if (_dirX != 0 || _dirZ != 0)
        {
            _characterStateType = CHARACTER.MOVE;
            return;
        }
        TrySkill();
    }

    // 스킬이 끝난 다음의 상태 변이
    private void SkillStateChange()
    {
        foreach (var skill in _skills)
        {
            if (!skill.isSkillUsing)
            {
                continue;
            }

            if (skill.EndSkill())
            {
                _characterStateType = CHARACTER.IDLE;
                return;
            }
        }
    }

    //움직임 상태 변이 
    private void MoveStateChange()
    {
        if (_dirX == 0 && _dirZ == 0)
        {
            _characterStateType = CHARACTER.IDLE;
        }
        TrySkill();
    }

    //임시로 경직
    private void Getinput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _characterStateType = CHARACTER.SPASTICITY;
        }
    }

    //임시 경직 시간
    private IEnumerator SpasticityCoroutine()
    {

        yield return new WaitForSeconds(_spastic);

        _characterStateType = CHARACTER.IDLE;
    }

}

using System.Collections;
using System.Collections.Generic;
using EsperFightersCup;
using UnityEngine;

public class ShockWaveSkillObject : SkillObject
{
    private static ShockWaveSkillData s_shockWaveSkillData;

    [SerializeField]
    private float _range = 3;

    [SerializeField]
    private float _onHitDuration = 0.5f;

    [SerializeField]
    private GameObject[] _firstCasting;

    [SerializeField]
    private GameObject[] _secondCasting;

    [SerializeField]
    [Header("Collider")]
    private Transform _colliderParentTransform;

    [SerializeField]
    private ColliderChecker _collider;

    [SerializeField]
    private GameObject _shockwaveUI;

    [SerializeField]
    [Tooltip("[세로, 가로]")]
    private Vector2 _colliderSize = new Vector2(0.5f, 2);

    private Vector3 _direction = Vector3.right;
    private Vector3 _startPos = Vector3.zero;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        // 사거리
        _range = GetCSVData<float>("Range");
        // 판정 범위 가로 세로
        _colliderSize = new Vector2(GetCSVData<float>("ShapeData_1"), GetCSVData<float>("ShapeData_2"));
        GameObjectUtil.ScaleGameObjects(_firstCasting, new Vector3(_range, 1, _range));

        var colliderScale = new Vector3(_colliderSize.x, 1, _colliderSize.y);
        GameObjectUtil.ScaleGameObjects(_secondCasting, colliderScale);
        _colliderParentTransform.localScale = colliderScale;
        _collider.OnCollision += SetHit;
        LoadShockWaveData();
    }

    public override void SetHit(ObjectBase to)
    {
        var knockBackBuff = AnalyzeBuff(to);
        if (knockBackBuff != null)
        {
            _buffOnCollision[0] = knockBackBuff;
        }

        _buffOnCollision[0].ValueVector3[0] = _direction;
        SetState(State.EndDelay);
        base.SetHit(to);
    }

    protected override IEnumerator OnReadyToUse()
    {
        if(!Author.photonView.IsMine)
        {
            yield break;
        }

        var isCanceled = false;
        Vector3 endPos;

        GameObjectUtil.ActiveGameObjects(_firstCasting);

        yield return new WaitUntil(() =>
        {
            // 우클릭 시 취소
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                isCanceled = true;
            }

            // 시작점 설정
            if (Input.GetMouseButtonDown(0))
            {
                _startPos = SetStartPos();
                if (float.IsPositiveInfinity(_startPos.x))
                {
                    isCanceled = true;
                }

                GameObjectUtil.ActiveGameObjects(_firstCasting, false);
                GameObjectUtil.ActiveGameObjects(_secondCasting);
            }
            // 끝점 설정
            else if (Input.GetMouseButton(0) && _startPos != Vector3.positiveInfinity)
            {
                endPos = GetMousePosition();
                _direction = Vector3.Normalize(endPos - _startPos);
                GameObjectUtil.TranslateGameObjects(_secondCasting, _startPos);
                var rotation = _direction == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(_direction);
                GameObjectUtil.RotateGameObjects(_secondCasting, rotation);
            }
            // 판정 범위 최종 계산
            else if (Input.GetMouseButtonUp(0))
            {
                return true;
            }

            return isCanceled;
        });

        if (isCanceled)
        {
            SetState(State.Canceled);
            yield break;
        }

        GameObjectUtil.ActiveGameObjects(_firstCasting, false);
        GameObjectUtil.ActiveGameObjects(_secondCasting, false);
        _shockwaveUI.SetActive(true);

        _shockwaveUI.transform.SetParent(GameObject.Find("UiObject").transform);

        _shockwaveUI.transform.SetPositionAndRotation(_startPos + _direction, _secondCasting[0].transform.rotation);
        _shockwaveUI.transform.localScale = _secondCasting[0].transform.localScale * 0.1f;
        SetNextState();
    }

    protected override IEnumerator OnFrontDelay()
    {
        ApplyMovementSpeed(State.FrontDelay);
        //충격파 애니메이션
        _player.Animator.SetTrigger("ShockWaveSkill");
        ParticleManager.Instance.PullParticle("ShockWave", _startPos - (_direction * 2), Quaternion.LookRotation(_direction));

        yield return new WaitForSeconds(FrontDelayMilliseconds / 1000.0f);
        SetNextState();
    }

    protected override IEnumerator OnUse()
    {
        if(!Author.photonView.IsMine)
        {
            ParticleManager.Instance.PullParticle("ShockWaveHand", _startPos, Quaternion.LookRotation(_direction));
            yield break;
        }

        ApplyMovementSpeed(State.Use);
        _colliderParentTransform.SetPositionAndRotation(_startPos, Quaternion.LookRotation(_direction));
        _colliderParentTransform.gameObject.SetActive(true);
        _colliderParentTransform.transform.position = _startPos;
        yield return new WaitUntil(() => WaitPhysicsUpdate());
        ParticleManager.Instance.PullParticle("ShockWaveHand", _startPos, Quaternion.LookRotation(_direction));

        _colliderParentTransform.gameObject.SetActive(false);
        SetNextState();
    }

    protected override IEnumerator OnEndDelay()
    {
        ApplyMovementSpeed(State.EndDelay);
        yield return new WaitForSeconds(EndDelayMilliseconds / 1000.0f);
        SetNextState();
    }

    protected override IEnumerator OnCanceled()
    {
        ApplyMovementSpeed(State.Canceled);
        SetState(State.Release);
        yield return null;
    }

    protected override IEnumerator OnRelease()
    {
        ApplyMovementSpeed(State.Release);
        Destroy(_shockwaveUI);
        Destroy(gameObject);
        yield return null;
    }

    private Vector3 SetStartPos()
    {
        var startPos = GetMousePosition();
        if (Vector3.Distance(startPos, transform.position) > _range)
        {
            return Vector3.positiveInfinity;
        }

        return startPos;
    }

    private Vector3 GetMousePosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray);
        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Floor"))
            {
                return hit.point;
            }
        }

        return Vector3.positiveInfinity;
    }

    protected override void OnHit(ObjectBase from, ObjectBase to, BuffObject.BuffStruct[] appendBuff)
    {
    }

    public override void OnPlayerHitEnter(GameObject other)
    {
    }

    private void LoadShockWaveData()
    {
        if (s_shockWaveSkillData != null)
        {
            return;
        }

        var csvData = CSVUtil.GetData("ShockWaveKnockBackDataTable");
        s_shockWaveSkillData = new ShockWaveSkillData();
        csvData.Get("TargetObject_ID", out s_shockWaveSkillData._idList);
        csvData.Get("FloatCheck", out s_shockWaveSkillData._floatCheckList);
        csvData.Get("ShockWave_MoveSpeed", out s_shockWaveSkillData._moveSpeedList);
        csvData.Get("ShockWave_MoveTime", out s_shockWaveSkillData._moveTimeList);
        csvData.Get("Damage", out s_shockWaveSkillData._damageList);
        csvData.Get("Groggy_Duration", out s_shockWaveSkillData._stunDurationList);
    }

    private BuffObject.BuffStruct AnalyzeBuff(ObjectBase to)
    {
        var target = to as Actor;
        if (target == null)
        {
            return null;
        }

        var targetID = target.ID;
        var isRaised = target.BuffController.ActiveBuffs.Exists(BuffObject.Type.Raise);
        var idList = s_shockWaveSkillData._idList;
        var floatCheckList = s_shockWaveSkillData._floatCheckList;
        var moveSpeedList = s_shockWaveSkillData._moveSpeedList;
        var moveTimeList = s_shockWaveSkillData._moveTimeList;
        var damageList = s_shockWaveSkillData._damageList;
        var stunDurationList = s_shockWaveSkillData._stunDurationList;

        for (var i = 0; i < idList.Count; ++i)
        {
            var id = (int)idList[i];
            // CSV의 타겟 ID가 아니면 확인할 필요가 없으니 1차적으로 필터링
            if (id != targetID)
            {
                continue;
            }

            var raiseCheck = floatCheckList[i] > 0;
            // CSV의 띄워짐 상태와 오브젝트의 띄워짐 상태를 확인하여 2차 필터링
            if (raiseCheck != isRaised)
            {
                continue;
            }

            var result = new BuffObject.BuffStruct
            {
                Type = BuffObject.Type.KnockBack,
                AllowDuplicates = false,
                Duration = moveTimeList[i] / 1000.0f,
                ValueFloat = new[]
                {
                    moveSpeedList[i],
                    damageList[i],
                    stunDurationList[i] / 1000.0f
                },
                ValueVector3 = new Vector3[1]
            };
            return result;
        }

        return null;
    }

    private class ShockWaveSkillData
    {
        public List<float> _damageList;
        public List<float> _floatCheckList;
        public List<float> _idList;
        public List<float> _moveSpeedList;
        public List<float> _moveTimeList;
        public List<float> _stunDurationList;
    }
}

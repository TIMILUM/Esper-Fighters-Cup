using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using EsperFightersCup;
using UnityEngine;

public class ShockWaveSkillObject : SkillObject
{
    private class ShockWaveSkillData
    {
        public List<int> TargetID { get; set; }
        public List<bool> Raise { get; set; }
        public List<int> MoveSpeed { get; set; }
        public List<int> MoveTime { get; set; }
        public List<int> Damage { get; set; }
        public List<int> StunDuration { get; set; }
    }

    [SerializeField] private float _onHitDuration = 0.5f;
    [SerializeField] private GameObject _shockwaveUI;
    [SerializeField] private GameObject[] _firstCasting;
    [SerializeField] private GameObject[] _secondCasting;
    [Header("Collider")]
    [SerializeField] private Transform _colliderParentTransform;
    [SerializeField] private ColliderChecker _collider;

    private ShockWaveSkillData _data;
    private Vector3 _startPos = Vector3.zero;
    private Vector3 _direction = Vector3.right;

    public override void SetHit(ObjectBase to)
    {
        var knockBackBuff = AnalyzeBuff(to);
        if (knockBackBuff is null)
        {
            return;
        }
        knockBackBuff.ValueVector3[0] = _direction;
        base.OnHit(this, to, new[] { knockBackBuff });
    }

    protected override void OnIntializeSkill()
    {
        base.OnIntializeSkill();
        LoadShockWaveData();

        var colliderScale = new Vector3(Size.x, 1, Size.y);
        GameObjectUtil.ScaleGameObjects(_secondCasting, colliderScale);
        _colliderParentTransform.localScale = colliderScale;
    }

    protected override async UniTask<bool> OnReadyToUseAsync(CancellationToken cancellation)
    {
        bool isCanceled = false;
        Vector3 endPos;

        GameObjectUtil.ScaleGameObjects(_firstCasting, new Vector3(Range, 1, Range));
        GameObjectUtil.ActiveGameObjects(_firstCasting);

        await UniTask.WaitUntil(() =>
        {
            // 우클릭 시 취소
            if (Input.GetMouseButtonDown(1))
            {
                isCanceled = true;
            }
            else
            {
                // 시작점 설정
                if (Input.GetMouseButtonDown(0))
                {
                    _startPos = SetStartPos();
                    if (float.IsPositiveInfinity(_startPos.x))
                    {
                        isCanceled = true;
                    }
                    else
                    {
                        GameObjectUtil.ActiveGameObjects(_firstCasting, false);
                        GameObjectUtil.ActiveGameObjects(_secondCasting);
                    }
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
            }

            return isCanceled;

        }, cancellationToken: cancellation);

        if (isCanceled)
        {
            return false;
        }

        GameObjectUtil.ActiveGameObjects(_firstCasting, false);
        GameObjectUtil.ActiveGameObjects(_secondCasting, false);

        return true;
    }

    protected override void BeforeFrontDelay()
    {
        var pos = _startPos + _direction;
        var rot = _secondCasting[0].transform.rotation;
        var scale = _secondCasting[0].transform.localScale * 0.1f;

        _shockwaveUI.SetActive(true);
        _shockwaveUI.transform.SetParent(GameObject.Find("UiObject").transform);

        _shockwaveUI.transform.SetPositionAndRotation(pos, rot);
        _shockwaveUI.transform.localScale = scale;

        //충격파 애니메이션
        AuthorPlayer.Animator.SetTrigger("ShockWaveSkill");
    }

    protected override async UniTask OnUseAsync()
    {
        ParticleManager.Instance.PullParticle("ShockWave", _startPos - (_direction * 2), Quaternion.LookRotation(_direction));
        ParticleManager.Instance.PullParticle("ShockWaveHand", _startPos, Quaternion.LookRotation(_direction));

        _colliderParentTransform.SetPositionAndRotation(_startPos, Quaternion.LookRotation(_direction));
        _collider.OnCollision += SetHit;
        _colliderParentTransform.transform.position = _startPos;
        _colliderParentTransform.gameObject.SetActive(true);

        await UniTask.Yield();

        _colliderParentTransform.gameObject.SetActive(false);
        _collider.OnCollision -= SetHit;
    }

    protected override void BeforeEndDelay()
    {
    }

    protected override void OnCancel()
    {
        ReleaseShockWave();
    }

    protected override void OnRelease()
    {
        ReleaseShockWave();
    }

    private void ReleaseShockWave()
    {
        // 중간에 스킬이 취소되어서 미처 제거하지 못한 코드들 여기서 제거
        GameObjectUtil.ActiveGameObjects(_firstCasting, false);
        GameObjectUtil.ActiveGameObjects(_secondCasting, false);
        _shockwaveUI.SetActive(false);
        _colliderParentTransform.gameObject.SetActive(false);
    }

    private Vector3 SetStartPos()
    {
        var startPos = GetMousePosition();
        if (Vector3.Distance(startPos, transform.position) > Range)
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

    private void LoadShockWaveData()
    {
        if (_data != null)
        {
            return;
        }

        var csvData = CSVUtil.GetData("ShockWaveKnockBackDataTable");
        _data = new ShockWaveSkillData();

        if (csvData.Get<int>("ShockWave_Data_ID", out var targetIds))
        {
            _data.TargetID = targetIds;
        }
        if (csvData.Get<int>("FloatCheck", out var floatChecks))
        {
            _data.Raise = floatChecks.Select(x => x > 0).ToList();
        }
        if (csvData.Get<int>("ShockWave_MoveSpeed", out var moveSpeeds))
        {
            _data.MoveSpeed = moveSpeeds;
        }
        if (csvData.Get<int>("ShockWave_MoveTime", out var moveTimes))
        {
            _data.MoveTime = moveTimes;
        }
        if (csvData.Get<int>("Damage", out var damages))
        {
            _data.Damage = damages;
        }
        if (csvData.Get<int>("Groggy_Duration", out var durations))
        {
            _data.StunDuration = durations;
        }
    }

    private BuffObject.BuffStruct AnalyzeBuff(ObjectBase to)
    {
        if (!(to is Actor target))
        {
            return null;
        }

        var targetID = target.ID;
        var targetIsRaised = target.BuffController.ActiveBuffs.Exists(BuffObject.Type.Raise);

        for (int i = 0; i < _data.TargetID.Count; i++)
        {
            var id = _data.TargetID[i];
            var raise = _data.Raise[i];
            var movespeed = _data.MoveSpeed[i];
            var movetime = _data.MoveTime[i];
            var damage = _data.Damage[i];
            var stunDuration = _data.StunDuration[i];

            // CSV의 타겟 ID가 아니면 확인할 필요가 없으니 1차적으로 필터링
            if (targetID != id)
            {
                continue;
            }
            // CSV의 띄워짐 상태와 오브젝트의 띄워짐 상태를 확인하여 2차 필터링
            if (targetIsRaised != raise)
            {
                continue;
            }

            var result = new BuffObject.BuffStruct
            {
                Type = BuffObject.Type.KnockBack,
                AllowDuplicates = false,
                Duration = movetime,
                ValueFloat = new float[] // TOOD: ValueFloat 대신 ValueInt가 돼야 할듯
                {
                    movespeed,
                    damage,
                    stunDuration
                },
                ValueVector3 = new Vector3[1]
            };
            return result;
        }

        return null;
    }
}

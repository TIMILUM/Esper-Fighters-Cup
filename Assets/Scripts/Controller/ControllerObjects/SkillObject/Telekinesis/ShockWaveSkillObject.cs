using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using EsperFightersCup;
using EsperFightersCup.UI.InGame.Skill;
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

    [Header("Collider")]
    [SerializeField] private ColliderChecker _collider;

    private ShockWaveSkillData _data;
    private SkillUI _castUI;
    private Vector3 _startPos = Vector3.zero;
    private Vector3 _direction = Vector3.right;
    private Vector2 _uiSize;

    public override void SetHit(ObjectBase to)
    {
        // TODO: 충격파 대쉬 적용
        var knockBackBuff = AnalyzeBuff(to);
        if (knockBackBuff is null)
        {
            return;
        }
        knockBackBuff.ValueVector3[0] = _direction;

        _buffOnCollision.Clear();
        _buffOnCollision.Add(knockBackBuff);

        base.SetHit(to);
    }

    protected override void OnInitializeSkill()
    {
        base.OnInitializeSkill();
        LoadShockWaveData();

        _uiSize = new Vector2(Size.y, Size.y) * 0.1f;

        _collider.transform.SetParent(null);
        GameObjectUtil.ScaleGameObject(_collider.gameObject, new Vector3(Size.x, 50, Size.y));
        _collider.OnCollision += SetHit;
    }

    protected override async UniTask<bool> OnReadyToUseAsync(CancellationToken cancellation)
    {
        if (_castUI == null)
        {
            print(Author);
            _castUI = GameUIManager.Instance.PlayLocal(Author, "ShockWave_Arrow", transform.position, _uiSize);
            GameObjectUtil.ActiveGameObject(_castUI.gameObject, false);
        }

        bool isCanceled = false;
        Vector3 endPos;

        _startPos = SetStartPos();
        if (float.IsPositiveInfinity(_startPos.x))
        {
            return false;
        }
        GameObjectUtil.ActiveGameObject(_castUI.gameObject, true);
        GameObjectUtil.TranslateGameObject(_castUI.gameObject, _startPos);

        await UniTask.WaitUntil(() =>
        {
            // 우클릭 시 취소
            if (Input.GetMouseButtonDown(1))
            {
                isCanceled = true;
            }
            // 끝점 설정
            else if (Input.GetMouseButton(0) && _startPos != Vector3.positiveInfinity)
            {
                endPos = GetMousePosition();
                _direction = Vector3.Normalize(endPos - _startPos);

                var rotation = _direction == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(_direction);
                GameObjectUtil.RotateGameObject(_castUI.gameObject, rotation);
            }
            // 판정 범위 최종 계산
            else if (Input.GetMouseButtonUp(0))
            {
                return true;
            }

            return isCanceled;

        }, cancellationToken: cancellation);

        GameObjectUtil.ActiveGameObject(_castUI.gameObject, false);
        return !isCanceled;
    }

    protected override void BeforeFrontDelay()
    {
        var pos = _castUI.transform.position;
        var rot = _castUI.transform.rotation.eulerAngles;

        GameUIManager.Instance.PlaySync(Author, "ShockWave_Range", new Vector2(pos.x, pos.z), _uiSize, rot.y, 0.5f);

        //충격파 애니메이션
        AuthorPlayer.Animator.SetTrigger("ShockWaveSkill");
        ParticleManager.Instance.PullParticleAttachedSync("Elena_ShockWave_Hand_Waver", 0);
    }

    protected override async UniTask OnUseAsync()
    {
        ParticleManager.Instance.PullParticleSync("Elena_ShockWave", _startPos + _direction + (Vector3.up * 1f), Quaternion.LookRotation(_direction));

        _collider.transform.SetPositionAndRotation(_castUI.transform.GetChild(0).position, Quaternion.LookRotation(_direction));

        GameObjectUtil.ActiveGameObject(_collider.gameObject, true);
        await UniTask.DelayFrame(3);
        GameObjectUtil.ActiveGameObject(_collider.gameObject, false);
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
        GameObjectUtil.ActiveGameObject(_castUI.gameObject, false);
        _collider.gameObject.SetActive(false);
    }

    private Vector3 SetStartPos()
    {
        var startPos = GetMousePosition();
        /*
        if (Vector3.Distance(startPos, transform.position) > Range)
        {
            return Vector3.positiveInfinity;
        }
        */
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

        if (csvData.Get<float>("ShockWave_Data_ID", out var targetIds))
        {
            _data.TargetID = targetIds.Select(x => (int)x).ToList();
        }
        if (csvData.Get<float>("FloatCheck", out var floatChecks))
        {
            _data.Raise = floatChecks.Select(x => x > 0).ToList();
        }
        if (csvData.Get<float>("ShockWave_MoveSpeed", out var moveSpeeds))
        {
            _data.MoveSpeed = moveSpeeds.Select(x => (int)x).ToList();
        }
        if (csvData.Get<float>("ShockWave_MoveTime", out var moveTimes))
        {
            _data.MoveTime = moveTimes.Select(x => (int)x).ToList();
        }
        if (csvData.Get<float>("Damage", out var damages))
        {
            _data.Damage = damages.Select(x => (int)x).ToList();
        }
        if (csvData.Get<float>("Groggy_Duration", out var durations))
        {
            _data.StunDuration = durations.Select(x => (int)x).ToList();
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
                Duration = movetime * 0.001f,
                ValueFloat = new float[]
                {
                    movespeed,
                    damage,
                    stunDuration * 0.001f
                },
                ValueVector3 = new Vector3[1]
            };

            return result;
        }

        return null;
    }
}

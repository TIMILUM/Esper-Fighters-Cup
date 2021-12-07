using System.Threading;
using Cysharp.Threading.Tasks;
using EsperFightersCup;
using UnityEngine;

public class PunchSkillObject : SkillObject
{
    [SerializeField, Tooltip("Skill_Effect_Data_1의 값")]
    private float _moveSpeed = 1.0f;

    [SerializeField]
    private ColliderChecker _colliderChecker;

    [SerializeField]
    private Transform _colliderTransform;

    private Vector3 _direction = Vector3.zero;


    protected override void OnInitializeSkill()
    {
        base.OnInitializeSkill();
        _colliderTransform.localScale = new Vector3(Size.x, 10, Size.y);
        _colliderChecker.gameObject.SetActive(false);
        _colliderChecker.OnCollision += SetHit;
        _moveSpeed = EffectData.x;
    }

    public override void SetHit(ObjectBase to)
    {
        var target = AnalyzeTargetObject(to);
        var knockBackBuff = new BuffObject.BuffStruct
        {
            Type = BuffObject.Type.KnockBack,
            Damage = 0,
            Duration = EffectDuration / SkillSpeed,
            AllowDuplicates = false,
            ValueVector3 = new[] { _direction },
            ValueFloat = new[] { SkillSpeed, 0, 0 },
            IsOnlyOnce = false,
        };

        if (target is null)
        {
            return;
        }

        _buffOnCollision.Clear();

        // 만약 부딫힌 오브젝트가 파괴되지 않고 밀려나는 경우
        if (target == to)
        {
            _buffOnCollision.Add(new BuffObject.BuffStruct
            {
                Type = BuffObject.Type.DecreaseHp,
                Damage = Damage,
                IsOnlyOnce = true
            });
            _buffOnCollision.Add(knockBackBuff);
        }
        else
        {
            knockBackBuff.Damage = Damage;
            var windLoadingObject = target as WindLoadingObject;
            windLoadingObject.SetBuffStack(new[] { knockBackBuff });
        }

        base.SetHit(to);
        AuthorPlayer.Animator.SetTrigger("Punch");
    }

    protected override async UniTask<bool> OnReadyToUseAsync(CancellationToken cancellation)
    {
        // await UniTask.WaitUntil(() =>
        // {
        //
        // }, cancellationToken: cancellation);

        Vector3 endPos = GetMousePosition();
        _direction = Vector3.Normalize(endPos - transform.position);


        await UniTask.Yield();
        return true;
    }

    protected override void BeforeFrontDelay()
    {
        SfxManager.Instance.PlaySFXSync("Punch", Author.transform.position);
    }

    protected override async UniTask OnUseAsync()
    {
        AuthorPlayer.Animator.SetTrigger("Tackle");
        var duration = Range / _moveSpeed;
        BuffController.GenerateBuff(new BuffObject.BuffStruct
        {
            Type = BuffObject.Type.KnockBack,
            Damage = 0,
            Duration = duration,
            AllowDuplicates = true,
            ValueVector3 = new[] { _direction },
            ValueFloat = new[] { _moveSpeed, 0, 0 },
            IsOnlyOnce = false,
        });

        _colliderChecker.gameObject.SetActive(true);

        await UniTask.WaitUntil(() => BuffController.ActiveBuffs[BuffObject.Type.KnockBack].Count <= 0);
    }

    protected override void BeforeEndDelay()
    {
        _colliderChecker.gameObject.SetActive(false);
    }

    protected override void OnRelease()
    {
    }

    protected override void OnCancel()
    {
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

    private ObjectBase AnalyzeTargetObject(ObjectBase to)
    {
        if (!(to is Actor target))
        {
            return null;
        }

        var targetHitSystem = to.GetComponent<ObjectHitSystem>();
        if (targetHitSystem == null)
        {
            return null;
        }

        var hitSystem = Author.GetComponent<ObjectHitSystem>();


        ParticleManager.Instance.PullParticleSync("Plank_Punch_Swing", Author.transform.position + Author.transform.right,
            Author.transform.rotation);
        ParticleManager.Instance.PullParticleSync("Plank_Punch_Swing", Author.transform.position - Author.transform.right,
            Author.transform.rotation);

        // 오브젝트가 파괴될 경우 풍압 오브젝트를 생성하여 날림
        if (targetHitSystem.Strength <= hitSystem.Strength && targetHitSystem.IsDestroyable)
        {
            var obj = InGameSkillManager.Instance.CreateSkillObject("WindLoadingObject", to.transform.position + (_direction * 1),
                Author.transform.rotation);
            targetHitSystem.Hit(Author.gameObject);
            obj.transform.rotation = Quaternion.LookRotation(_direction);
            obj.transform.localScale = new Vector3(EffectSize.x, 1, EffectSize.y);
            return obj.GetComponent<ObjectBase>();
        }

        return to;
    }
}

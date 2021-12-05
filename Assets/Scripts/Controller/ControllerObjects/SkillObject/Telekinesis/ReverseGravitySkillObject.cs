using System.Threading;
using Cysharp.Threading.Tasks;
using EsperFightersCup;
using EsperFightersCup.UI;
using UnityEngine;

public class ReverseGravitySkillObject : SkillObject
{
    [SerializeField] private ColliderChecker _collider;
    // [SerializeField] private float _fragmentRaiseHeight = 5f;
    [SerializeField] private float _objectRaiseHeight = 3f;

    // milliseconds
    private float _raiseDelay;
    // milliseconds
    private float _raiseTime;

    private Vector2 _uiSize;
    private SkillUI _rangeUI;
    private SkillUI _castUI;

    public override void SetHit(ObjectBase to)
    {
        var newBuff = new BuffObject.BuffStruct
        {
            Type = BuffObject.Type.Raise,
            Duration = _raiseTime,
            ValueFloat = new[] { _objectRaiseHeight }
        };

        _buffOnCollision.Clear();
        _buffOnCollision.Add(newBuff);

        base.SetHit(to);
    }

    protected override void OnInitializeSkill()
    {
        base.OnInitializeSkill();
        LoadSkillData();

        _uiSize = Size * 0.1f;

        _collider.gameObject.SetActive(false);
        _collider.transform.SetParent(null);
        GameObjectUtil.ScaleGameObject(_collider.gameObject, new Vector3(Size.x * 0.5f, 5, Size.y * 0.5f));
        _collider.OnCollision += SetHit;
    }

    protected override async UniTask<bool> OnReadyToUseAsync(CancellationToken cancellation)
    {
        if (_rangeUI == null)
        {
            // 범위 UI의 반지름 = Range
            var rangeSize = new Vector2(Range, Range) * 2f;
            _rangeUI = GameUIManager.Instance.PlayLocal(Author, "Skill_Range", transform.position, rangeSize * 0.1f);
            GameObjectUtil.ActiveGameObject(_rangeUI.gameObject, false);
        }
        if (_castUI == null)
        {
            _castUI = GameUIManager.Instance.PlayLocal(Author, "ReverseGravity_Casting", transform.position, _uiSize);
            GameObjectUtil.ActiveGameObject(_castUI.gameObject, false);
        }

        var isCanceled = false;

        _rangeUI.ChangeTarget(AuthorPlayer.photonView.ViewID);
        GameObjectUtil.ActiveGameObject(_rangeUI.gameObject, true);

        await UniTask.WaitUntil(() =>
        {
            var mousePos = GetMousePosition();
            var distance = Vector3.Distance(Author.transform.position, mousePos);

            if (distance < Range)
            {
                if (!_castUI.gameObject.activeInHierarchy)
                {
                    GameObjectUtil.ActiveGameObject(_castUI.gameObject, true);
                }
                GameObjectUtil.TranslateGameObject(_castUI.gameObject, mousePos);
            }
            else
            {
                GameObjectUtil.ActiveGameObject(_castUI.gameObject, false);
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                isCanceled = true;
            }
            else if (Input.GetKeyUp(InputKey))
            {
                if (distance > Range)
                {
                    isCanceled = true;
                }
                return true;
            }
            return isCanceled;

        }, cancellationToken: cancellation);

        GameObjectUtil.ActiveGameObject(_rangeUI.gameObject, false);
        GameObjectUtil.ActiveGameObject(_castUI.gameObject, false);

        return !isCanceled;
    }

    protected override void BeforeFrontDelay()
    {
        ParticleManager.Instance.PullParticleAttachedSync("Elena_ShockWave_Hand_Waver", 0);

        //Idle 상태일때 애니메이션 실행
        if (AuthorPlayer.Animator.Local.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            AuthorPlayer.Animator.SetTrigger("ReverseGravityUnder");
        }

        //하체는 그냥 실행
        AuthorPlayer.Animator.SetTrigger("ReverseGravityA");

        ParticleManager.Instance.PullParticleSync("Elena_ReverseGravity", _castUI.transform.position, Quaternion.identity);

        var duration = (FrontDelayMilliseconds + EndDelayMilliseconds) * 0.001f;
        GameUIManager.Instance.PlaySync(Author, "ReverseGravity_Range", _castUI.Position, _uiSize, duration: duration);
    }

    protected override async UniTask OnUseAsync()
    {
        InstantiateStoneAsync(_castUI.transform.position).Forget();
        await UniTask.Yield();
    }

    protected override void BeforeEndDelay()
    {
    }

    protected override void OnRelease()
    {
        ReleaseObjects();
    }

    protected override void OnCancel()
    {
        ReleaseObjects();
    }

    private async UniTask InstantiateStoneAsync(Vector3 position)
    {
        var fragment = InGameSkillManager.Instance.CreateSkillObject("Fragment", position);
        // _lineRendererObj.SetActive(true);

        if (fragment == null)
        {
            return;
        }

        await UniTask.DelayFrame(3);
        if (fragment.TryGetComponent<AStaticObject>(out var staticObject))
        {
            staticObject.BuffController.GenerateBuff(new BuffObject.BuffStruct
            {
                Type = BuffObject.Type.Raise,
                Duration = _raiseTime,
                ValueFloat = new[] { _objectRaiseHeight + 2f }
            });
        }

        /*
        var stoneGathering = fragment.transform.Find("stone_gathering");
        while (stoneGathering.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("CreateRock"))
        {
            //_lineRenderer.SetPosition(0, _fragmentUI.transform.position);
            //_lineRenderer.SetPosition(1, AuthorPlayer.transform.position);
            await UniTask.Yield();
        }
        */
        await UniTask.Delay((int)_raiseDelay);
        ParticleManager.Instance.PullParticleSync("Elena_ReverseGravity_Glow", position, Quaternion.identity);

        // 혹시 해당 비동기메소드가 여러번 실행될 경우 콜라이더 체크가 중간에 취소되는 일 없게 lock 적용
        Monitor.Enter(_collider);
        // _fragment = fragment;
        GameObjectUtil.TranslateGameObject(_collider.gameObject, position);
        GameObjectUtil.ActiveGameObject(_collider.gameObject, true);
        await UniTask.DelayFrame(3);
        GameObjectUtil.ActiveGameObject(_collider.gameObject, false);
        Monitor.Exit(_collider);
    }

    private void ReleaseObjects()
    {
        GameObjectUtil.ActiveGameObject(_collider.gameObject, false);
        GameObjectUtil.ActiveGameObject(_rangeUI.gameObject, false);
        GameObjectUtil.ActiveGameObject(_castUI.gameObject, false);
    }

    private void LoadSkillData()
    {
        _raiseDelay = GetCSVData<float>("Skill_Effect_Data_1");
        _raiseTime = GetCSVData<float>("Skill_Effect_Data_2") * 0.001f;
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
}

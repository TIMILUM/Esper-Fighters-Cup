using System.Threading;
using Cysharp.Threading.Tasks;
using EsperFightersCup;
using EsperFightersCup.UI.InGame.Skill;
using UnityEngine;

public class ReverseGravitySkillObject : SkillObject
{
    [SerializeField] private ColliderChecker _collider;
    [SerializeField] private LineRenderer _lineRenderer;

    private Vector2 _uiSize;
    private SkillUI _rangeUI;
    private SkillUI _castUI;

    private GameObject _fragmentObj;
    private GameObject _fragmentUI;

    private Vector3 _endMousePos;

    public override void SetHit(ObjectBase to)
    {
        if (_fragmentObj != null && _fragmentObj.GetComponent<ObjectBase>() == to)
        {
            return;
        }
        base.SetHit(to);
    }

    protected override void OnInitializeSkill()
    {
        base.OnInitializeSkill();

        _uiSize = Size * 0.1f;
        var rangeSize = 0.1f * Range * Vector2.one;
        _rangeUI = GameUIManager.Instance.PlayLocal("Skill_Range", transform.position, 0f, rangeSize);
        _castUI = GameUIManager.Instance.PlayLocal("ReverseGravity_Casting", transform.position, 0f, _uiSize);

        GameObjectUtil.ActiveGameObject(_rangeUI.gameObject, false);
        GameObjectUtil.ActiveGameObject(_castUI.gameObject, false);

        _collider.transform.SetParent(null);
        _collider.OnCollision += SetHit;
    }

    protected override async UniTask<bool> OnReadyToUseAsync(CancellationToken cancellation)
    {
        var isCanceled = false;

        _rangeUI.ChangeTarget(AuthorPlayer.photonView.ViewID);
        GameObjectUtil.ActiveGameObject(_rangeUI.gameObject, true);

        await UniTask.WaitUntil(() =>
        {
            var mousePos = GetMousePosition();
            GameObjectUtil.TranslateGameObject(_castUI.gameObject, mousePos);

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                isCanceled = true;
                return isCanceled;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                /*
                _fragmentUI = InGameSkillManager.Instance.CreateSkillUI("ThrowUI", _fragmentCasting.transform.position);
                _fragmentCasting.transform.SetParent(null);
                _fragmentUI.transform.localScale = _fragmentCasting.transform.localScale;
                _fragmentCasting.transform.SetParent(transform);
                _fragmentUI.transform.SetParent(GameObject.Find("UiObject").transform);
                */
                _endMousePos = mousePos;
                return true;
            }
            return isCanceled;

        }, cancellationToken: cancellation);

        GameObjectUtil.ActiveGameObject(_rangeUI.gameObject, false);
        GameObjectUtil.ActiveGameObject(_castUI.gameObject, false);
        return false;
    }

    protected override void BeforeFrontDelay()
    {
        //Idle 상태일때 애니메이션 실행
        if (AuthorPlayer.Animator.Local.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            AuthorPlayer.Animator.SetTrigger("ReverseGravityUnder");
        }

        //하체는 그냥 실행
        AuthorPlayer.Animator.SetTrigger("ReverseGravityA");
    }

    protected override async UniTask OnUseAsync()
    {
        _fragmentObj = InGameSkillManager.Instance.CreateSkillObject("Stone", _endMousePos);
        GameObjectUtil.ActiveGameObject(_collider.gameObject, false);

        _collider.OnCollision += SetHit;
        await UniTask.Yield();
        _collider.OnCollision -= SetHit;
    }

    protected override void BeforeEndDelay()
    {
        GameObjectUtil.ActiveGameObject(_collider.gameObject);
        GameObjectUtil.TranslateGameObject(_collider.gameObject, _castUI.transform.position);
    }

    protected override void OnRelease()
    {
        InGameSkillManager.Instance.DestroySkillObj(_fragmentUI);
        ReleaseObjects();
    }

    protected override void OnCancel()
    {
        ReleaseObjects();
    }

    private void ReleaseObjects()
    {
        GameObjectUtil.ActiveGameObject(_rangeUI.gameObject, false);
        GameObjectUtil.ActiveGameObject(_castUI.gameObject, false);
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

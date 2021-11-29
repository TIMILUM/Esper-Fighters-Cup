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

    //private GameObject _fragmentObj;
    //private GameObject _fragmentUI;

    private Vector3 _endMousePos;

    public override void SetHit(ObjectBase to)
    {
        /*
        if (_fragmentObj != null && _fragmentObj.GetComponent<ObjectBase>() == to)
        {
            return;
        }
        */
        base.SetHit(to);
    }

    protected override void OnInitializeSkill()
    {
        base.OnInitializeSkill();

        _uiSize = Size * 0.1f;

        // 범위 UI의 반지름 = Range
        var rangeSize = new Vector2(Range, Range) * 2f;
        _rangeUI = GameUIManager.Instance.PlayLocal("Skill_Range", transform.position, 0f, rangeSize * 0.1f);
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
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                if (distance <= Range)
                {
                    /*
                    _fragmentUI = InGameSkillManager.Instance.CreateSkillUI("ThrowUI", _fragmentCasting.transform.position);
                    _fragmentCasting.transform.SetParent(null);
                    _fragmentUI.transform.localScale = _fragmentCasting.transform.localScale;
                    _fragmentCasting.transform.SetParent(transform);
                    _fragmentUI.transform.SetParent(GameObject.Find("UiObject").transform);
                    */
                    _endMousePos = mousePos;
                }
                else
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
        // _fragmentObj = InGameSkillManager.Instance.CreateSkillObject("Stone", _endMousePos);
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
        // InGameSkillManager.Instance.DestroySkillObj(_fragmentUI);
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

using System.Threading;
using Cysharp.Threading.Tasks;
using EsperFightersCup;
using UnityEngine;

public class ThrowSkillObject : SkillObject
{
    [SerializeField] private GameObject _fragmentCasting;
    [SerializeField] private GameObject _hitBox;
    [SerializeField] private ColliderChecker _collider;

    private GameObject _fragmentObj;
    private GameObject _fragmentUI;

    private Vector3 _endMousePos;

    protected override void OnIntializeSkill()
    {
        base.OnIntializeSkill();
        GameObjectUtil.ScaleGameObject(_fragmentCasting, new Vector3(Range * 2.0f, 1.0f, Range * 2.0f));
        GameObjectUtil.ScaleGameObject(_hitBox, new Vector3(Range * 2.0f, 1.0f, Range * 2.0f));
    }

    public override void SetHit(ObjectBase to)
    {
        if (_fragmentObj != null && _fragmentObj.GetComponent<ObjectBase>() == to)
        {
            return;
        }
        base.SetHit(to);
    }

    protected override async UniTask<bool> OnReadyToUseAsync(CancellationToken cancellation)
    {
        var isCanceled = false;

        GameObjectUtil.ActiveGameObject(_fragmentCasting);

        await UniTask.WaitUntil(() =>
        {
            var mousePos = GetMousePosition();
            GameObjectUtil.TranslateGameObject(_fragmentCasting, mousePos);

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                isCanceled = true;
                return isCanceled;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                _fragmentUI = InGameSkillManager.Instance.CreateSkillUI("ThrowUI", _fragmentCasting.transform.position);
                _fragmentCasting.transform.SetParent(null);
                _fragmentUI.transform.localScale = _fragmentCasting.transform.localScale;
                _fragmentCasting.transform.SetParent(transform);
                _fragmentUI.transform.SetParent(GameObject.Find("UiObject").transform);
                _endMousePos = mousePos;
                return true;
            }
            return isCanceled;

        }, cancellationToken: cancellation);

        GameObjectUtil.ActiveGameObject(_fragmentCasting, false);
        return isCanceled;
    }

    protected override void BeforeFrontDelay()
    {
        //Idle 상태일때 애니메이션 실행
        if (AuthorPlayer.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            AuthorPlayer.Animator.SetTrigger("ReverseGravityUnder");
        }

        //하체는 그냥 실행
        AuthorPlayer.Animator.SetTrigger("ReverseGravityA");
    }

    protected override async UniTask OnUseAsync()
    {
        _fragmentObj = InGameSkillManager.Instance.CreateSkillObject("Stone", _endMousePos);
        GameObjectUtil.ActiveGameObject(_hitBox, false);

        _collider.OnCollision += SetHit;
        await UniTask.Yield();
        _collider.OnCollision -= SetHit;
    }

    protected override void BeforeEndDelay()
    {
        GameObjectUtil.ActiveGameObject(_hitBox);
        GameObjectUtil.TranslateGameObject(_hitBox, _fragmentCasting.transform.position);
    }

    protected override void OnRelease()
    {
        InGameSkillManager.Instance.DestroySkillObj(_fragmentUI);
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
}

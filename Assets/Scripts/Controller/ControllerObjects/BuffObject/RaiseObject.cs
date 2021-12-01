using DG.Tweening;
using UnityEngine;

public class RaiseObject : BuffObject
{
    private float _limitPosY;
    private Sequence _raising;

    public override Type BuffType => Type.Raise;

    public override void OnBuffGenerated()
    {
        // BuffStruct Help
        // ----------------
        // ValueFloat[0] : limitPosY (0이면 스턴 효과 없음)
        // ----------------
        _limitPosY = Info.ValueFloat[0];

        if (Author.photonView.IsMine)
        {
            // useGravity 동기화됨
            if (Author is APlayer)
            {
                Author.Rigidbody.useGravity = false;
            }
            var startPos = Author.Rigidbody.position;
            _raising = DOTween.Sequence()
                .Append(Author.Rigidbody.DOMove(new Vector3(startPos.x, _limitPosY, startPos.z), Info.Duration * 0.001f))
                .SetLink(gameObject, LinkBehaviour.KillOnDisable)
                .SetEase(Ease.OutCubic)
                .OnUpdate(() =>
                {
                    if (Controller.ActiveBuffs.Exists(Type.KnockBack))
                    {
                        _raising.Kill();
                        Controller.ReleaseBuff(this);
                    }
                });
        }
    }

    public override void OnBuffReleased()
    {
        if (Author.photonView.IsMine)
        {
            _raising.Kill();
            if (Author is APlayer)
            {
                Author.Rigidbody.useGravity = true;
            }
            else
            {
                Controller.GenerateBuff(new BuffStruct()
                {
                    Type = Type.Falling,
                    ValueFloat = new float[2] { 0.0f, 0.0f }
                });
            }
        }
    }

    /*
    private IEnumerator Raise()
    {
        var startTime = Time.time;
        var waitForFixedUpdate = new WaitForFixedUpdate();
        var startPos = Author.Rigidbody.position;
        var endPos = new Vector3(startPos.x, _limitPosY, startPos.z);
        var duration = Info.Duration * 0.001f;

        while (!Controller.ActiveBuffs.Exists(Type.KnockBack))
        {
            var currentTime = Time.time - startTime;
            Author.Rigidbody.position = Vector3.Lerp(startPos, endPos, currentTime / duration);
            yield return waitForFixedUpdate;
        }
        Controller.ReleaseBuff(this);
    }
    */
}

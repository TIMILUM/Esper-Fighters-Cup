using System.Collections;
using UnityEngine;

public class RaiseObject : BuffObject
{
    private float _limitPosY;
    private Coroutine _raising;

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
            Author.Rigidbody.useGravity = false;
            _raising = StartCoroutine(Raise());
        }
    }

    public override void OnBuffReleased()
    {
        if (Author.photonView.IsMine)
        {
            Author.Rigidbody.useGravity = true;
            StopCoroutine(_raising);
        }
    }

    private IEnumerator Raise()
    {
        var startTime = Time.time;
        var waitForFixedUpdate = new WaitForFixedUpdate();
        var startPos = Author.Rigidbody.position;
        var endPos = new Vector3(startPos.x, _limitPosY, startPos.z);

        while (!Controller.ActiveBuffs.Exists(Type.KnockBack))
        {
            var currentTime = Time.time - startTime;
            Author.Rigidbody.position = Vector3.Lerp(startPos, endPos, currentTime / Info.Duration);
            yield return waitForFixedUpdate;
        }
        Controller.ReleaseBuff(this);
    }
}

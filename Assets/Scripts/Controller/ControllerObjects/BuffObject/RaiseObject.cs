using System.Collections;
using DG.Tweening;
using UnityEngine;

public class RaiseObject : BuffObject
{
    [SerializeField, FMODUnity.EventRef]
    private string _objectWaverSound;

    private float _limitPosY;
    private Sequence _raising;
    private Coroutine _checking;

    private FMOD.Studio.EventInstance _audioInstance;

    public override Type BuffType => Type.Raise;

    public override void OnBuffGenerated()
    {
        // BuffStruct Help
        // ----------------
        // ValueFloat[0] : limitPosY
        // ----------------
        _limitPosY = Info.ValueFloat[0];

        if (Author.photonView.IsMine)
        {
            // useGravity 동기화됨
            if (Author is APlayer)
            {
                Author.Rigidbody.useGravity = false;
            }
            var startPos = Author.transform.position;
            _raising = DOTween.Sequence()
                .Append(Author.Rigidbody.DOMove(new Vector3(startPos.x, _limitPosY, startPos.z), Info.Duration))
                .SetEase(Ease.OutCubic)
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);

            _checking = StartCoroutine(CheckBuffRelease());
        }

        _audioInstance = FMODUnity.RuntimeManager.CreateInstance(_objectWaverSound);
        _audioInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Author.transform, Author.Rigidbody));
        _audioInstance.start();
    }

    public override void OnBuffReleased()
    {
        if (Author.photonView.IsMine)
        {
            if (_checking != null)
            {
                StopCoroutine(_checking);
            }
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

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_checking != null)
        {
            StopCoroutine(_checking);
        }

        _audioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _audioInstance.release();
        _audioInstance.clearHandle();
    }

    private IEnumerator CheckBuffRelease()
    {
        while (!Controller.ActiveBuffs.Exists(Type.KnockBack))
        {
            yield return null;
        }
        _raising.Kill();
        Controller.ReleaseBuff(this);
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

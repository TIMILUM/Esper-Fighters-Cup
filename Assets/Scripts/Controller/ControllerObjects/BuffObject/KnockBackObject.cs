using System.Collections;
using UnityEngine;

public class KnockBackObject : BuffObject
{
    private Vector3 _normalizedDirection = Vector3.zero;
    private float _speed = 1;
    // 넉백 후 오브젝트와 충돌 시 해당값 만큼 HP가 줄어듬
    private float _decreaseHp = 0;
    // 넉백 후 오브젝트와 충돌 시 해당값 만큼 스턴 지속기간(초) 지정됨
    private float _durationStunSeconds = 0;
    private float _playerChracterID = -1;
    private Coroutine _moving;

    public Vector3 NormalizedDirection
    {
        get => _normalizedDirection;
        set => _normalizedDirection = value;
    }

    public override Type BuffType => Type.KnockBack;

    public override void OnBuffGenerated()
    {
        // BuffStruct Help
        // ---------------
        // ValueVector3[0]  : _normalizedDirection
        // ---------------
        // ValueFloat[0]    : _speed
        // ValueFloat[1]    : _decreaseHp (0이면 HP감소 효과 없음)
        // ValueFloat[2]    : _durationStunSeconds (0이면 스턴 효과 없음)
        // ---------------
        _normalizedDirection = Info.ValueVector3[0].normalized;
        _speed = Info.ValueFloat[0];
        _decreaseHp = Info.ValueFloat[1];
        _durationStunSeconds = Info.ValueFloat[2];

        if (Info.ValueFloat.Length > 3)
        {
            _playerChracterID = Info.ValueFloat[3];
        }

        if (Author.photonView.IsMine)
        {
            _moving = StartCoroutine(Knockback());
        }
    }

    public override void OnBuffReleased()
    {
        if (Author.photonView.IsMine)
        {
            StopCoroutine(_moving);
        }
        /*
        if (Author is AStaticObject)
        {
            Author.BuffController.GenerateBuff(new BuffStruct()
            {
                Type = Type.Falling,
                ValueFloat = new float[2] { 0.0f, 0.0f }
            });
        }
        */
    }

    private IEnumerator Knockback()
    {
        var waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            Author.Rigidbody.position += _speed * Time.deltaTime * _normalizedDirection;
            yield return waitForFixedUpdate;
        }
    }

    public override void OnPlayerHitEnter(GameObject other)
    {
        if (!other.TryGetComponent(out Actor otherActor) && !other.CompareTag("Wall"))
        {
            return;
        }

        if (otherActor != null && otherActor.photonView.ViewID == _playerChracterID)
        {
            return;
        }

        Author.Rigidbody.velocity = -_normalizedDirection * 2; // 넉백 후 충돌로 인한 튕기는 효과 추가

        GenerateAfterBuff(Controller);

        if (otherActor != null)
        {
            GenerateAfterBuff(otherActor.BuffController);
        }

        Controller.ReleaseBuff(this);
    }

    // 넉백 후 오브젝트와 충돌 시 추가적으로 얻는 버프를 이 함수에서 생성함
    private void GenerateAfterBuff(BuffController target)
    {
        if (_decreaseHp > 0)
        {
            target.GenerateBuff(new BuffStruct()
            {
                Type = Type.DecreaseHp,
                Damage = _decreaseHp,
                IsOnlyOnce = true
            });
        }

        if (_durationStunSeconds > 0)
        {
            target.GenerateBuff(new BuffStruct()
            {
                Type = Type.Stun,
                Duration = _durationStunSeconds
            });
        }
    }
}

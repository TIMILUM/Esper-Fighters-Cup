using Photon.Pun;
using UnityEngine;

public class SawBladeMoveState : SawBladeFSMBase
{
    // Update is called once per frame
    private void Update()
    {
        var direction = _sawBladeObject.Direction;
        if (direction == Vector3.one)
        {
            ChangeState(SawBladeFSMSystem.StateEnum.HitWall);
            return;
        }

        var position = transform.position;
        position += _sawBladeObject.Speed * Time.deltaTime * direction;
        transform.position = position;

        var endPosition = _sawBladeObject.EndPosition;
        // 끝지점에 다다르면 움직임을 종료하고 HitWall상태로 전환
        var endDistance = Vector3.Distance(endPosition.position, position);
        if (endDistance < 0.5f && gameObject)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    protected override void Initialize()
    {
        State = SawBladeFSMSystem.StateEnum.Move;
    }

    public override void StartState()
    {
        _sawBladeObject.SawBladeAnimator.SetBool("isActive", true);
    }

    public override void EndState()
    {
    }
}

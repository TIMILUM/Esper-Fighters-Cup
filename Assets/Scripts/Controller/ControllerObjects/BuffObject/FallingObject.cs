using UnityEngine;

namespace EsperFightersCup
{
    public class FallingObject : BuffObject
    {
        private float _decreaseHp = 0;
        private float _durationStunSeconds = 0;

        public override Type BuffType => Type.Falling;

        public override void OnBuffGenerated()
        {
            // BuffStruct Help
            // ValueFloat[0]    : _decreaseHp (0이면 HP감소 효과 없음)
            // ValueFloat[1]    : _durationStunSeconds (0이면 스턴 효과 없음)
            // ---------------

            _decreaseHp = Info.ValueFloat[0];
            _durationStunSeconds = Info.ValueFloat[1];
        }

        public override void OnPlayerHitEnter(GameObject other)
        {
            if (!other.TryGetComponent(out Actor otherActor) && !other.CompareTag("Wall"))
            {
                return;
            }

            if (otherActor is null)
            {
                return;
            }

            if (otherActor.ControllerManager.TryGetController(ControllerManager.Type.BuffController, out BuffController otherController))
            {
                GenerateAfterBuff(otherController);
            }
        }

        private void GenerateAfterBuff(BuffController controller)
        {
            if (_durationStunSeconds == 0)
            {
                return;
            }

            controller.GenerateBuff(new BuffStruct()
            {
                Type = Type.DecreaseHp,
                Damage = _decreaseHp,
                Duration = 0.001f
            });

            if (_durationStunSeconds > 0)
            {
                controller.GenerateBuff(new BuffStruct()
                {
                    Type = Type.Stun,
                    Duration = _durationStunSeconds
                });
            }
        }
    }
}

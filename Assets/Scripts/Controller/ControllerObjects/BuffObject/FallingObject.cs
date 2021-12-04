using UnityEngine;

namespace EsperFightersCup
{
    public class FallingObject : BuffObject
    {
        // private float _range;
        private float _decreaseHp = 0;
        private float _durationStunSeconds = 0;
        private float _dropSkillEffect = 0;

        public override Type BuffType => Type.Falling;

        public override void OnBuffGenerated()
        {
            // BuffStruct Help
            // ValueFloat[0]    : _decreaseHp (0이면 HP감소 효과 없음)
            // ValueFloat[1]    : _durationStunSeconds (0이면 스턴 효과 없음)
            // ValueFloat[2]    : _range (AfterBuff 적용 범위)
            // ---------------
            _decreaseHp = Info.ValueFloat[0];
            _durationStunSeconds = Info.ValueFloat[1];

            if (Info.ValueFloat.Length == 3)
                _dropSkillEffect = Info.ValueFloat[2];

            // _range = 1f; // Info.ValueFloat[2];
        }

        public override void OnBuffReleased()
        {
            // 나중에 ObjectHitSystem 개선되면 고치기
            var pos = Author.transform.position;
            pos.y = 0.03f;

            /*
            var targets = Physics.OverlapSphere(pos, _range, (1 << 7) | (1 << 8)); // Object, Character

            foreach (var target in targets)
            {
                var actor = target.GetComponent<Actor>();
                var controller = actor.BuffController;
                GenerateAfterBuff(controller);
            }
            */
            if (_dropSkillEffect != 0)
                ParticleManager.Instance.PullParticleToLocal("DropSkillEffect", pos, Quaternion.identity);

            ParticleManager.Instance.PullParticleLocal("Break_Dust", pos, Quaternion.identity);
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
                IsOnlyOnce = true
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

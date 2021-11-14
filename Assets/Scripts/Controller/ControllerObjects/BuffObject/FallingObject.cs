using UnityEngine;

namespace EsperFightersCup
{
    public class FallingObject : BuffObject
    {

        private float _decreaseHp = 0;
        private float _durationStunSeconds = 0;
        private AStaticObject _character;


        private void Reset()
        {
            _name = "";
            _buffStruct.Type = Type.Falling;

            _character = Author as AStaticObject;

            if (!(_character is null))
            {
                //_character.CharacterAnimatorSync.SetTrigger("Knockback");
            }
        }


        public override void SetBuffStruct(BuffStruct buffStruct)
        {
            // BuffStruct Help
            // ValueFloat[0]    : _decreaseHp (0이면 HP감소 효과 없음)
            // ValueFloat[1]    : _durationStunSeconds (0이면 스턴 효과 없음)
            // ---------------

            base.SetBuffStruct(buffStruct);
            _decreaseHp = buffStruct.ValueFloat[0];
            _durationStunSeconds = buffStruct.ValueFloat[1];
        }


        protected override void Update()
        {
            base.Update();
            if (transform.position.y < 0.5f) // TODO: transform.position 대신 rigidbody.position 사용
            {
                _buffStruct.Duration = 0.1f;
            }

            if (Author.ControllerManager.TryGetController(
ControllerManager.Type.BuffController, out BuffController myController))
            {

                if (myController.GetBuff(Type.KnockBack) != null)
                {
                    myController.ReleaseBuff(this);
                }
            }
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


            if (Author.ControllerManager.TryGetController(
    ControllerManager.Type.BuffController, out BuffController myController))
            {
                GenerateAfterBuff(myController);
                myController.ReleaseBuff(this);
            }


            if (otherActor.ControllerManager.TryGetController(
             ControllerManager.Type.BuffController, out BuffController otherController))
            {
                GenerateAfterBuff(otherController);
            }
        }

        protected override void OnHit(ObjectBase from, ObjectBase to, BuffStruct[] appendBuff)
        {
        }

        private void GenerateAfterBuff(BuffController controller)
        {
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

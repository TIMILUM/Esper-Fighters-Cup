using UnityEngine;

namespace EsperFightersCup
{
    public class SlidingObject : BuffObject
    {

        [SerializeField]
        [Tooltip("이동 방향 입니다.")]
        private Vector3 _startPosition = Vector3.zero;
        private Vector3 _endPosition;

        private ACharacter _character;
        private Rigidbody _rigidbody;
        private float _moveTime;
        private float _currentTime = 0;


        protected override void OnRegistered()
        {
            _rigidbody = Author.GetComponent<Rigidbody>();
        }



        protected override void Reset()
        {
            base.Reset();

            _name = "";
            _buffStruct.Type = Type.Sliding;

            _character = Author as ACharacter;

            if (!(_character is null))
            {
                _character.Animator.SetTrigger("Sliding");
            }
        }


        public override void OnPlayerHitEnter(GameObject other)
        {
            // 부딪히면 버프 삭제
            if (Author.ControllerManager.TryGetController(ControllerManager.Type.BuffController, out BuffController myController))
            {

                myController.ReleaseBuff(this);

            }

        }

        protected override void OnHit(ObjectBase from, ObjectBase to, BuffStruct[] appendBuff)
        {
            throw new System.NotImplementedException();
        }

        public override void SetBuffStruct(BuffStruct buffStruct)
        {
            // BuffStruct Help
            // ---------------
            // ValueVector3[0]  : _startPosition
            // ValueVector3[1]  : _endPosition
            // ---------------
            // ValueFloat[0]    : _time
            // ---------------

            base.SetBuffStruct(buffStruct);
            _startPosition = buffStruct.ValueVector3[0];
            _endPosition = buffStruct.ValueVector3[1];
            _moveTime = buffStruct.ValueFloat[0];
        }
        protected override void Update()
        {
            base.Update();
            if (!IsRegistered || !Author.photonView.IsMine)
            {
                return;
            }
            _currentTime += Time.deltaTime * 1000.0f;
            var realTime = _currentTime / _moveTime;
            if (realTime > 1.0f)
            {
                if (Author.ControllerManager.TryGetController(ControllerManager.Type.BuffController, out BuffController myController))
                {
                    myController.ReleaseBuff(this);
                }
                return;
            }

            Author.transform.position = Vector3.Lerp(_startPosition, _endPosition, realTime);


        }
    }
}

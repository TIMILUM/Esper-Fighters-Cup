using UnityEngine;

namespace EsperFightersCup
{
    public class GrabObject : BuffObject
    {
        private ACharacter _character;
        private Collider[] _colliders;
        private Rigidbody _rigid;


        [SerializeField]
        private float _grabSecond;

        private float _currentSecond;
        private Vector3 _startPos = Vector3.zero;
        private Vector3 _endPos = Vector3.zero;


        private void Reset()
        {
            _name = "";
            _buffStruct.Type = Type.Grab;


            _character = Author as ACharacter;
            if (!(_character is null))
            {
                _character.CharacterAnimatorSync.SetTrigger("Hit");
            }
        }
        protected override void Start()
        {
            base.Start();
            _rigid = Author.GetComponent<Rigidbody>();
            _colliders = Author.GetComponentsInChildren<Collider>();
            foreach (var collider in _colliders)
            {
                collider.isTrigger = true;
                _rigid.useGravity = false;

            }




        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var collider in _colliders)
            {
                collider.isTrigger = false;
                _rigid.useGravity = true;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (_startPos != _buffStruct.ValueVector3[1])
                _startPos = _buffStruct.ValueVector3[1];

            _endPos = _buffStruct.ValueVector3[0];

            if (_grabSecond == 0)
                _grabSecond = 1;

            if (_currentSecond < 1.0f) _currentSecond += Time.deltaTime / _grabSecond;

            Author.transform.position = Vector3.Lerp(_startPos, _endPos + Vector3.up * 3.0f, _currentSecond);

        }

        public override void OnPlayerHitEnter(GameObject other)
        {

        }
        protected override void OnHit(ObjectBase from, ObjectBase to, BuffStruct[] appendBuff)
        {

        }



    }
}

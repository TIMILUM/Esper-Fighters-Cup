using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EsperFightersCup
{
    public class GrabObject : BuffObject
    {

        private ACharacter _character;
        private Collider[] _colliders;

        private void Reset()
        {
            _name = "";
            _buffStruct.Type = Type.Grab;


            _character = Author as ACharacter;
            if (!(_character is null))
            {
                _character.CharacterAnimatorSync.SetTrigger("Knockback");
            }
        }
        protected override void Start()
        {
            base.Start();

            _colliders = Author.GetComponentsInChildren<Collider>();
            foreach (var collider in _colliders)
            {
                collider.isTrigger = true;
            }

        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var collider in _colliders)
            {
                collider.isTrigger = false;
            }
        }


        public override void OnPlayerHitEnter(GameObject other)
        {

        }
        protected override void OnHit(ObjectBase from, ObjectBase to, BuffStruct[] appendBuff)
        {

        }



    }
}

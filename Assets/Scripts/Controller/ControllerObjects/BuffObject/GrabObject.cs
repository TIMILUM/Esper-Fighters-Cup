using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EsperFightersCup
{
    public class GrabObject : BuffObject
    {

        private ACharacter _character;

        private void Reset()
        {
            _name = "";
            _buffStruct.Type = Type.Grab;

            _character = Author as ACharacter;

            if (!(_character is null))
            {
                //_character.CharacterAnimatorSync.SetTrigger("Knockback");
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

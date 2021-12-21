using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EsperFightersCup
{
    public class SkillRock : AStaticObject
    {
        [SerializeField]
        private Transform _particleTargetTransform;
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!photonView.IsMine)
            {
                return;
            }

            ParticleManager.Instance.PullParticleSync("SkillRock_Object_Destroy", _particleTargetTransform.position, _particleTargetTransform.rotation);
        }
    }
}

using UnityEngine;

namespace EsperFightersCup
{
    public class GrabObject : BuffObject
    {
        private Collider[] _colliders;

        public override Type BuffType => Type.Grab;

        public override void OnBuffGenerated()
        {
            _colliders = Author.GetComponentsInChildren<Collider>();
            foreach (var collider in _colliders)
            {
                collider.isTrigger = true;
            }

            if (Author is APlayer player)
            {
                player.Animator.SetTrigger("Knockback");
            }
        }

        public override void OnBuffReleased()
        {
            foreach (var collider in _colliders)
            {
                collider.isTrigger = false;
            }
        }
    }
}

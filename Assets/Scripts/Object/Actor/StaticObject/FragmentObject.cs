using System.Collections;
using UnityEngine;

namespace EsperFightersCup
{
    public class FragmentObject : AStaticObject
    {
        [SerializeField] private Animator _anim;

        private Collider[] _colliders;

        protected override void Start()
        {
            base.Start();
            _colliders = GetComponents<Collider>();
            foreach (var collider in _colliders)
            {
                collider.enabled = false;
            }
            StartCoroutine(CheckAnimationIsCompleted());
        }

        private IEnumerator CheckAnimationIsCompleted()
        {
            if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }

            foreach (var collider in _colliders)
            {
                collider.enabled = true;
            }
        }
    }
}

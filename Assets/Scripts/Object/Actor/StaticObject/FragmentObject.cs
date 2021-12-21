using System.Collections;
using UnityEngine;

namespace EsperFightersCup
{
    public class FragmentObject : AStaticObject
    {
        [SerializeField] private Animator _anim;

        private Collider _collider;

        protected override void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.enabled = false;
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            StartCoroutine(CheckAnimationIsCompleted());
        }

        private IEnumerator CheckAnimationIsCompleted()
        {
            while (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }

            _collider.enabled = true;
        }
    }
}

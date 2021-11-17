using UnityEngine;

namespace EsperFightersCup
{
    public class RockObject : AStaticObject
    {
        [SerializeField] private Animator _anim;
        [SerializeField] private GameObject _fristObj;
        [SerializeField] private GameObject _rock;

        private Collider[] _colliders;
        private Rigidbody _rigid;

        protected override void Start()
        {
            base.Start();
            _colliders = GetComponents<Collider>();
            _rigid = GetComponent<Rigidbody>();
            _rigid.useGravity = false;
            foreach (var collider in _colliders)
            {
                collider.enabled = false;
            }
        }
        private void Update()
        {
            if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("CreateRock"))
            {
                _fristObj.SetActive(false);
                _rock.SetActive(true);
                _rigid.useGravity = true;
                foreach (var collider in _colliders)
                {
                    collider.enabled = true;
                }
            }


        }


    }
}

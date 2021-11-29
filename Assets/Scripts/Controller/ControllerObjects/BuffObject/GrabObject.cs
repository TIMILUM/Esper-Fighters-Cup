using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace EsperFightersCup
{
    public class GrabObject : BuffObject
    {
        private Collider[] _colliders;

        public override Type BuffType => Type.Grab;
        private Coroutine _moving;
        private int _targetView;
        private PhotonView _target;
        private Rigidbody _rigd = null;



        public override void OnBuffGenerated()
        {
            _colliders = Author.GetComponentsInChildren<Collider>();
            _targetView = (int)Info.ValueFloat[0];
            _target = PhotonNetwork.GetPhotonView(_targetView);


            foreach (var collider in _colliders)
            {
                collider.isTrigger = true;
            }

            if (Author is APlayer player)
            {
                player.Animator.SetTrigger("Knockback");
            }


            if (Author.photonView.IsMine)
            {

                if (Author.GetComponent<Actor>() as ACharacter != null)
                {
                    _rigd = Author.GetComponent<Actor>().GetComponent<Rigidbody>();
                    _rigd.useGravity = false;
                }

                _moving = StartCoroutine(Grab());


            }
        }



        public override void OnBuffReleased()
        {
            foreach (var collider in _colliders)
            {
                collider.isTrigger = false;
            }

            if (Author.photonView.IsMine)
            {
                StopCoroutine(_moving);

                if (_rigd != null)
                    _rigd.useGravity = true;
            }




        }

        private IEnumerator Grab()
        {
            var startPos = Author.transform.position;
            var currentType = 0.0f;
            var targettarns = _target.transform;
            var boundy = 3.0f;

            if (Author.photonView.IsMine)
            {
                while (!Controller.ActiveBuffs.Exists(Type.KnockBack))
                {

                    if (currentType <= 1.0f)
                        currentType += Time.deltaTime * 10;


                    Author.transform.position = Vector3.Lerp(startPos, targettarns.position +
                        targettarns.up * boundy, currentType);
                    yield return null;
                }

            }

            Controller.ReleaseBuff(this);
        }

    }
}

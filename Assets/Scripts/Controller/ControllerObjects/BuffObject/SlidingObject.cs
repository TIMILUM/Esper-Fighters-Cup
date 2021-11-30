using System.Collections;
using UnityEngine;

namespace EsperFightersCup
{
    public class SlidingObject : BuffObject
    {
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private float _moveTime;
        private Coroutine _moving;


        public override Type BuffType => Type.Sliding;

        public override void OnBuffGenerated()
        {
            // BuffStruct Help
            // ---------------
            // ValueVector3[0]  : _startPosition
            // ValueVector3[1]  : _endPosition
            // ---------------
            // ValueFloat[0]    : _time
            // ---------------
            _startPosition = Info.ValueVector3[0];
            _endPosition = Info.ValueVector3[1];
            _moveTime = Info.ValueFloat[0];
            SlidingAnimationCancel(false);

            if (Author.photonView.IsMine)
            {
                _moving = StartCoroutine(Slide());
            }


        }

        public override void OnPlayerHitEnter(GameObject other)
        {
            SlidingAnimationCancel(true);
            Controller.ReleaseBuff(this);
        }

        private IEnumerator Slide()
        {
            var currentTime = 0f;
            var realTime = 0f;
            var waitForFixedUpdate = new WaitForFixedUpdate();



            while (realTime <= 1.0f - Mathf.Epsilon)
            {
                currentTime += Time.deltaTime * 1000.0f;
                realTime = currentTime / _moveTime;
                Author.Rigidbody.position = Vector3.Lerp(_startPosition, _endPosition, realTime);
                yield return waitForFixedUpdate;
            }
            Controller.ReleaseBuff(this);
        }

        private void SlidingAnimationCancel(bool isCancel)
        {
            var AuthorPlayer = Author as APlayer;
            if (AuthorPlayer != null)
                AuthorPlayer.Animator.SetBool("Cancel", isCancel);
        }
    }
}

using UnityEngine;

namespace EsperFightersCup.UI.InGame
{
    public class CharacterPositionUI : MonoBehaviour
    {
        [SerializeField] private Material _localPlayerUI;
        [SerializeField] private Material _enemyUI;

        public Transform TargetPlayer { get; set; }
        public bool IsLocalPlayer
        {
            get => _isLocalPlayer;
            set
            {
                GetComponent<Renderer>().material = value ? _localPlayerUI : _enemyUI;
                _isLocalPlayer = value;
            }
        }
        private bool _isLocalPlayer;

        private void Update()
        {
            if (TargetPlayer)
            {
                FollowPlayer();
            }
        }

        private void FollowPlayer()
        {
            if (Physics.Raycast(TargetPlayer.position, Vector3.down, out var hit, 100f, 1 << 6))
            {
                var position = hit.point + new Vector3(0, 0.01f, 0);
                // var rotation = TargetPlayer.rotation;
                // transform.SetPositionAndRotation(position, rotation);
                transform.position = position;
            }
        }
    }
}

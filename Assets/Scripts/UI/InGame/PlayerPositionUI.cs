using UnityEngine;

namespace EsperFightersCup.UI.InGame
{
    public class PlayerPositionUI : MonoBehaviour
    {
        public Transform TargetPlayer { get; set; }

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

using UnityEngine;

public class StunObject : BuffObject
{
    public override Type BuffType => Type.Stun;

    public override void OnBuffGenerated()
    {
        if (Author is APlayer player && player.Animator != null)
        {
            player.Animator.SetTrigger("Hit");

            var position = Author.transform.position + new Vector3(0f, 0.01f, 0f);
            ParticleManager.Instance.PullParticle("Hit", position, Quaternion.identity);
        }
    }
}

using UnityEngine;

public class StunObject : BuffObject
{
    public override Type BuffType => Type.Stun;

    public override void OnBuffGenerated()
    {
        if (Author is APlayer player && player.Animator != null)
        {
            player.Animator.SetTrigger("Hit", false);

            var position = Author.transform.position + new Vector3(0f, 0.01f, 0f);
            ParticleManager.Instance.PullParticleLocal("Character_Hit", position, Quaternion.Euler(90f, 0f, 0f));
        }
    }

    public override void OnBuffReleased()
    {
        base.OnBuffReleased();
    }
}

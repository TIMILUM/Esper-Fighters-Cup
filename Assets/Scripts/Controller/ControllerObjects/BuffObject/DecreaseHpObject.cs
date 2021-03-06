using UnityEngine;

public class DecreaseHpObject : BuffObject
{
    public override Type BuffType => Type.DecreaseHp;

    public override void OnBuffGenerated()
    {
        if (Author.photonView.IsMine && Author is APlayer player)
        {
            player.HP -= (int)Info.Damage;
            Debug.Log("Actor : " + player.name + ", HP : " + player.HP);
        }
    }
}

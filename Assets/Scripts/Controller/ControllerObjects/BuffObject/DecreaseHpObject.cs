using UnityEngine;

public class DecreaseHpObject : BuffObject
{
    public override Type BuffType => Type.DecreaseHp;

    protected override void Reset()
    {
        base.Reset();
        Info.Type = Type.DecreaseHp;
    }

    public override void OnBuffGenerated()
    {
        if (Author.photonView.IsMine)
        {
            Author.HP -= (int)Info.Damage;
            Debug.Log("Actor : " + Author.name + ", HP : " + Author.HP);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACharacter : Actor
{
    // Start is called before the first frame update
    [SerializeField]
    private float _characterHp;
    public float CharacterHP
    {
        get => _characterHp;
        set => _characterHp = value;
    }

    protected override void Start()
    {
        base.Start();
    }
}

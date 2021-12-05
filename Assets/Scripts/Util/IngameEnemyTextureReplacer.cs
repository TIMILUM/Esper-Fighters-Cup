using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class IngameEnemyTextureReplacer : MonoBehaviour
{
    [SerializeField]
    private Texture2D _enemyTexture;

    private Actor _actor = null;
    private Material _material = null;

    private void Awake()
    {
        _actor = GetComponentInParent<Actor>();
        _material = GetComponentInChildren<Renderer>()?.material;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_actor == null)
        {
            return;
        }

        if (_actor.photonView.IsMine)
        {
            return;
        }

        _material.SetTexture("_BaseMap", _enemyTexture);
    }
}

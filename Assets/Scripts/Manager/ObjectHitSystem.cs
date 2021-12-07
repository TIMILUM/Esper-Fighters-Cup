using System;
using Photon.Pun;
using UnityEngine;

public class HitEventArgs : EventArgs
{
    public GameObject Other { get; }
    public bool IsDestroy { get; }

    public HitEventArgs(GameObject other, bool isDestroy)
    {
        Other = other;
        IsDestroy = isDestroy;
    }
}

public class ObjectHitSystem : MonoBehaviourPun
{
    [SerializeField, Tooltip("값은 런타임 시 자동으로 입력됩니다.")]
    private float _strength;

    [SerializeField, Tooltip("값은 Actor를 상속받고 있을 경우에만 자동으로 입력됩니다. 그 외에는 수동으로 입력하셔야합니다.")]
    private int _objectID;

    [SerializeField, FMODUnity.EventRef]
    private string _hitSound;

    [Header("Destroy Effects")]
    [SerializeField, Tooltip("파괴 모션이 나타날 포지션을 뜻합니다. 기본값은 현재 포지션입니다.")]
    private Transform _destroyEffectPosition = null;

    [SerializeField, Tooltip("파괴 시 나타날 파티클의 이름을 작성합니다.")]
    private string _particleName = "Object_Destroy";

    private Vector3 _collisionDirection = Vector3.up;
    private Actor _actor;

    /// <summary>
    /// 강도값
    /// </summary>
    public float Strength => _strength;

    /// <summary>
    /// 오브젝트가 파괴가 가능한지 여부
    /// </summary>
    public bool IsDestroyable { get; private set; } = true;

    /// <summary>
    /// 오브젝트가 충돌하고나서 삭제될 상태인지 여부
    /// </summary>
    public bool IsDestroyed { get; private set; } = false;

    public event EventHandler<HitEventArgs> OnHit;

    private void Awake()
    {
        _actor = GetComponentInParent<Actor>();
        if (_actor != null)
        {
            _objectID = _actor.ID;
        }
    }

    private void Start()
    {
        var csvData = CSVUtil.GetData("ObjectHitSystemDataTable");

        if (!csvData.Get<float>("Obj_ID", out var idList))
        {
            return;
        }
        var index = idList.FindIndex(x => (int)x == _objectID);
        if (index < 0)
        {
            return;
        }

        if (csvData.Get<float>("Strength", out var strengthList))
        {
            _strength = strengthList[index];
        }
        if (csvData.Get<float>("Destroyable", out var destroyableList))
        {
            IsDestroyable = destroyableList[index] > 0;
        }
    }

    private void Update()
    {
        if (!_actor || !_actor.photonView.IsMine || !IsDestroyable)
        {
            return;
        }

        if (IsDestroyed)
        {
            DestroyObject();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        _collisionDirection = Vector3.Normalize(transform.position - other.contacts[0].point);
        Hit(other.gameObject);
    }

    public void Hit(GameObject other)
    {
        var otherHitSystem = other.GetComponent<ObjectHitSystem>();
        if (otherHitSystem == null)
        {
            return;
        }

        var otherStrength = otherHitSystem._strength;
        var difference = _strength - otherStrength;
        // 본인의 강도가 더 높은 경우
        if (difference > 0 && otherHitSystem.IsDestroyable)
        {
            // otherHitSystem._isDestroy = true;
        }
        // 상대의 강도가 더 높은 경우
        else if (difference < 0 && IsDestroyable)
        {
            IsDestroyed = true;
        }
        // 둘 다 강도값이 같은 경우
        else if (difference == 0)
        {
            IsDestroyed = IsDestroyable;
            otherHitSystem.IsDestroyed = otherHitSystem.IsDestroyable;
        }

        if (otherHitSystem.IsDestroyed)
        {
            otherHitSystem.DestroyObject();
        }

        if (!string.IsNullOrEmpty(_hitSound))
        {
            var instance = FMODUnity.RuntimeManager.CreateInstance(_hitSound);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, gameObject.transform);
            if (IsDestroyed)
            {
                instance.setParameterByName("DestroyCheck", 1f);
            }
            instance.start();
            instance.release();
        }

        OnHit?.Invoke(this, new HitEventArgs(other, IsDestroyed));
        if (IsDestroyed && photonView.IsMine)
        {
            DestroyObject();
        }
    }

    private void DestroyObject()
    {
        if (!gameObject || !photonView.IsMine)
        {
            return;
        }

        if (_destroyEffectPosition == null)
        {
            _destroyEffectPosition = transform;
        }
        ParticleManager.Instance.PullParticleSync(_particleName, _destroyEffectPosition.position, Quaternion.LookRotation(_collisionDirection));
        PhotonNetwork.Destroy(gameObject);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Unity.Mathematics;
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
    private Actor _actor = null;

    /// <summary>
    /// 강도값
    /// </summary>
    public float Strength => _strength;

    /// <summary>
    /// 오브젝트가 파괴가 가능한지 여부
    /// </summary>
    public bool IsDestroyable { get; set; } = true;

    /// <summary>
    /// 오브젝트가 충돌하고나서 삭제될 상태인지 여부
    /// </summary>
    public bool IsDestroyed { get; private set; } = false;

    public event EventHandler<HitEventArgs> OnHit;

    private static List<BuffObject.Type> s_activateBuffList = null;

    private void SetActiveBuffList()
    {
        s_activateBuffList.Add(BuffObject.Type.Sliding);
        s_activateBuffList.Add(BuffObject.Type.Raise);
        s_activateBuffList.Add(BuffObject.Type.Falling);
        s_activateBuffList.Add(BuffObject.Type.KnockBack);
        s_activateBuffList.Add(BuffObject.Type.DecreaseHp);
    }

    private void Awake()
    {
        if (s_activateBuffList == null)
        {
            s_activateBuffList = new List<BuffObject.Type>();
            SetActiveBuffList();
        }
        
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
        /*
        if (_actor is null || !_actor.photonView.IsMine || !IsDestroyable)
        {
            return;
        }

        if (IsDestroyed)
        {
            DestroyObject();
        }
        */
    }

    private void OnCollisionEnter(Collision other)
    {
        _collisionDirection = Vector3.Normalize(new Vector3(transform.position.x - other.contacts[0].point.x, 0 , transform.position.z - other.contacts[0].point.z));
        Hit(other.gameObject);
    }

    public void Hit(GameObject other, float customStrength = -1, bool forceActive = false)
    {
        var otherHitSystem = other.GetComponent<ObjectHitSystem>();
        var strength = customStrength < 0 ? _strength : customStrength;
        if (otherHitSystem == null)
        {
            return;
        }

        bool isActiveHitSystem = _actor != null && HasActiveBuff(_actor);

        var otherActor = other.GetComponent<Actor>();
        if (otherActor != null && HasActiveBuff(otherActor))
        {
            isActiveHitSystem = true;
        }

        if (!isActiveHitSystem && !forceActive)
        {
            return;
        }

        var otherStrength = otherHitSystem._strength;
        var difference = strength - otherStrength;
        // 본인의 강도가 더 높은 경우
        if (difference > 0 && otherHitSystem.IsDestroyable)
        {
            otherHitSystem.IsDestroyed = true;
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

        /*
        if (otherHitSystem.IsDestroyed)
        {
            otherHitSystem.DestroyObject();
        }
        */

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
            if (!string.IsNullOrEmpty(_particleName))
            {
                if (_destroyEffectPosition == null)
                {
                    _destroyEffectPosition = transform;
                }
                var rotation = Quaternion.LookRotation(_collisionDirection) * quaternion.Euler(-90, 0, 0);;
                ParticleManager.Instance.PullParticleSync(_particleName, _destroyEffectPosition.position,rotation);

            }
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private bool HasActiveBuff(Actor actor)
    {
        if (s_activateBuffList == null)
        {
            s_activateBuffList = new List<BuffObject.Type>();
            SetActiveBuffList();
        }
        return s_activateBuffList.Any(buffType => actor.BuffController.ActiveBuffs[buffType].Count > 0);
    }
}

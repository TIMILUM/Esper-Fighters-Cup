using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class HitInfo
{
    public GameObject Other { get; }
    public bool IsDestroy { get; }

    public HitInfo(GameObject other, bool isDestroy)
    {
        Other = other;
        IsDestroy = isDestroy;
    }
}

public class ObjectHitSystem : MonoBehaviourPun
{
    [SerializeField, Tooltip("값은 런타임 시 자동으로 입력됩니다.")]
    private float _strength;
    public float Strength => _strength;

    public bool IsDestroyable { get; private set; } = true;

    [SerializeField, Tooltip("값은 Actor를 상속받고 있을 경우에만 자동으로 입력됩니다. 그 외에는 수동으로 입력하셔야합니다.")]
    private int _objectID;

    private Actor _actor;
    private bool _isDestroy = false;

    public event UnityAction<HitInfo> OnHit;

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

        if (_isDestroy)
        {
            DestroyObject();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        OnPlayerHitEnter(other.gameObject);
    }

    public void OnPlayerHitEnter(GameObject other)
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
            otherHitSystem._isDestroy = true;
        }
        // 상대의 강도가 더 높은 경우
        else if (difference < 0 && IsDestroyable)
        {
            _isDestroy = true;
        }
        // 둘 다 강도값이 같은 경우
        else if (difference == 0)
        {
            _isDestroy = IsDestroyable;
            otherHitSystem._isDestroy = otherHitSystem.IsDestroyable;
        }

        if (otherHitSystem._isDestroy)
        {
            otherHitSystem.DestroyObject();
        }

        if (_isDestroy)
        {
            DestroyObject();
        }

        OnHit?.Invoke(new HitInfo(other, _isDestroy));
    }

    private void DestroyObject()
    {
        ParticleManager.Instance.PullParticleSync("Object_Destroy", transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(gameObject);
    }
}

using Photon.Pun;
using UnityEngine;
public class ObjectHitSystem : MonoBehaviourPunCallbacks
{
    [SerializeField, Tooltip("값은 런타임 시 자동으로 입력됩니다.")]
    private float _strength;
    public float Strength => _strength;
    
    private bool _isDestroyable = true;
    public bool IsDestroyable => _isDestroyable;

    [SerializeField, Tooltip("값은 Actor를 상속받고 있을 경우에만 자동으로 입력됩니다. 그 외에는 수동으로 입력하셔야합니다.")]
    private int _objectID;

    private Actor _actor;
    private bool _isDestroy = false;

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
            _isDestroyable = destroyableList[index] > 0;
        }
    }

    private void Update()
    {
        if (!_actor || !_actor.photonView.IsMine || !_isDestroyable)
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
        if (difference > 0 && otherHitSystem._isDestroyable)
        {
            otherHitSystem._isDestroy = true;
        }
        // 상대의 강도가 더 높은 경우
        else if (difference < 0 && _isDestroyable)
        {
            _isDestroy = true;
        }
        // 둘 다 강도값이 같은 경우
        else if (difference == 0)
        {
            _isDestroy = _isDestroyable;
            otherHitSystem._isDestroy = otherHitSystem._isDestroyable;
        }

        if (otherHitSystem._isDestroy)
        {
            otherHitSystem.DestroyObject();
        }

        if (_isDestroy)
        {
            DestroyObject();
        }
    }

    private void DestroyObject()
    {
        var pv = gameObject.GetComponentInChildren<PhotonView>();
        if (pv != null)
        {
            // PhotonNetwork.OpCleanRpcBuffer(photonView);
            PhotonNetwork.Destroy(photonView);
            PhotonNetwork.SendAllOutgoingCommands();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

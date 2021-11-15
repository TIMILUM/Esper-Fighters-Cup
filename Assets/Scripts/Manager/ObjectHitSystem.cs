using System;
using Photon.Pun;
using UnityEngine;
public class ObjectHitSystem : MonoBehaviourPunCallbacks
{
    [SerializeField, Tooltip("값은 런타임 시 자동으로 입력됩니다.")]
    private float _strength;
    private bool _isDestroyable = true;

    [SerializeField, Tooltip("값은 Actor를 상속받고 있을 경우에만 자동으로 입력됩니다. 그 외에는 수동으로 입력하셔야합니다.")]
    private int _objectID;
    private int _csvIndex = 0;

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
        csvData.Get<float>("Obj_ID", out var idList);
        _csvIndex = idList.FindIndex(x => (int)x == _objectID);
        _strength = GetCSVData<float>(csvData, "Strength");
        _isDestroyable = GetCSVData<bool>(csvData, "Destroyable");
    }

    private void Update()
    {
        if (!_isDestroyable || !_actor.photonView.IsMine)
        {
            return;
        }

        if (_isDestroy)
        {
            var pv = gameObject.GetComponentInChildren<PhotonView>();
            if (pv != null)
            {
                PhotonNetwork.Destroy(photonView);
            }
            else
            {
                Destroy(gameObject);
            }
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
    }

    private T GetCSVData<T>(CSVData csvData, string key)
    {
        if (!csvData.Get<T>(key, out var valueList))
        {
            throw new Exception("Error to parse csv data.");
        }

        return valueList[_csvIndex];
    }
}

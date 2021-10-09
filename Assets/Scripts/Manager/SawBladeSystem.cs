using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class SawBladeSystem : MonoBehaviour
{
    [SerializeField]
    private SawBladeObject _sawBladeObjectPrefab = null;

    [SerializeField]
    private Transform _positionRoot = null;

    [SerializeField]
    private Transform _sawBladeRoot = null;
    
    private List<SawBladePosition> _bladePositions = new List<SawBladePosition>();

    // Start is called before the first frame update
    private void Start()
    {
        // 알아서 하위 오브젝트의 포지션 데이터를 들고옴
        for (var i = 0; i < _positionRoot.childCount; ++i)
        {
            var child = _positionRoot.GetChild(i);
            var start = child.Find("start");
            var end = child.Find("end");
            
            _bladePositions.Add(new SawBladePosition
            {
                _startTransform = start, 
                _endTransform = end
            });
        }
    }
    
    /// <summary>
    /// 톱날 오브젝트를 생성합니다.
    /// </summary>
    public void GenerateSawBladeObject()
    {
        var position = GetSawBladePositions();
        var sawBladeGameObject = PhotonNetwork.Instantiate("Prefabs/StaticObjects/" + _sawBladeObjectPrefab.name, position._startTransform.position, Quaternion.identity);
        var sawBladeObject = sawBladeGameObject.GetComponent<SawBladeObject>();
        sawBladeObject.SetDirection(position._startTransform, position._endTransform);
    }

    private SawBladePosition GetSawBladePositions()
    {
        var index = Random.Range(0, _bladePositions.Count);
        return new SawBladePosition
        { 
            _startTransform = _bladePositions[index]._startTransform,
            _endTransform = _bladePositions[index]._endTransform
        };
    }

    private class SawBladePosition
    {
        public Transform _startTransform;
        public Transform _endTransform;
    }
}

using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SawBladePattern : MonoBehaviour
{
    [SerializeField]
    private Transform[] _pathList;

    private readonly List<SawBladePosition> _bladePositions = new List<SawBladePosition>();
    
    // Start is called before the first frame update
    private void Start()
    {
        for (var i = 0; i < _pathList.Length; ++i)
        {
            var path = _pathList[i];
            var start = path.Find("start");
            var end = path.Find("end");

            _bladePositions.Add(new SawBladePosition
            {
                _startTransform = start,
                _endTransform = end
            });
        }
    }

    /// <summary>
    ///     톱날 오브젝트를 생성합니다.
    /// </summary>
    public void GenerateSawBladeObject()
    {
        foreach (var bladePosition in _bladePositions)
        {
            var sawBladeGameObject = PhotonNetwork.Instantiate("Prefabs/StaticObjects/SawBladeObject",
                bladePosition._startTransform.position, Quaternion.identity);
            var sawBladeObject = sawBladeGameObject.GetComponent<SawBladeObject>();
            sawBladeObject.SetDirection(bladePosition._startTransform, bladePosition._endTransform);
        }
    }

    private class SawBladePosition
    {
        public Transform _endTransform;
        public Transform _startTransform;
    }
}

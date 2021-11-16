using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SawBladePattern : MonoBehaviour
{
    public const string SawBladePrefabLocation = "Prefab/StaticObject/SawBladeObject";

    [SerializeField]
    private Transform[] _pathList;

    private readonly List<SawBladePosition> _bladePositions = new List<SawBladePosition>();

    private void Start()
    {
        for (var i = 0; i < _pathList.Length; ++i)
        {
            var path = _pathList[i];
            var start = path.Find("start");
            var end = path.Find("end");

            _bladePositions.Add(new SawBladePosition
            {
                Start = start,
                End = end
            });
        }
    }

    /// <summary>
    /// 톱날 오브젝트를 생성합니다.
    /// </summary>
    public List<SawBladeObject> GenerateSawBladeObject()
    {
        var sawblades = new List<SawBladeObject>(_bladePositions.Count);

        foreach (var bladePosition in _bladePositions)
        {
            var sawBladeGameObject = PhotonNetwork.Instantiate(SawBladePrefabLocation,
                bladePosition.Start.position, Quaternion.identity);
            var sawBladeObject = sawBladeGameObject.GetComponent<SawBladeObject>();
            sawBladeObject.SetDirection(bladePosition.Start, bladePosition.End);

            sawblades.Add(sawBladeObject);
        }

        return sawblades;
    }

    private class SawBladePosition
    {
        public Transform Start { get; set; }
        public Transform End { get; set; }
    }
}

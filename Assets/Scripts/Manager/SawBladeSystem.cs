using System.Collections.Generic;
using EsperFightersCup;
using UnityEngine;

public class SawBladeSystem : Singleton<SawBladeSystem>
{
    [SerializeField]
    private SawBladeObject _sawBladeObjectPrefab;

    [SerializeField]
    private Transform _positionRoot;

    [SerializeField]
    private Transform _sawBladeRoot;

    [SerializeField]
    private Transform _patternRoot;

    private readonly List<SawBladePattern> _bladePatterns = new List<SawBladePattern>();

    public Dictionary<int, SawBladeObject> LocalSpawnedSawBlades { get; } = new Dictionary<int, SawBladeObject>();

    // Start is called before the first frame update
    private void Start()
    {
        // 알아서 하위 오브젝트의 패턴 데이터를 들고옴
        for (var i = 0; i < _patternRoot.childCount; ++i)
        {
            var child = _patternRoot.GetChild(i).GetComponent<SawBladePattern>();
            _bladePatterns.Add(child);
        }
    }

    /// <summary>
    ///     톱날 오브젝트를 패턴을 통해 생성합니다.
    /// </summary>
    public void GenerateSawBlade()
    {
        var pattern = GetSawBladePattern();
        pattern.GenerateSawBladeObject();
    }

    private SawBladePattern GetSawBladePattern()
    {
        var index = Random.Range(0, _bladePatterns.Count);
        return _bladePatterns[index];
    }
}

using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    /// <summary>
    /// 각각의 파티클 종류 정보
    /// </summary>
    [System.Serializable]
    private class ParticleInfo
    {
        [SerializeField] private string _particleName;
        [SerializeField] private GameObject _particleFrefab;
        [SerializeField] private int _maxNum = 30;
        [Tooltip("파티클 라이프 타임입니다. (단위는 밀리세컨드 입니다.)")]
        [SerializeField] private int _lifeTime = 3000;

        public string ParticleName { get => _particleName; set => _particleName = value; }
        public GameObject ParticleFrefab { get => _particleFrefab; set => _particleFrefab = value; }
        public int MaxNum { get => _maxNum; set => _maxNum = value; }
        public int LifeTime { get => _lifeTime; set => _lifeTime = value; }
    }

    /// <summary>
    /// 파티클에 필요한 변수
    /// </summary>
    private class Particle
    {
        public string _particleName;
        public GameObject _particle;
        public float _lifeTime;
        public float _startTime;

        public Particle(GameObject particleFrefab, float lifeTime)
        {
            _particle = particleFrefab;
            _lifeTime = lifeTime;
        }

        public void StartParticle(string particleName)
        {
            _startTime = Time.time;
            _particleName = particleName;
        }
    }



    /// <summary>
    /// 싱글턴으로 제작
    /// </summary>
    private static ParticleManager s_instance;


    /// <summary>
    /// 각각의 파티클 종류 정보
    /// </summary>
    [SerializeField]
    private List<ParticleInfo> _particleInfo = new List<ParticleInfo>();

    /// <summary>
    /// 파티클 Pool
    /// </summary>
    private Dictionary<string, Queue<Particle>> _paticleList = new Dictionary<string, Queue<Particle>>();

    /// <summary>
    /// 지금 사용중인 파티클
    /// </summary>
    private List<Particle> _activeParticle = new List<Particle>();
    /// <summary>
    /// 파티클 인스턴스
    /// </summary>
    ///

    /// <summary>
    /// 삭제할 파티클 큐
    /// </summary>
    private Queue<Particle> _removeParticle = new Queue<Particle>();
    public static ParticleManager Instance
    {
        get
        {
            if (null == s_instance)
                return null;
            return s_instance;
        }
    }


    private void Awake()
    {
        if (s_instance == null) s_instance = this;

        foreach (var info in _particleInfo)
        {
            if (!_paticleList.ContainsKey(info.ParticleName))
                _paticleList.Add(info.ParticleName, new Queue<Particle>());

            for (int i = 0; i < info.MaxNum; i++)
            {
                var clone = Instantiate(info.ParticleFrefab, transform);
                clone.SetActive(false);
                _paticleList[info.ParticleName].Enqueue(new Particle(clone, info.LifeTime));
            }
        }
    }


    /// <summary>
    /// 파티클 실행
    /// </summary>
    /// <param name="particleName"> 파티클 이름</param>
    /// <param name="pos"> 파티클 시작 위치</param>
    /// <param name="angle">파티클 앵글 </param>
    public void PullParticle(string particleName, Vector3 pos, Quaternion angle)
    {
        if (!_paticleList.ContainsKey(particleName))
        {
            Debug.LogError("동일한 파티클 이름이 없습니다.");
            return;
        }

        if (_paticleList[particleName].Count == 0)
        {
            Debug.Log("파티클이 없습니다.");
            return;
        }

        var clon = _paticleList[particleName].Dequeue();
        clon._particle.SetActive(true);

        clon._particle.transform.SetPositionAndRotation(pos, angle);
        clon.StartParticle(particleName);
        _activeParticle.Add(clon);
    }

    /// <summary>
    /// 큐에 넣는 작업
    /// </summary>
    public void PutParticle()
    {

        /// <summary>
        /// foreach에 List.remove함수 불러오면 에러 메시지 발생해서 큐를 만들어
        /// 삭제를 했습니다.
        /// </summary>

        foreach (var item in _activeParticle)
        {
            float currentTime = (Time.time - item._startTime) * 1000;

            if (currentTime >= item._lifeTime)
            {
                item._particle.SetActive(false);
                _paticleList[item._particleName].Enqueue(item);
                _removeParticle.Enqueue(item);

            }

        }

        while (_removeParticle.Count != 0)
            _activeParticle.Remove(_removeParticle.Dequeue());

    }

    private void Update()
    {
        PutParticle();
    }

}

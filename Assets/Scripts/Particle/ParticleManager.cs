using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;




public class ParticleManager : MonoBehaviour
{

    /// <summary>
    /// 각각의 파티클 종류 정보
    /// </summary>
    [System.Serializable]
    private class ParticleInfo
    {
        public string _particleName;
        public GameObject _particleFrefab;
        public int _maxNum = 30;
        [Tooltip("파티클 라이프 타임입니다. (단위는 밀리세컨드 입니다.)")]
        public int _lifeTime = 3000;
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
    private static ParticleManager _sinstance;


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
    public static ParticleManager sInstance
    {
        get
        {
            if (null == _sinstance)
                return null;
            return _sinstance;
        }
    }


    private void Awake()
    {
        if (_sinstance == null) _sinstance = this;

        foreach (var info in _particleInfo)
        {
            if (!_paticleList.ContainsKey(info._particleName))
                _paticleList.Add(info._particleName, new Queue<Particle>());

            for (int i = 0; i < info._maxNum; i++)
            {
                var clone = Instantiate(info._particleFrefab, transform);
                clone.SetActive(false);
                _paticleList[info._particleName].Enqueue(new Particle(clone, info._lifeTime));
            }
        }
    }


    /// <summary>
    /// 파티클 실행
    /// </summary>
    /// <param name="particleName"> 파티클 이름</param>
    /// <param name="Pos"> 파티클 시작 위치</param>
    /// <param name="Angle">파티클 앵글 </param>
    public void PullParticle(string particleName, Vector3 Pos, Quaternion Angle)
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

        clon._particle.transform.position = Pos;
        clon._particle.transform.rotation = Angle;
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

using System.Collections.Generic;
using EsperFightersCup.Net;
using ExitGames.Client.Photon;
using UnityEngine;

public class ParticleManager : PunEventCallbacks
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
        public string Name { get; set; }
        public GameObject Object { get; set; }
        public float LifeTime { get; set; }
        public float StartTime { get; set; }

        public Particle(GameObject particleFrefab, float lifeTime)
        {
            Object = particleFrefab;
            LifeTime = lifeTime;
        }

        public void StartParticle(string particleName)
        {
            StartTime = Time.time;
            Name = particleName;
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
    private Dictionary<string, Queue<Particle>> _particleList = new Dictionary<string, Queue<Particle>>();

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
            if (!_particleList.ContainsKey(info.ParticleName))
                _particleList.Add(info.ParticleName, new Queue<Particle>());

            for (int i = 0; i < info.MaxNum; i++)
            {
                var clone = Instantiate(info.ParticleFrefab, transform);
                clone.SetActive(false);
                _particleList[info.ParticleName].Enqueue(new Particle(clone, info.LifeTime));
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
        PacketSender.Broadcast(new GameParticlePlayEvent(particleName, pos, angle.eulerAngles), SendOptions.SendUnreliable);
    }

    /// <summary>
    /// 큐에 넣는 작업
    /// </summary>
    public void PutParticle()
    {
        // foreach에 List.remove함수 불러오면 에러 메시지 발생해서 큐를 만들어 삭제를 했습니다.
        foreach (var item in _activeParticle)
        {
            float currentTime = (Time.time - item.StartTime) * 1000;

            if (currentTime >= item.LifeTime)
            {
                item.Object.SetActive(false);
                _particleList[item.Name].Enqueue(item);
                _removeParticle.Enqueue(item);
            }
        }

        while (_removeParticle.Count != 0)
        {
            _activeParticle.Remove(_removeParticle.Dequeue());
        }
    }

    private void Update()
    {
        PutParticle();
    }

    protected override void OnGameEventReceived(GameEventArguments args)
    {
        if (args.Code != GameProtocol.ParticlePlay)
        {
            return;
        }

        var data = (GameParticlePlayEvent)args.EventData;

        if (!_particleList.TryGetValue(data.Name, out var particleQueue))
        {
            Debug.LogError("동일한 파티클 이름이 없습니다.");
            return;
        }

        if (particleQueue.Count == 0)
        {
            Debug.Log("파티클이 없습니다.");
            return;
        }

        var clon = particleQueue.Dequeue();
        clon.Object.SetActive(true);

        clon.Object.transform.SetPositionAndRotation(data.Position, Quaternion.Euler(data.Angle));
        clon.StartParticle(data.Name);
        _activeParticle.Add(clon);
    }
}

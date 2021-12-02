using UnityEngine;
using UnityEngine.SceneManagement;

namespace EsperFightersCup
{
    [RequireComponent(typeof(FMODUnity.StudioEventEmitter))]
    public class OutgameBGMManager : Singleton<OutgameBGMManager>
    {
        private FMODUnity.StudioEventEmitter _emitter;

        protected override void Awake()
        {
            base.Awake();

            _emitter = GetComponent<FMODUnity.StudioEventEmitter>();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _emitter.Play();     //배경음악 재생
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnSceneChanged(Scene current, Scene next)
        {
            if (next.name == "GameScene")
            {
                _emitter.Stop();
            }
            else if (current.name == "GameScene")
            {
                _emitter.Play();
            }
        }
    }
}

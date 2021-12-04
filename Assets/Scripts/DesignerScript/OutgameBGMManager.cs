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
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            switch (scene.name)
            {
                case "GameScene":
                case "ResultScene":
                    if (_emitter.IsPlaying())
                    {
                        _emitter.Stop();
                    }
                    break;

                default:
                    if (!_emitter.IsPlaying())
                    {
                        _emitter.Play();
                    }
                    break;
            }
        }
    }
}

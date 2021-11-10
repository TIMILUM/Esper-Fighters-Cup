using EsperFightersCup.Util;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace EsperFightersCup.Manager
{
    public sealed class AudioManager : Singleton<AudioManager>
    {
        public static float MasterVolume
        {
            get => Instance._masterVolume;
            set => Instance._masterVolume = value;
        }

        [SerializeField, Range(0f, 1f)] private float _masterVolume;

        private float _cachedMasterVolume;
        private VCA _masterVCAController;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitInstance()
        {
            CreateNewSingletonObject();
        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            try
            {
                _masterVCAController = RuntimeManager.GetVCA("vca:/Master");
                if (_masterVCAController.isValid())
                {
                    _masterVCAController.getVolume(out _masterVolume);
                }
            }
            catch (VCANotFoundException)
            {
                Debug.LogWarning("MasterVolume을 찾을 수 없습니다.");
                return;
            }
        }

        private void LateUpdate()
        {
            if (_masterVolume != _cachedMasterVolume && _masterVCAController.isValid())
            {
                _masterVCAController.setVolume(_masterVolume);
                _cachedMasterVolume = _masterVolume;
                Debug.Log($"Set master volume to {_masterVolume}");
            }
        }
    }
}

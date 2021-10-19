using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace EsperFightersCup.Manager
{
    public sealed class AudioManager : MonoBehaviour
    {
        private static AudioManager s_instance;

        public static float MasterVolume
        {
            get => s_instance._masterVolume;
            set => s_instance._masterVolume = value;
        }

        [SerializeField, Range(0f, 1f)] private float _masterVolume;

        private float _cachedMasterVolume;
        private VCA _masterVCAController;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitInstance()
        {
            new GameObject("Audio Manager").AddComponent<AudioManager>();
        }

        private void Awake()
        {
            if (s_instance)
            {
                Destroy(s_instance);
            }
            s_instance = this;
            DontDestroyOnLoad(s_instance);
        }

        private void Start()
        {
            _masterVCAController = RuntimeManager.GetVCA("vca:/Master");
            if (_masterVCAController.isValid())
            {
                _masterVCAController.getVolume(out _masterVolume);
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

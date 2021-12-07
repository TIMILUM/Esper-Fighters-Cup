using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace EsperFightersCup.Manager
{
    public sealed class AudioManager : Singleton<AudioManager>
    {
        public const string MasterVolumePrefKey = "master-volume";
        public const string BGMVolumePrefKey = "bgm-volume";
        public const string AMBVolumePrefKey = "amb-volume";
        public const string SFXVolumePrefKey = "sfx-volume";
        public const string UISFXVolumePrefKey = "uisfx-volume";

        public static float MasterVolume
        {
            get => Instance._masterVolume;
            set => Instance._masterVolume = value;
        }

        public static float BGMVolume
        {
            get => Instance._bgmVolume;
            set => Instance._bgmVolume = value;
        }

        public static float AMBVolume
        {
            get => Instance._ambVolume;
            set => Instance._ambVolume = value;
        }

        public static float SFXVolume
        {
            get => Instance._sfxVolume;
            set => Instance._sfxVolume = value;
        }

        public static float UISFXVolume
        {
            get => Instance._uiSfxVolume;
            set => Instance._uiSfxVolume = value;
        }

        [SerializeField, InspectorName("Master Volume"), Range(0f, 1f)]
        private float _masterVolume;
        [SerializeField, InspectorName("BGM Volume"), Range(0f, 1f)]
        private float _bgmVolume;
        [SerializeField, InspectorName("AMB Volume"), Range(0f, 1f)]
        private float _ambVolume;
        [SerializeField, InspectorName("SFX Volume"), Range(0f, 1f)]
        private float _sfxVolume;
        [SerializeField, InspectorName("UI SFX Volume"), Range(0f, 1f)]
        private float _uiSfxVolume;

        private VCA _masterVCA;
        private VCA _bgmVCA;
        private VCA _ambVCA;
        private VCA _sfxVCA;
        private VCA _uiSfxVCA;

        private float _cachedMasterVolume;
        private float _cachedBGMVolume;
        private float _cachedAMBVolume;
        private float _cachedSFXVolume;
        private float _cachedUISFXVolume;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitInstance()
        {
            CreateNewSingletonObject();
        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            if (TryGetVCAController("MasterVolume", out _masterVCA))
            {
                _masterVCA.getVolume(out _masterVolume);
            }
            if (TryGetVCAController("BGMVolume", out _bgmVCA))
            {
                _bgmVCA.getVolume(out _bgmVolume);
            }
            if (TryGetVCAController("AMBVolume", out _ambVCA))
            {
                _ambVCA.getVolume(out _ambVolume);
            }
            if (TryGetVCAController("SFXVolume", out _sfxVCA))
            {
                _sfxVCA.getVolume(out _sfxVolume);
            }
            if (TryGetVCAController("UISFXVolume", out _uiSfxVCA))
            {
                _uiSfxVCA.getVolume(out _uiSfxVolume);
            }

            MasterVolume = PlayerPrefs.GetFloat(MasterVolumePrefKey, 0.5f);
            BGMVolume = PlayerPrefs.GetFloat(BGMVolumePrefKey, 1f);
            AMBVolume = PlayerPrefs.GetFloat(AMBVolumePrefKey, 1f);
            SFXVolume = PlayerPrefs.GetFloat(SFXVolumePrefKey, 1f);
            UISFXVolume = PlayerPrefs.GetFloat(UISFXVolumePrefKey, 1f);
        }

        private void LateUpdate()
        {
            SetVolume(_masterVCA, ref _cachedMasterVolume, _masterVolume);
            SetVolume(_bgmVCA, ref _cachedBGMVolume, _bgmVolume);
            SetVolume(_ambVCA, ref _cachedAMBVolume, _ambVolume);
            SetVolume(_sfxVCA, ref _cachedSFXVolume, _sfxVolume);
            SetVolume(_uiSfxVCA, ref _cachedUISFXVolume, _uiSfxVolume);
        }

        private bool TryGetVCAController(string name, out VCA vca)
        {
            try
            {
                vca = RuntimeManager.GetVCA($"vca:/{name}");
                return vca.isValid();
            }
            catch (VCANotFoundException)
            {
                Debug.LogWarning($"{name}을 찾을 수 없습니다.");
                vca = default;
                return false;
            }
        }

        private void SetVolume(VCA vca, ref float cachedVolume, float newVolume)
        {
            if (!vca.isValid() || newVolume == cachedVolume)
            {
                return;
            }

            // vca.getPath(out var path);
            if (vca.setVolume(newVolume) == FMOD.RESULT.OK)
            {
                cachedVolume = newVolume;
                // Debug.Log($"Set {path} to {newVolume}");
            }
            else
            {
                // Debug.Log($"Faild to set {path} to {newVolume}");
            }
        }
    }
}

using EsperFightersCup.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup.UI
{
    public class AudioSettingsPanel : SettingPanel
    {
        [SerializeField] private Slider _masterVolumeSetting;
        [SerializeField] private Slider _bgmVolumeSetting;
        [SerializeField] private Slider _ambVolumeSetting;
        [SerializeField] private Slider _sfxVolumeSetting;
        [SerializeField] private Slider _uiSfxVolumeSetting;

        public const string MasterVolumePrefKey = "master-volume";
        public const string BGMVolumePrefKey = "bgm-volume";
        public const string AMBVolumePrefKey = "amb-volume";
        public const string SFXVolumePrefKey = "sfx-volume";
        public const string UISFXVolumePrefKey = "uisfx-volume";

        protected override void Awake()
        {
            base.Awake();

            _masterVolumeSetting.onValueChanged.AddListener(HandleMasterVolumeChanged);
            _bgmVolumeSetting.onValueChanged.AddListener(HandleBGMVolumeChanged);
            _ambVolumeSetting.onValueChanged.AddListener(HandleAMBVolumeChanged);
            _sfxVolumeSetting.onValueChanged.AddListener(HandleSFXVolumeChanged);
            _uiSfxVolumeSetting.onValueChanged.AddListener(HandleUISFXVolumeChanged);
        }

        private void HandleMasterVolumeChanged(float value)
        {
            AudioManager.MasterVolume = value;
        }

        private void HandleBGMVolumeChanged(float value)
        {
            AudioManager.BGMVolume = value;
        }

        private void HandleAMBVolumeChanged(float value)
        {
            AudioManager.AMBVolume = value;
        }

        private void HandleSFXVolumeChanged(float value)
        {
            AudioManager.SFXVolume = value;
        }

        private void HandleUISFXVolumeChanged(float value)
        {
            AudioManager.UISFXVolume = value;
        }

        public override void Show()
        {
            base.Show();

            _masterVolumeSetting.value = AudioManager.MasterVolume;
            _bgmVolumeSetting.value = AudioManager.BGMVolume;
            _ambVolumeSetting.value = AudioManager.AMBVolume;
            _sfxVolumeSetting.value = AudioManager.SFXVolume;
            _uiSfxVolumeSetting.value = AudioManager.UISFXVolume;
        }

        public override void Save()
        {
            PlayerPrefs.SetFloat(AudioManager.MasterVolumePrefKey, AudioManager.MasterVolume);
            PlayerPrefs.SetFloat(AudioManager.BGMVolumePrefKey, AudioManager.BGMVolume);
            PlayerPrefs.SetFloat(AudioManager.AMBVolumePrefKey, AudioManager.AMBVolume);
            PlayerPrefs.SetFloat(AudioManager.SFXVolumePrefKey, AudioManager.SFXVolume);
            PlayerPrefs.SetFloat(AudioManager.UISFXVolumePrefKey, AudioManager.UISFXVolume);
        }
    }
}

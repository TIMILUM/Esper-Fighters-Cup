using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup.UI.Settings
{
    public class DisplaySettingsPanel : SettingPanel
    {
        [SerializeField] private ToggleGroup _resolutionSettings;
        [SerializeField] private ToggleGroup _screenModeSettings;

        private IEnumerable<ResolutionOption> _resolutions;
        private IEnumerable<ScreenModeOption> _screenModes;

        public const string ResolutionWidthPrefKey = "screen-width";
        public const string ResolutionHeightPrefKey = "screen-height";
        public const string ScreenModePrefKey = "screen-mode";

        protected override void Awake()
        {
            base.Awake();
            _resolutions = InitResolutionSettings(_resolutionSettings);
            _screenModes = InitScreenModeSettings(_screenModeSettings);
        }

        private IEnumerable<ResolutionOption> InitResolutionSettings(ToggleGroup settings)
        {
            var width = PlayerPrefs.GetInt(ResolutionWidthPrefKey, Screen.width);
            var height = PlayerPrefs.GetInt(ResolutionHeightPrefKey, Screen.height);

            var optionsRaw = settings.transform.GetComponentsInChildren<ResolutionOption>();

            var options = new List<ResolutionOption>();
            for (int i = 0; i < optionsRaw.Length; i++)
            {
                if (i >= Screen.resolutions.Length)
                {
                    optionsRaw[i].gameObject.SetActive(false);
                    continue;
                }

                var option = optionsRaw[i];
                var resolution = Screen.resolutions[i];
                var label = option.gameObject.GetComponentInChildren<Text>();

                option.Width = resolution.width;
                option.Height = resolution.height;
                label.text = $"{option.Width}x{option.Height}";
                option.gameObject.SetActive(true);
                options.Add(option);

                var toggle = option.gameObject.GetComponent<Toggle>();
                settings.RegisterToggle(toggle);

                if (option.Width == width && option.Height == height)
                {
                    settings.NotifyToggleOn(toggle);
                }
            }

            return options;
        }

        private IEnumerable<ScreenModeOption> InitScreenModeSettings(ToggleGroup settings)
        {
            var screenMode = (FullScreenMode)PlayerPrefs.GetInt(ScreenModePrefKey, (int)Screen.fullScreenMode);

            var options = settings.transform.GetComponentsInChildren<ScreenModeOption>();
            foreach (var option in options)
            {
                var toggle = option.gameObject.GetComponent<Toggle>();
                settings.RegisterToggle(toggle);

                if (option.ScreenMode == screenMode)
                {
                    settings.NotifyToggleOn(toggle);
                    break;
                }
            }
            return options;
        }

        public override void Show()
        {
            base.Show();

            foreach (var option in _resolutions)
            {
                if (option.Width == Screen.width && option.Height == Screen.height)
                {
                    _resolutionSettings.NotifyToggleOn(option.gameObject.GetComponent<Toggle>());
                    break;
                }
            }

            foreach (var option in _screenModes)
            {
                if (option.ScreenMode == Screen.fullScreenMode)
                {
                    _screenModeSettings.NotifyToggleOn(option.gameObject.GetComponent<Toggle>());
                    break;
                }
            }

            Debug.Log($"Current Diplay settings: {Screen.width}x{Screen.height}, {Screen.fullScreenMode}");
        }

        public override void Save()
        {
            var resolution = _resolutionSettings.ActiveToggles().First()?.GetComponent<ResolutionOption>() ?? null;
            var mode = _screenModeSettings.ActiveToggles().First()?.GetComponent<ScreenModeOption>() ?? null;

            if (resolution is null || mode is null)
            {
                Debug.LogError("Display settings does not saved!");
                return;
            }

            Screen.SetResolution(resolution.Width, resolution.Height, mode.ScreenMode);

            PlayerPrefs.SetInt(ResolutionWidthPrefKey, resolution.Width);
            PlayerPrefs.SetInt(ResolutionHeightPrefKey, resolution.Height);
            PlayerPrefs.SetInt(ScreenModePrefKey, (int)mode.ScreenMode);
        }
    }
}

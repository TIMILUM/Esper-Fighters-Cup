using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup.UI
{
    [Serializable]
    public class Settings
    {
        [SerializeField] private Button _settingButton;
        [SerializeField] private SettingPanel _panel;

        public Button SettingButton => _settingButton;
        public SettingPanel SettingPanel => _panel;
    }

    public class SettingsPanel : MonoBehaviour
    {
        [SerializeField] private List<Settings> _settings;

        private RectTransform _transform;
        private CanvasGroup _canvasGroup;

        private void Start()
        {
            _transform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();

            _transform.anchoredPosition = Vector2.up * 10f;
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _settings.First()?.SettingButton.onClick?.Invoke();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                Save();
            }
        }

        public void OnButtonSelected(Button selectedButton)
        {
            foreach (var item in _settings)
            {
                if (item.SettingButton == selectedButton)
                {
                    item.SettingButton.targetGraphic.color = Color.green;
                    if (item.SettingPanel)
                    {
                        item.SettingPanel.Show();
                    }
                }
                else
                {
                    item.SettingButton.targetGraphic.color = Color.gray;
                    if (item.SettingPanel)
                    {
                        item.SettingPanel.Hide();
                    }
                }
            }
        }

        public void Show()
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            DOTween.Sequence()
                .SetLink(gameObject)
                .Join(_transform.DOAnchorPos(Vector2.zero, 0.25f))
                .Join(_canvasGroup.DOFade(1f, 0.25f));

            foreach (var item in _settings)
            {
                Debug.Assert(item.SettingButton, gameObject);

                if (!item.SettingPanel)
                {
                    continue;
                }
                item.SettingPanel.Hide();
                item.SettingPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }

            _settings.First()?.SettingButton.onClick?.Invoke();
        }

        public void Hide()
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            DOTween.Sequence()
                .SetLink(gameObject)
                .Join(_transform.DOAnchorPos(Vector2.up * 10f, 0.25f))
                .Join(_canvasGroup.DOFade(0f, 0.25f));
        }

        public void Save()
        {
            foreach (var item in _settings)
            {
                if (item.SettingPanel)
                {
                    item.SettingPanel.Save();
                }
            }
        }
    }
}

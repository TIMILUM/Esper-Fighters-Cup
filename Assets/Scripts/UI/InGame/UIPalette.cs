using System.Collections.Generic;
using EsperFightersCup.UI.InGame.Skill;
using UnityEngine;

namespace EsperFightersCup.UI.InGame
{
    [RequireComponent(typeof(SkillUI))]
    public class UIPalette : MonoBehaviour
    {
        [System.Serializable]
        private class Palette
        {
            [SerializeField] private SpriteRenderer _renderer;
            [SerializeField] private List<Sprite> _paletteUI;

            public SpriteRenderer Renderer => _renderer;
            public IReadOnlyList<Sprite> PaletteUIs => _paletteUI;
        }

        [SerializeField] private List<Palette> _targetUI;

        private void Start()
        {
            var target = GetComponent<SkillUI>().Target;
            if (target == null)
            {
                Debug.LogWarning("타겟 플레이어를 찾을 수 없습니다!", gameObject);
                return;
            }

            var paletteIndex = target.PaletteIndex;
            foreach (var palette in _targetUI)
            {
                if (paletteIndex >= palette.PaletteUIs.Count)
                {
                    Debug.LogWarning($"{paletteIndex}번 팔레트에 해당하는 UI를 찾을 수 없습니다.", gameObject);
                    return;
                }

                palette.Renderer.sprite = palette.PaletteUIs[paletteIndex];
            }
        }
    }
}

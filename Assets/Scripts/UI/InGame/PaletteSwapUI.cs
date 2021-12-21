using System;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup.UI
{
    [RequireComponent(typeof(Image))]
    public class PaletteSwapUI : MonoBehaviour
    {
        [SerializeField] private PaletteSwapItem<Sprite>[] _characterPalettes;

        public Image TargetImage { get; private set; }
        public PaletteSwapItem<Sprite>[] CharacterPalettes => _characterPalettes;

        private void Awake()
        {
            TargetImage = GetComponent<Image>();
        }

        public void Swap(ACharacter.Type character, int index)
        {
            var palette = Array.Find(_characterPalettes, palette => palette.Character == character);
            if (palette == null)
            {
                Debug.LogWarning($"{character} 타입과 일치하는 팔레트를 찾지 못했습니다.", gameObject);
                return;
            }

            if (index < 0 || index >= palette.Palettes.Length)
            {
                Debug.LogWarning($"{index}번과 일치하는 팔레트 이미지를 찾지 못했습니다.", gameObject);
                return;
            }

            TargetImage.sprite = palette.Palettes[index];
        }
    }
}

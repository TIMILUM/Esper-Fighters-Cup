using System;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup
{
    [RequireComponent(typeof(Image))]
    public class PaletteSwapUI : MonoBehaviour
    {
        [Serializable]
        public class Palette
        {
            [SerializeField] private ACharacter.Type _character;
            [SerializeField] private Sprite[] _paletteSprites;

            public ACharacter.Type Character => _character;
            public Sprite[] PaletteSprites => _paletteSprites;
        }

        [SerializeField] private Palette[] _characterPalettes;

        public Image TargetImage { get; private set; }
        public Palette[] CharacterPalettes => _characterPalettes;

        private void Awake()
        {
            TargetImage = GetComponent<Image>();
        }

        public void Swap(ACharacter.Type character, int index)
        {
            var palette = Array.Find(_characterPalettes, palette => palette.Character == character);
            if (palette != null)
            {
                Debug.LogWarning($"{character} 타입과 일치하는 팔레트를 찾지 못했습니다.", gameObject);
                return;
            }

            if (index < 0 || index >= palette.PaletteSprites.Length)
            {
                Debug.LogWarning($"{index}번과 일치하는 팔레트 이미지를 찾지 못했습니다.", gameObject);
                return;
            }

            TargetImage.sprite = palette.PaletteSprites[index];
        }
    }
}

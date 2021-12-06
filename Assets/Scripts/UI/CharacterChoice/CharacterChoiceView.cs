using System;
using EsperFightersCup.Manager;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace EsperFightersCup.UI
{
    public class CharacterChoiceView : MonoBehaviourPunCallbacks
    {
        private class CharacterChoiceInfo
        {
            public GameObject Character { get; set; }
            public ACharacter.Type Type { get; set; }
            public int PaletteIndex { get; set; }
        }

        [SerializeField]
        private Button _localPlayerSubmitButton;
        [SerializeField]
        private PaletteSwapItem<GameObject>[] _localCharacterPalettes;
        [SerializeField]
        private PaletteSwapItem<GameObject>[] _otherCharacterPalettes;

        private CharacterChoiceInfo _localCharacterInfo;
        private CharacterChoiceInfo _otherCharacterInfo;

        public void ChangeViewCharacter(int sender, ACharacter.Type type)
        {
            if (sender == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                if (_localCharacterInfo != null)
                {
                    _localCharacterInfo.Character.SetActive(false);
                }
                var palette = Array.Find(_localCharacterPalettes, x => x.Character == type);
                int index = _otherCharacterInfo != null && _otherCharacterInfo.Type == type ? 1 : 0;
                var go = palette.Palettes[index];
                _localCharacterInfo = new CharacterChoiceInfo
                {
                    Character = go,
                    Type = type,
                    PaletteIndex = index
                };
                go.SetActive(true);
                CharacterChoiceSystem.Instance.ChooseCharacter = (type, index);
            }
            else
            {
                if (_otherCharacterInfo != null)
                {
                    _otherCharacterInfo.Character.SetActive(false);
                }
                var palette = Array.Find(_otherCharacterPalettes, x => x.Character == type);
                int index = _localCharacterInfo != null && _localCharacterInfo.Type == type ? 1 : 0;
                var go = palette.Palettes[index];
                _otherCharacterInfo = new CharacterChoiceInfo
                {
                    Character = go,
                    Type = type,
                    PaletteIndex = index
                };
                go.SetActive(true);
            }
        }
    }
}

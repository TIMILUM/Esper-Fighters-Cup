using System;
using UnityEngine;

namespace EsperFightersCup
{
    [Serializable]
    public class PaletteSwapItem<T>
    {
        [SerializeField] private ACharacter.Type _character;
        [SerializeField] private T[] _palettes;

        public ACharacter.Type Character => _character;
        public T[] Palettes => _palettes;
    }
}

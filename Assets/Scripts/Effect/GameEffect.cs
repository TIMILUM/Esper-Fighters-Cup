using Cysharp.Threading.Tasks;
using EsperFightersCup.Manager;
using UnityEngine;

namespace EsperFightersCup.Effect
{
    public abstract class GameEffect : MonoBehaviour
    {
        public string Name { get; set; }

        protected abstract UniTask RunEffectAsync();

        private void OnEnable()
        {
            EffectLifeCycleAsync().Forget();
        }

        private async UniTask EffectLifeCycleAsync()
        {
            await RunEffectAsync();
            EffectSystem.Instance.EffectPools[Name].Release(gameObject);
        }
    }
}

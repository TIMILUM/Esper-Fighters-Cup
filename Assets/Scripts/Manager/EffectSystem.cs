using System;
using System.Collections.Generic;
using EsperFightersCup.Effect;
using EsperFightersCup.Net;
using UnityEngine;
using UnityEngine.Pool;

namespace EsperFightersCup.Manager
{
    [Serializable]
    public class EffectInfo
    {
        [SerializeField] private string _name;
        [SerializeField] private GameEffect _effect;

        public string Name => _name;
        public GameEffect Effect => _effect;
    }

    public class EffectSystem : PunEventSingleton<EffectSystem>
    {
        [SerializeField] private EffectInfo[] _effectPrefabs;

        private readonly Dictionary<string, ObjectPool<GameObject>> _effectPools = new Dictionary<string, ObjectPool<GameObject>>();

        public IReadOnlyDictionary<string, ObjectPool<GameObject>> EffectPools => _effectPools;

        protected override void Awake()
        {
            base.Awake();
        }

        private GameObject OnCreateEffect()
        {
            var newEffect = new GameObject("Effect");
            return newEffect;
        }

        private void OnTakeEffectFromPool(GameObject effect)
        {
            // effect.SetActive(true);
        }

        private void OnReturnEffectToPool(GameObject effect)
        {
            effect.SetActive(false);
        }

        private void OnDestroyEffect(GameObject effect)
        {
            Destroy(effect);
        }

        protected override void OnGameEventReceived(GameEventArguments args)
        {
            if (args.Code != EventCode.PlayEffect)
            {
                return;
            }

            var data = (GameEffectPlayEvent)args.EventData;

            GameObject effect;
            if (_effectPools.TryGetValue(data.Id, out var pool))
            {
                effect = pool.Get();
            }
            else
            {
                var testPool = new DictionaryPool<string, GameEffect>();


                var newPool = new ObjectPool<GameObject>(OnCreateEffect, OnTakeEffectFromPool, OnReturnEffectToPool, OnDestroyEffect);
                _effectPools.Add(data.Id, newPool);

                effect = newPool.Get();
            }

            effect.transform.SetPositionAndRotation(data.Position, Quaternion.Euler(data.Rotation));
            effect.SetActive(true);
        }
    }
}

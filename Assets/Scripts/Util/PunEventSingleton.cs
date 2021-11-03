using EsperFightersCup.Net;
using UnityEngine;

namespace EsperFightersCup.Util
{
    /// <summary>
    /// Awake를 통해 생성되는 <see cref="PunEventCallbacks"/> 기반의 싱글톤입니다.
    /// </summary>
    /// <typeparam name="T">싱글톤을 사용하려는 컴포넌트</typeparam>
    public class PunEventSingleton<T> : PunEventCallbacks where T : PunEventCallbacks
    {
        /// <summary>
        /// 현재 씬에 존재하는 싱글톤 오브젝트입니다.
        /// </summary>
        public static T Instance { get; private set; } = null;

        protected virtual void Awake()
        {
            Instance = FindObjectOfType<T>();
            Debug.Assert(Instance, $"{typeof(T)} 싱글톤 오브젝트를 찾지 못했습니다.");
        }

        protected virtual void OnDestroy()
        {
            Instance = null;
        }
    }
}

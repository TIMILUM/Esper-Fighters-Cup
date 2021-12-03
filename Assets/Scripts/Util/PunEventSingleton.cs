using EsperFightersCup.Net;
using UnityEngine;

namespace EsperFightersCup
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
        public static T Instance { get; private set; }
        public static string SingletonName { get; } = typeof(T).Name;

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = gameObject.GetComponent<T>();
            }
            else
            {
                Debug.LogWarning($"Can not instantiate {SingletonName} singleton game object because it is already exist");
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            Instance = null;
        }

        protected static T CreateNewSingletonObject()
        {
            return new GameObject(SingletonName).AddComponent<T>();
        }
    }
}

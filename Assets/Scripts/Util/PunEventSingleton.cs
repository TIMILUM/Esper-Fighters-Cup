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
        /// 현재 씬에 존재하는 싱글톤 오브젝트입니다.<para/>
        /// 존재하지 않는 경우 새로운 싱글톤 오브젝트를 생성하고 반환합니다.
        /// </summary>
        public static T Instance { get; private set; }
        public static string SingletonName { get; } = typeof(T).Name;

        protected virtual void Awake()
        {
            if (Instance)
            {
                Debug.LogWarning($"Destory old one and create new {SingletonName} singleton game object");
                Destroy(Instance.gameObject);
            }

            Instance = gameObject.GetComponent<T>();
            Debug.Assert(Instance, $"Cannot found {SingletonName} singleton game object");
        }

        protected virtual void OnDestroy()
        {
            Instance = null;
        }

        protected static void CreateNewSingleton()
        {
            new GameObject(SingletonName).AddComponent<T>();
        }
    }
}

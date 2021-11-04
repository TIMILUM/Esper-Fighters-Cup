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
        private static T s_instance;

        /// <summary>
        /// 현재 씬에 존재하는 싱글톤 오브젝트입니다.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = FindObjectOfType<T>();
                    if (s_instance == null)
                    {
                        s_instance = CreateNewSingletonObject();
                    }
                }
                return s_instance;
            }
        }
        public static string SingletonName { get; } = typeof(T).Name;

        protected virtual void Awake()
        {
            if (s_instance == null)
            {
                s_instance = gameObject.GetComponent<T>();
            }
            else
            {
                Debug.LogWarning($"Can not instantiate {SingletonName} singleton game object because it is already exist");
                Destroy(this);
            }
        }

        protected virtual void OnDestroy()
        {
            s_instance = null;
        }

        protected static T CreateNewSingletonObject()
        {
            return new GameObject(SingletonName).AddComponent<T>();
        }
    }
}

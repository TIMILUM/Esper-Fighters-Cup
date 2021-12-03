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
        private static readonly object s_instanceLock = new object();

        /// <summary>
        /// 현재 씬에 존재하는 싱글톤 오브젝트입니다.
        /// </summary>
        public static T Instance { get; private set; }
        public static string SingletonName { get; } = typeof(T).Name;


        protected virtual void Awake()
        {
            lock (s_instanceLock)
            {
                if (Instance == null)
                {
                    Instance = gameObject.GetComponent<T>();

                }
                else
                {
                    Debug.LogWarning($"Can not instantiate {SingletonName} singleton game object because it is already exist");

                    // 한 오브젝트에 컴포넌트가 중복 추가된 경우도 있어서 같은 게임오브젝트인지 체크
                    if (Instance.gameObject == gameObject)
                    {
                        Destroy(this);
                        return;
                    }
                    Destroy(gameObject);
                }
            }
        }

        protected virtual void OnDestroy()
        {
            lock (s_instanceLock)
            {
                if (Instance == this)
                {
                    Instance = null;
                }
            }
        }

        protected static T CreateNewSingletonObject()
        {
            return new GameObject(SingletonName).AddComponent<T>();
        }
    }
}

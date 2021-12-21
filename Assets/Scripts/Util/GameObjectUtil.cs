using System.Collections.Generic;
using UnityEngine;

namespace EsperFightersCup
{
    public static class GameObjectUtil
    {
        public static void ActiveGameObjects(IEnumerable<GameObject> gameObjects, bool isActive = true)
        {
            foreach (var gameObj in gameObjects)
            {
                gameObj.SetActive(isActive);
            }
        }

        public static void RotateGameObjects(IEnumerable<GameObject> gameObjects, Quaternion rotation)
        {
            foreach (var gameObj in gameObjects)
            {
                gameObj.transform.rotation = rotation;
            }
        }

        public static void ScaleGameObjects(IEnumerable<GameObject> gameObjects, Vector3 scale)
        {
            foreach (var gameObj in gameObjects)
            {
                gameObj.transform.localScale = scale;
            }
        }

        public static void ActiveGameObject(GameObject gameObject, bool isActive = true)
        {
            gameObject.SetActive(isActive);
        }

        public static void TranslateGameObjects(IEnumerable<GameObject> gameObjects, Vector3 position)
        {
            foreach (var gameObj in gameObjects)
            {
                gameObj.transform.position = position;
            }
        }

        public static void RotateGameObject(GameObject gameObject, Quaternion rotation)
        {
            gameObject.transform.rotation = rotation;
        }

        public static void ScaleGameObject(GameObject gameObject, Vector3 scale)
        {
            gameObject.transform.localScale = scale;
        }

        public static void TranslateGameObject(GameObject gameObject, Vector3 position)
        {
            gameObject.transform.position = position;
        }

        public static void SetParentGameObjects(IEnumerable<GameObject> gameObjects, string transformName)
        {
            var parent = GameObject.Find(transformName);
            if (parent == null)
            {
                return;
            }

            foreach (var gameObj in gameObjects)
            {
                gameObj.transform.SetParent(parent.transform);
            }
        }

        public static void DestoryGameObjects(IEnumerable<GameObject> gameObjects)
        {
            foreach (var gameObj in gameObjects)
            {
                UnityEngine.Object.Destroy(gameObj);
            }
        }
    }
}

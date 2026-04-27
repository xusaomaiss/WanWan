using System.Collections.Generic;
using UnityEngine;

namespace Wanwan.Runtime.Pools
{
    public class GameObjectPool
    {
        private readonly string poolName;
        private readonly int defaultSize;
        private readonly Stack<GameObject> pool = new Stack<GameObject>();

        public GameObjectPool(string name, int defaultSize)
        {
            poolName = name;
            this.defaultSize = defaultSize;
        }

        public void PreWarm()
        {
            for (int i = 0; i < defaultSize; i++)
            {
                var obj = CreateObject();
                obj.SetActive(false);
                pool.Push(obj);
            }
        }

        public GameObject Rent()
        {
            GameObject obj = pool.Count > 0 ? pool.Pop() : CreateObject();
            ResetRuntimeObject(obj);
            obj.SetActive(true);
            return obj;
        }

        public void Return(GameObject obj)
        {
            if (obj == null) return;
            obj.SetActive(false);
            pool.Push(obj);
        }

        private GameObject CreateObject()
        {
            return new GameObject(poolName);
        }

        private static void ResetRuntimeObject(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }

            for (int i = obj.transform.childCount - 1; i >= 0; i--)
            {
                var child = obj.transform.GetChild(i).gameObject;
                Object.DestroyImmediate(child);
            }

            Component[] components = obj.GetComponents<Component>();
            for (int i = components.Length - 1; i >= 0; i--)
            {
                if (components[i] is Transform)
                {
                    continue;
                }

                Object.DestroyImmediate(components[i]);
            }

            obj.transform.localScale = Vector3.one;
            obj.transform.rotation = Quaternion.identity;
        }
    }
}

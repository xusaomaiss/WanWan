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
    }
}

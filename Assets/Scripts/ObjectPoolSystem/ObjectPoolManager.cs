using System;
using System.Collections;
using System.Collections.Generic;
using MFrame.Common;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MFrame.ObjectPoolSystem
{
    public class ObjectPoolManager : MonoBehaviour
    {
        #region Singleton

        private static ObjectPoolManager _instance = null;

        public static ObjectPoolManager Instance
        {
            get{
                if (!_instance)
                {
                    GameObject obj = new GameObject("ObjectPoolManager");
                    DontDestroyOnLoad(obj);
                    _instance = obj.AddComponent<ObjectPoolManager>();
                }

                return _instance;
            }
        }

        #endregion

        protected Dictionary<GameObject, ObjectPool> prefab2pool;
        protected Dictionary<GameObject, ObjectPool> instance2pool;

        #region static method

        static void DestroyGameObject(ObjectPoolManager mgr, GameObject obj)
        {
            if (mgr)
            {
                mgr.InternalDestroy(obj);
            }
            else
            {
                Destroy(obj);
            }
        }

        static void DestroyGameObject(ObjectPoolManager mgr, GameObject obj, float delay, bool isIgnoreTimescale)
        {
            if (mgr)
            {
                if (isIgnoreTimescale)
                {
                    mgr.InternalDestroyIgnoreTimescal(obj, delay);
                }
                else
                {
                    mgr.InternalDestroy(obj, delay);
                }
            }
            else
            {
                Destroy(obj);
            }
        }

        public static void DestroyPooled(GameObject obj, float delay = 0)
        {
            if (delay > 0)
            {
                DestroyGameObject(_instance, obj, delay, true);
            }
            else
            {
                DestroyGameObject(_instance, obj);
            }
        }

        private static ObjectPool IsObjectPooled(ObjectPoolManager mgr, GameObject obj)
        {
            ObjectPool pool;
            mgr.instance2pool.TryGetValue(obj, out pool);
            return pool;
        }

        public static ObjectPool IsPooled(GameObject obj)
        {
            return _instance ? IsObjectPooled(_instance, obj) : null;
        }

        private static PoolStatus GetObjectPoolStatus(ObjectPoolManager mgr, GameObject obj)
        {
            if (obj)
            {
                ObjectPool pool;
                if (mgr.instance2pool.TryGetValue(obj, out pool))
                {
                    return pool.Constains(obj) ? PoolStatus.Recycled : PoolStatus.Pooled;
                }
            }

            return PoolStatus.None;
        }

        public static PoolStatus GetPoolStatus(GameObject obj)
        {
            return _instance ? GetObjectPoolStatus(_instance, obj) : PoolStatus.None;
        }

        private static GameObject AddChild(ObjectPoolManager mgr, GameObject parent, GameObject child, int siblingIdx)
        {
            GameObject go = null;
            if (child)
            {
                if (mgr)
                {
                    go = mgr.InternalCreate(child, Vector3.zero, Quaternion.identity, parent ? parent.transform : null,
                        siblingIdx);
                }
                else
                {
                    go = Instantiate(child);
                    ObjectPool.InitObjectTransform(go, parent ? parent.transform : null, Vector3.zero,
                        Quaternion.identity, siblingIdx);
                }

                if (go)
                {
                    Transform tsf = go.transform;
                    if (parent)
                    {
                        go.layer = parent.layer;
                    }

                    tsf.localScale = Vector3.one;
                    go.name = child.name;
                }
            }

            return go;
        }

        public static GameObject AddChild(GameObject parent, GameObject child, int siblingIdx = -1)
        {
            return _instance ? AddChild(_instance, parent, child, siblingIdx) : null;
        }

        #endregion


        #region private method

        private void Awake()
        {
            prefab2pool = new Dictionary<GameObject, ObjectPool>();
            instance2pool = new Dictionary<GameObject, ObjectPool>();
        }

        private ObjectPool CreatePool(GameObject prefab)
        {
            GameObject obj = new GameObject(prefab.name + "Pool");
            var pool = obj.AddComponent<ObjectPool>();
            pool.Prefab = prefab;
            return pool;
        }

        private GameObject InternalCreate(GameObject prefab, Vector3 pos, Quaternion rotation, Transform parent, int siblingIdx)
        {
            //TODO: 这里有一个疑问，如果prefab的AssetBundle卸载后重新加载，这时load返回的prefab是不是和第一次的prefab就不一样了
            //TODO: 如果不一样的话，prefab2pool是不是会无法释放，一直存在内存中，直到退出游戏
            ObjectPool pool;
            if (!prefab2pool.TryGetValue(prefab, out pool))
            {
                pool = CreatePool(prefab);
                pool.transform.parent = transform;
                prefab2pool[prefab] = pool;
            }

            GameObject obj = pool.Instantiate(pos, rotation, parent, siblingIdx);

            instance2pool[obj] = pool;

            return obj;
        }

        private void InternalDestroy(GameObject obj)
        {
            if (obj)
            {
                ObjectPool pool;
                if (instance2pool.TryGetValue(obj, out pool))
                {
                    pool.Recycle(obj);
                }
                else
                {
                    MLog.W("Destroying non-pooled object [{0}]", obj.name);
                    Object.Destroy(obj);
                }
            }
        }

        private IEnumerator InternalDestroyIgnoreTimescal(GameObject obj, float delay)
        {
            if (obj)
            {
                var t = Time.unscaledTime + delay;
                for (;;)
                {
                    yield return null;
                    if (Time.unscaledTime >= t)
                    {
                        InternalDestroy(obj);
                        break;
                    }
                }
            }
        }

        private IEnumerable InternalDestroy(GameObject obj, float delay)
        {
            if (obj)
            {
                var t = Time.time + delay;
                for (;;)
                {
                    yield return null;
                    if (Time.time >= t)
                    {
                        InternalDestroy(obj);
                    }
                }
            }
        }

        #endregion

        private void OnDisable()
        {
            if (instance2pool != null)
            {
                foreach (var item in instance2pool.Keys)
                {
                    Destroy(item);
                }
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
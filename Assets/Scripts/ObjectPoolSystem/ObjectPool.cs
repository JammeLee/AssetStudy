using System.Collections.Generic;
using MFrame.Common;
using UnityEngine;

namespace MFrame.ObjectPoolSystem
{
    public enum PoolStatus
    {
        None,
        Pooled,
        Recycled
    }
    public class ObjectPool : MonoBehavior
    {
        //模板
        public GameObject Prefab;

        private Queue<GameObject> pool;

        public int Count
        {
            get { return pool.Count; }
        }

        public void Awake()
        {
            pool = new Queue<GameObject>();
        }

        public GameObject Instantiate(Vector3 position, Quaternion rotation, Transform parent, int siblingIdx)
        {
            //尝试从缓存中拿出一个对象
            GameObject obj = pool.Count > 0 ? pool.Dequeue() : null;

            if (obj)
            {
                InitObjectTransform(obj, parent, position, rotation, siblingIdx);
                obj.SetActive(true);

                var list = ListPool<Component>.Get();
                obj.GetComponents(typeof(IPoolable), list);
                if (list.Count > 0)
                {
                    obj.hideFlags &= ~HideFlags.HideInHierarchy;
                    foreach (var item in list)
                    {
                        ((IPoolable)item).OnRestart();
                    }
                }
                else
                {
                    //如果该对象创建后没有调用过Start方法，则在此会调用第二次
                    obj.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
                }
                ListPool<Component>.Release(list);
            }
            else
            {
                obj = Object.Instantiate(Prefab);
                //TODO: AssetLoader.AssignEditorShaders(obj);
                InitObjectTransform(obj, parent, position, rotation, siblingIdx);
            }

            return obj;
        }

        public void Recycle(GameObject obj)
        {
            if (!pool.Contains(obj))
            {
                pool.Enqueue(obj);

                if (!obj.activeInHierarchy)
                {
                    MLog.D("被回收对象[{0}]不是Active状态，所以无法接收到OnRecycle消息", obj);
                }

                var list = ListPool<Component>.Get();
                obj.GetComponents(typeof(IPoolable), list);
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        ((IPoolable)item).OnRecycle();
                    }

                    obj.hideFlags |= HideFlags.HideInHierarchy;
                }
                else
                {
                    obj.SendMessage("OnRecycle", SendMessageOptions.DontRequireReceiver);
                    //Deactive
                    obj.SetActive(false);
                    //把回收对象放入此对象池下
                    obj.transform.SetParent(transform, false);
                }
                ListPool<Component>.Release(list);
            }
            else
            {
                MLog.I("正在尝试回收一个已回收对象：[{0}]", obj.name);
            }
        }

        public static void InitObjectTransform(GameObject obj, Transform parent, Vector3 position, Quaternion rotation,
            int siblingIdx)
        {
            var tsf = obj.transform;
            tsf.SetParent(parent, false);
            tsf.localRotation = rotation;
            var rect = tsf as RectTransform;
            if (rect)
            {
                rect.anchoredPosition3D = position;
            }
            else
            {
                tsf.localPosition = position;
            }

            if (parent && siblingIdx != -1)
            {
                var childCount = parent.childCount;
                if (siblingIdx < 0)
                {
                    tsf.SetSiblingIndex(childCount + siblingIdx);
                }
                else
                {
                    tsf.SetSiblingIndex(siblingIdx);
                }
            }
        }

        public bool Constains(GameObject obj)
        {
            return pool.Contains(obj);
        }

        private void OnDisable()
        {
            if (pool != null)
            {
                while (pool.Count > 0)
                {
                    var item = pool.Dequeue();
                    if (item != null)
                    {
                        Destroy(item);
                    }
                }
            }
        }
    }
}
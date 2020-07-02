using System.Collections.Generic;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace MFrame.Common
{
    public abstract class PoolBase<T>
    {
        protected abstract T New();
        
        private readonly Stack<T> m_Stack = new Stack<T>();
        private readonly System.Action<T> m_OnGet;
        private readonly System.Action<T> m_OnRelease;
        private readonly int m_Limit;
        
        public int countAll { get; private set; }
        public int countActive
        {
            get { return countAll - countInactive; }
        }

        public int countInactive
        {
            get
            {
                lock (m_Stack)
                {
                    return m_Stack.Count;
                }
            }
        }

        public PoolBase(System.Action<T> onGet, System.Action<T> onRelease, int limit)
        {
            m_OnGet = onGet;
            m_OnRelease = onRelease;
            m_Limit = limit;
        }

        public T Get()
        {
            lock (m_Stack)
            {
                T element;
                if (m_Stack.Count == 0)
                {
                    element = New();
                    countAll++;
                    MLog.I("Pool add {0}", this);
                }
                else
                {
                    element = m_Stack.Pop();
                }

                if (m_OnGet != null)
                {
                    m_OnGet(element);
                }

                return element;
            }
        }

        public void Release(T element)
        {
            lock (m_Stack)
            {
                if (m_Limit > 0)
                {
                    if (m_Stack.Count == m_Limit)
                    {
                        countAll--;
                        return;
                    }
                    Assert.IsTrue(m_Stack.Count < m_Limit);
                }

                if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                {
                    UnityEngine.Debug.LogErrorFormat("{0} is already released to pool", element);
                }

                if (m_OnRelease != null)
                {
                    m_OnRelease(element);
                }
                m_Stack.Push(element);
            }
        }

        public void Clear()
        {
            lock (m_Stack)
            {
                m_Stack.Clear();
            }

            countAll = 0;
        }

        public override string ToString()
        {
            return string.Format("<{0}>: {1}/{2}", typeof(T), countActive, countAll);
        }
    }

    public class Pool<T> : PoolBase<T> where T : new()
    {
        public Pool(System.Action<T> onGet, System.Action<T> onRelease, int limit = 0) : base(onGet, onRelease, limit)
        {
        }

        protected override T New()
        {
            return new T();
        }
    }

    public class ObjPool<T> : PoolBase<T> where T : Object
    {
        private readonly T m_Obj;
        public ObjPool(T Obj, System.Action<T> onGet, System.Action<T> onRelease, int limit = 0) : base(onGet, onRelease, limit)
        {
            m_Obj = Obj;
        }

        protected override T New()
        {
            return Object.Instantiate(m_Obj);
        }

        public override string ToString()
        {
            return string.Format("<{0}>: {1}/{2}", m_Obj, countActive, countAll);
        }
    }

    public static class ListPool<T>
    {
        private static readonly Pool<List<T>> m_ListPool = new Pool<List<T>>(null, l=>l.Clear());

        public static List<T> Get()
        {
            return m_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            m_ListPool.Release(toRelease);
        }

        public static void Clear()
        {
            m_ListPool.Clear();
        }

        public static string Info
        {
            get { return m_ListPool.ToString(); }
        }
    }

    public class Poolable<T> where T : Poolable<T>, new()
    {
        protected static readonly Pool<T> _Pool = new Pool<T>(t=>t.OnGet(), t=>t.OnRelease());

        public static T Get()
        {
            return _Pool.Get();
        }

        public static void Release(T element)
        {
            _Pool.Release(element);
        }
        
        protected virtual void OnGet(){}
        protected virtual void OnRelease(){}

        public static string Info
        {
            get { return _Pool.ToString(); }
        }
    }
    
    
    
    
    
    
    
    
    
}
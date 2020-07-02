
namespace MFrame.Common
{
    // Mono单例
    public class MonoSingleton<T> : MonoBehavior where T : MonoBehavior
    {
        public static T Instance { get; private set; }

        protected virtual void Awaking() { }
        
        protected virtual void Destroying() {}

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this as T;
            }
            else
            {
                MLog.E("单例{0}重复出现，已销毁", typeof(T).FullName);
                Destroy(gameObject);
            }
            Awaking();
        }

        private void OnDestroy()
        {
            if (this == Instance)
            {
                Destroying();
                Instance = null;
            }
        }
    }

    public class Singleton<T> where T : class, new()
    {
        protected Singleton() { }

        private static T m_Instance;

        public static T Instance
        {
            get
            {
                if (null == m_Instance)
                {
                    m_Instance = new T();
                }

                return m_Instance;
            }
        }

        public static void Release()
        {
            m_Instance = null;
        }
    }
}
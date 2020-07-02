using UnityEngine;

namespace MFrame.Common
{
    public class MonoBehavior : MonoBehaviour
    {
        private Transform m_Trans;
        public Transform CachedTransform{get{
            if (!m_Trans)
            {
                m_Trans = transform;
            }

            return m_Trans;
        }}
    }
}
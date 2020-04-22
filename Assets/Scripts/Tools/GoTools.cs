using UnityEngine;

namespace MFrame.Tools
{
    public static class GoTools
    {
        /// <summary>
        /// 以圆心center，做a到b的球形差值。
        /// </summary>
        /// <param name="a">起始位置</param>
        /// <param name="b">目标位置</param>
        /// <param name="t">单位时间</param>
        /// <param name="center">圆心</param>
        /// <returns></returns>
        public static Vector3 Slerp(Vector3 a, Vector3 b, float t, Vector3 center)
        {
            Vector3 res = Vector3.zero;
            var vA = a - center;
            var vB = b - center;

            res = Vector3.Slerp(vA, vB, t);
            res += center;

            return res;
        }
    }
}
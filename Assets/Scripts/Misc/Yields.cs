using System.Collections.Generic;
using MFrame.Generic;
using UnityEngine;

namespace MFrame.Misc
{
    public static class Yields
    {
        public static readonly WaitForEndOfFrame EndOfFrame = new WaitForEndOfFrame();

        private static readonly Dictionary<Boxing<int>, WaitForSeconds> WaitForSecondsPool =
            new Dictionary<Boxing<int>, WaitForSeconds>();

        public static WaitForSeconds Seconds(float seconds)
        {
            var secInt = Mathf.CeilToInt(seconds * 10);

            var waitForSeconds = WaitForSecondsPool.GetValue(secInt);
            if (waitForSeconds == null)
            {
                waitForSeconds = new WaitForSeconds(seconds / 10f);
                WaitForSecondsPool.Add(secInt, waitForSeconds);
            }

            return waitForSeconds;
        }

        private static readonly Dictionary<Boxing<int>, WaitForSecondsRealtime> WaitForSecondsRealtimePool =
            new Dictionary<Boxing<int>, WaitForSecondsRealtime>();

        public static WaitForSecondsRealtime RealSeconds(float seconds)
        {
            var secInt = Mathf.CeilToInt(seconds * 10);

            var waitForSecondsRealtime = WaitForSecondsRealtimePool.GetValue(secInt);
            if (waitForSecondsRealtime == null)
            {
                waitForSecondsRealtime = new WaitForSecondsRealtime(secInt / 10f);
                WaitForSecondsRealtimePool.Add(secInt, waitForSecondsRealtime);
            }

            return waitForSecondsRealtime;
        }
    }
}
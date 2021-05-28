using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MFrame.Generic
{
    public static class DicBoxingExtensions
    {
        public static TValue GetValue<TKey, TValue>(this IDictionary<Boxing<TKey>, TValue> self, TKey key,
            TValue defValue = default(TValue)) where TKey : struct where TValue : class
        {
            var boxingKey = Boxing<TKey>.Key;
            boxingKey.Value = key;

            TValue value;
            return self.TryGetValue(boxingKey, out value) ? value : defValue;
        }

        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, Boxing<TValue>> self, TKey key,
            TValue defValue = default(TValue)) where TKey : class where TValue : struct
        {
            Boxing<TValue> value;

            return self.TryGetValue(key, out value) ? value.Value : defValue;
        }

        public static TValue GetValue<TKey, TValue>(this IDictionary<Boxing<TKey>, Boxing<TValue>> self, TKey key,
            TValue defValue = default(TValue)) where TKey : struct where TValue : struct
        {
            var boxingKey = Boxing<TKey>.Key;
            boxingKey.Value = key;

            Boxing<TValue> value;

            return self.TryGetValue(boxingKey, out value) ? value.Value : defValue;
        }

        public static bool Remove<TKey, TValue>(this IDictionary<Boxing<TKey>, TValue> self, TKey key)
            where TKey : struct
        {
            var boxingKey = Boxing<TKey>.Key;
            boxingKey.Value = key;

            return self.Remove(boxingKey);
        }
    }
}
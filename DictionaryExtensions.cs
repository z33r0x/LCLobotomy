using System.Collections.Generic;
using System.Linq;

public static class DictionaryExtensions
{
    public static bool HasNullKeys<TKey, TValue>(this Dictionary<TKey, TValue> dictionary) where TKey : UnityEngine.Object
    {
        foreach (var key in dictionary.Keys)
        {
            if (key == null)
            {
                return true;
            }
        }

        return false;
    }


    public static void RemoveNullKeys<TKey, TValue>(this Dictionary<TKey, TValue> dictionary) where TKey : UnityEngine.Object
    {
        if (!dictionary.HasNullKeys())
        {
            return;
        }

        foreach (var key in dictionary.Keys.ToArray())
        {
            if (key == null)
            {
                dictionary.Remove(key);
            }
        }
    }
}
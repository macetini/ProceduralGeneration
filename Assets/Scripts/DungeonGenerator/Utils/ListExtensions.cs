using System;
using System.Collections.Generic;

namespace Assets.Scripts.DungeonGenerator.Utils
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> list, Random rnd)
        {
            for (var i = 0; i < list.Count; i++)
            {
                list.Swap(i, rnd.Next(i, list.Count));
            }
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            (list[j], list[i]) = (list[i], list[j]);
        }

        public static bool WithinRange<T>(this IList<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }

        public static T[] RemoveFromArray<T>(this T[] original, T itemToRemove)
        {
            int numIdx = Array.IndexOf(original, itemToRemove);
            if (numIdx == -1)
            {
                return original;
            }

            List<T> tmp = new(original);
            tmp.RemoveAt(numIdx);

            return tmp.ToArray();
        }
    }
}
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Utils
{
    public static class ListExtensions
    {
        public static IList<object> CloneList<T>(this IList<T> list) where T : ICloneable
        {
            if (list == null) return null;

            IList<object> clonedList = new List<object>(list.Count);
            foreach (ICloneable item in list)
            {
                object clonedItem = item.Clone();
                clonedList.Add(clonedItem);
            }

            return clonedList;
        }

        //TODO - Check all shuffled list with set random seed. Maybe new seed every time is better?
        public static void Shuffle<T>(this IList<T> list, Random rnd = null)
        {
            rnd ??= new Random();

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

        //TODO - Put in separate class.
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
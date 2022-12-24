using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary
{
    public static class Extensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
                yield return item;
            }
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var i = 0;
            foreach (var item in source)
            {
                action(item, i);
                yield return item;
                i++;
            }
        }

        public static void Swap<T>(this T[] array, int first, int second)
        {
            var temp = array[first];
            array[first] = array[second];
            array[second] = temp;
        }

        public static int FirstIndex<T>(this IEnumerable<T> array, Func<T, bool> predicate)
        {
            var i = 0;
            foreach (var item in array)
            {
                if (predicate(item))
                    break;
                i++;
            }
            return i;
        }

        public static void Shuffle<T>(this T[] array)
        {
            var random = new Random();
            for (int i = 0; i < array.Length; i++)
            {
                array.Swap(i, random.Next(0, array.Length));
            }
        }
    }
}

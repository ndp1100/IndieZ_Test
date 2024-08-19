using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Game.Utilities
{
    public static class ExtensionMethods
    {
        public static T GetRandom<T>(this IEnumerable<T> collection)
        {
            var count = collection.Count();
            if (count != 0)
            {
                return collection.ElementAt(UnityEngine.Random.Range(0, count));
            }
            return default(T);
        }

        public static T GetRandom<T>(this IEnumerable<T> collection, Random random)
        {
            var count = collection.Count();
            if (count != 0)
            {
                return collection.ElementAt(random.Next(0, count));
            }
            return default(T);
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Queue<T> lastElements = new Queue<T>();
            foreach (T element in source)
            {
                lastElements.Enqueue(element);
                if (lastElements.Count > count)
                {
                    lastElements.Dequeue();
                }
            }

            return lastElements;
        }

        public static void SetActiveIfNot(this GameObject go, bool enable)
        {
            if (go != null && go.activeSelf != enable)
                go.SetActive(enable);
        }

        public static Transform FindChildRecursive(this Transform current, string name)
        {
            //sb.Append(current.name + "\n");
 
            // check if the current bone is the bone we're looking for, if so return it
            if (current.name == name)
                return current;
 
            // search through child bones for the bone we're looking for
            for (int i = 0; i < current.childCount; ++i)
            {
                // the recursive step; repeat the search one step deeper in the hierarchy
                var child = current.GetChild(i);
 
                Transform found = FindChildRecursive(child, name);
 
                // a transform was returned by the search above that is not null,
                // it must be the bone we're looking for
                if (found != null)
                    return found;
            }
 
            // bone with name was not found
            return null;
        }
    }
}
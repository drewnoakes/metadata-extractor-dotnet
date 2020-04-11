using System;
using System.Collections.Generic;
using System.Linq;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    public static class BoxQuery
    {
        public static IEnumerable<T> Descendants<T>(this IEnumerable<Box> source) where T:Box =>
            source.Traverse(b=>b.Children()).OfType<T>();
        public static T Descendant<T>(this IEnumerable<Box> source) where T:Box =>
            source.Descendants<T>().FirstOrDefault();

        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> roots, Func<T,IEnumerable<T>> children)
        {
            var queue = new Queue<T>();
            EnqueAll(roots);
            while(queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;
                EnqueAll(children(current));
            }
            void EnqueAll(IEnumerable<T> all)
            {
                foreach (var item in all)
                {
                    queue.Enqueue(item);
                }
            }
        }
    }
}

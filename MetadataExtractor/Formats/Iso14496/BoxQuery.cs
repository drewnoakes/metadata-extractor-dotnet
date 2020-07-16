// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace MetadataExtractor.Formats.Iso14496
{
    internal static class BoxQuery
    {
        public static T Descendant<T>(this IEnumerable<Box> source) where T : Box
        {
            return source.Descendants().OfType<T>().FirstOrDefault();
        }

        public static IEnumerable<Box> Descendants(this IEnumerable<Box> roots)
        {
            var queue = new Queue<Box>();
            EnqueueAll(roots);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;
                EnqueueAll(current.Children());
            }

            void EnqueueAll(IEnumerable<Box> all)
            {
                foreach (var item in all)
                {
                    queue.Enqueue(item);
                }
            }
        }
    }
}

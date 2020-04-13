// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MetadataExtractor.Formats.Heif.Iso14496Parser
{
    internal static class BoxQuery
    {
        public static IEnumerable<T> Descendants<T>(this IEnumerable<Box> source) where T : Box =>
            source.Traverse(b => b.Children()).OfType<T>();

        public static T Descendant<T>(this IEnumerable<Box> source) where T : Box =>
            source.Descendants<T>().FirstOrDefault();

        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> roots, Func<T, IEnumerable<T>> children)
        {
            var queue = new Queue<T>();
            EnqueueAll(roots);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;
                EnqueueAll(children(current));
            }

            void EnqueueAll(IEnumerable<T> all)
            {
                foreach (var item in all)
                {
                    queue.Enqueue(item);
                }
            }
        }
    }
}

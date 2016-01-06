﻿using System.Collections.Generic;

namespace ML.BC.Infrastructure
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items)
        {
            if (items == null)
            {
                return;
            }
            foreach (var item in items)
            {
                source.Add(item);
            }
        }
    }
}

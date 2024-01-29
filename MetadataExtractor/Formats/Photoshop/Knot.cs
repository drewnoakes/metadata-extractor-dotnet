// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.Photoshop
{
    /// <summary>
    /// Represents a knot created by Photoshop:
    /// <list type="bullet">
    /// <item>Linked knot</item>
    /// <item>Unlinked knot</item>
    /// </list>
    /// </summary>
    /// <author>Payton Garland</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public class Knot(string type)
    {
        private readonly double[] _points = new double[6];

        /// <summary>
        /// Add/Get an individual coordinate value (x or y) to/from
        /// points array (6 points per knot)
        /// </summary>
        /// <param name="index"></param>
        /// <returns>an individual coordinate value</returns>
        /// <remarks>Define the indexer to allow client code to use [] notation</remarks>
        public double this[int index]
        {
            get => _points[index];
            set => _points[index] = value;
        }

        /// <summary>
        /// Get the type of knot (linked or unlinked)
        /// </summary>
        public string Type { get; } = type;
    }
}

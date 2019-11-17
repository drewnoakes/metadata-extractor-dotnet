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
    public class Knot
    {
        private double[] p_points = new double[6];

        public Knot(string type)
        {
            Type = type;
        }

        /// <summary>
        /// Add an individual coordinate value (x or y) to
        /// points array (6 points per knot)
        /// </summary>
        /// <param name="index">location of point to be added in points</param>
        /// <param name="point">coordinate value to be added to points</param>
        public void SetPoint(int index, double point)
        {
            p_points[index] = point;
        }

        /// <summary>
        /// Get an individual coordinate value (x or y)
        /// </summary>
        /// <param name="index"></param>
        /// <returns>an individual coordinate value</returns>
        public double GetPoint(int index)
        {
            return p_points[index];
        }

        /// <summary>
        /// Get the type of knot (linked or unlinked)
        /// </summary>
        public string Type { get; private set; }
    }
}

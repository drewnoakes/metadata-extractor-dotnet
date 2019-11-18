// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace MetadataExtractor.Formats.Photoshop
{
    /// <summary>
    /// Represents a subpath created by Photoshop:
    /// <list type="bullet">
    /// <item>Closed Bezier knot, linked</item>
    /// <item>Closed Bezier knot, unlinked</item>
    /// <item>Open Bezier knot, linked</item>
    /// <item>Open Bezier knot, unlinked</item>
    /// </list>
    /// </summary>
    /// <author>Payton Garland</author>
    /// <author>Kevin Mott https://github.com/kwhopper</author>
    public class Subpath
    {
        private readonly List<Knot> _knots = new List<Knot>();

        public Subpath(string type = "")
        {
            Type = type;
        }

        /// <summary>
        /// Appends a knot (set of 3 points) into the list
        /// </summary>
        /// <param name="knot"></param>
        public void Add(Knot knot)
        {
            _knots.Add(knot);
        }

        /// <summary>
        /// Gets size of knots list
        /// </summary>
        /// <returns>size of knots List</returns>
        public int KnotCount
        {
            get { return _knots.Count; }
        }

        ///<summary>
        ///Return a read-only list of Knots
        ///</summary>
        public IEnumerable<Knot> Knots
        {
            get { return _knots; }
        }

        public string Type { get; }
    }
}

// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Text;
using MetadataExtractor.Formats.Exif.Makernotes;

namespace MetadataExtractor
{
    /// <summary>Models information about a face in an image.</summary>
    /// <remarks>
    /// When a face is detected, the camera believes that a face is present at a given location in
    /// the image, but is not sure whose face it is.  When a face is recognised, then the face is
    /// both detected and identified as belonging to a known person.
    /// <para />
    /// Currently this is only used by <see cref="PanasonicMakernoteDirectory"/>.
    /// </remarks>
    /// <author>Philipp Sandhaus, Drew Noakes</author>
    public sealed class Face
    {
        public Face(int x, int y, int width, int height, string? name = null, Age? age = null)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Name = name;
            Age = age;
        }

        public int X { get; }

        public int Y { get; }

        public int Width { get; }

        public int Height { get; }

        public string? Name { get; }

        public Age? Age { get; }

        #region Equality and hashing

        private bool Equals(Face other)
        {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height && string.Equals(Name, other.Name) && Equals(Age, other.Age);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is Face face && Equals(face);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode*397) ^ Y;
                hashCode = (hashCode*397) ^ Width;
                hashCode = (hashCode*397) ^ Height;
                hashCode = (hashCode*397) ^ (Name?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (Age?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        #endregion

        public override string ToString()
        {
            var result = new StringBuilder();

            result.Append("x: ").Append(X);
            result.Append(" y: ").Append(Y);
            result.Append(" width: ").Append(Width);
            result.Append(" height: ").Append(Height);

            if (Name != null)
                result.Append(" name: ").Append(Name);

            if (Age != null)
                result.Append(" age: ").Append(Age.ToFriendlyString());

            return result.ToString();
        }
    }
}

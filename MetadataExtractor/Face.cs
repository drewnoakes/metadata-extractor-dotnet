// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

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
    public sealed class Face(int x, int y, int width, int height, string? name = null, Age? age = null) : IEquatable<Face>
    {
        public int X { get; } = x;

        public int Y { get; } = y;

        public int Width { get; } = width;

        public int Height { get; } = height;

        public string? Name { get; } = name;

        public Age? Age { get; } = age;

        #region Equality and hashing

        public bool Equals(Face? other)
        {
            if (ReferenceEquals(this, other))
                return true;
            return other is not null && X == other.X && Y == other.Y && Width == other.Width && Height == other.Height && string.Equals(Name, other.Name) && Equals(Age, other.Age);
        }

        public override bool Equals(object? obj)
        {
            return obj is Face face && Equals(face);
        }

        public override int GetHashCode()
        {
#if NET8_0_OR_GREATER
            HashCode hash = new();
            hash.Add(X);
            hash.Add(Y);
            hash.Add(Width);
            hash.Add(Height);
            hash.Add(Name);
            hash.Add(Age);
            return hash.ToHashCode();
#else
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Width;
                hashCode = (hashCode * 397) ^ Height;
                hashCode = (hashCode * 397) ^ (Name?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Age?.GetHashCode() ?? 0);
                return hashCode;
            }
#endif
        }

        #endregion

        public override string ToString()
        {
            var result = new StringBuilder();

            result.Append("x: ").Append(X);
            result.Append(" y: ").Append(Y);
            result.Append(" width: ").Append(Width);
            result.Append(" height: ").Append(Height);

            if (Name is not null)
                result.Append(" name: ").Append(Name);

            if (Age is not null)
                result.Append(" age: ").Append(Age.ToFriendlyString());

            return result.ToString();
        }
    }
}

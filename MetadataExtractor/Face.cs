#region License
//
// Copyright 2002-2017 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

using System.Text;
using JetBrains.Annotations;
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
        public Face(int x, int y, int width, int height, [CanBeNull] string name = null, [CanBeNull] Age age = null)
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

        [CanBeNull]
        public string Name { get; }

        [CanBeNull]
        public Age Age { get; }

        #region Equality and hashing

        private bool Equals([NotNull] Face other)
        {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height && string.Equals(Name, other.Name) && Equals(Age, other.Age);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
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

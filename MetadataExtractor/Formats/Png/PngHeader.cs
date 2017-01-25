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

using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngHeader
    {
        /// <exception cref="PngProcessingException"/>
        public PngHeader([NotNull] byte[] bytes)
        {
            if (bytes.Length != 13)
                throw new PngProcessingException("PNG header chunk must have exactly 13 data bytes");

            var reader = new SequentialByteArrayReader(bytes);

            ImageWidth = reader.GetInt32();
            ImageHeight = reader.GetInt32();
            BitsPerSample = reader.GetByte();
            ColorType = PngColorType.FromNumericValue(reader.GetByte());
            CompressionType = reader.GetByte();
            FilterMethod = reader.GetByte();
            InterlaceMethod = reader.GetByte();
        }

        public int ImageWidth { get; }

        public int ImageHeight { get; }

        public byte BitsPerSample { get; }

        [NotNull]
        public PngColorType ColorType { get; }

        public byte CompressionType { get; }

        public byte FilterMethod { get; }

        public byte InterlaceMethod { get; }
    }
}

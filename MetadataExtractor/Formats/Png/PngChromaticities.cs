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

using System.IO;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.Png
{
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class PngChromaticities
    {
        public int WhitePointX { get; }
        public int WhitePointY { get; }
        public int RedX { get; }
        public int RedY { get; }
        public int GreenX { get; }
        public int GreenY { get; }
        public int BlueX { get; }
        public int BlueY { get; }

        /// <exception cref="PngProcessingException"/>
        public PngChromaticities([NotNull] byte[] bytes)
        {
            if (bytes.Length != 8*4)
                throw new PngProcessingException("Invalid number of bytes");

            var reader = new SequentialByteArrayReader(bytes);

            try
            {
                WhitePointX = reader.GetInt32();
                WhitePointY = reader.GetInt32();
                RedX = reader.GetInt32();
                RedY = reader.GetInt32();
                GreenX = reader.GetInt32();
                GreenY = reader.GetInt32();
                BlueX = reader.GetInt32();
                BlueY = reader.GetInt32();
            }
            catch (IOException ex)
            {
                throw new PngProcessingException(ex);
            }
        }
    }
}

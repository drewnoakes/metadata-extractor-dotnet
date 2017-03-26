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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MetadataExtractor.Formats.Netpbm
{
    /// <summary>
    /// Reads metadata from Netpbm files.
    /// </summary>
    /// <remarks>
    /// Resources:
    /// <list type="bullet">
    ///     <item>https://en.wikipedia.org/wiki/Netpbm_format</item>
    ///     <item>http://netpbm.sourceforge.net/doc/ppm.html</item>
    ///     <item>http://paulbourke.net/dataformats/ppm/</item>
    /// </list>
    /// </remarks>
    /// <author>Drew Noakes https://drewnoakes.com</author>
    public sealed class NetpbmReader
    {
        public NetpbmHeaderDirectory Extract(Stream stream)
        {
            var directory = new NetpbmHeaderDirectory();

            var reader = new StreamReader(stream, Encoding.UTF8);

            using (var words = ReadWords(reader).GetEnumerator())
            {
                if (!words.MoveNext())
                    throw new IOException("Unexpected EOF.");
                var magic = words.Current;

                if (magic[0] != 'P')
                    throw new ImageProcessingException("Invalid Netpbm magic number");
                var magicNum = magic[1] - '0';
                if (magicNum < 1 || magicNum > 7)
                    throw new ImageProcessingException("Invalid Netpbm magic number");

                directory.Set(NetpbmHeaderDirectory.TagFormatType, magicNum);

                if (!words.MoveNext())
                    throw new IOException("Unexpected EOF.");

                if (!int.TryParse(words.Current, out int width))
                    throw new IOException("Width is not parseable as an integer.");

                directory.Set(NetpbmHeaderDirectory.TagWidth, width);

                if (!words.MoveNext())
                    throw new IOException("Unexpected EOF.");

                if (!int.TryParse(words.Current, out int height))
                    throw new IOException("Height is not parseable as an integer.");

                directory.Set(NetpbmHeaderDirectory.TagHeight, height);

                if (!words.MoveNext())
                    throw new IOException("Unexpected EOF.");

                if (magicNum != 1 && magicNum != 6)
                {
                    if (!int.TryParse(words.Current, out int maxValue))
                        throw new IOException("MaxValue is not parseable as an integer.");

                    directory.Set(NetpbmHeaderDirectory.TagMaximumValue, maxValue);
                }
            }

            return directory;
        }

        private static IEnumerable<string> ReadWords(TextReader reader)
        {
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                    yield break;

                var commentFromIndex = line.IndexOf('#');
                if (commentFromIndex != -1)
                    line = line.Substring(0, commentFromIndex);

                var words = line.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                    yield return word.Trim();
            }
        }
    }
}
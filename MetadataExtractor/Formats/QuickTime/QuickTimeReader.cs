#region License
//
// Copyright 2002-2017 Drew Noakes
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using MetadataExtractor.IO;

namespace MetadataExtractor.Formats.QuickTime
{
    /// <summary>
    /// Models data provided to callbacks invoked when reading QuickTime atoms via <see cref="QuickTimeReader.ProcessAtoms"/>.
    /// </summary>
    public sealed class AtomCallbackArgs
    {
        /// <summary>
        /// Gets the 32-bit unsigned integer that identifies the atom's type.
        /// </summary>
        public uint Type { get; }

        /// <summary>
        /// The length of the atom data, in bytes. If the atom extends to the end of the file, this value is zero.
        /// </summary>
        public long Size { get; }

        /// <summary>
        /// Gets the stream from which atoms are being read.
        /// </summary>
        public Stream Stream { get; }

        /// <summary>
        /// Gets the position within <see cref="Stream"/> at which this atom's data started.
        /// </summary>
        public long StartPosition { get; }

        /// <summary>
        /// Gets a sequential reader from which this atom's contents may be read.
        /// </summary>
        /// <remarks>
        /// It is backed by <see cref="Stream"/>, so manipulating the stream's position will influence this reader.
        /// </remarks>
        public SequentialStreamReader Reader { get; }

        /// <summary>
        /// Gets and sets whether the callback wishes processing to terminate.
        /// </summary>
        public bool Cancel { get; set; }

        public AtomCallbackArgs(uint type, long size, Stream stream, long startPosition, SequentialStreamReader reader)
        {
            Type = type;
            Size = size;
            Stream = stream;
            StartPosition = startPosition;
            Reader = reader;
        }

        /// <summary>
        /// Gets the string representation of this atom's type.
        /// </summary>
        [NotNull]
        public string TypeString
        {
            get
            {
                var bytes = BitConverter.GetBytes(Type);
                bytes = bytes.Reverse().ToArray();
#if NETSTANDARD1_3
                return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
#else
                return Encoding.ASCII.GetString(bytes);
#endif
            }
        }

        /// <summary>
        /// Computes the number of bytes remaining in the atom, given the <see cref="Stream"/> position.
        /// </summary>
        public long BytesLeft => Size - (Stream.Position - StartPosition);
    }

    /// <summary>
    /// Static class for processing atoms the QuickTime container format.
    /// </summary>
    /// <remarks>
    /// QuickTime file format specification: https://developer.apple.com/library/mac/documentation/QuickTime/QTFF/qtff.pdf
    /// </remarks>
    public static class QuickTimeReader
    {
        /// <summary>
        /// Reads atom data from <paramref name="stream"/>, invoking <paramref name="handler"/> for each atom encountered.
        /// </summary>
        /// <param name="stream">The stream to read atoms from.</param>
        /// <param name="handler">A callback function to handle each atom.</param>
        /// <param name="stopByBytes">The maximum number of bytes to process before discontinuing.</param>
        public static void ProcessAtoms([NotNull] Stream stream, [NotNull] Action<AtomCallbackArgs> handler, long stopByBytes = -1)
        {
            var reader = new SequentialStreamReader(stream);

            var seriesStartPos = stream.Position;

            while (stopByBytes == -1 || stream.Position < seriesStartPos + stopByBytes)
            {
                var atomStartPos = stream.Position;

                // Length of the atom's data, in bytes, including size bytes
                long atomSize;
                try
                {
                    atomSize = reader.GetUInt32();
                }
                catch (IOException)
                {
                    // TODO don't use exception to trap end of stream
                    return;
                }

                // Typically four ASCII characters, but may be non-printable.
                // By convention, lowercase 4CCs are reserved by Apple.
                var atomType = reader.GetUInt32();

                if (atomSize == 1)
                {
                    // Size doesn't fit in 32 bits so read the 64 bit size here
                    atomSize = checked((long)reader.GetUInt64());
                }
                else
                {
                    Debug.Assert(atomSize >= 8, "Atom should be at least 8 bytes long");
                }

                var args = new AtomCallbackArgs(atomType, atomSize, stream, atomStartPos, reader);

                handler(args);

                if (args.Cancel)
                    return;

                if (atomSize == 0)
                    return;

                var toSkip = atomStartPos + atomSize - stream.Position;

                if (toSkip < 0)
                    throw new Exception("Handler moved stream beyond end of atom");

                reader.TrySkip(toSkip);
            }
        }
    }
}
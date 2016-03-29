#region License
//
// Copyright 2002-2016 Drew Noakes
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
using System.Diagnostics;
using System.IO;

namespace MetadataExtractor.Formats.QuickTime
{
    public static class QuicktimeMetadataReader
    {
        private static readonly DateTimeOffset _epoch = new DateTimeOffset(1904, 1, 1, 0, 0, 0, new TimeSpan(0));

        public static
#if NET35 || PORTABLE
            IList<Directory>
#else
            IReadOnlyList<Directory>
#endif
            ReadMetadata(Stream stream)
        {
            var directories = new List<Directory>();

            Action<AtomCallbackArgs> trakHandler = a =>
            {
                switch (a.TypeString)
                {
                    case "tkhd":
                    {
                        var directory = new QuickTimeTrackHeaderDirectory();
                        directory.Set(QuickTimeTrackHeaderDirectory.TagVersion, a.Reader.GetByte());
                        directory.Set(QuickTimeTrackHeaderDirectory.TagFlags, a.Reader.GetBytes(3));
                        directory.Set(QuickTimeTrackHeaderDirectory.TagCreated, _epoch.AddTicks(TimeSpan.TicksPerSecond*a.Reader.GetUInt32()));
                        directory.Set(QuickTimeTrackHeaderDirectory.TagModified, _epoch.AddTicks(TimeSpan.TicksPerSecond*a.Reader.GetUInt32()));
                        directory.Set(QuickTimeTrackHeaderDirectory.TagTrackId, a.Reader.GetUInt32());
                        a.Reader.Skip(4L);
                        directory.Set(QuickTimeTrackHeaderDirectory.TagDuration, a.Reader.GetUInt32());
                        a.Reader.Skip(8L);
                        directory.Set(QuickTimeTrackHeaderDirectory.TagLayer, a.Reader.GetUInt16());
                        directory.Set(QuickTimeTrackHeaderDirectory.TagAlternateGroup, a.Reader.GetUInt16());
                        directory.Set(QuickTimeTrackHeaderDirectory.TagVolume, a.Reader.Get16BitFixedPoint());
                        a.Reader.Skip(2L);
                        a.Reader.GetBytes(36);
                        directory.Set(QuickTimeTrackHeaderDirectory.TagWidth, a.Reader.Get32BitFixedPoint());
                        directory.Set(QuickTimeTrackHeaderDirectory.TagHeight, a.Reader.Get32BitFixedPoint());
                        directories.Add(directory);
                        break;
                    }
                }
            };

//            Action<AtomCallbackArgs> clipHandler = a =>
//            {
//                Debug.WriteLine($"- Atom {a.TypeString} of size {a.Size}");
//            };

            var moovHandler = (Action<AtomCallbackArgs>) (a =>
            {
                Debug.WriteLine($"- Atom {a.TypeString} of size {a.Size}");

                switch (a.TypeString)
                {
                    case "mvhd":
                    {
                        var directory = new QuicktimeMovieHeaderDirectory();
                        directory.Set(QuicktimeMovieHeaderDirectory.TagVersion, a.Reader.GetByte());
                        directory.Set(QuicktimeMovieHeaderDirectory.TagFlags, a.Reader.GetBytes(3));
                        directory.Set(QuicktimeMovieHeaderDirectory.TagCreated, _epoch.AddTicks(TimeSpan.TicksPerSecond*a.Reader.GetUInt32()));
                        directory.Set(QuicktimeMovieHeaderDirectory.TagModified, _epoch.AddTicks(TimeSpan.TicksPerSecond*a.Reader.GetUInt32()));
                        var timeScale = a.Reader.GetUInt32();
                        directory.Set(QuicktimeMovieHeaderDirectory.TagTimeScale, timeScale);
                        directory.Set(QuicktimeMovieHeaderDirectory.TagDuration, TimeSpan.FromSeconds(a.Reader.GetUInt32()/(double) timeScale));
                        directory.Set(QuicktimeMovieHeaderDirectory.TagPreferredRate, a.Reader.Get32BitFixedPoint());
                        directory.Set(QuicktimeMovieHeaderDirectory.TagPreferredVolume, a.Reader.Get16BitFixedPoint());
                        a.Reader.Skip(10);
                        directory.Set(QuicktimeMovieHeaderDirectory.TagMatrix, a.Reader.GetBytes(36));
                        directory.Set(QuicktimeMovieHeaderDirectory.TagPreviewTime, a.Reader.GetUInt32());
                        directory.Set(QuicktimeMovieHeaderDirectory.TagPreviewDuration, a.Reader.GetUInt32());
                        directory.Set(QuicktimeMovieHeaderDirectory.TagPosterTime, a.Reader.GetUInt32());
                        directory.Set(QuicktimeMovieHeaderDirectory.TagSelectionTime, a.Reader.GetUInt32());
                        directory.Set(QuicktimeMovieHeaderDirectory.TagSelectionDuration, a.Reader.GetUInt32());
                        directory.Set(QuicktimeMovieHeaderDirectory.TagCurrentTime, a.Reader.GetUInt32());
                        directory.Set(QuicktimeMovieHeaderDirectory.TagNextTrackId, a.Reader.GetUInt32());
                        directories.Add(directory);
                        break;
                    }
                    case "trak":
                    {
                        QuickTimeReader.ProcessAtoms(stream, trakHandler, a.BytesLeft);
                        break;
                    }
//                    case "clip":
//                    {
//                        QuickTimeReader.ProcessAtoms(stream, clipHandler, a.BytesLeft);
//                        break;
//                    }
//                    case "prfl":
//                    {
//                        a.Reader.Skip(4L);
//                        var partId = a.Reader.GetUInt32();
//                        var featureCode = a.Reader.GetUInt32();
//                        var featureValue = string.Join(" ", a.Reader.GetBytes(4).Select(v => v.ToString("X2")).ToArray());
//                        Debug.WriteLine($"PartId={partId} FeatureCode={featureCode} FeatureValue={featureValue}");
//                        break;
//                    }
                }
            });

            Action<AtomCallbackArgs> handler = a =>
            {
                Debug.WriteLine($"- Atom {a.TypeString} of size {a.Size}");
                switch (a.TypeString)
                {
                    case "moov":
                    {
                        QuickTimeReader.ProcessAtoms(stream, moovHandler, a.BytesLeft);
                        break;
                    }
                    case "ftyp":
                    {
                        var directory = new QuicktimeFileTypeDirectory();
                        directory.Set(QuicktimeFileTypeDirectory.TagMajorBrand, a.Reader.Get4ccString());
                        directory.Set(QuicktimeFileTypeDirectory.TagMinorVersion, a.Reader.GetUInt32());
                        var compatibleBrands = new List<string>();
                        while (a.BytesLeft >= 4)
                            compatibleBrands.Add(a.Reader.Get4ccString());
                        directory.Set(QuicktimeFileTypeDirectory.TagCompatibleBrands, compatibleBrands);
                        directories.Add(directory);
                        break;
                    }
                }
            };

            QuickTimeReader.ProcessAtoms(stream, handler);

            return directories;
        }
    }
}
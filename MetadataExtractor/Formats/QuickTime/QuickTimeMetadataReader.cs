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
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace MetadataExtractor.Formats.QuickTime
{
    public static class QuickTimeMetadataReader
    {
        private static readonly DateTime _epoch = new DateTime(1904, 1, 1);

        [NotNull]
        public static DirectoryList ReadMetadata([NotNull] Stream stream)
        {
            var directories = new List<Directory>();

            void TrakHandler(AtomCallbackArgs a)
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
            }

            void MoovHandler(AtomCallbackArgs a)
            {
                switch (a.TypeString)
                {
                    case "mvhd":
                    {
                        var directory = new QuickTimeMovieHeaderDirectory();
                        directory.Set(QuickTimeMovieHeaderDirectory.TagVersion, a.Reader.GetByte());
                        directory.Set(QuickTimeMovieHeaderDirectory.TagFlags, a.Reader.GetBytes(3));
                        directory.Set(QuickTimeMovieHeaderDirectory.TagCreated, _epoch.AddTicks(TimeSpan.TicksPerSecond*a.Reader.GetUInt32()));
                        directory.Set(QuickTimeMovieHeaderDirectory.TagModified, _epoch.AddTicks(TimeSpan.TicksPerSecond*a.Reader.GetUInt32()));
                        var timeScale = a.Reader.GetUInt32();
                        directory.Set(QuickTimeMovieHeaderDirectory.TagTimeScale, timeScale);
                        directory.Set(QuickTimeMovieHeaderDirectory.TagDuration, TimeSpan.FromSeconds(a.Reader.GetUInt32()/(double) timeScale));
                        directory.Set(QuickTimeMovieHeaderDirectory.TagPreferredRate, a.Reader.Get32BitFixedPoint());
                        directory.Set(QuickTimeMovieHeaderDirectory.TagPreferredVolume, a.Reader.Get16BitFixedPoint());
                        a.Reader.Skip(10);
                        directory.Set(QuickTimeMovieHeaderDirectory.TagMatrix, a.Reader.GetBytes(36));
                        directory.Set(QuickTimeMovieHeaderDirectory.TagPreviewTime, a.Reader.GetUInt32());
                        directory.Set(QuickTimeMovieHeaderDirectory.TagPreviewDuration, a.Reader.GetUInt32());
                        directory.Set(QuickTimeMovieHeaderDirectory.TagPosterTime, a.Reader.GetUInt32());
                        directory.Set(QuickTimeMovieHeaderDirectory.TagSelectionTime, a.Reader.GetUInt32());
                        directory.Set(QuickTimeMovieHeaderDirectory.TagSelectionDuration, a.Reader.GetUInt32());
                        directory.Set(QuickTimeMovieHeaderDirectory.TagCurrentTime, a.Reader.GetUInt32());
                        directory.Set(QuickTimeMovieHeaderDirectory.TagNextTrackId, a.Reader.GetUInt32());
                        directories.Add(directory);
                        break;
                    }
                    case "trak":
                    {
                        QuickTimeReader.ProcessAtoms(stream, TrakHandler, a.BytesLeft);
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
            }

            void Handler(AtomCallbackArgs a)
            {
                switch (a.TypeString)
                {
                    case "moov":
                    {
                        QuickTimeReader.ProcessAtoms(stream, MoovHandler, a.BytesLeft);
                        break;
                    }
                    case "ftyp":
                    {
                        var directory = new QuickTimeFileTypeDirectory();
                        directory.Set(QuickTimeFileTypeDirectory.TagMajorBrand, a.Reader.Get4ccString());
                        directory.Set(QuickTimeFileTypeDirectory.TagMinorVersion, a.Reader.GetUInt32());
                        var compatibleBrands = new List<string>();
                        while (a.BytesLeft >= 4)
                            compatibleBrands.Add(a.Reader.Get4ccString());
                        directory.Set(QuickTimeFileTypeDirectory.TagCompatibleBrands, compatibleBrands);
                        directories.Add(directory);
                        break;
                    }
                }
            }

            QuickTimeReader.ProcessAtoms(stream, Handler);

            return directories;
        }
    }
}
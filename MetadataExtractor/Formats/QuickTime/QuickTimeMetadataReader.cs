// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Exif.Makernotes;
using MetadataExtractor.Formats.Tiff;
using MetadataExtractor.Formats.Xmp;
using MetadataExtractor.IO;
using MetadataExtractor.Util;
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

        public static DirectoryList ReadMetadata(Stream stream)
        {
            var directories = new List<Directory>();

            QuickTimeReader.ProcessAtoms(stream, Handler);

            return directories;

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
                        directory.Set(QuickTimeTrackHeaderDirectory.TagMatrix, a.Reader.GetMatrix());
                        directory.Set(QuickTimeTrackHeaderDirectory.TagWidth, a.Reader.Get32BitFixedPoint());
                        directory.Set(QuickTimeTrackHeaderDirectory.TagHeight, a.Reader.Get32BitFixedPoint());
                        SetRotation(directory);
                        directories.Add(directory);
                        break;
                    }
                }
            }

            static void SetRotation(QuickTimeTrackHeaderDirectory directory)
            {
                var width = directory.GetInt32(QuickTimeTrackHeaderDirectory.TagWidth);
                var height = directory.GetInt32(QuickTimeTrackHeaderDirectory.TagHeight);
                if (width == 0 || height == 0 || directory.GetObject(QuickTimeTrackHeaderDirectory.TagRotation) != null) return;

                if (directory.GetObject(QuickTimeTrackHeaderDirectory.TagMatrix) is float[] matrix && matrix.Length > 5)
                {
                    var x = matrix[1] + matrix[4];
                    var y = matrix[0] + matrix[3];
                    var theta = Math.Atan2(x, y);
                    var degree = ((180 / Math.PI) * theta) - 45;
                    if (degree < 0)
                        degree += 360;

                    directory.Set(QuickTimeTrackHeaderDirectory.TagRotation, degree);
                }
            }

            void UuidHandler(AtomCallbackArgs a)
            {
                switch (a.TypeString)
                {
                    case "CMT1":
                    {
                        var handler = new QuickTimeTiffHandler<ExifIfd0Directory>(directories);
                        var reader = new IndexedSeekingReader(a.Stream, (int)a.Reader.Position);
                        TiffReader.ProcessTiff(reader, handler);
                        break;
                    }
                    case "CMT2":
                    {
                        var handler = new QuickTimeTiffHandler<ExifSubIfdDirectory>(directories);
                        var reader = new IndexedSeekingReader(a.Stream, (int)a.Reader.Position);
                        TiffReader.ProcessTiff(reader, handler);
                        break;
                    }
                    case "CMT3":
                    {
                        var handler = new QuickTimeTiffHandler<CanonMakernoteDirectory>(directories);
                        var reader = new IndexedSeekingReader(a.Stream, (int)a.Reader.Position);
                        TiffReader.ProcessTiff(reader, handler);
                        break;
                    }
                    case "CMT4":
                    {
                        var handler = new QuickTimeTiffHandler<GpsDirectory>(directories);
                        var reader = new IndexedSeekingReader(a.Stream, (int)a.Reader.Position);
                        TiffReader.ProcessTiff(reader, handler);
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
                    case "uuid":
                    {
                        var CR3 = new byte[] { 0x85, 0xc0, 0xb6, 0x87, 0x82, 0x0f, 0x11, 0xe0, 0x81, 0x11, 0xf4, 0xce, 0x46, 0x2b, 0x6a, 0x48 };
                        var uuid = a.Reader.GetBytes(CR3.Length);
                        if (CR3.RegionEquals(0, CR3.Length, uuid))
                        {
                            QuickTimeReader.ProcessAtoms(stream, UuidHandler, a.BytesLeft);
                        }
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
                    case "uuid":
                    {
                        var XMP = new byte[] { 0xbe, 0x7a, 0xcf, 0xcb, 0x97, 0xa9, 0x42, 0xe8, 0x9c, 0x71, 0x99, 0x94, 0x91, 0xe3, 0xaf, 0xac };
                        if (a.BytesLeft >= XMP.Length)
                        {
                            var uuid = a.Reader.GetBytes(XMP.Length);
                            if (XMP.RegionEquals(0, XMP.Length, uuid))
                            {
                                var xmpBytes = a.Reader.GetNullTerminatedBytes((int)a.BytesLeft);
                                var xmpDirectory = new XmpReader().Extract(xmpBytes);
                                directories.Add(xmpDirectory);
                            }
                        }
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
#if NET35
                        directory.Set(QuickTimeFileTypeDirectory.TagCompatibleBrands, string.Join(", ", compatibleBrands.ToArray()));
#else
                        directory.Set(QuickTimeFileTypeDirectory.TagCompatibleBrands, string.Join(", ", compatibleBrands));
#endif
                        directories.Add(directory);
                        break;
                    }
                }
            }
        }
    }
}

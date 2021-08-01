// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

#pragma warning disable CS8602 // Dereference of a possibly null reference
#pragma warning disable CS8604 // Dereference of a possibly null reference

namespace MetadataExtractor.Formats.QuickTime
{
    public static class QuickTimeMetadataReader
    {
        private static readonly DateTime _epoch = new(1904, 1, 1);
        private static readonly int[] _supportedAtomValueTypes = { 1, 13, 14, 23, 27 };

        public static DirectoryList ReadMetadata(Stream stream)
        {
            var directories = new List<Directory>();
            var metaDataKeys = new List<string>();
            QuickTimeMetadataHeaderDirectory? metaHeaderDirectory = null;

            QuickTimeReader.ProcessAtoms(stream, Handler);

            return directories;

            QuickTimeMetadataHeaderDirectory GetMetaHeaderDirectory()
            {
                if (metaHeaderDirectory == null)
                {
                    metaHeaderDirectory = new QuickTimeMetadataHeaderDirectory();
                    directories.Add(metaHeaderDirectory);
                }

                return metaHeaderDirectory;
            }

            void TrakHandler(AtomCallbackArgs a)
            {
                switch (a.TypeString)
                {
                    case "tkhd":
                    {
                        var directory = new QuickTimeTrackHeaderDirectory();
                        directory.Set(QuickTimeTrackHeaderDirectory.TagVersion, a.Reader.GetByte());
                        directory.Set(QuickTimeTrackHeaderDirectory.TagFlags, a.Reader.GetBytes(3));
                        directory.Set(QuickTimeTrackHeaderDirectory.TagCreated, _epoch.AddTicks(TimeSpan.TicksPerSecond * a.Reader.GetUInt32()));
                        directory.Set(QuickTimeTrackHeaderDirectory.TagModified, _epoch.AddTicks(TimeSpan.TicksPerSecond * a.Reader.GetUInt32()));
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
                if (width == 0 || height == 0 || directory.GetObject(QuickTimeTrackHeaderDirectory.TagRotation) != null)
                    return;

                if (directory.GetObject(QuickTimeTrackHeaderDirectory.TagMatrix) is float[] { Length: > 5 } matrix)
                {
                    var x = matrix[1] + matrix[4];
                    var y = matrix[0] + matrix[3];
                    var theta = Math.Atan2(y, x);
                    var degree = RadiansToDegrees(theta) - 45;

                    directory.Set(QuickTimeTrackHeaderDirectory.TagRotation, degree);

                    static double RadiansToDegrees(double radians) => (180 / Math.PI) * radians;
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

            void UserDataHandler(AtomCallbackArgs a)
            {
                switch (a.TypeString)
                {
                    case "?xyz":
                        var stringSize = a.Reader.GetUInt16();
                        a.Reader.Skip(2); // uint16 language code
                        var stringBytes = a.Reader.GetBytes(stringSize);

                        // TODO parse ISO 6709 string into GeoLocation? GeoLocation does not (currently) support altitude, where ISO 6709 does
                        GetMetaHeaderDirectory().Set(
                            QuickTimeMetadataHeaderDirectory.TagGpsLocation,
                            new StringValue(stringBytes, Encoding.UTF8));
                        break;
                }
            }

            void MetaDataHandler(AtomCallbackArgs a)
            {
                // see https://developer.apple.com/library/archive/documentation/QuickTime/QTFF/Metadata/Metadata.html
                switch (a.TypeString)
                {
                    case "keys":
                    {
                        a.Reader.Skip(4); // 1 byte version, 3 bytes flags
                        var entryCount = a.Reader.GetUInt32();
                        for (int i = 1; i <= entryCount; i++)
                        {
                            var keySize = a.Reader.GetUInt32();
                            var keyValueSize = (int)keySize - 8;
                            a.Reader.Skip(4); // uint32: key namespace
                            var keyValue = a.Reader.GetBytes(keyValueSize);
                            metaDataKeys.Add(Encoding.UTF8.GetString(keyValue));
                        }
                        break;
                    }
                    case "ilst":
                    {
                        // Iterate over the list of Metadata Item Atoms.
                        for (int i = 0; i < metaDataKeys.Count; i++)
                        {
                            long atomSize = a.Reader.GetUInt32();
                            if (atomSize < 24)
                            {
                                GetMetaHeaderDirectory().AddError("Invalid ilist atom type");
                                a.Reader.Skip(atomSize - 4);
                                continue;
                            }
                            var atomType = a.Reader.GetUInt32();

                            // Indexes into the metadata item keys atom are 1-based (1…entry_count).
                            // atom type for each metadata item atom is the index of the key
                            if (atomType < 1 || atomType > metaDataKeys.Count)
                            {
                                GetMetaHeaderDirectory().AddError("Invalid ilist atom type");
                                a.Reader.Skip(atomSize - 8);
                                continue;
                            }
                            var key = metaDataKeys[(int)atomType - 1];

                            // Value Atom
                            a.Reader.Skip(8); // uint32 type indicator, uint32 locale indicator

                            // Data Atom
                            var dataTypeIndicator = a.Reader.GetUInt32();
                            if (!_supportedAtomValueTypes.Contains((int)dataTypeIndicator))
                            {
                                GetMetaHeaderDirectory().AddError($"Unsupported type indicator \"{dataTypeIndicator}\" for key \"{key}\"");
                                a.Reader.Skip(atomSize - 20);
                                continue;
                            }

                            // locale not supported yet.
                            a.Reader.Skip(4);

                            var data = a.Reader.GetBytes((int)atomSize - 24);
                            if (QuickTimeMetadataHeaderDirectory.TryGetTag(key, out int tag))
                            {
                                object value = dataTypeIndicator switch
                                {
                                    // UTF-8
                                    1 => new StringValue(data, Encoding.UTF8),

                                    // BE Float32 (used for User Rating)
                                    23 => BitConverter.ToSingle(BitConverter.IsLittleEndian ? data.Reverse().ToArray() : data, 0),

                                    // 13 JPEG
                                    // 14 PNG
                                    // 27 BMP
                                    _ => data
                                };

                                value = tag switch
                                {
                                    QuickTimeMetadataHeaderDirectory.TagCreationDate => DateTime.Parse(((StringValue)value).ToString()),
                                    QuickTimeMetadataHeaderDirectory.TagLocationDate => DateTime.Parse(((StringValue)value).ToString()),
                                    _ => value,
                                };

                                GetMetaHeaderDirectory().Set(tag, value);
                            }
                            else
                            {
                                GetMetaHeaderDirectory().AddError($"Unsupported ilist key \"{key}\"");
                            }
                        }

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
                        directory.Set(QuickTimeMovieHeaderDirectory.TagCreated, _epoch.AddTicks(TimeSpan.TicksPerSecond * a.Reader.GetUInt32()));
                        directory.Set(QuickTimeMovieHeaderDirectory.TagModified, _epoch.AddTicks(TimeSpan.TicksPerSecond * a.Reader.GetUInt32()));
                        var timeScale = a.Reader.GetUInt32();
                        directory.Set(QuickTimeMovieHeaderDirectory.TagTimeScale, timeScale);
                        directory.Set(QuickTimeMovieHeaderDirectory.TagDuration, TimeSpan.FromSeconds((double)a.Reader.GetUInt32() / (timeScale == 0 ? 1 : timeScale)));
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
                        var cr3 = new byte[] { 0x85, 0xc0, 0xb6, 0x87, 0x82, 0x0f, 0x11, 0xe0, 0x81, 0x11, 0xf4, 0xce, 0x46, 0x2b, 0x6a, 0x48 };
                        var uuid = a.Reader.GetBytes(cr3.Length);
                        if (cr3.RegionEquals(0, cr3.Length, uuid))
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
                    case "meta":
                    {
                        QuickTimeReader.ProcessAtoms(stream, MetaDataHandler, a.BytesLeft);
                        break;
                    }
                    case "udta":
                    {
                        QuickTimeReader.ProcessAtoms(stream, UserDataHandler, a.BytesLeft);
                        break;
                    }
                    /*
                    case "clip":
                    {
                        QuickTimeReader.ProcessAtoms(stream, clipHandler, a.BytesLeft);
                        break;
                    }
                    case "prfl":
                    {
                        a.Reader.Skip(4L);
                        var partId = a.Reader.GetUInt32();
                        var featureCode = a.Reader.GetUInt32();
                        var featureValue = string.Join(" ", a.Reader.GetBytes(4).Select(v => v.ToString("X2")).ToArray());
                        Debug.WriteLine($"PartId={partId} FeatureCode={featureCode} FeatureValue={featureValue}");
                        break;
                    }
                    */
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
                        var xmp = new byte[] { 0xbe, 0x7a, 0xcf, 0xcb, 0x97, 0xa9, 0x42, 0xe8, 0x9c, 0x71, 0x99, 0x94, 0x91, 0xe3, 0xaf, 0xac };
                        if (a.BytesLeft >= xmp.Length)
                        {
                            var uuid = a.Reader.GetBytes(xmp.Length);
                            if (xmp.RegionEquals(0, xmp.Length, uuid))
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
                        {
                            compatibleBrands.Add(a.Reader.Get4ccString());
                        }
                        directory.Set(QuickTimeFileTypeDirectory.TagCompatibleBrands, compatibleBrands.ToArray());
                        directories.Add(directory);
                        break;
                    }
                }
            }
        }
    }
}

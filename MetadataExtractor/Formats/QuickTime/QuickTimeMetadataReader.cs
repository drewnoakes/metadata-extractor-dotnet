// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Exif.Makernotes;
using MetadataExtractor.Formats.Iso14496;
using MetadataExtractor.Formats.Tiff;
using MetadataExtractor.Formats.Xmp;

namespace MetadataExtractor.Formats.QuickTime
{
    public static class QuickTimeMetadataReader
    {
        private static readonly DateTime _epoch = new(1904, 1, 1);
        private static readonly int[] _supportedAtomValueTypes = [1, 13, 14, 23, 27];

        public static IReadOnlyList<Directory> ReadMetadata(Stream stream)
        {
            var directories = new List<Directory>();
            var metaDataKeys = new List<string>();
            var metaDataHandlerType = string.Empty;
            QuickTimeMetadataHeaderDirectory? metaHeaderDirectory = null;

            QuickTimeReader.ProcessAtoms(stream, Handler);

            return directories;

            QuickTimeMetadataHeaderDirectory GetMetaHeaderDirectory()
            {
                if (metaHeaderDirectory is null)
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
                if (width == 0 || height == 0 || directory.GetObject(QuickTimeTrackHeaderDirectory.TagRotation) is not null)
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
                switch (a.Type)
                {
                    case 0xa978797a: // "?xyz" (with a copyright symbol)
                    {
                        var stringSize = a.Reader.GetUInt16();
                        a.Reader.Skip(2); // uint16 language code
                        var stringBytes = a.Reader.GetBytes(stringSize);

                        // TODO parse ISO 6709 string into GeoLocation? GeoLocation does not (currently) support altitude, where ISO 6709 does
                        GetMetaHeaderDirectory().Set(
                            QuickTimeMetadataHeaderDirectory.TagGpsLocation,
                            new StringValue(stringBytes, Encoding.UTF8));
                        break;
                    }
                    case 0x6d657461: // "meta":
                    {
                        a.Reader.Skip(4);
                        QuickTimeReader.ProcessAtoms(stream, MetaDataHandler, a.BytesLeft);
                        break;
                    }
                    case 0x584d505f: // "XMP_" (XMP metadata)
                    {
                        var xmpBytes = a.Reader.GetNullTerminatedBytes((int)a.BytesLeft);
                        var xmpDirectory = new XmpReader().Extract(xmpBytes);
                        directories.Add(xmpDirectory);
                        break;
                    }
                }
            }

            void MetaDataHandler(AtomCallbackArgs a)
            {
                // see https://developer.apple.com/library/archive/documentation/QuickTime/QTFF/Metadata/Metadata.html
                switch (a.TypeString)
                {
                    case "hdlr":
                    {
                        // QuickTime Handler Tags
                        a.Reader.Skip(8);

                        var handlerType = a.Reader.GetUInt32();
                        metaDataHandlerType = TypeStringConverter.ToTypeString(handlerType);

                        // metaDataHandlerType:
                        // mdir => Metadata Item List Tags
                        // mdta => Metadata Keys Tags

                        break;
                    }
                    case "keys":
                    {
                        // This directory contains a list of key names which are used to decode tags written by the "mdta" handler.

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
                        // ilst is used by both mdir (ItemList Tags) and mdta (Keys Tags) handlers

                        if (metaDataHandlerType == "mdir")
                        {
                            // ItemList Tags
                            QuickTimeReader.ProcessAtoms(stream, MetaDataItemListTagsHandler, a.BytesLeft);
                        }
                        else if (metaDataHandlerType == "mdta")
                        {
                            // Iterate over the list of Metadata Item Atoms.
                            QuickTimeReader.ProcessAtoms(stream, MetaDataKeysTagsHandler, a.BytesLeft);
                        }

                        break;
                    }
                }
            }

            void MetaDataTagHandler(AtomCallbackArgs a, string key, bool keyShouldBeValid = false)
            {
                // Value Atom
                a.Reader.Skip(8); // uint32 type indicator, uint32 locale indicator

                // Data Atom
                var dataTypeIndicator = a.Reader.GetUInt32();
                if (!_supportedAtomValueTypes.Contains((int)dataTypeIndicator))
                {
                    if (keyShouldBeValid)
                    {
                        GetMetaHeaderDirectory().AddError($"Unsupported type indicator \"{dataTypeIndicator}\" for key \"{key}\"");
                    }
                    return;
                }

                // locale not supported yet.
                a.Reader.Skip(4);

                var data = a.Reader.GetBytes((int)a.Size - 24);
                if (QuickTimeMetadataHeaderDirectory.TryGetTag(key, out int tag))
                {
                    object value = dataTypeIndicator switch
                    {
                        // UTF-8
                        1 => new StringValue(data, Encoding.UTF8),

                        // BE Float32 (used for User Rating)
                        23 => BitConverter.ToSingle(BitConverter.IsLittleEndian ? Reversed(data) : data, startIndex: 0),

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
                    if (keyShouldBeValid)
                    {
                        GetMetaHeaderDirectory().AddError($"Unsupported ilst key \"{key}\"");
                    }
                }

                static byte[] Reversed(byte[] data)
                {
                    Array.Reverse(data);
                    return data;
                }
            }

            void MetaDataKeysTagsHandler(AtomCallbackArgs a)
            {
                // Indexes into the metadata item keys atom are 1-based (1…entry_count).
                // atom type for each metadata item atom is the index of the key
                if (a.Type < 1 || a.Type > metaDataKeys.Count)
                {
                    GetMetaHeaderDirectory().AddError("Invalid ilst atom type");
                    return;
                }

                var key = metaDataKeys[(int)a.Type - 1];

                MetaDataTagHandler(a, key, true);
            }

            void MetaDataItemListTagsHandler(AtomCallbackArgs a)
            {
                var key = a.TypeString;
                if (key.Length < 1)
                {
                    return;
                }
                if (key[0] == 0xa9 || key[0] == 0x40)
                {
                    //Tag ID's beginning with the copyright symbol (hex 0xa9) are multi-language text
                    //some stupid Ricoh programmer used the '@' (hex 0x40) symbol instead of the copyright symbol in these tag ID's for the Ricoh Theta Z1 and maybe other models

                    //The uint32 locale indicator following the key (which is skipped by the MetaDataTagHandler-call as its not supported (yet)) contains the locale information for the tag.
                    //It contains a dash followed by a 3-character ISO 639-2 language code to the tag name.

                    //For now we don't support those, we will strip the copyright/@-sign
                    key = key.Substring(1);

                    if (string.IsNullOrWhiteSpace(key))
                    {
                        return; // no valid key
                    }
                }

                MetaDataTagHandler(a, key);
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
                        ReadOnlySpan<byte> cr3 = [0x85, 0xc0, 0xb6, 0x87, 0x82, 0x0f, 0x11, 0xe0, 0x81, 0x11, 0xf4, 0xce, 0x46, 0x2b, 0x6a, 0x48];
                        Span<byte> actual = stackalloc byte[cr3.Length];
                        a.Reader.GetBytes(actual);
                        if (cr3.SequenceEqual(actual))
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
                        var featureValue = string.Join(" ", a.Reader.GetBytes(4).Select(v => v.ToString("X2")));
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
                        ReadOnlySpan<byte> xmp = [0xbe, 0x7a, 0xcf, 0xcb, 0x97, 0xa9, 0x42, 0xe8, 0x9c, 0x71, 0x99, 0x94, 0x91, 0xe3, 0xaf, 0xac];
                        if (a.BytesLeft >= xmp.Length)
                        {
                            Span<byte> uuid = stackalloc byte[16]; // xmp length is 16

                            a.Reader.GetBytes(uuid);
                            if (xmp.SequenceEqual(uuid))
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

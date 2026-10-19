// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Globalization;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Exif.Makernotes;
using MetadataExtractor.Formats.Tiff;
using MetadataExtractor.Formats.Xmp;

#pragma warning disable CS8602 // Dereference of a possibly null reference
#pragma warning disable CS8604 // Dereference of a possibly null reference

namespace MetadataExtractor.Formats.QuickTime;

public static class QuickTimeMetadataReader
{
    private static readonly DateTime _epoch = new(1904, 1, 1);
    private static readonly int[] _supportedAtomValueTypes = [1, 13, 14, 23, 27];
    private static readonly string[] _itemListDatePatterns =
    [
        "yyyy-MM-dd",
        "yyyy:MM:dd",
        "yyyy-MM-dd'T'HH:mm:ss.FFFFFFF",
        "yyyy-MM-dd HH:mm:ss.FFFFFFF",
        "yyyy:MM:dd HH:mm:ss.FFFFFFF"
    ];

    public static IReadOnlyList<Directory> ReadMetadata(Stream stream)
    {
        var directories = new List<Directory>();
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

        void AddError(string message)
        {
            GetMetaHeaderDirectory().AddError(message);
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
                    var stringSize = a.Reader.GetUInt16();
                    a.Reader.Skip(2); // uint16 language code
                    var stringBytes = a.Reader.GetBytes(stringSize);

                    // TODO parse ISO 6709 string into GeoLocation? GeoLocation does not (currently) support altitude, where ISO 6709 does
                    GetMetaHeaderDirectory().Set(
                        QuickTimeMetadataHeaderDirectory.TagGpsLocation,
                        new StringValue(stringBytes, Encoding.UTF8));
                    break;
                case 0x584D505F: // "XMP_" (XMP metadata)
                    var xmpBytes = a.Reader.GetNullTerminatedBytes((int)a.BytesLeft);
                    var xmpDirectory = new XmpReader().Extract(xmpBytes);
                    directories.Add(xmpDirectory);
                    break;
                case 0x6D657461: // "meta" (iTunes-style user-data metadata)
                    a.Reader.Skip(4);
                    ReadMetaData(a, itemListByDefault: true);
                    break;
                case 0x58747261: // "Xtra" (Windows Media metadata, surfaced in the Windows Properties dialog)
                    ReadXtraBox(a);
                    break;
            }
        }

        void UserDataIlstHandler(AtomCallbackArgs a)
        {
            var key = a.TypeString;
            if (key[0] is '\u00a9' or '@')
                key = key.Substring(1);

            ReadMetadataItem(a, key, keyShouldBeValid: false);
        }

        void ReadMetadataItem(AtomCallbackArgs item, string key, bool keyShouldBeValid)
        {
            ReadDataAtoms(item, (dataTypeIndicator, data) =>
            {
                if (!_supportedAtomValueTypes.Contains((int)dataTypeIndicator))
                {
                    if (keyShouldBeValid)
                        AddError($"Unsupported type indicator \"{dataTypeIndicator}\" for key \"{key}\"");
                    return;
                }

                if (!QuickTimeMetadataHeaderDirectory.TryGetTag(key, out int tag))
                {
                    if (keyShouldBeValid)
                        AddError($"Unsupported ilst key \"{key}\"");
                    return;
                }

                if (dataTypeIndicator == 23 && data.Length != 4)
                {
                    AddError($"Invalid Float32 value for key \"{key}\"");
                    return;
                }

                object value = dataTypeIndicator switch
                {
                    1 => new StringValue(data, Encoding.UTF8),
                    23 => BitConverter.ToSingle(BitConverter.IsLittleEndian ? Reversed(data) : data, 0),
                    _ => data
                };

                value = tag switch
                {
                    QuickTimeMetadataHeaderDirectory.TagCreationDate when !keyShouldBeValid && key == "day" => ParseItemListDate(value),
                    QuickTimeMetadataHeaderDirectory.TagCreationDate or
                    QuickTimeMetadataHeaderDirectory.TagLocationDate => ParseDate(((StringValue)value).ToString()),
                    _ => value,
                };

                GetMetaHeaderDirectory().Set(tag, value);

                static byte[] Reversed(byte[] bytes)
                {
                    Array.Reverse(bytes);
                    return bytes;
                }

                static object ParseDate(string text)
                {
                    if (DirectoryExtensions.TryParseDateTimeOffset(text, out var offset))
                        return offset;

                    return DateTime.Parse(text);
                }

                static object ParseItemListDate(object value)
                {
                    if (value is not StringValue stringValue)
                        return value;

                    var text = stringValue.ToString();
                    if (DirectoryExtensions.TryParseDateTimeOffset(text, out var offset))
                        return offset;

                    // Exact formats require a complete date, rather than filling in missing components.
                    return DateTime.TryParseExact(text, _itemListDatePatterns, CultureInfo.InvariantCulture,
                        DateTimeStyles.AllowWhiteSpaces, out var date) ? date : value;
                }
            });
        }

        void ReadDataAtoms(AtomCallbackArgs item, Action<uint, byte[]> handler)
        {
            item.ProcessChildren(data =>
            {
                if (data.TypeString != "data")
                    return;

                if (data.BytesLeft < 8 || data.BytesLeft - 8 > int.MaxValue)
                {
                    GetMetaHeaderDirectory().AddError("Invalid metadata data atom size");
                    return;
                }

                var typeIndicator = data.Reader.GetUInt32();
                data.Reader.Skip(4); // locale
                handler(typeIndicator, data.Reader.GetBytes((int)data.BytesLeft));
            }, AddError);
        }

        void ReadXtraBox(AtomCallbackArgs a)
        {
            // The Windows 'Xtra' box is a flat list of tag entries (not nested atoms), so unlike
            // the atom-structured boxes we must advance to each entry's end manually using its
            // declared size, otherwise unhandled or trailing entries would misalign parsing.
            long xtraEnd = a.Reader.Position + a.BytesLeft;
            while (a.Reader.Position + 12 <= xtraEnd)
            {
                long entryStart = a.Reader.Position;
                long entrySize = a.Reader.GetUInt32();
                if (entrySize < 12 || entryStart + entrySize > xtraEnd)
                    break;
                long keyNameSize = a.Reader.GetUInt32();
                if (keyNameSize > entrySize)
                    break;
                string keyName = a.Reader.GetString((int)keyNameSize, Encoding.UTF8);
                long entryCount = a.Reader.GetUInt32();
                switch (keyName)
                {
                    case "WM/SubTitle":
                        GetMetaHeaderDirectory().Set(QuickTimeMetadataHeaderDirectory.TagSubtitle, ReadXtraValues(a.Reader, entryCount));
                        break;
                    case "WM/SharedUserRating":
                        a.Reader.GetUInt32(); // value length
                        a.Reader.GetUInt16(); // value type
                        GetMetaHeaderDirectory().Set(QuickTimeMetadataHeaderDirectory.TagRating, a.Reader.GetInt64());
                        break;
                    case "WM/Category":
                        GetMetaHeaderDirectory().Set(QuickTimeMetadataHeaderDirectory.TagCategory, ReadXtraValues(a.Reader, entryCount));
                        break;
                    case "WM/Mood":
                        GetMetaHeaderDirectory().Set(QuickTimeMetadataHeaderDirectory.TagMood, ReadXtraValues(a.Reader, entryCount));
                        break;
                }

                // Advance to the end of this entry regardless of which fields we consumed.
                long consumed = a.Reader.Position - entryStart;
                if (entrySize > consumed)
                    a.Reader.Skip(entrySize - consumed);
            }

            static string ReadXtraValues(SequentialReader reader, long entryCount)
            {
                var values = new List<string>();
                for (long i = 0; i < entryCount; i++)
                {
                    long valueSize = reader.GetUInt32();
                    reader.GetUInt16(); // value type
                    if (valueSize < 6)
                        break;
                    values.Add(reader.GetString((int)(valueSize - 6), Encoding.Unicode).Replace("\0", ""));
                }
                return string.Join(" | ", values);
            }
        }

        void ReadMetaData(AtomCallbackArgs container, bool itemListByDefault = false)
        {
            var metaDataKeys = new List<string>();
            var metaDataHandlerType = string.Empty;
            container.ProcessChildren(MetaDataHandler, AddError);

            void MetaDataHandler(AtomCallbackArgs a)
            {
                // see https://developer.apple.com/library/archive/documentation/QuickTime/QTFF/Metadata/Metadata.html
                switch (a.TypeString)
                {
                    case "hdlr":
                        a.Reader.Skip(8); // version/flags and pre-defined
                        metaDataHandlerType = a.Reader.Get4ccString();
                        break;
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
                        if (metaDataHandlerType == "mdir" || (itemListByDefault && metaDataHandlerType != "mdta"))
                        {
                            a.ProcessChildren(UserDataIlstHandler, AddError);
                            break;
                        }

                        a.ProcessChildren(item =>
                        {
                            // Indexes into the metadata item keys atom are 1-based (1…entry_count).
                            if (item.Type < 1 || item.Type > metaDataKeys.Count)
                            {
                                GetMetaHeaderDirectory().AddError("Invalid ilst atom type");
                                return;
                            }
                            var key = metaDataKeys[(int)item.Type - 1];

                            ReadMetadataItem(item, key, keyShouldBeValid: true);
                        }, AddError);

                        break;
                    }
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
                    ReadOnlySpan<byte> cr3 = [0x85, 0xc0, 0xb6, 0x87, 0x82, 0x0f, 0x11, 0xe0, 0x81, 0x11, 0xf4, 0xce, 0x46, 0x2b, 0x6a, 0x48];
                    Span<byte> actual = stackalloc byte[cr3.Length];
                    a.Reader.GetBytes(actual);
                    if (cr3.SequenceEqual(actual))
                    {
                        a.ProcessChildren(UuidHandler, AddError);
                    }
                    break;
                }
                case "trak":
                {
                    a.ProcessChildren(TrakHandler, AddError);
                    break;
                }
                case "meta":
                {
                    ReadMetaData(a);
                    break;
                }
                case "udta":
                {
                    a.ProcessChildren(UserDataHandler, AddError);
                    break;
                }
                /*
                case "clip":
                {
                    a.ProcessChildren(clipHandler, AddError);
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
                    a.ProcessChildren(MoovHandler, AddError);
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

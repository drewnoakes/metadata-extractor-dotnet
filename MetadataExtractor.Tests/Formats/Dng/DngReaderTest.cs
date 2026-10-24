// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Exif.Makernotes;
using MetadataExtractor.Formats.Tiff;

namespace MetadataExtractor.Formats.Dng;

public sealed class DngReaderTest
{
    private const int Version = 0xC612;
    private const int Model = 0xC614;
    private const int FutureTag = 0xC7FF;
    private const int Width = ExifDirectoryBase.TagImageWidth;

    [Theory]
    [CombinatorialData]
    public void ReadsDngAndPreservesOriginalTypesValuesOrderAndErrors(
        bool bigTiff, bool bigEndian, bool imageReader, [CombinatorialValues(1, 7)] int versionFormat)
    {
        var image = new ImageIfd(
            new Field(Version, versionFormat, new byte[] { 1, 4, 0, 0 }),
            Ascii(Model, "Synthetic Camera"),
            Shorts(DngDirectory.TagCfaLayout, 2),
            Longs(DngDirectory.TagWhiteLevel, 16383, 16382),
            new Field(DngDirectory.TagBlackLevel, 5, new Rational[] { new(3, 2) }),
            new Field(DngDirectory.TagColorMatrix1, 10, new Rational[] { new(-3, 2), new(7, 4) }),
            Bytes(DngDirectory.TagDngPrivateData, 7, 8, 9, 0, 255),
            new Field(DngDirectory.TagNoiseProfile, 12, new double[] { 0.125, -0.25 }),
            Bytes(FutureTag, 7, 11, 22, 33, 44, 55),
            // A malformed unrelated tag must retain its error without blocking DNG interpretation.
            new Field(0xC7FE, 42, new byte[] { 1 }));
        var bytes = Build(bigTiff, bigEndian, image);

        var directories = Read(bytes, imageReader);

        var source = Assert.Single(directories.OfType<ExifIfd0Directory>());
        var dng = Assert.Single(directories.OfType<DngDirectory>());
        Assert.Same(source, dng.Parent);
        Assert.Equal(8, dng.TagCount);
        Assert.Empty(dng.Errors);
        Assert.Single(source.Errors);
        Assert.Equal("1.4.0.0", dng.GetDescription(Version));
        Assert.Equal("Synthetic Camera", Assert.IsType<StringValue>(dng.GetObject(Model)).ToString());
        Assert.Equal((ushort)2, Assert.IsType<ushort>(dng.GetObject(DngDirectory.TagCfaLayout)));
        Assert.Equal(new uint[] { 16383, 16382 }, Assert.IsType<uint[]>(dng.GetObject(DngDirectory.TagWhiteLevel)));
        Assert.Equal(new Rational(3, 2), Assert.IsType<Rational>(dng.GetObject(DngDirectory.TagBlackLevel)));
        Assert.Equal(new Rational[] { new(-3, 2), new(7, 4) }, Assert.IsType<Rational[]>(dng.GetObject(DngDirectory.TagColorMatrix1)));
        Assert.Equal(new byte[] { 8, 9, 0, 255 }, Assert.IsType<byte[]>(dng.GetObject(DngDirectory.TagDngPrivateData)));
        Assert.Equal(new[] { 0.125, -0.25 }, Assert.IsType<double[]>(dng.GetObject(DngDirectory.TagNoiseProfile)));
        Assert.Equal(new byte[] { 11, 22, 33, 44, 55 }, Assert.IsType<byte[]>(source.GetObject(FutureTag)));
        Assert.False(dng.ContainsTag(FutureTag));
        Assert.False(dng.ContainsTag(Width));
        Assert.Equal(source.Tags.Where(tag => dng.HasTagName(tag.Type)).Select(tag => tag.Type), dng.Tags.Select(tag => tag.Type));
        foreach (var tag in dng.Tags)
            Assert.Same(source.GetObject(tag.Type), dng.GetObject(tag.Type));
        AssertSourcesUnchanged(bytes, directories);
    }

    [Theory]
    [CombinatorialData]
    public void RejectsMalformedRootMarkerWithoutDiscardingRawMetadata(
        bool bigTiff, bool bigEndian,
        [CombinatorialValues("short", "long", "signed-byte", "ascii", "empty", "scalar", "three", "five", "zero-major")] string marker)
    {
        var field = marker switch
        {
            "short" => Shorts(Version, 1, 4, 0, 0),
            "long" => Longs(Version, 1, 4, 0, 0),
            "signed-byte" => new Field(Version, 6, new byte[] { 1, 4, 0, 0 }),
            "ascii" => Ascii(Version, "1400"),
            "empty" => Bytes(Version, 7),
            "scalar" => Bytes(Version, 1, 1),
            "three" => Bytes(Version, 1, 1, 4, 0),
            "five" => Bytes(Version, 7, 1, 4, 0, 0, 0),
            "zero-major" => Bytes(Version, 1, 0, 4, 0, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(marker))
        };
        var root = new ImageIfd(field, Ascii(Model, "Still raw"));
        var child = new ImageIfd(Bytes(Version, 1, 1, 4, 0, 0), Shorts(DngDirectory.TagCfaLayout, 2));
        root.Links[0x014A] = [child];
        var bytes = Build(bigTiff, bigEndian, root, child);

        var directories = Read(bytes);

        Assert.Empty(directories.OfType<DngDirectory>());
        var source = Assert.Single(directories.OfType<ExifIfd0Directory>());
        Assert.Contains("Invalid DNG version", Assert.Single(source.Errors));
        Assert.True(source.ContainsTag(Version));
        Assert.Equal("Still raw", source.GetString(Model));
        Assert.Equal(321u, Assert.IsType<uint>(source.GetObject(Width)));
        var childSource = Assert.Single(directories.OfType<ExifSubIfdDirectory>());
        Assert.Empty(childSource.Errors);
        Assert.Equal(new byte[] { 1, 4, 0, 0 }, Assert.IsType<byte[]>(childSource.GetObject(Version)));
        Assert.Same(source, childSource.Parent);
    }

    [Theory]
    [CombinatorialData]
    public void RequiresMarkerInRootNotInAChildOrFollower(bool bigTiff, bool bigEndian, bool follower)
    {
        var root = new ImageIfd(Ascii(Model, "No root marker"));
        var other = new ImageIfd(Bytes(Version, 1, 1, 4, 0, 0), Ascii(Model, "Child marker"));
        if (follower)
            root.Next = other;
        else
            root.Links[0x014A] = [other];
        var bytes = Build(bigTiff, bigEndian, root, other);

        var directories = Read(bytes);

        Assert.Empty(directories.OfType<DngDirectory>());
        Assert.Equal(2, directories.Count);
        Assert.All(directories, directory => Assert.Empty(directory.Errors));
        Assert.Equal("No root marker", directories[0].GetString(Model));
        Assert.Equal(new byte[] { 1, 4, 0, 0 }, Assert.IsType<byte[]>(directories[1].GetObject(Version)));
        AssertSourcesUnchanged(bytes, directories);
    }

    [Theory]
    [CombinatorialData]
    public void VisitsNestedAndMultipleSubIfdsBeforeRootMarkerAndIncludesImageFollowers(bool bigTiff, bool bigEndian)
    {
        var root = new ImageIfd(Bytes(Version, 1, 2, 0, 0, 0), Ascii(Model, "Root"));
        var first = new ImageIfd(Ascii(Model, "First"));
        var second = new ImageIfd(Ascii(Model, "Second"));
        var nested = new ImageIfd(Ascii(Model, "Nested"));
        var childFollower = new ImageIfd(Ascii(Model, "Child follower"));
        var thumbnail = new ImageIfd(Ascii(Model, "Thumbnail"));
        var imageFollower = new ImageIfd(Ascii(Model, "Image follower"));
        var empty = new ImageIfd();
        root.Links[0x014A] = [first, second];
        first.Links[0x014A] = [nested, empty];
        first.Next = childFollower;
        root.Next = thumbnail;
        thumbnail.Next = imageFollower;
        var bytes = Build(bigTiff, bigEndian, root, first, second, nested, childFollower, thumbnail, imageFollower, empty);

        var directories = Read(bytes);

        var sources = directories.Where(directory => directory is not DngDirectory).ToArray();
        var companions = directories.OfType<DngDirectory>().ToArray();
        Assert.Equal(8, sources.Length);
        Assert.Equal(7, companions.Length);
        Assert.IsType<ExifIfd0Directory>(sources[0]);
        Assert.Null(sources[0].Parent);
        var byModel = sources.Where(source => source.ContainsTag(Model)).ToDictionary(source => source.GetString(Model)!);
        Assert.Same(byModel["Root"], byModel["First"].Parent);
        Assert.Same(byModel["Root"], byModel["Second"].Parent);
        Assert.Same(byModel["First"], byModel["Nested"].Parent);
        Assert.Same(byModel["First"], byModel["Child follower"].Parent);
        Assert.Same(byModel["Root"], byModel["Thumbnail"].Parent);
        Assert.Same(byModel["Thumbnail"], byModel["Image follower"].Parent);
        foreach (var entry in byModel)
        {
            var name = entry.Key;
            var source = entry.Value;
            var dng = Assert.Single(companions, directory => ReferenceEquals(directory.Parent, source));
            Assert.Equal(name, dng.GetString(Model));
            Assert.Equal(name == "Root" ? 2 : 1, dng.TagCount);
            Assert.Empty(dng.Errors);
        }
        Assert.Equal("2.0.0.0", companions.Single(directory => ReferenceEquals(directory.Parent, sources[0])).GetDescription(Version));
        Assert.DoesNotContain(companions, directory => directory.Parent == sources.Single(source => !source.ContainsTag(Model)));
        Assert.All(sources, source => Assert.Empty(source.Errors));
        AssertSourcesUnchanged(bytes, directories);
    }

    [Theory]
    [CombinatorialData]
    public void ExcludesTrueExifGpsInteropAndMakernoteBranches(bool bigTiff, bool bigEndian)
    {
        var root = new ImageIfd(Bytes(Version, 1, 1, 4, 0, 0), Ascii(Model, "Root"));
        var image = new ImageIfd(Ascii(Model, "Image"));
        var exif = new ImageIfd(Bytes(Version, 1, 1, 4, 0, 0), Ascii(Model, "Exif"),
            Bytes(ExifDirectoryBase.TagMakernote, 7, SonyMakernote(bigTiff, bigEndian)));
        var gps = new ImageIfd(Ascii(Model, "GPS"));
        var interop = new ImageIfd(Ascii(Model, "Interop"));
        var exifChild = new ImageIfd(Ascii(Model, "Exif child"));
        var exifFollower = new ImageIfd(Ascii(Model, "Exif follower"));
        var gpsChild = new ImageIfd(Ascii(Model, "GPS child"));
        root.Links[0x014A] = [image];
        root.Links[0x8769] = [exif];
        root.Links[0x8825] = [gps];
        exif.Links[0xA005] = [interop];
        exif.Links[0x014A] = [exifChild];
        exif.Next = exifFollower;
        gps.Links[0x014A] = [gpsChild];
        var bytes = Build(bigTiff, bigEndian, root, image, exif, gps, interop, exifChild, exifFollower, gpsChild);

        var directories = Read(bytes);

        var companions = directories.OfType<DngDirectory>().ToArray();
        Assert.Equal(new[] { "Root", "Image" }, companions.Select(directory => directory.GetString(Model)));
        Assert.Equal(2, companions.Length);
        var exifSource = Assert.Single(directories.OfType<ExifSubIfdDirectory>(), directory => directory.GetString(Model) == "Exif");
        var gpsSource = Assert.Single(directories.OfType<GpsDirectory>());
        var interopSource = Assert.Single(directories.OfType<ExifInteropDirectory>());
        var maker = Assert.Single(directories.OfType<SonyType1MakernoteDirectory>());
        Assert.Equal("GPS", gpsSource.GetString(Model));
        Assert.Equal("Interop", interopSource.GetString(Model));
        Assert.Equal(new byte[] { 1, 4, 0, 0 }, Assert.IsType<byte[]>(maker.GetObject(Version)));
        Assert.Equal((ushort)17, Assert.IsType<ushort>(maker.GetObject(DngDirectory.TagMakerNoteSafety)));
        Assert.Same(exifSource, maker.Parent);
        Assert.Same(exifSource, interopSource.Parent);
        Assert.All(directories, directory => Assert.Empty(directory.Errors));
        AssertSourcesUnchanged(bytes, directories);
    }

    [Theory]
    [CombinatorialData]
    public void DoesNotTreatEmbeddedExifOrJpegAsDng(bool bigEndian, bool jpeg)
    {
        var bytes = Build(false, bigEndian, new ImageIfd(
            Bytes(Version, 1, 1, 4, 0, 0), Ascii(Model, "Embedded Camera")));
        IReadOnlyList<Directory> directories;
        if (jpeg)
        {
            using var stream = new MemoryStream();
            byte[] payload = [.. ExifReader.JpegSegmentPreamble, .. bytes];
            stream.WriteByte(0xFF);
            stream.WriteByte(0xD8);
            stream.WriteByte(0xFF);
            stream.WriteByte(0xE1);
            WriteUnsigned(stream, (ulong)(payload.Length + 2), 2, bigEndian: true);
            stream.Write(payload, 0, payload.Length);
            stream.WriteByte(0xFF);
            stream.WriteByte(0xD9);
            stream.Position = 0;
            directories = ImageMetadataReader.ReadMetadata(stream);
        }
        else
        {
            directories = new ExifReader().Extract(new ByteArrayReader(bytes), exifStartOffset: 0);
        }

        Assert.Empty(directories.OfType<DngDirectory>());
        var source = Assert.Single(directories.OfType<ExifIfd0Directory>());
        Assert.Equal("Embedded Camera", source.GetString(Model));
        Assert.Equal(new byte[] { 1, 4, 0, 0 }, Assert.IsType<byte[]>(source.GetObject(Version)));
        Assert.False(source.HasTagName(Version));
        Assert.Empty(source.Errors);
    }

    [Theory]
    [CombinatorialData]
    public void FilenameDoesNotReplaceContentDetection(
        [CombinatorialValues("dng", "ordinary", "sony")] string content, bool imageReader)
    {
        var root = content switch
        {
            "dng" => new ImageIfd(Bytes(Version, 1, 1, 4, 0, 0), Ascii(Model, "Real DNG")),
            "ordinary" => new ImageIfd(Ascii(ExifDirectoryBase.TagMake, "Ordinary TIFF")),
            "sony" => new ImageIfd(Ascii(ExifDirectoryBase.TagMake, "SONY"),
                Bytes(DngDirectory.TagDngPrivateData, 7, 83, 79, 78, 89, 0, 1, 2, 3)),
            _ => throw new ArgumentOutOfRangeException(nameof(content))
        };
        var bytes = Build(false, false, root);
        var path = Path.Combine(Path.GetTempPath(), $"metadata-extractor-{Guid.NewGuid():N}.dng");
        try
        {
            File.WriteAllBytes(path, bytes);
            var directories = imageReader ? ImageMetadataReader.ReadMetadata(path) : TiffMetadataReader.ReadMetadata(path);

            var source = Assert.Single(directories.OfType<ExifIfd0Directory>());
            Assert.Empty(source.Errors);
            Assert.Equal(321u, Assert.IsType<uint>(source.GetObject(Width)));
            if (content == "dng")
            {
                var dng = Assert.Single(directories.OfType<DngDirectory>());
                Assert.Same(source, dng.Parent);
                Assert.Equal("Real DNG", dng.GetString(Model));
                Assert.Equal("1.4.0.0", dng.GetDescription(Version));
            }
            else
            {
                Assert.Empty(directories.OfType<DngDirectory>());
                Assert.Equal(content == "sony" ? "SONY" : "Ordinary TIFF", source.GetString(ExifDirectoryBase.TagMake));
                if (content == "sony")
                    Assert.Equal(new byte[] { 83, 79, 78, 89, 0, 1, 2, 3 },
                        Assert.IsType<byte[]>(source.GetObject(DngDirectory.TagDngPrivateData)));
                else
                    Assert.False(source.ContainsTag(DngDirectory.TagDngPrivateData));
            }
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Theory]
    [CombinatorialData]
    public void KeepsHistoricallyCollidingLiteralTagIdsDistinct(bool bigTiff, bool bigEndian)
    {
        var bytes = Build(bigTiff, bigEndian, new ImageIfd(
            Bytes(Version, 1, 1, 4, 0, 0),
            new Field(0xC628, 5, new Rational[] { new(1, 2), new(1, 1), new(3, 4) }),
            new Field(0xC629, 5, new Rational[] { new(1, 3), new(2, 3) }),
            Longs(0xC62D, 42),
            new Field(0xC62E, 5, new Rational[] { new(9, 10) }),
            Bytes(0xC634, 7, 9, 8, 7),
            Shorts(0xC635, 1)));

        var directories = Read(bytes);

        var dng = Assert.Single(directories.OfType<DngDirectory>());
        var source = Assert.Single(directories.OfType<ExifIfd0Directory>());
        Assert.Same(source, dng.Parent);
        Assert.Equal(7, dng.TagCount);
        Assert.Equal(new Rational[] { new(1, 2), new(1, 1), new(3, 4) },
            Assert.IsType<Rational[]>(dng.GetObject(DngDirectory.TagAsShotNeutral)));
        Assert.Equal(new Rational[] { new(1, 3), new(2, 3) },
            Assert.IsType<Rational[]>(dng.GetObject(DngDirectory.TagAsShotWhiteXY)));
        Assert.Equal(42u, Assert.IsType<uint>(dng.GetObject(DngDirectory.TagBayerGreenSplit)));
        Assert.Equal(new Rational(9, 10), Assert.IsType<Rational>(dng.GetObject(DngDirectory.TagLinearResponseLimit)));
        Assert.Equal(new byte[] { 9, 8, 7 }, Assert.IsType<byte[]>(dng.GetObject(DngDirectory.TagDngPrivateData)));
        Assert.Equal((ushort)1, Assert.IsType<ushort>(dng.GetObject(DngDirectory.TagMakerNoteSafety)));
        Assert.Equal("Safe", dng.GetDescription(DngDirectory.TagMakerNoteSafety));
        Assert.Empty(source.Errors);
        Assert.Empty(dng.Errors);
        AssertSourcesUnchanged(bytes, directories);
    }

    private static IReadOnlyList<Directory> Read(byte[] bytes, bool imageReader = false)
    {
        using var stream = new MemoryStream(bytes);
        return imageReader ? ImageMetadataReader.ReadMetadata(stream) : TiffMetadataReader.ReadMetadata(stream);
    }

    private static void AssertSourcesUnchanged(byte[] bytes, IReadOnlyList<Directory> directories)
    {
        var expected = new ExifReader().Extract(new ByteArrayReader(bytes), exifStartOffset: 0).ToArray();
        var actual = directories.Where(directory => directory is ExifDirectoryBase or SonyType1MakernoteDirectory).ToArray();
        Assert.Equal(expected.Length, actual.Length);
        for (var i = 0; i < expected.Length; i++)
        {
            Assert.IsType(expected[i].GetType(), actual[i]);
            Assert.Equal(expected[i].Errors, actual[i].Errors);
            Assert.Equal(expected[i].Tags.Select(tag => tag.Type), actual[i].Tags.Select(tag => tag.Type));
            Assert.Equal(Array.IndexOf(expected, expected[i].Parent), Array.IndexOf(actual, actual[i].Parent));
            foreach (var tag in expected[i].Tags)
            {
                var expectedValue = expected[i].GetObject(tag.Type)!;
                var actualValue = actual[i].GetObject(tag.Type);
                Assert.IsType(expectedValue.GetType(), actualValue);
                if (expectedValue is Array array)
                    Assert.Equal(array.Cast<object>(), ((Array)actualValue!).Cast<object>());
                else if (expectedValue is StringValue text)
                    Assert.Equal(text.Bytes, ((StringValue)actualValue!).Bytes);
                else
                    Assert.Equal(expectedValue, actualValue);
            }
        }
    }

    private static byte[] SonyMakernote(bool bigTiff, bool bigEndian)
    {
        // Inline values need no offset rebasing. The Exif handler uses the enclosing TIFF's layout.
        var tiff = Build(bigTiff, bigEndian, new ImageIfd(
            Bytes(Version, 1, 1, 4, 0, 0), Shorts(DngDirectory.TagMakerNoteSafety, 17)));
        return [.. "SONY DSC\0\0\0\0"u8, .. tiff.AsSpan(bigTiff ? 16 : 8)];
    }

    private static Field Bytes(int tag, int format, params byte[] values) => new(tag, format, values);
    private static Field Shorts(int tag, params ushort[] values) => new(tag, 3, values);
    private static Field Longs(int tag, params uint[] values) => new(tag, 4, values);
    private static Field Ascii(int tag, string value) => new(tag, 2, Encoding.ASCII.GetBytes(value + "\0"));

    private sealed record Field(int Tag, int Format, Array Values);

    private sealed class ImageIfd(params Field[] fields)
    {
        public Field[] Fields { get; } = [Longs(Width, 321), .. fields];
        public Dictionary<int, ImageIfd[]> Links { get; } = [];
        public ImageIfd? Next { get; set; }
    }

    // Synthesize only the TIFF envelope; all parsing/normalization and classification use public readers.
    private static byte[] Build(bool bigTiff, bool bigEndian, params ImageIfd[] images)
    {
        var countSize = bigTiff ? 8 : 2;
        var entrySize = bigTiff ? 20 : 12;
        var offsetSize = bigTiff ? 8 : 4;
        var headerSize = bigTiff ? 16 : 8;
        var offsets = new Dictionary<ImageIfd, int>();
        var position = headerSize;
        foreach (var image in images)
        {
            if (bigTiff)
                position = (position + 7) & ~7;
            offsets.Add(image, position);
            position += countSize + (image.Fields.Length + image.Links.Count) * entrySize + offsetSize;
        }

        using var stream = new MemoryStream();
        stream.SetLength(position);
        Write(bigEndian ? 0x4D4DUL : 0x4949UL, 2);
        Write(bigTiff ? 43UL : 42UL, 2);
        if (bigTiff)
        {
            Write(8, 2);
            Write(0, 2);
        }
        Write((ulong)headerSize, offsetSize);
        foreach (var image in images)
        {
            stream.Position = offsets[image];
            var fields = image.Fields.Concat(image.Links.Select(link =>
                Longs(link.Key, link.Value.Select(child => (uint)offsets[child]).ToArray())))
                .OrderBy(field => field.Tag).ToArray();
            Write((ulong)fields.Length, countSize);
            foreach (var field in fields)
            {
                var data = Encode(field.Values, bigEndian);
                Write((ulong)field.Tag, 2);
                Write((ulong)field.Format, 2);
                Write((ulong)field.Values.Length, bigTiff ? 8 : 4);
                if (data.Length <= offsetSize)
                {
                    stream.Write(data, 0, data.Length);
                    for (var i = data.Length; i < offsetSize; i++)
                        stream.WriteByte(0);
                }
                else
                {
                    var dataOffset = (stream.Length + 7) & ~7L;
                    Write((ulong)dataOffset, offsetSize);
                    var resume = stream.Position;
                    stream.Position = dataOffset;
                    stream.Write(data, 0, data.Length);
                    stream.Position = resume;
                }
            }
            Write(image.Next is null ? 0UL : (ulong)offsets[image.Next], offsetSize);
        }
        return stream.ToArray();

        void Write(ulong value, int size) => WriteUnsigned(stream, value, size, bigEndian);
    }

    private static byte[] Encode(Array values, bool bigEndian)
    {
        using var stream = new MemoryStream();
        foreach (var value in values)
        {
            switch (value)
            {
                case byte v: stream.WriteByte(v); break;
                case ushort v: WriteUnsigned(stream, v, 2, bigEndian); break;
                case uint v: WriteUnsigned(stream, v, 4, bigEndian); break;
                case Rational v:
                    WriteUnsigned(stream, unchecked((uint)v.Numerator), 4, bigEndian);
                    WriteUnsigned(stream, unchecked((uint)v.Denominator), 4, bigEndian);
                    break;
                case double v:
                    var bytes = BitConverter.GetBytes(v);
                    if (BitConverter.IsLittleEndian == bigEndian)
                        Array.Reverse(bytes);
                    stream.Write(bytes, 0, bytes.Length);
                    break;
                default: throw new ArgumentException("Unsupported synthetic TIFF value type.");
            }
        }
        return stream.ToArray();
    }

    private static void WriteUnsigned(Stream stream, ulong value, int size, bool bigEndian)
    {
        for (var i = 0; i < size; i++)
            stream.WriteByte((byte)(value >> (8 * (bigEndian ? size - i - 1 : i))));
    }
}

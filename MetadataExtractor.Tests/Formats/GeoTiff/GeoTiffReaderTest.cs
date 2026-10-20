// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Tiff;

namespace MetadataExtractor.Formats.GeoTiff;

public sealed class GeoTiffReaderTest
{
    private const int Keys = ExifDirectoryBase.TagGeoTiffGeoKeys;
    private const int DoubleParams = ExifDirectoryBase.TagGeoTiffGeoDoubleParams;
    private const int AsciiParams = ExifDirectoryBase.TagGeoTiffGeoAsciiParams;
    private const int Sentinel = ExifDirectoryBase.TagImageWidth;
    private const int PrivateKey = 65000;

    [Theory]
    [CombinatorialData]
    public void ReadsSharedPoolsAndRetainsSourceMetadata(bool bigTiff, bool bigEndian, bool exifReader)
    {
        ushort[] keys =
        [
            1, 1, 1, 10,
            GeoTiffDirectory.TagModelType, 0, 1, 2,
            GeoTiffDirectory.TagRasterType, 0, 1, 1,
            GeoTiffDirectory.TagGeographicType, 0, 1, 4326,
            GeoTiffDirectory.TagGeographicAngularUnits, 0, 1, 9102,
            PrivateKey, Keys, 1, 44,
            PrivateKey + 1, Keys, 2, 45,
            GeoTiffDirectory.TagGeographicSemiMajorAxis, DoubleParams, 1, 1,
            GeoTiffDirectory.TagGeographicToWgs84, DoubleParams, 2, 2,
            GeoTiffDirectory.TagCitation, AsciiParams, 6, 2,
            GeoTiffDirectory.TagGeographicCitation, AsciiParams, 4, 8,
            60000, 50000, 40000
        ];
        double[] doubles = [-9.5, 6378137.25, 1.5, -2.75];
        byte[] ascii = Encoding.ASCII.GetBytes("xxA|B|||End|");
        double[] scale = [0.5, 0.25, 0];
        double[] tiePoint = [0, 0, 0, 123.5, -45.25, 0];
        double[] transformation = [1, 0, 0, 123, 0, -1, 0, 456, 0, 0, 1, 0, 0, 0, 0, 1];
        var image = new ImageIfd(
            Shorts(Keys, keys), Doubles(DoubleParams, doubles), Ascii(ascii),
            Doubles(ExifDirectoryBase.TagPixelScale, scale),
            Doubles(ExifDirectoryBase.TagModelTiePoint, tiePoint),
            Doubles(ExifDirectoryBase.TagModelTransformation, transformation));

        var directories = Read(Build(bigTiff, bigEndian, image), exifReader);

        var source = Assert.Single(directories.OfType<ExifIfd0Directory>());
        var geo = Assert.Single(directories.OfType<GeoTiffDirectory>());
        Assert.Same(source, geo.Parent);
        Assert.Empty(source.Errors);
        Assert.Empty(geo.Errors);
        Assert.Equal(10, geo.TagCount);
        Assert.Equal((ushort)2, Assert.IsType<ushort>(geo.GetObject(GeoTiffDirectory.TagModelType)));
        Assert.Equal((ushort)1, Assert.IsType<ushort>(geo.GetObject(GeoTiffDirectory.TagRasterType)));
        Assert.Equal((ushort)4326, Assert.IsType<ushort>(geo.GetObject(GeoTiffDirectory.TagGeographicType)));
        Assert.Equal("Geographic", geo.GetDescription(GeoTiffDirectory.TagModelType));
        Assert.Equal("Angular Degree", geo.GetDescription(GeoTiffDirectory.TagGeographicAngularUnits));
        Assert.Equal(new ushort[] { 60000 }, Assert.IsType<ushort[]>(geo.GetObject(PrivateKey)));
        Assert.Equal(new ushort[] { 50000, 40000 }, Assert.IsType<ushort[]>(geo.GetObject(PrivateKey + 1)));
        Assert.Equal(new[] { 6378137.25 }, Assert.IsType<double[]>(geo.GetObject(GeoTiffDirectory.TagGeographicSemiMajorAxis)));
        Assert.Equal(new[] { 1.5, -2.75 }, Assert.IsType<double[]>(geo.GetObject(GeoTiffDirectory.TagGeographicToWgs84)));
        Assert.Equal("A|B||", Assert.IsType<string>(geo.GetObject(GeoTiffDirectory.TagCitation)));
        Assert.Equal("End", Assert.IsType<string>(geo.GetObject(GeoTiffDirectory.TagGeographicCitation)));
        Assert.Equal(keys, Assert.IsType<ushort[]>(source.GetObject(Keys)));
        Assert.Equal(doubles, Assert.IsType<double[]>(source.GetObject(DoubleParams)));
        Assert.Equal(ascii, Assert.IsType<StringValue>(source.GetObject(AsciiParams)).Bytes);
        Assert.Equal(scale, Assert.IsType<double[]>(source.GetObject(ExifDirectoryBase.TagPixelScale)));
        Assert.Equal(tiePoint, Assert.IsType<double[]>(source.GetObject(ExifDirectoryBase.TagModelTiePoint)));
        Assert.Equal(transformation, Assert.IsType<double[]>(source.GetObject(ExifDirectoryBase.TagModelTransformation)));
        Assert.Equal(34264, ExifDirectoryBase.TagModelTransformation);
        Assert.Equal("Model Transformation", source.GetTagName(ExifDirectoryBase.TagModelTransformation));
        AssertSentinel(source);
    }

    [Theory]
    [CombinatorialData]
    public void NormalizesScalarDoublePoolToOneElementArray(bool bigTiff, bool bigEndian)
    {
        ushort[] keys = [1, 1, 0, 1, GeoTiffDirectory.TagGeographicSemiMajorAxis, DoubleParams, 1, 0];
        var directories = Read(Build(bigTiff, bigEndian, new ImageIfd(Shorts(Keys, keys), Doubles(DoubleParams, 6378137.5))));

        var source = Assert.Single(directories.OfType<ExifIfd0Directory>());
        var geo = Assert.Single(directories.OfType<GeoTiffDirectory>());
        Assert.Same(source, geo.Parent);
        Assert.Empty(geo.Errors);
        Assert.Equal(new[] { 6378137.5 }, Assert.IsType<double[]>(geo.GetObject(GeoTiffDirectory.TagGeographicSemiMajorAxis)));
        Assert.Equal(6378137.5, Assert.IsType<double>(source.GetObject(DoubleParams)));
        Assert.Equal(keys, Assert.IsType<ushort[]>(source.GetObject(Keys)));
        AssertSentinel(source);
    }

    [Theory]
    [CombinatorialData]
    public void RejectsNonAsciiSlicesWithoutPoisoningLaterAsciiKeys(bool bigTiff, bool bigEndian)
    {
        ushort[] keys =
        [
            1, 1, 1, 5,
            PrivateKey, AsciiParams, 2, 0,
            PrivateKey + 1, AsciiParams, 4, 1,
            PrivateKey + 2, AsciiParams, 1, 3,
            PrivateKey + 3, AsciiParams, 2, 3,
            PrivateKey + 4, AsciiParams, 1, 2
        ];
        // A UTF-8 decoder would combine the first two bytes, and character-indexed
        // slicing would then select the wrong characters for subsequent keys.
        byte[] ascii = [0xc3, 0xa9, (byte)'X', (byte)'|', (byte)'|'];
        var image = new ImageIfd(Shorts(Keys, keys), Ascii(ascii));

        var directories = Read(Build(bigTiff, bigEndian, image));

        var source = Assert.Single(directories.OfType<ExifIfd0Directory>());
        var geo = Assert.Single(directories.OfType<GeoTiffDirectory>());
        Assert.Same(source, geo.Parent);
        Assert.Equal(2, geo.Errors.Count());
        Assert.Equal(3, geo.TagCount);
        Assert.False(geo.ContainsTag(PrivateKey));
        Assert.False(geo.ContainsTag(PrivateKey + 1));
        Assert.Equal("", Assert.IsType<string>(geo.GetObject(PrivateKey + 2)));
        Assert.Equal("|", Assert.IsType<string>(geo.GetObject(PrivateKey + 3)));
        Assert.Equal("X", Assert.IsType<string>(geo.GetObject(PrivateKey + 4)));
        AssertRawFields(source, image);
    }

    [Theory]
    [CombinatorialData]
    public void ReadsAsciiSliceWhoseOffsetPlusCountExceedsUshortRange(
        bool bigTiff, bool bigEndian, [CombinatorialValues(2, 65535)] int offset)
    {
        const ushort count = ushort.MaxValue;
        var ascii = Enumerable.Repeat((byte)'A', offset + count).ToArray();
        ascii[0] = (byte)'x';
        ascii[1] = (byte)'x';
        ascii[ascii.Length - 1] = (byte)'|';
        var image = new ImageIfd(
            Shorts(Keys, 1, 1, 1, 1, GeoTiffDirectory.TagCitation, AsciiParams, count, (ushort)offset),
            Ascii(ascii));

        var directories = Read(Build(bigTiff, bigEndian, image));

        var source = Assert.Single(directories.OfType<ExifIfd0Directory>());
        var geo = Assert.Single(directories.OfType<GeoTiffDirectory>());
        Assert.Same(source, geo.Parent);
        Assert.Empty(geo.Errors);
        Assert.Equal(1, geo.TagCount);
        Assert.Equal(new string('A', count - 1), Assert.IsType<string>(geo.GetObject(GeoTiffDirectory.TagCitation)));
        AssertRawFields(source, image);
    }

    [Theory]
    [CombinatorialData]
    public void RejectsNonAsciiBytesAtAnyPositionInTheSlice(
        bool bigTiff, bool bigEndian,
        [CombinatorialRange(0, 4)] int position,
        [CombinatorialValues(128, 255)] int invalidByte)
    {
        byte[] ascii = [(byte)'A', (byte)'B', (byte)'C', (byte)'|'];
        ascii[position] = (byte)invalidByte;
        var image = new ImageIfd(
            Shorts(Keys, 1, 1, 1, 2,
                GeoTiffDirectory.TagCitation, AsciiParams, 4, 0,
                GeoTiffDirectory.TagRasterType, 0, 1, 2),
            Ascii(ascii));

        var directories = Read(Build(bigTiff, bigEndian, image));

        var source = Assert.Single(directories.OfType<ExifIfd0Directory>());
        var geo = Assert.Single(directories.OfType<GeoTiffDirectory>());
        Assert.Same(source, geo.Parent);
        Assert.Single(geo.Errors);
        Assert.Equal(1, geo.TagCount);
        Assert.False(geo.ContainsTag(GeoTiffDirectory.TagCitation));
        Assert.Equal((ushort)2, Assert.IsType<ushort>(geo.GetObject(GeoTiffDirectory.TagRasterType)));
        AssertRawFields(source, image);
    }

    [Theory]
    [CombinatorialData]
    public void SliceAcrossTiffStringTerminatorIsAnErrorAndLaterKeysStillDecode(bool bigTiff, bool bigEndian)
    {
        ushort[] keys =
        [
            1, 1, 1, 2,
            GeoTiffDirectory.TagCitation, AsciiParams, 4, 0,
            GeoTiffDirectory.TagRasterType, 0, 1, 2
        ];
        var image = new ImageIfd(Shorts(Keys, keys), Ascii((byte)'A', 0, (byte)'B', (byte)'|'));

        var directories = Read(Build(bigTiff, bigEndian, image));

        var source = Assert.Single(directories.OfType<ExifIfd0Directory>());
        var geo = Assert.Single(directories.OfType<GeoTiffDirectory>());
        Assert.Same(source, geo.Parent);
        Assert.Single(geo.Errors);
        Assert.Equal(1, geo.TagCount);
        Assert.False(geo.ContainsTag(GeoTiffDirectory.TagCitation));
        Assert.Equal((ushort)2, Assert.IsType<ushort>(geo.GetObject(GeoTiffDirectory.TagRasterType)));
        // TIFF string normalization stops at NUL before GeoTIFF sees the pool.
        // Preserve that original StringValue rather than asserting on discarded bytes.
        Assert.Equal(new byte[] { (byte)'A' }, Assert.IsType<StringValue>(source.GetObject(AsciiParams)).Bytes);
        Assert.Equal(keys, Assert.IsType<ushort[]>(source.GetObject(Keys)));
        Assert.Empty(source.Errors);
        AssertSentinel(source);
    }

    [Theory]
    [CombinatorialData]
    public void KeepsPoolsLocalToIfd0StandardSubIfdsAndFollowerImages(bool bigTiff, bool bigEndian, bool exifReader)
    {
        var names = new[] { "Root|", "First child|", "Second child|", "Thumbnail|", "Follower image|" };
        var images = names.Select((name, i) => new ImageIfd(
            Shorts(Keys,
                1, 1, 0, 4,
                GeoTiffDirectory.TagModelType, 0, 1, (ushort)(i + 1),
                PrivateKey, Keys, 1, 20,
                PrivateKey + 1, DoubleParams, 1, 1,
                GeoTiffDirectory.TagCitation, AsciiParams, (ushort)name.Length, 0,
                (ushort)(50000 + i)),
            Doubles(DoubleParams, i + 0.125, i + 10.25),
            Ascii(Encoding.ASCII.GetBytes(name)))).ToArray();
        images[0].Children = [images[1], images[2]];
        images[0].Next = images[3];
        images[3].Next = images[4];

        var directories = Read(Build(bigTiff, bigEndian, images), exifReader);

        Assert.Equal(10, directories.Count);
        var sources = directories.Where(directory => directory is not GeoTiffDirectory).ToArray();
        Assert.IsType<ExifIfd0Directory>(sources[0]);
        Assert.IsType<ExifSubIfdDirectory>(sources[1]);
        Assert.IsType<ExifSubIfdDirectory>(sources[2]);
        Assert.IsType<ExifThumbnailDirectory>(sources[3]);
        Assert.IsType<ExifImageDirectory>(sources[4]);
        Assert.Null(sources[0].Parent);
        Assert.Same(sources[0], sources[1].Parent);
        Assert.Same(sources[0], sources[2].Parent);
        Assert.Same(sources[0], sources[3].Parent);
        Assert.Same(sources[3], sources[4].Parent);
        for (var i = 0; i < sources.Length; i++)
        {
            var source = sources[i];
            var geo = Assert.Single(directories.OfType<GeoTiffDirectory>(), directory => ReferenceEquals(source, directory.Parent));
            Assert.Empty(geo.Errors);
            Assert.Equal(4, geo.TagCount);
            Assert.Equal((ushort)(i + 1), Assert.IsType<ushort>(geo.GetObject(GeoTiffDirectory.TagModelType)));
            Assert.Equal(new ushort[] { (ushort)(50000 + i) }, Assert.IsType<ushort[]>(geo.GetObject(PrivateKey)));
            Assert.Equal(new[] { i + 10.25 }, Assert.IsType<double[]>(geo.GetObject(PrivateKey + 1)));
            Assert.Equal(names[i].Substring(0, names[i].Length - 1), Assert.IsType<string>(geo.GetObject(GeoTiffDirectory.TagCitation)));
            AssertRawFields(source, images[i]);
        }
    }

    [Theory]
    [CombinatorialData]
    public void MissingPoolsDoNotBorrowFromParentOrSiblings(bool bigTiff, bool bigEndian)
    {
        ushort[] keys =
        [
            1, 1, 0, 3,
            PrivateKey, DoubleParams, 1, 0,
            GeoTiffDirectory.TagCitation, AsciiParams, 4, 0,
            GeoTiffDirectory.TagRasterType, 0, 1, 2
        ];
        var root = new ImageIfd(Shorts(Keys, keys), Doubles(DoubleParams, 12.5), Ascii(Encoding.ASCII.GetBytes("Own|")));
        var child = new ImageIfd(Shorts(Keys, keys));
        var sibling = new ImageIfd(Shorts(Keys, keys));
        var thumbnail = new ImageIfd(Shorts(Keys, keys));
        var follower = new ImageIfd(Shorts(Keys, keys));
        root.Children = [child, sibling];
        root.Next = thumbnail;
        thumbnail.Next = follower;

        var directories = Read(Build(bigTiff, bigEndian, root, child, sibling, thumbnail, follower));

        Assert.Equal(5, directories.OfType<GeoTiffDirectory>().Count());
        var rootSource = Assert.Single(directories.OfType<ExifIfd0Directory>());
        var rootGeo = Assert.Single(directories.OfType<GeoTiffDirectory>(), directory => ReferenceEquals(rootSource, directory.Parent));
        Assert.Empty(rootGeo.Errors);
        Assert.Equal(new[] { 12.5 }, Assert.IsType<double[]>(rootGeo.GetObject(PrivateKey)));
        Assert.Equal("Own", rootGeo.GetString(GeoTiffDirectory.TagCitation));
        AssertRawFields(rootSource, root);
        foreach (var geo in directories.OfType<GeoTiffDirectory>().Where(directory => !ReferenceEquals(rootSource, directory.Parent)))
        {
            Assert.Equal(2, geo.Errors.Count());
            Assert.Equal(1, geo.TagCount);
            Assert.False(geo.ContainsTag(PrivateKey));
            Assert.False(geo.ContainsTag(GeoTiffDirectory.TagCitation));
            Assert.Equal((ushort)2, Assert.IsType<ushort>(geo.GetObject(GeoTiffDirectory.TagRasterType)));
            var source = Assert.IsAssignableFrom<ExifDirectoryBase>(geo.Parent);
            Assert.Equal(keys, Assert.IsType<ushort[]>(source.GetObject(Keys)));
            Assert.False(source.ContainsTag(DoubleParams));
            Assert.False(source.ContainsTag(AsciiParams));
            Assert.Empty(source.Errors);
            AssertSentinel(source);
        }
    }

    [Theory]
    [CombinatorialData]
    public void RejectsInvalidHeadersWithoutThrowingOrPartiallyDecoding(
        bool bigTiff, bool bigEndian, bool exifReader,
        [CombinatorialRange(0, 16)] int scenario)
    {
        var field = scenario switch
        {
            0 => Shorts(Keys),
            1 => Shorts(Keys, 1),
            2 => Shorts(Keys, 1, 1),
            3 => Shorts(Keys, 1, 1, 0),
            4 => new Field(Keys, 7, new byte[] { 1, 1, 0, 1, 0, 4, 1, 2 }),
            5 => Longs(Keys, 1, 1, 0, 1, GeoTiffDirectory.TagModelType, 0, 1, 2),
            6 => new Field(Keys, 8, new short[] { 1, 1, 0, 1, GeoTiffDirectory.TagModelType, 0, 1, 2 }),
            7 => Shorts(Keys, 2, 1, 0, 1, GeoTiffDirectory.TagModelType, 0, 1, 2),
            8 => Shorts(Keys, 1, 0, 0, 1, GeoTiffDirectory.TagModelType, 0, 1, 2),
            9 => Shorts(Keys, 1, 2, 0, 1, GeoTiffDirectory.TagModelType, 0, 1, 2),
            10 => Shorts(Keys, 1, 1, 2, 1, GeoTiffDirectory.TagModelType, 0, 1, 2),
            // The first key is valid, but the declared table must be validated in
            // its entirety before decoding any key.
            11 => Shorts(Keys, 1, 1, 0, 2, GeoTiffDirectory.TagModelType, 0, 1, 2),
            12 => Shorts(Keys, 1, 1, 0, 1, GeoTiffDirectory.TagModelType, 0, 1),
            13 => Shorts(Keys, 1, 1, 0, ushort.MaxValue, GeoTiffDirectory.TagModelType, 0, 1, 2),
            14 => Shorts(Keys, 0, 1, 0, 1, GeoTiffDirectory.TagModelType, 0, 1, 2),
            15 => Shorts(Keys, 1, 1, ushort.MaxValue, 1, GeoTiffDirectory.TagModelType, 0, 1, 2),
            _ => throw new ArgumentOutOfRangeException(nameof(scenario))
        };
        var image = new ImageIfd(field, Doubles(DoubleParams, 1.25), Ascii(Encoding.ASCII.GetBytes("Raw|")));

        var directories = Read(Build(bigTiff, bigEndian, image), exifReader);

        Assert.Equal(2, directories.Count);
        var source = Assert.Single(directories.OfType<ExifIfd0Directory>());
        var geo = Assert.Single(directories.OfType<GeoTiffDirectory>());
        Assert.Same(source, geo.Parent);
        Assert.NotEmpty(geo.Errors);
        Assert.Equal(0, geo.TagCount);
        AssertRawFields(source, image);
    }

    [Theory]
    [CombinatorialData]
    public void AcceptsBothSupportedVersionsWithZeroKeys(
        bool bigTiff, bool bigEndian, [CombinatorialValues(0, 1)] int minorVersion)
    {
        var image = new ImageIfd(Shorts(Keys, 1, 1, (ushort)minorVersion, 0),
            Doubles(DoubleParams, 9.25), Ascii(Encoding.ASCII.GetBytes("Unused|")));

        var directories = Read(Build(bigTiff, bigEndian, image));

        var source = Assert.Single(directories.OfType<ExifIfd0Directory>());
        var geo = Assert.Single(directories.OfType<GeoTiffDirectory>());
        Assert.Same(source, geo.Parent);
        Assert.Empty(geo.Errors);
        Assert.Equal(0, geo.TagCount);
        AssertRawFields(source, image);
    }

    [Theory]
    [CombinatorialData]
    public void PoolsWithoutAKeyDirectoryRemainOrdinaryTiffMetadata(bool bigTiff, bool bigEndian)
    {
        var image = new ImageIfd(Doubles(DoubleParams, 1.25, 2.5), Ascii(Encoding.ASCII.GetBytes("Unused|")));

        var directories = Read(Build(bigTiff, bigEndian, image));

        var source = Assert.IsType<ExifIfd0Directory>(Assert.Single(directories));
        AssertRawFields(source, image);
        Assert.False(source.ContainsTag(Keys));
    }

    [Theory]
    [CombinatorialData]
    public void InvalidKeyReferencesReportErrorsAndContinueWithLaterKeys(
        bool bigTiff, bool bigEndian,
        [CombinatorialValues(
            "zero-count", "zero-short-count", "zero-double-count", "zero-ascii-count", "inline-multiple", "unsupported-location",
            "short-header", "short-table", "short-end", "short-overrun",
            "double-missing", "double-short", "double-long", "double-float", "double-bytes", "double-ascii",
            "double-scalar-end", "double-scalar-overrun", "double-array-end", "double-array-overrun",
            "ascii-missing", "ascii-bytes", "ascii-short", "ascii-double", "ascii-end", "ascii-overrun",
            "high-short-offset", "high-double-offset", "high-ascii-offset")] string scenario)
    {
        (int location, int count, int offset, Field? pool) = scenario switch
        {
            "zero-count" => (0, 0, 123, null),
            "zero-short-count" => (Keys, 0, 12, null),
            "zero-double-count" => (DoubleParams, 0, 0, Doubles(DoubleParams, 7.5)),
            "zero-ascii-count" => (AsciiParams, 0, 0, Ascii(Encoding.ASCII.GetBytes("A|"))),
            "inline-multiple" => (0, 2, 123, null),
            "unsupported-location" => (500, 1, 0, Shorts(500, 5, 6)),
            "short-header" => (Keys, 1, 3, null),
            "short-table" => (Keys, 1, 11, null),
            "short-end" => (Keys, 1, 14, null),
            "short-overrun" => (Keys, 2, 13, null),
            "double-missing" => (DoubleParams, 1, 0, null),
            "double-short" => (DoubleParams, 1, 0, Shorts(DoubleParams, 1, 2)),
            "double-long" => (DoubleParams, 1, 0, Longs(DoubleParams, 1, 2)),
            "double-float" => (DoubleParams, 1, 0, new Field(DoubleParams, 11, new float[] { 1, 2 })),
            "double-bytes" => (DoubleParams, 1, 0, new Field(DoubleParams, 7, new byte[] { 1, 2 })),
            "double-ascii" => (DoubleParams, 1, 0, new Field(DoubleParams, 2, Encoding.ASCII.GetBytes("1|"))),
            "double-scalar-end" => (DoubleParams, 1, 1, Doubles(DoubleParams, 7.5)),
            "double-scalar-overrun" => (DoubleParams, 2, 0, Doubles(DoubleParams, 7.5)),
            "double-array-end" => (DoubleParams, 1, 2, Doubles(DoubleParams, 7.5, 8.5)),
            "double-array-overrun" => (DoubleParams, 2, 1, Doubles(DoubleParams, 7.5, 8.5)),
            "ascii-missing" => (AsciiParams, 1, 0, null),
            "ascii-bytes" => (AsciiParams, 1, 0, new Field(AsciiParams, 7, new byte[] { 65, 124 })),
            "ascii-short" => (AsciiParams, 1, 0, Shorts(AsciiParams, 65, 124)),
            "ascii-double" => (AsciiParams, 1, 0, Doubles(AsciiParams, 65, 124)),
            "ascii-end" => (AsciiParams, 1, 2, Ascii(Encoding.ASCII.GetBytes("A|"))),
            "ascii-overrun" => (AsciiParams, 2, 1, Ascii(Encoding.ASCII.GetBytes("A|"))),
            "high-short-offset" => (Keys, ushort.MaxValue, ushort.MaxValue, null),
            "high-double-offset" => (DoubleParams, ushort.MaxValue, ushort.MaxValue, Doubles(DoubleParams, 7.5)),
            "high-ascii-offset" => (AsciiParams, ushort.MaxValue, ushort.MaxValue, Ascii(Encoding.ASCII.GetBytes("A|"))),
            _ => throw new ArgumentOutOfRangeException(nameof(scenario))
        };
        ushort[] keys =
        [
            1, 1, 0, 2,
            PrivateKey, (ushort)location, (ushort)count, (ushort)offset,
            GeoTiffDirectory.TagRasterType, 0, 1, 2,
            77, 88
        ];
        var image = pool is null
            ? new ImageIfd(Shorts(Keys, keys))
            : new ImageIfd(Shorts(Keys, keys), pool);

        var directories = Read(Build(bigTiff, bigEndian, image));

        var source = Assert.Single(directories.OfType<ExifIfd0Directory>());
        var geo = Assert.Single(directories.OfType<GeoTiffDirectory>());
        Assert.Same(source, geo.Parent);
        Assert.Single(geo.Errors);
        Assert.Equal(1, geo.TagCount);
        Assert.False(geo.ContainsTag(PrivateKey));
        Assert.Equal((ushort)2, Assert.IsType<ushort>(geo.GetObject(GeoTiffDirectory.TagRasterType)));
        AssertRawFields(source, image);
    }

    [Theory]
    [CombinatorialData]
    public void FirstDuplicateWinsEvenWhenItsValueIsInvalid(bool bigTiff, bool bigEndian, bool invalidFirst)
    {
        var image = new ImageIfd(Shorts(Keys,
            1, 1, 1, 3,
            PrivateKey, 0, (ushort)(invalidFirst ? 0 : 1), 12345,
            PrivateKey, 0, 1, 54321,
            GeoTiffDirectory.TagRasterType, 0, 1, 2));

        var directories = Read(Build(bigTiff, bigEndian, image));

        var source = Assert.Single(directories.OfType<ExifIfd0Directory>());
        var geo = Assert.Single(directories.OfType<GeoTiffDirectory>());
        Assert.Same(source, geo.Parent);
        Assert.Equal(invalidFirst ? 2 : 1, geo.Errors.Count());
        Assert.Equal(invalidFirst ? 1 : 2, geo.TagCount);
        if (invalidFirst)
            Assert.False(geo.ContainsTag(PrivateKey));
        else
            Assert.Equal((ushort)12345, Assert.IsType<ushort>(geo.GetObject(PrivateKey)));
        Assert.Equal((ushort)2, Assert.IsType<ushort>(geo.GetObject(GeoTiffDirectory.TagRasterType)));
        AssertRawFields(source, image);
    }

    [Theory]
    [CombinatorialData]
    public void AcceptsHighUnsignedOffsetsAndSlicesEndingExactlyAtPoolEnd(
        bool bigTiff, bool bigEndian, [CombinatorialValues(Keys, DoubleParams, AsciiParams)] int location)
    {
        const ushort offset = ushort.MaxValue;
        ushort[] keys = [1, 1, 0, 1, PrivateKey, (ushort)location, 2, offset];
        Field pool;
        if (location == Keys)
        {
            Array.Resize(ref keys, offset + 2);
            keys[offset] = 50000;
            keys[offset + 1] = 60000;
            pool = Shorts(Keys, keys);
        }
        else if (location == DoubleParams)
        {
            var values = new double[offset + 2];
            values[offset] = -123.5;
            values[offset + 1] = 456.25;
            pool = Doubles(DoubleParams, values);
        }
        else
        {
            var values = Enumerable.Repeat((byte)'x', offset + 2).ToArray();
            values[offset] = (byte)'Q';
            values[offset + 1] = (byte)'|';
            pool = Ascii(values);
        }
        var image = location == Keys ? new ImageIfd(pool) : new ImageIfd(Shorts(Keys, keys), pool);

        var directories = Read(Build(bigTiff, bigEndian, image));

        var source = Assert.Single(directories.OfType<ExifIfd0Directory>());
        var geo = Assert.Single(directories.OfType<GeoTiffDirectory>());
        Assert.Same(source, geo.Parent);
        Assert.Empty(geo.Errors);
        Assert.Equal(1, geo.TagCount);
        if (location == Keys)
            Assert.Equal(new ushort[] { 50000, 60000 }, Assert.IsType<ushort[]>(geo.GetObject(PrivateKey)));
        else if (location == DoubleParams)
            Assert.Equal(new[] { -123.5, 456.25 }, Assert.IsType<double[]>(geo.GetObject(PrivateKey)));
        else
            Assert.Equal("Q", Assert.IsType<string>(geo.GetObject(PrivateKey)));
        AssertRawFields(source, image);
    }

    [Theory]
    [CombinatorialData]
    public void PreservesUnknownNumericDescriptorCodesAndPrivateKeyIds(bool bigTiff, bool bigEndian)
    {
        var ids = new[]
        {
            GeoTiffDirectory.TagModelType, GeoTiffDirectory.TagRasterType,
            GeoTiffDirectory.TagGeographicType, GeoTiffDirectory.TagGeodeticDatum,
            GeoTiffDirectory.TagGeographicPrimeMeridian, GeoTiffDirectory.TagGeographicLinearUnits,
            GeoTiffDirectory.TagGeographicAngularUnits, GeoTiffDirectory.TagGeographicEllipsoid,
            GeoTiffDirectory.TagProjectedCSType, GeoTiffDirectory.TagProjection,
            GeoTiffDirectory.TagProjectedCoordinateTransform, GeoTiffDirectory.TagProjLinearUnits,
            GeoTiffDirectory.TagVerticalCSType, GeoTiffDirectory.TagVerticalDatum, GeoTiffDirectory.TagVerticalUnits
        };
        var keys = new List<ushort> { 1, 1, 1, (ushort)(ids.Length + 1) };
        foreach (var id in ids)
            keys.AddRange([(ushort)id, 0, 1, 60000]);
        keys.AddRange([ushort.MaxValue, 0, 1, 50000]);
        var image = new ImageIfd(Shorts(Keys, keys.ToArray()));

        var directories = Read(Build(bigTiff, bigEndian, image));

        var source = Assert.Single(directories.OfType<ExifIfd0Directory>());
        var geo = Assert.Single(directories.OfType<GeoTiffDirectory>());
        Assert.Same(source, geo.Parent);
        Assert.Empty(geo.Errors);
        Assert.Equal(ids.Length + 1, geo.TagCount);
        foreach (var id in ids)
        {
            Assert.Equal((ushort)60000, Assert.IsType<ushort>(geo.GetObject(id)));
            Assert.Equal("Unknown (60000)", geo.GetDescription(id));
        }
        Assert.Equal((ushort)50000, Assert.IsType<ushort>(geo.GetObject(ushort.MaxValue)));
        Assert.Equal("50000", geo.GetDescription(ushort.MaxValue));
        AssertRawFields(source, image);
    }

    [Theory]
    [CombinatorialData]
    public void DescribesAdditionalProjectedCodesAndUsesDistinctLongitudeAndAngleLabels(
        bool bigTiff, bool bigEndian,
        [CombinatorialValues(28191, 28192, 28193, 32064, 32065, 32066, 32067, 32074, 32075, 32076, 32077, 60000)] int code)
    {
        var expected = code switch
        {
            28191 => "Palestine 1923 Palestine Grid",
            28192 => "Palestine 1923 Palestine Belt",
            28193 => "Palestine 1923 Israeli CS Grid",
            32064 => "NAD27 BLM 14N FtUS",
            32065 => "NAD27 BLM 15N FtUS",
            32066 => "NAD27 BLM 16N FtUS",
            32067 => "NAD27 BLM 17N FtUS",
            32074 => "NAD27 BLM 14N Feet",
            32075 => "NAD27 BLM 15N Feet",
            32076 => "NAD27 BLM 16N Feet",
            32077 => "NAD27 BLM 17N Feet",
            60000 => "Unknown (60000)",
            _ => throw new ArgumentOutOfRangeException(nameof(code))
        };
        var image = new ImageIfd(
            Shorts(Keys, 1, 1, 1, 3,
                GeoTiffDirectory.TagProjectedCSType, 0, 1, (ushort)code,
                GeoTiffDirectory.TagGeographicPrimeMeridianLong, DoubleParams, 1, 0,
                GeoTiffDirectory.TagProjRectifiedGridAngle, DoubleParams, 1, 1),
            Doubles(DoubleParams, 2.33722917, 0.75));

        var directories = Read(Build(bigTiff, bigEndian, image));

        var source = Assert.Single(directories.OfType<ExifIfd0Directory>());
        var geo = Assert.Single(directories.OfType<GeoTiffDirectory>());
        Assert.Same(source, geo.Parent);
        Assert.Empty(geo.Errors);
        Assert.Equal(3, geo.TagCount);
        Assert.Equal((ushort)code, Assert.IsType<ushort>(geo.GetObject(GeoTiffDirectory.TagProjectedCSType)));
        Assert.Equal(expected, geo.GetDescription(GeoTiffDirectory.TagProjectedCSType));
        Assert.Equal("Prime Meridian Longitude", geo.GetTagName(GeoTiffDirectory.TagGeographicPrimeMeridianLong));
        Assert.Equal("Projection Rectified Grid Angle", geo.GetTagName(GeoTiffDirectory.TagProjRectifiedGridAngle));
        Assert.Equal(new[] { 2.33722917 }, Assert.IsType<double[]>(geo.GetObject(GeoTiffDirectory.TagGeographicPrimeMeridianLong)));
        Assert.Equal(new[] { 0.75 }, Assert.IsType<double[]>(geo.GetObject(GeoTiffDirectory.TagProjRectifiedGridAngle)));
        AssertRawFields(source, image);
    }

    private static IReadOnlyList<Directory> Read(byte[] bytes, bool exifReader = false)
    {
        if (exifReader)
            return new ExifReader().Extract(new ByteArrayReader(bytes), exifStartOffset: 0);

        using var stream = new MemoryStream(bytes);
        return TiffMetadataReader.ReadMetadata(stream);
    }

    private static void AssertSentinel(Directory source)
    {
        Assert.Equal(321u, Assert.IsType<uint>(source.GetObject(Sentinel)));
    }

    private static void AssertRawFields(Directory source, ImageIfd image)
    {
        Assert.Empty(source.Errors);
        foreach (var field in image.Fields)
        {
            var actual = source.GetObject(field.Tag);
            if (field.Format == 2)
            {
                Assert.Equal((byte[])field.Values, Assert.IsType<StringValue>(actual).Bytes);
            }
            else if (field.Values.Length == 1 && field.Format != 7)
            {
                var expected = field.Values.GetValue(0)!;
                Assert.IsType(expected.GetType(), actual);
                Assert.Equal(expected, actual);
            }
            else
            {
                Assert.IsType(field.Values.GetType(), actual);
                Assert.Equal(field.Values.Cast<object>(), Assert.IsAssignableFrom<Array>(actual).Cast<object>());
            }
        }
        AssertSentinel(source);
    }

    private static Field Shorts(int tag, params ushort[] values) => new(tag, 3, values);
    private static Field Longs(int tag, params uint[] values) => new(tag, 4, values);
    private static Field Doubles(int tag, params double[] values) => new(tag, 12, values);
    private static Field Ascii(params byte[] values) => new(AsciiParams, 2, values);

    private sealed record Field(int Tag, int Format, Array Values);

    private sealed class ImageIfd(params Field[] fields)
    {
        public Field[] Fields { get; } = [Longs(Sentinel, 321), .. fields];
        public ImageIfd[] Children { get; set; } = [];
        public ImageIfd? Next { get; set; }
    }

    // Only the TIFF envelope is synthesized here. GeoTIFF decoding always goes through
    // TiffMetadataReader/ExifReader, including real TIFF scalar/array normalization.
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
            var count = image.Fields.Length + (image.Children.Length == 0 ? 0 : 1);
            position += countSize + count * entrySize + offsetSize;
        }

        using var stream = new MemoryStream();
        stream.SetLength(position);
        Write(bigEndian ? 0x4d4dUL : 0x4949UL, 2);
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
            var fields = image.Fields.AsEnumerable();
            if (image.Children.Length != 0)
                fields = fields.Append(Longs(ExifDirectoryBase.TagSubIfdOffset, image.Children.Select(child => (uint)offsets[child]).ToArray()));
            var sorted = fields.OrderBy(field => field.Tag).ToArray();
            Write((ulong)sorted.Length, countSize);
            foreach (var field in sorted)
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
                    // Align out-of-line values even for BigTIFF, making pointer and inline
                    // paths independently observable under both byte orders.
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
                case short v: WriteUnsigned(stream, unchecked((ushort)v), 2, bigEndian); break;
                case uint v: WriteUnsigned(stream, v, 4, bigEndian); break;
                case float v: WriteBits(BitConverter.GetBytes(v)); break;
                case double v: WriteBits(BitConverter.GetBytes(v)); break;
                default: throw new ArgumentException("Unsupported synthetic TIFF value type.");
            }
        }
        return stream.ToArray();

        void WriteBits(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian == bigEndian)
                Array.Reverse(bytes);
            stream.Write(bytes, 0, bytes.Length);
        }
    }

    private static void WriteUnsigned(Stream stream, ulong value, int size, bool bigEndian)
    {
        for (var i = 0; i < size; i++)
        {
            var shift = 8 * (bigEndian ? size - i - 1 : i);
            stream.WriteByte((byte)(value >> shift));
        }
    }
}

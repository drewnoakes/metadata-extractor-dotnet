// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.QuickTime;

namespace MetadataExtractor.Formats.Heif;

public sealed class HeifMetadataReaderTest
{
    private const int PrimaryItemLength = 14;
    private const int HiddenItemsLength = 40;

    [Theory]
    [InlineData("heic", false)]
    [InlineData("heic", true)]
    [InlineData("avif", false)]
    [InlineData("avif", true)]
    public void ReadMetadata_HeifAndAvif_ReadsImageProperties(string brand, bool extendedSize)
    {
        using var stream = CreateFile(writer =>
        {
            // Keep ftyp conventional for file-type detection; exercise both header sizes below it.
            WriteFileType(writer, brand);
            WriteBoxHeader(writer, "meta", 4 + HeaderLength(extendedSize) + 6 + ImagePropertiesLength(extendedSize), extendedSize);
            WriteUInt32(writer, 0);
            WritePrimaryItem(writer, 1, extendedSize);
            WriteImageProperties(writer, extendedSize);
        });

        var directories = ImageMetadataReader.ReadMetadata(stream);

        AssertFileType(directories, brand);
        AssertImageProperties(directories);
        AssertErrors(directories, expected: false);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(99)]
    [InlineData(100)]
    [InlineData(101)]
    public void ReadMetadata_ContainerNestingLimit_OnlyHidesLeavesBeyond100(int depth)
    {
        using var stream = CreateFile(writer =>
        {
            WriteFileType(writer, "heic");
            WriteNestedContainers(writer, "dinf", depth, PrimaryItemLength);
            WritePrimaryItem(writer, 1);
            WriteImageProperties(writer);
        }, depth * 8 + 256);

        var directories = HeifMetadataReader.ReadMetadata(stream);

        AssertFileType(directories, "heic");
        if (depth <= 100)
            AssertImageProperties(directories);
        else
            Assert.Empty(directories.OfType<HeicImagePropertiesDirectory>());
        AssertErrors(directories, expected: depth > 100);
    }

    [Theory]
    [InlineData("dinf", 100)]
    [InlineData("dinf", 101)]
    [InlineData("meta", 100)]
    [InlineData("meta", 101)]
    [InlineData("dref", 100)]
    [InlineData("dref", 101)]
    [InlineData("iinf", 100)]
    [InlineData("iinf", 101)]
    [InlineData("iprp", 100)]
    [InlineData("iprp", 101)]
    [InlineData("ipco", 100)]
    [InlineData("ipco", 101)]
    public void ReadMetadata_AllContainerTypes_EnforceNestingLimit(string containerType, int depth)
    {
        using var stream = CreateFile(writer =>
        {
            WriteNestedContainers(writer, containerType, depth, PrimaryItemLength);
            WritePrimaryItem(writer, 1);
            // A following root sibling must still be parsed after unwinding the entire chain.
            WriteFileType(writer, "heic");
        }, depth * 16 + 256);

        var directories = HeifMetadataReader.ReadMetadata(stream);

        AssertFileType(directories, "heic");
        AssertErrors(directories, expected: depth > 100);
    }

    [Theory]
    [InlineData("heic")]
    [InlineData("avif")]
    public void ReadMetadata_TenThousandContainers_SkipsHiddenItemsAndReadsFollowingSibling(string brand)
    {
        const int Depth = 10_000;
        using var stream = CreateFile(writer =>
        {
            WriteFileType(writer, brand);
            WriteNestedContainers(writer, "dinf", Depth, HiddenItemsLength);
            WriteHiddenItems(writer);
            WritePrimaryItem(writer, 1);
            WriteImageProperties(writer);
        }, Depth * 8 + 256);

        var directories = ImageMetadataReader.ReadMetadata(stream);

        AssertErrors(directories, expected: true);
        AssertFileType(directories, brand);
        // If the hidden thumbnail reference were parsed, a second image-properties directory would appear.
        AssertImageProperties(directories);
    }

    [Theory]
    [InlineData("dinf", false, false)]
    [InlineData("dinf", false, true)]
    [InlineData("dinf", true, false)]
    [InlineData("dinf", true, true)]
    [InlineData("meta", true, false)]
    [InlineData("dref", true, false)]
    [InlineData("iinf", true, false)]
    [InlineData("iprp", true, false)]
    [InlineData("ipco", true, false)]
    public void ReadMetadata_RejectedContainer_SkipsExactlyToFollowingSibling(string containerType, bool extendedSize, bool hasPayload)
    {
        int rejectedPayloadLength = hasPayload ? HiddenItemsLength : 0;
        using var stream = CreateFile(writer =>
        {
            WriteFileType(writer, "heic");
            WriteNestedContainers(writer, "dinf", 100, HeaderLength(extendedSize) + rejectedPayloadLength + PrimaryItemLength);
            WriteBoxHeader(writer, containerType, rejectedPayloadLength, extendedSize);
            if (hasPayload)
                WriteHiddenItems(writer);
            // Empty full boxes deliberately have no version/count prefix: the guard must run before reading it.
            // This pitm is a sibling inside the 100th dinf, not a root-level recovery shortcut.
            WritePrimaryItem(writer, 1);
            WriteImageProperties(writer);
        }, 1_024);

        var directories = HeifMetadataReader.ReadMetadata(stream);

        AssertErrors(directories, expected: true);
        AssertFileType(directories, "heic");
        AssertImageProperties(directories);
    }

    [Fact]
    public void ReadMetadata_ManySiblingContainers_CountsDepthRatherThanTotalContainers()
    {
        using var stream = CreateFile(writer =>
        {
            WriteFileType(writer, "heic");
            for (int i = 0; i < 128; i++)
                WriteBoxHeader(writer, "dinf", 0);
            WriteNestedContainers(writer, "dinf", 100, PrimaryItemLength);
            WritePrimaryItem(writer, 1);
            WriteImageProperties(writer);
        }, 2_048);

        var directories = HeifMetadataReader.ReadMetadata(stream);

        AssertErrors(directories, expected: false);
        AssertFileType(directories, "heic");
        AssertImageProperties(directories);
    }

    private static void AssertImageProperties(IReadOnlyList<Directory> directories)
    {
        var image = Assert.Single(directories.OfType<HeicImagePropertiesDirectory>());
        Assert.Equal("HEIC Primary Item Properties", image.Name);
        Assert.Equal(640, image.GetInt32(HeicImagePropertiesDirectory.TagImageWidth));
        Assert.Equal(480, image.GetInt32(HeicImagePropertiesDirectory.TagImageHeight));
    }

    private static void AssertFileType(IReadOnlyList<Directory> directories, string brand)
    {
        var fileType = Assert.Single(directories.OfType<QuickTimeFileTypeDirectory>());
        Assert.Equal(brand, fileType.GetString(QuickTimeFileTypeDirectory.TagMajorBrand));
        Assert.Equal(brand, fileType.GetString(QuickTimeFileTypeDirectory.TagCompatibleBrands));
    }

    private static void AssertErrors(IReadOnlyList<Directory> directories, bool expected)
    {
        var errors = directories.SelectMany(directory => directory.Errors).ToArray();
        if (expected)
        {
            var errorDirectory = Assert.Single(directories.OfType<ErrorDirectory>());
            Assert.Equal("Box nesting too deep.", Assert.Single(errorDirectory.Errors));
            Assert.Single(errors);
        }
        else
        {
            Assert.Empty(errors);
            Assert.Empty(directories.OfType<ErrorDirectory>());
        }
    }

    private static MemoryStream CreateFile(Action<BinaryWriter> writeBoxes, int capacity = 256)
    {
        var stream = new MemoryStream(capacity);
        using (var writer = new BinaryWriter(stream, Encoding.ASCII, leaveOpen: true))
        {
            writeBoxes(writer);
            // The existing reader ignores a box ending exactly at EOF. Keep meaningful boxes before this padding.
            WriteBoxHeader(writer, "free", 0);
        }
        stream.Position = 0;
        return stream;
    }

    private static void WriteNestedContainers(BinaryWriter writer, string containerType, int depth, int leafLength)
    {
        int prefixLength = containerType switch
        {
            "meta" => 4,
            "dref" => 8,
            "iinf" => 6,
            _ => 0
        };

        // Emit outer-to-inner headers into one buffer: no recursion or repeated payload concatenation.
        for (int remaining = depth; remaining > 0; remaining--)
        {
            WriteBoxHeader(writer, containerType, prefixLength + (remaining - 1) * (8 + prefixLength) + leafLength);
            if (prefixLength > 0)
                WriteUInt32(writer, 0); // Full-box version and flags.
            if (containerType == "dref")
                WriteUInt32(writer, 1); // Entry count.
            if (containerType == "iinf")
                WriteUInt16(writer, 1); // Version-zero entry count.
        }
    }

    private static void WriteFileType(BinaryWriter writer, string brand)
    {
        WriteBoxHeader(writer, "ftyp", 12);
        WriteFourCc(writer, brand);
        WriteUInt32(writer, 0);
        WriteFourCc(writer, brand);
    }

    private static void WritePrimaryItem(BinaryWriter writer, ushort itemId, bool extendedSize = false)
    {
        WriteBoxHeader(writer, "pitm", 6, extendedSize);
        WriteUInt32(writer, 0);
        WriteUInt16(writer, itemId);
    }

    private static void WriteHiddenItems(BinaryWriter writer)
    {
        WritePrimaryItem(writer, 2);
        WriteBoxHeader(writer, "iref", 18);
        WriteUInt32(writer, 0);
        WriteBoxHeader(writer, "thmb", 6);
        WriteUInt16(writer, 2);
        WriteUInt16(writer, 1);
        WriteUInt16(writer, 1);
    }

    private static int ImagePropertiesLength(bool extendedSize) => 4 * HeaderLength(extendedSize) + 28;

    private static void WriteImageProperties(BinaryWriter writer, bool extendedSize = false)
    {
        int headerLength = HeaderLength(extendedSize);
        WriteBoxHeader(writer, "iprp", 3 * headerLength + 28, extendedSize);
        WriteBoxHeader(writer, "ipco", headerLength + 12, extendedSize);
        WriteBoxHeader(writer, "ispe", 12, extendedSize);
        WriteUInt32(writer, 0);
        WriteUInt32(writer, 640);
        WriteUInt32(writer, 480);

        WriteBoxHeader(writer, "ipma", 16, extendedSize);
        WriteUInt32(writer, 0);
        WriteUInt32(writer, 2);
        for (ushort itemId = 1; itemId <= 2; itemId++)
        {
            WriteUInt16(writer, itemId);
            writer.Write((byte)1); // One associated property.
            writer.Write((byte)1); // Non-essential property at index one.
        }
    }

    private static int HeaderLength(bool extendedSize) => extendedSize ? 16 : 8;

    private static void WriteBoxHeader(BinaryWriter writer, string type, int payloadLength, bool extendedSize = false)
    {
        WriteUInt32(writer, extendedSize ? 1u : (uint)(8 + payloadLength));
        WriteFourCc(writer, type);
        if (extendedSize)
        {
            WriteUInt32(writer, 0); // High half of the 64-bit size.
            WriteUInt32(writer, (uint)(16 + payloadLength));
        }
    }

    private static void WriteFourCc(BinaryWriter writer, string value)
    {
        for (int i = 0; i < 4; i++)
            writer.Write((byte)value[i]);
    }

    private static void WriteUInt16(BinaryWriter writer, ushort value)
    {
        writer.Write((byte)(value >> 8));
        writer.Write((byte)value);
    }

    private static void WriteUInt32(BinaryWriter writer, uint value)
    {
        writer.Write((byte)(value >> 24));
        writer.Write((byte)(value >> 16));
        writer.Write((byte)(value >> 8));
        writer.Write((byte)value);
    }
}

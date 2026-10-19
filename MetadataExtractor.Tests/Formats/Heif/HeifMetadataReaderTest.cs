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
            // Exercise parsing through a trailing padding box as well.
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

    // ISO box types are internal, so these fixtures exercise BoxLocation and BoxReader
    // through the public reader instead of reflection or a new friend assembly.
    [Theory]
    [CombinatorialData]
    public void ReadsLastFileTypeBoxEndingExactlyAtEof(bool extended)
    {
        byte[] bytes = [
            .. Box("free", [], extended),
            .. Box("ftyp", [.. Encoding.ASCII.GetBytes("heic"), 0, 0, 0, 0, .. Encoding.ASCII.GetBytes("mif1")], extended)];
        using var stream = new MemoryStream(bytes);

        var directory = Assert.Single(HeifMetadataReader.ReadMetadata(stream).OfType<QuickTimeFileTypeDirectory>());

        Assert.Equal("heic", directory.GetString(QuickTimeFileTypeDirectory.TagMajorBrand));
        Assert.Equal("mif1", directory.GetString(QuickTimeFileTypeDirectory.TagCompatibleBrands));
        Assert.False(directory.HasError);
    }

    [Theory]
    [CombinatorialData]
    public void ReadsMetaAndLastNestedImagePropertyEndingExactlyAtEof(bool extendedMeta, bool extendedProperty)
    {
        // meta > iprp > ipco > ispe all end exactly at EOF. Put ipma first so
        // the final leaf is a metadata-bearing box, not merely an unknown box.
        var ispe = Box("ispe", [0, 0, 0, 0, .. UInt32(640), .. UInt32(480)], extendedProperty);
        var ipma = Box("ipma", [0, 0, 0, 0, .. UInt32(1), 0, 1, 1, 0x81]);
        var iprp = Box("iprp", [.. ipma, .. Box("ipco", ispe, extendedProperty)], extendedProperty);
        var pitm = Box("pitm", [0, 0, 0, 0, 0, 1]);
        var bytes = Box("meta", [0, 0, 0, 0, .. pitm, .. iprp], extendedMeta);
        using var stream = new MemoryStream(bytes);

        var directory = Assert.Single(HeifMetadataReader.ReadMetadata(stream).OfType<HeicImagePropertiesDirectory>());

        Assert.Equal(640, directory.GetInt32(HeicImagePropertiesDirectory.TagImageWidth));
        Assert.Equal(480, directory.GetInt32(HeicImagePropertiesDirectory.TagImageHeight));
        Assert.False(directory.HasError);
    }

    [Theory]
    [CombinatorialData]
    public void SizeZeroIncludesHeaderAtNonZeroOrigin(bool metadataContainer)
    {
        // ftyp's last compatible brand would be lost if the eight-byte header were
        // omitted from BoxLocation.Length. meta also checks the container boundary.
        var ftyp = Box("ftyp", [.. Encoding.ASCII.GetBytes("heic"), 0, 0, 0, 0, .. Encoding.ASCII.GetBytes("mif1")]);
        var last = metadataContainer ? Box("meta", [0, 0, 0, 0, .. ftyp]) : ftyp;
        Array.Clear(last, 0, 4);
        byte[] bytes = [.. Box("free", [4, 5, 6, 7]), .. last];
        using var stream = new MemoryStream(bytes);

        var directory = Assert.Single(HeifMetadataReader.ReadMetadata(stream).OfType<QuickTimeFileTypeDirectory>());

        Assert.Equal("heic", directory.GetString(QuickTimeFileTypeDirectory.TagMajorBrand));
        Assert.Equal("mif1", directory.GetString(QuickTimeFileTypeDirectory.TagCompatibleBrands));
        Assert.False(directory.HasError);
    }

    [Theory]
    [CombinatorialData]
    public void RejectsInvalidExtendedSizes(
        [CombinatorialValues(0UL, 1UL, 7UL, 8UL, 15UL, 0x8000000000000000UL, ulong.MaxValue)] ulong size)
    {
        byte[] bytes = [.. UInt32(1), .. Encoding.ASCII.GetBytes("free"), .. UInt64(size)];
        using var stream = new MemoryStream(bytes);

        Assert.Throws<ImageProcessingException>(() => HeifMetadataReader.ReadMetadata(stream));
        Assert.Equal(16, stream.Position);
    }

    [Theory]
    [CombinatorialData]
    public void RejectsOrdinarySizeSmallerThanHeader([CombinatorialValues(2U, 7U)] uint size)
    {
        using var stream = new MemoryStream([.. UInt32(size), .. Encoding.ASCII.GetBytes("free")]);

        Assert.Throws<ImageProcessingException>(() => HeifMetadataReader.ReadMetadata(stream));
        Assert.Equal(8, stream.Position);
    }

    [Theory]
    [CombinatorialData]
    public void RejectsChildExtendingBeyondParentEvenWhenPhysicalBytesExist(bool extended)
    {
        byte[] childHeader = [
            .. UInt32(extended ? 1U : 16U), .. Encoding.ASCII.GetBytes("free"),
            .. (extended ? UInt64(24) : [])];
        var parent = Box("meta", [0, 0, 0, 0, .. childHeader]);
        byte[] bytes = [.. parent, .. Box("free", new byte[16])];
        using var stream = new ProgressGuardStream(bytes);

        Assert.Throws<ImageProcessingException>(() => HeifMetadataReader.ReadMetadata(stream));
        Assert.Equal(parent.Length, stream.Position);
    }

    [Theory]
    [CombinatorialData]
    public void RejectsBoxExtendingBeyondPhysicalEof(bool extended)
    {
        var bytes = Box("free", [1, 2, 3, 4], extended);
        using var stream = new MemoryStream(bytes.Take(bytes.Length - 1).ToArray());

        Assert.Throws<ImageProcessingException>(() => HeifMetadataReader.ReadMetadata(stream));
        Assert.Equal(extended ? 16 : 8, stream.Position);
    }

    [Theory]
    [CombinatorialData]
    public void RejectsTruncatedChildHeaderWithoutLooping(
        [CombinatorialValues("meta", "iprp", "ipco", "iref")] string container,
        [CombinatorialValues(1, 7)] int tailBytes)
    {
        byte[] prefix = container is "meta" or "iref" ? new byte[4] : [];
        var bytes = Box(container, [.. prefix, .. new byte[tailBytes]]);
        using var stream = new ProgressGuardStream(bytes);

        Assert.Throws<ImageProcessingException>(() => HeifMetadataReader.ReadMetadata(stream));
        Assert.Equal(8 + prefix.Length, stream.Position);
    }

    [Fact]
    public void SkipsExtendedBoxLargerThanIntMaxValueToReadFollowingMetadata()
    {
        const long largeSize = (long)int.MaxValue + 4096;
        byte[] header = [.. UInt32(1), .. Encoding.ASCII.GetBytes("free"), .. UInt64((ulong)largeSize)];
        var ftyp = Box("ftyp", [.. Encoding.ASCII.GetBytes("heic"), 0, 0, 0, 0, .. Encoding.ASCII.GetBytes("mif1")]);
        using var stream = new SparseStream(largeSize + ftyp.Length, (0, header), (largeSize, ftyp));

        var directory = Assert.Single(HeifMetadataReader.ReadMetadata(stream).OfType<QuickTimeFileTypeDirectory>());

        Assert.Equal("heic", directory.GetString(QuickTimeFileTypeDirectory.TagMajorBrand));
        Assert.Equal("mif1", directory.GetString(QuickTimeFileTypeDirectory.TagCompatibleBrands));
        Assert.Equal(new[] { largeSize, 0L }, stream.SeekTargets);
        Assert.Equal(header.Length + ftyp.Length, stream.BytesRead);
        Assert.False(directory.HasError);
    }

    [Fact]
    public void SizeZeroBoxLargerThanIntMaxValueSkipsToActualEof()
    {
        const long length = (long)int.MaxValue + 8192;
        byte[] headers = [.. Box("free", []), .. UInt32(0), .. Encoding.ASCII.GetBytes("free")];
        using var stream = new SparseStream(length, (0, headers));

        var directories = HeifMetadataReader.ReadMetadata(stream);

        Assert.Empty(directories);
        Assert.Equal(new[] { length, 0L }, stream.SeekTargets);
        Assert.Equal(headers.Length, stream.BytesRead);
    }

    private static byte[] Box(string type, byte[] payload, bool extended = false)
    {
        return [
            .. UInt32(extended ? 1U : (uint)(8 + payload.Length)),
            .. Encoding.ASCII.GetBytes(type),
            .. (extended ? UInt64((ulong)(16 + payload.Length)) : []),
            .. payload];
    }

    private static byte[] UInt32(uint value) =>
        [(byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value];

    private static byte[] UInt64(ulong value) =>
        [.. UInt32((uint)(value >> 32)), .. UInt32((uint)value)];

    /// <summary>Turns a non-advancing parser loop into a deterministic failure, without timing or threads.</summary>
    private sealed class ProgressGuardStream(byte[] bytes) : MemoryStream(bytes)
    {
        private int _positionReads;

        public override long Position
        {
            get
            {
                if (++_positionReads > 1024)
                    throw new InvalidOperationException("Box reader failed to make progress.");
                return base.Position;
            }
            set => base.Position = value;
        }
    }

    /// <summary>Models huge seekable boxes using only their small headers and metadata payloads.</summary>
    private sealed class SparseStream(long length, params (long Offset, byte[] Bytes)[] segments) : Stream
    {
        public int BytesRead { get; private set; }
        public List<long> SeekTargets { get; } = [];
        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => length;
        public override long Position { get; set; }

        public override int ReadByte()
        {
            if (Position >= Length)
                return -1;
            if (++BytesRead > 1024)
                throw new InvalidOperationException("Reader failed to seek past a sparse payload.");
            var position = Position++;
            foreach (var segment in segments)
            {
                var index = position - segment.Offset;
                if (index >= 0 && index < segment.Bytes.Length)
                    return segment.Bytes[(int)index];
            }
            return 0;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = 0;
            while (read < count && Position < Length)
                buffer[offset + read++] = (byte)ReadByte();
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            Position = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => Position + offset,
                SeekOrigin.End => Length + offset,
                _ => throw new ArgumentOutOfRangeException(nameof(origin))
            };
            SeekTargets.Add(Position);
            return Position;
        }

        public override void Flush() { }
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}

// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Avi;
using MetadataExtractor.Formats.Wav;
using MetadataExtractor.Formats.WebP;

namespace MetadataExtractor.Formats.Riff;

public sealed class RiffReaderTest
{
    private const string HiddenDate = "hidden leaf";
    private const string FollowingDate = "following sibling";
    private const int AviHeaderLength = 48;

    [Theory]
    [InlineData("LIST", "hdrl", 0)]
    [InlineData("LIST", "hdrl", 1)]
    [InlineData("LIST", "hdrl", 99)]
    [InlineData("LIST", "hdrl", 100)]
    [InlineData("LIST", "hdrl", 101)]
    [InlineData("RIFF", "AVI ", 1)]
    [InlineData("RIFF", "AVI ", 99)]
    [InlineData("RIFF", "AVI ", 100)]
    [InlineData("RIFF", "AVI ", 101)]
    [InlineData("LIST", "strl", 100)]
    [InlineData("LIST", "AVI ", 100)]
    public void ProcessRiff_ContainerNestingLimit_OnlyHidesLeavesBeyond100(string chunkType, string listName, int depth)
    {
        using var stream = CreateFile("AVI ", depth * 12 + AviHeaderLength + ChunkLength(FollowingDate.Length), writer =>
        {
            WriteNestedContainers(writer, chunkType, listName, depth, AviHeaderLength);
            WriteAviHeader(writer);
            WriteDate(writer, FollowingDate);
        });

        var directories = ImageMetadataReader.ReadMetadata(stream);
        var avi = Assert.Single(directories.OfType<AviDirectory>());

        Assert.Equal(FollowingDate, avi.GetString(AviDirectory.TagDateTimeOriginal));
        if (depth <= 100)
        {
            AssertAviProperties(avi);
        }
        else
        {
            Assert.False(avi.ContainsTag(AviDirectory.TagWidth));
            Assert.False(avi.ContainsTag(AviDirectory.TagHeight));
            Assert.False(avi.ContainsTag(AviDirectory.TagStreams));
        }
        AssertErrors(directories, expected: depth > 100);
        Assert.Equal(stream.Length, stream.Position);
    }

    [Theory]
    [InlineData("LIST", "hdrl")]
    [InlineData("RIFF", "AVI ")]
    public void ProcessRiff_TenThousandContainers_SkipsHiddenLeafAndReadsFollowingSibling(string chunkType, string listName)
    {
        const int Depth = 10_000;
        int hiddenLength = ChunkLength(HiddenDate.Length);
        using var stream = CreateFile("AVI ", Depth * 12 + hiddenLength + AviHeaderLength, writer =>
        {
            WriteNestedContainers(writer, chunkType, listName, Depth, hiddenLength);
            WriteDate(writer, HiddenDate);
            WriteAviHeader(writer);
        });

        var directories = ProcessAvi(stream);
        var avi = Assert.Single(directories.OfType<AviDirectory>());

        AssertErrors(directories, expected: true);
        AssertAviProperties(avi);
        Assert.False(avi.ContainsTag(AviDirectory.TagDateTimeOriginal));
        Assert.Equal(stream.Length, stream.Position);
    }

    [Theory]
    [InlineData("LIST", false)]
    [InlineData("LIST", true)]
    [InlineData("RIFF", false)]
    [InlineData("RIFF", true)]
    public void ProcessRiff_RejectedContainer_SkipsExactlyToFollowingSibling(string chunkType, bool hasPayload)
    {
        int hiddenLength = hasPayload ? ChunkLength(HiddenDate.Length) : 0;
        string listName = chunkType == "LIST" ? "hdrl" : "AVI ";
        using var stream = CreateFile("AVI ", 101 * 12 + hiddenLength + AviHeaderLength, writer =>
        {
            WriteNestedContainers(writer, chunkType, listName, 100, 12 + hiddenLength + AviHeaderLength);
            WriteNestedContainers(writer, chunkType, listName, 1, hiddenLength);
            if (hasPayload)
                WriteDate(writer, HiddenDate);
            // This is a sibling of the rejected 101st container, inside the 100th container.
            WriteAviHeader(writer);
        });

        var directories = ProcessAvi(stream);
        var avi = Assert.Single(directories.OfType<AviDirectory>());

        AssertErrors(directories, expected: true);
        AssertAviProperties(avi);
        Assert.False(avi.ContainsTag(AviDirectory.TagDateTimeOriginal));
        Assert.Equal(stream.Length, stream.Position);
    }

    [Theory]
    [InlineData("WAVE", "LIST")]
    [InlineData("WAVE", "RIFF")]
    [InlineData("WEBP", "LIST")]
    [InlineData("WEBP", "RIFF")]
    public void ProcessRiff_UnacceptedDeepLists_SkipsWithoutNestingError(string form, string chunkType)
    {
        const int Depth = 10_000;
        int metadataLength = form == "WAVE" ? 24 : 18;
        using var stream = CreateFile(form, Depth * 12 + 2 * metadataLength, writer =>
        {
            WriteNestedContainers(writer, chunkType, form, Depth, metadataLength);
            WriteMetadataChunk(writer, form, hidden: true);
            WriteMetadataChunk(writer, form, hidden: false);
        });
        var directories = new List<Directory>();
        IRiffHandler handler = form == "WAVE"
            ? new WavRiffHandler(directories)
            : new WebPRiffHandler(directories);

        new RiffReader().ProcessRiff(new SequentialStreamReader(stream), handler);

        AssertErrors(directories, expected: false);
        if (form == "WAVE")
        {
            var format = Assert.Single(directories.OfType<WavFormatDirectory>());
            Assert.Equal(1, format.GetInt32(WavFormatDirectory.TagFormat));
            Assert.Equal(2, format.GetInt32(WavFormatDirectory.TagChannels));
            Assert.Equal(48_000, format.GetInt32(WavFormatDirectory.TagSamplesPerSec));
            Assert.Equal(192_000, format.GetInt32(WavFormatDirectory.TagBytesPerSec));
            Assert.Equal(4, format.GetInt32(WavFormatDirectory.TagBlockAlign));
            Assert.Equal(16, format.GetInt32(WavFormatDirectory.TagBitsPerSample));
        }
        else
        {
            var image = Assert.Single(directories.OfType<WebPDirectory>());
            Assert.Equal(640, image.GetInt32(WebPDirectory.TagImageWidth));
            Assert.Equal(480, image.GetInt32(WebPDirectory.TagImageHeight));
            Assert.True(image.GetBoolean(WebPDirectory.TagHasAlpha));
            Assert.True(image.GetBoolean(WebPDirectory.TagIsAnimation));
        }
        Assert.Equal(stream.Length, stream.Position);
    }

    [Fact]
    public void ProcessRiff_ManySiblingContainers_CountsDepthRatherThanTotalContainers()
    {
        using var stream = CreateFile("AVI ", (128 + 100) * 12 + AviHeaderLength, writer =>
        {
            for (int i = 0; i < 128; i++)
                WriteNestedContainers(writer, "LIST", "hdrl", 1, 0);
            WriteNestedContainers(writer, "LIST", "strl", 100, AviHeaderLength);
            WriteAviHeader(writer);
        });

        var directories = ProcessAvi(stream);
        var avi = Assert.Single(directories.OfType<AviDirectory>());

        AssertErrors(directories, expected: false);
        AssertAviProperties(avi);
        Assert.Equal(stream.Length, stream.Position);
    }

    private static IReadOnlyList<Directory> ProcessAvi(Stream stream)
    {
        var directories = new List<Directory>();
        new RiffReader().ProcessRiff(new SequentialStreamReader(stream), new AviRiffHandler(directories));
        return directories;
    }

    private static void AssertAviProperties(AviDirectory avi)
    {
        Assert.Equal(640, avi.GetInt32(AviDirectory.TagWidth));
        Assert.Equal(480, avi.GetInt32(AviDirectory.TagHeight));
        Assert.Equal(2, avi.GetInt32(AviDirectory.TagStreams));
    }

    private static void AssertErrors(IReadOnlyList<Directory> directories, bool expected)
    {
        var errors = directories.SelectMany(directory => directory.Errors);
        if (expected)
            Assert.Equal("RIFF nesting too deep.", Assert.Single(errors));
        else
            Assert.Empty(errors);
    }

    private static MemoryStream CreateFile(string form, int chunksLength, Action<BinaryWriter> writeChunks)
    {
        var stream = new MemoryStream(12 + chunksLength);
        using (var writer = new BinaryWriter(stream, Encoding.ASCII, leaveOpen: true))
        {
            WriteFourCc(writer, "RIFF");
            writer.Write(4 + chunksLength);
            WriteFourCc(writer, form);
            writeChunks(writer);
        }
        stream.Position = 0;
        return stream;
    }

    private static void WriteNestedContainers(BinaryWriter writer, string chunkType, string listName, int depth, int leafLength)
    {
        // Sizes are known up front, so even 10,000 levels need only one forward pass over one buffer.
        for (int remaining = depth; remaining > 0; remaining--)
        {
            WriteChunkHeader(writer, chunkType, 4 + (remaining - 1) * 12 + leafLength);
            WriteFourCc(writer, listName);
        }
    }

    private static void WriteAviHeader(BinaryWriter writer)
    {
        WriteChunkHeader(writer, "avih", 40);
        for (int i = 0; i < 6; i++)
            writer.Write(0);
        writer.Write(2); // Streams, at offset 24.
        writer.Write(0);
        writer.Write(640); // Width, at offset 32.
        writer.Write(480);
    }

    private static int ChunkLength(int payloadLength) => 8 + payloadLength + payloadLength % 2;

    private static void WriteDate(BinaryWriter writer, string value)
    {
        var payload = Encoding.ASCII.GetBytes(value);
        WriteChunkHeader(writer, "IDIT", payload.Length);
        writer.Write(payload);
        if (payload.Length % 2 != 0)
            writer.Write((byte)0);
    }

    private static void WriteMetadataChunk(BinaryWriter writer, string form, bool hidden)
    {
        if (form == "WAVE")
        {
            WriteChunkHeader(writer, "fmt ", 16);
            writer.Write((ushort)1); // PCM.
            writer.Write((ushort)(hidden ? 1 : 2));
            writer.Write(hidden ? 22_050 : 48_000);
            writer.Write(hidden ? 22_050 : 192_000);
            writer.Write((ushort)(hidden ? 1 : 4));
            writer.Write((ushort)(hidden ? 8 : 16));
        }
        else
        {
            WriteChunkHeader(writer, "VP8X", 10);
            writer.Write((byte)0x12); // Alpha and animation flags.
            writer.Write(new byte[3]);
            WriteUInt24(writer, hidden ? 31 : 639); // Width minus one.
            WriteUInt24(writer, hidden ? 23 : 479); // Height minus one.
        }
    }

    private static void WriteChunkHeader(BinaryWriter writer, string fourCc, int payloadLength)
    {
        WriteFourCc(writer, fourCc);
        writer.Write(payloadLength);
    }

    private static void WriteFourCc(BinaryWriter writer, string value)
    {
        for (int i = 0; i < 4; i++)
            writer.Write((byte)value[i]);
    }

    private static void WriteUInt24(BinaryWriter writer, int value)
    {
        writer.Write((byte)value);
        writer.Write((byte)(value >> 8));
        writer.Write((byte)(value >> 16));
    }
}

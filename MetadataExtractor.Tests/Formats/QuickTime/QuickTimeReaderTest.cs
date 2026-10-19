// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace MetadataExtractor.Formats.QuickTime;

public sealed class QuickTimeReaderTest
{
    [Fact]
    public void ProcessChildren_DeepNestingSkipsPayloadAndResumesAtSibling()
    {
        var bytes = Concat(
            NestedContainers(10_000, MovieHeader(30, 90)),
            MovieHeader());
        using var stream = new MemoryStream(bytes);

        var result = ReadMovieHeaders(stream);

        Assert.Equal(101, result.ContainerCount);
        Assert.Equal("Atom nesting too deep.", Assert.Single(result.Errors));
        Assert.Equal((600U, 1200U), Assert.Single(result.MovieHeaders));
        Assert.Equal(stream.Length, stream.Position);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(99, true)]
    [InlineData(100, true)]
    [InlineData(101, false)]
    public void ProcessChildren_EnforcesExactDepthBoundary(int depth, bool readsMovieHeader)
    {
        using var stream = new MemoryStream(NestedContainers(depth, MovieHeader()));

        var result = ReadMovieHeaders(stream);

        Assert.Equal(depth, result.ContainerCount);
        if (readsMovieHeader)
        {
            Assert.Equal((600U, 1200U), Assert.Single(result.MovieHeaders));
            Assert.Empty(result.Errors);
        }
        else
        {
            Assert.Empty(result.MovieHeaders);
            Assert.Equal("Atom nesting too deep.", Assert.Single(result.Errors));
        }
        Assert.Equal(stream.Length, stream.Position);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(true, true)]
    [InlineData(false, true)]
    public void ProcessChildren_RejectedContainerDoesNotConsumeSiblingHeader(bool extendedSize, bool hasPayload)
    {
        var rejectedPayload = hasPayload ? MovieHeader(30, 90) : Array.Empty<byte>();
        var rejectedContainer = Box("moov", rejectedPayload, extendedSize);

        // At depth 100, the depth-101 moov and mvhd are siblings. In particular, an
        // empty extended-size moov is exactly 16 bytes: skipping eight more loses mvhd.
        var bytes = NestedContainers(100, Concat(rejectedContainer, MovieHeader()));
        using var stream = new MemoryStream(bytes);

        var result = ReadMovieHeaders(stream);

        Assert.Equal(101, result.ContainerCount);
        Assert.Equal("Atom nesting too deep.", Assert.Single(result.Errors));
        Assert.Equal((600U, 1200U), Assert.Single(result.MovieHeaders));
        Assert.Equal(stream.Length, stream.Position);
    }

    [Theory]
    [InlineData("qt  ", false)]
    [InlineData("qt  ", true)]
    [InlineData("isom", false)]
    [InlineData("isom", true)]
    public void ReadMetadata_RecognisesMovieAndMp4WithEitherContainerHeader(string majorBrand, bool extendedSize)
    {
        var brand = Encoding.ASCII.GetBytes(majorBrand);
        var fileType = Box("ftyp", Concat(brand, new byte[4], brand));
        var bytes = Concat(fileType, Box("moov", MovieHeader(), extendedSize));
        using var stream = new MemoryStream(bytes);

        var directories = ImageMetadataReader.ReadMetadata(stream);

        var fileTypeDirectory = Assert.Single(directories.OfType<QuickTimeFileTypeDirectory>());
        Assert.Equal(majorBrand, fileTypeDirectory.GetString(QuickTimeFileTypeDirectory.TagMajorBrand));

        var movieHeader = Assert.Single(directories.OfType<QuickTimeMovieHeaderDirectory>());
        Assert.Equal(600U, movieHeader.GetUInt32(QuickTimeMovieHeaderDirectory.TagTimeScale));
        Assert.Equal(TimeSpan.FromSeconds(2), movieHeader.GetObject(QuickTimeMovieHeaderDirectory.TagDuration));
        Assert.All(directories, directory => Assert.Empty(directory.Errors));
        Assert.Equal(stream.Length, stream.Position);
    }

    private static (int ContainerCount, List<(uint TimeScale, uint Duration)> MovieHeaders, List<string> Errors)
        ReadMovieHeaders(Stream stream)
    {
        var containerCount = 0;
        var movieHeaders = new List<(uint TimeScale, uint Duration)>();
        var errors = new List<string>();

        void Handler(AtomCallbackArgs a)
        {
            switch (a.TypeString)
            {
                case "moov":
                    containerCount++;
                    a.ProcessChildren(Handler, errors.Add);
                    // Verify consumption here, before ProcessAtoms can skip unread bytes.
                    Assert.Equal(a.StartPosition + a.Size, a.Stream.Position);
                    break;
                case "mvhd":
                    a.Reader.Skip(12);
                    movieHeaders.Add((a.Reader.GetUInt32(), a.Reader.GetUInt32()));
                    break;
            }
        }

        QuickTimeReader.ProcessAtoms(stream, Handler);

        return (containerCount, movieHeaders, errors);
    }

    private static byte[] NestedContainers(int depth, byte[] payload)
    {
        // Write each header once into a single buffer, even for the 10,000-level fixture.
        var buffer = new byte[depth * 8 + payload.Length];
        for (var i = 0; i < depth; i++)
        {
            var offset = i * 8;
            WriteUInt32(buffer, offset, (uint)(buffer.Length - offset));
            buffer[offset + 4] = (byte)'m';
            buffer[offset + 5] = (byte)'o';
            buffer[offset + 6] = (byte)'o';
            buffer[offset + 7] = (byte)'v';
        }
        Array.Copy(payload, 0, buffer, depth * 8, payload.Length);
        return buffer;
    }

    private static byte[] MovieHeader(uint timeScale = 600, uint duration = 1200)
    {
        var payload = new byte[100];
        WriteUInt32(payload, 12, timeScale);
        WriteUInt32(payload, 16, duration);
        return Box("mvhd", payload);
    }

    private static byte[] Box(string type, byte[] payload, bool extendedSize = false)
    {
        var headerSize = extendedSize ? 16 : 8;
        var buffer = new byte[headerSize + payload.Length];
        WriteUInt32(buffer, 0, extendedSize ? 1U : (uint)buffer.Length);
        Encoding.ASCII.GetBytes(type).CopyTo(buffer, 4);
        if (extendedSize)
        {
            WriteUInt32(buffer, 8, 0);
            WriteUInt32(buffer, 12, (uint)buffer.Length);
        }
        Array.Copy(payload, 0, buffer, headerSize, payload.Length);
        return buffer;
    }

    private static byte[] Concat(params byte[][] arrays)
    {
        var buffer = new byte[arrays.Sum(array => array.Length)];
        var offset = 0;
        foreach (var array in arrays)
        {
            Array.Copy(array, 0, buffer, offset, array.Length);
            offset += array.Length;
        }
        return buffer;
    }

    private static void WriteUInt32(byte[] buffer, int offset, uint value)
    {
        buffer[offset] = (byte)(value >> 24);
        buffer[offset + 1] = (byte)(value >> 16);
        buffer[offset + 2] = (byte)(value >> 8);
        buffer[offset + 3] = (byte)value;
    }
}

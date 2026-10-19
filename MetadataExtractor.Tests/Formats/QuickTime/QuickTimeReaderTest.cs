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

    [Theory]
    [CombinatorialData]
    public void AcceptsEmptyAtomAtMinimumHeaderSize(bool extended)
    {
        var bytes = Header("free", extended ? 16U : 8U, extended);
        using var stream = new MemoryStream(bytes);
        var calls = 0;

        QuickTimeReader.ProcessAtoms(stream, atom =>
        {
            calls++;
            Assert.Equal("free", atom.TypeString);
            Assert.Equal(bytes.Length, atom.Size);
            Assert.Equal(0, atom.BytesLeft);
        });

        Assert.Equal(1, calls);
        Assert.Equal(bytes.Length, stream.Position);
    }

    [Theory]
    [CombinatorialData]
    public void SkipsUnreadPayloadAndContinuesAtFollowingSibling(bool extended)
    {
        var header = Header("free", extended ? 19U : 11U, extended);
        byte[] bytes = [.. header, 11, 22, 33, .. Header("next", 10), 44, 55];
        using var stream = new MemoryStream(bytes);
        var types = new List<string>();

        QuickTimeReader.ProcessAtoms(stream, atom =>
        {
            types.Add(atom.TypeString);
            Assert.Same(stream, atom.Stream);
            if (atom.TypeString == "free")
            {
                Assert.Equal(0, atom.StartPosition);
                Assert.Equal(extended ? 19 : 11, atom.Size);
                Assert.Equal(3, atom.BytesLeft);
                Assert.Equal(11, atom.Reader.GetByte());
                Assert.Equal(2, atom.BytesLeft);
                // Leave two bytes unread, as an unknown-box handler would.
            }
            else
            {
                Assert.Equal(header.Length + 3, atom.StartPosition);
                Assert.Equal(10, atom.Size);
                Assert.Equal(2, atom.BytesLeft);
                Assert.Equal(new byte[] { 44, 55 }, atom.Reader.GetBytes(2));
                Assert.Equal(0, atom.BytesLeft);
            }
        });

        Assert.Equal(new[] { "free", "next" }, types);
        Assert.Equal(bytes.Length, stream.Position);
    }

    [Fact]
    public void SizeZeroPreservesPublicSizeButCountsPayloadToEof()
    {
        // Start at a non-zero offset and include apparent box bytes in the payload:
        // size zero consumes them, rather than interpreting a following sibling.
        byte[] payload = [12, 34, .. Header("fake", 8)];
        byte[] bytes = [99, 88, .. Header("mdat", 0), .. payload];
        using var stream = new MemoryStream(bytes);
        stream.Position = 2;
        var calls = 0;

        QuickTimeReader.ProcessAtoms(stream, atom =>
        {
            calls++;
            Assert.Equal("mdat", atom.TypeString);
            Assert.Equal(0, atom.Size); // Documented public API, not the effective size.
            Assert.Equal(2, atom.StartPosition);
            Assert.Equal(payload.Length, atom.BytesLeft);
            Assert.Equal(12, atom.Reader.GetByte());
            Assert.Equal(payload.Length - 1, atom.BytesLeft);
            Assert.Equal(payload.Skip(1).ToArray(), atom.Reader.GetBytes(payload.Length - 1));
            Assert.Equal(0, atom.BytesLeft);
        });

        Assert.Equal(1, calls);
        Assert.Equal(bytes.Length, stream.Position);
    }

    [Theory]
    [InlineData(2, false)]
    [InlineData(3, false)]
    [InlineData(4, false)]
    [InlineData(5, false)]
    [InlineData(6, false)]
    [InlineData(7, false)]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(7, true)]
    [InlineData(8, true)]
    [InlineData(15, true)]
    public void RejectsSizesSmallerThanHeaderBeforeCallback(uint declaredSize, bool extended)
    {
        using var stream = new MemoryStream(Header("bad!", declaredSize, extended));
        var calls = 0;

        QuickTimeReader.ProcessAtoms(stream, _ => calls++);

        Assert.Equal(0, calls);
        Assert.Equal(extended ? 16 : 8, stream.Position);
    }

    [Fact]
    public void RejectsExtendedSizeAboveLongMaxValueBeforeCallback()
    {
        var bytes = Header("bad!", 16, extended: true);
        bytes[8] = 0x80; // 2^63 + 16
        using var stream = new MemoryStream(bytes);
        var calls = 0;

        QuickTimeReader.ProcessAtoms(stream, _ => calls++);

        Assert.Equal(0, calls);
        Assert.Equal(16, stream.Position);
    }

    [Theory]
    [CombinatorialData]
    public void TruncatedExtendedHeaderDoesNotInvokeCallback([CombinatorialValues(0, 4, 7, 8, 12, 15)] int available)
    {
        using var stream = new MemoryStream(Header("bad!", 16, extended: true).Take(available).ToArray());
        var calls = 0;

        QuickTimeReader.ProcessAtoms(stream, _ => calls++);

        Assert.Equal(0, calls);
        Assert.InRange(stream.Position, 0, available);
    }

    [Theory]
    [CombinatorialData]
    public void ChildExtendingBeyondParentDoesNotInvokeChildHandler(bool extended)
    {
        var child = Header("bad!", extended ? 24U : 16U, extended);
        // The child declares eight more bytes than its parent contains. A following
        // sibling supplies physical bytes, but must not be swallowed by that child.
        byte[] bytes = [.. Header("moov", (uint)(8 + child.Length)), .. child, .. Header("next", 8)];
        using var stream = new MemoryStream(bytes);
        var types = new List<string>();

        QuickTimeReader.ProcessAtoms(stream, atom =>
        {
            types.Add(atom.TypeString);
            if (atom.TypeString == "moov")
                QuickTimeReader.ProcessAtoms(stream, childAtom => types.Add(childAtom.TypeString), atom.BytesLeft);
        });

        Assert.Equal(new[] { "moov", "next" }, types);
        Assert.Equal(bytes.Length, stream.Position);
    }

    [Theory]
    [CombinatorialData]
    public void ChildWithTruncatedPayloadDoesNotInvokeChildHandler(bool extended)
    {
        var child = Header("bad!", extended ? 24U : 16U, extended);
        byte[] bytes = [.. Header("moov", (uint)(8 + child.Length)), .. child];
        using var stream = new MemoryStream(bytes);
        var children = 0;
        var parents = 0;

        QuickTimeReader.ProcessAtoms(stream, atom =>
        {
            parents++;
            QuickTimeReader.ProcessAtoms(stream, _ => children++, atom.BytesLeft);
        });

        Assert.Equal(1, parents);
        Assert.Equal(0, children);
        Assert.Equal(bytes.Length, stream.Position);
    }

    [Theory]
    [InlineData(4, false)]
    [InlineData(7, false)]
    [InlineData(8, true)]
    [InlineData(15, true)]
    public void HeaderCrossingParentBoundaryDoesNotInvokeCallback(int parentBytes, bool extended)
    {
        // Enough physical bytes exist; only the declared parent boundary is short.
        using var stream = new MemoryStream(Header("free", extended ? 16U : 8U, extended));
        var calls = 0;

        QuickTimeReader.ProcessAtoms(stream, _ => calls++, parentBytes);

        Assert.Equal(0, calls);
        Assert.InRange(stream.Position, 0, parentBytes);
    }

    private static byte[] Header(string type, uint size, bool extended = false)
    {
        var bytes = new byte[extended ? 16 : 8];
        WriteUInt32(0, extended ? 1U : size);
        for (var i = 0; i < 4; i++)
            bytes[4 + i] = (byte)type[i];
        if (extended)
            WriteUInt32(12, size);
        return bytes;

        void WriteUInt32(int offset, uint value)
        {
            for (var i = 0; i < 4; i++)
                bytes[offset + i] = (byte)(value >> (24 - 8 * i));
        }
    }
}

// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Iso14496;

namespace MetadataExtractor.Formats.QuickTime;

/// <summary>
/// Models data provided to callbacks invoked when reading QuickTime atoms via <see cref="QuickTimeReader.ProcessAtoms(Stream, Action{AtomCallbackArgs}, long)"/>.
/// </summary>
public sealed class AtomCallbackArgs(uint type, long size, Stream stream, long startPosition, SequentialStreamReader reader)
{
    private const int MaxAtomDepth = 100;

    internal int Depth { get; init; }

    /// <summary>
    /// Gets the 32-bit unsigned integer that identifies the atom's type.
    /// </summary>
    public uint Type { get; } = type;

    /// <summary>
    /// Gets the length of the atom, including its header, in bytes.
    /// If the atom extends to the end of the file, this value is zero.
    /// </summary>
    public long Size { get; } = size;

    /// <summary>
    /// Gets the stream from which atoms are being read.
    /// </summary>
    public Stream Stream { get; } = stream;

    /// <summary>
    /// Gets the position within <see cref="Stream"/> at which this atom's header started.
    /// </summary>
    public long StartPosition { get; } = startPosition;

    /// <summary>
    /// Gets a sequential reader from which this atom's contents may be read.
    /// </summary>
    /// <remarks>
    /// It is backed by <see cref="Stream"/>, so manipulating the stream's position will influence this reader.
    /// </remarks>
    public SequentialStreamReader Reader { get; } = reader;

    /// <summary>
    /// Gets and sets whether the callback wishes processing to terminate.
    /// </summary>
    public bool Cancel { get; set; }

    /// <summary>
    /// Gets the string representation of this atom's type.
    /// </summary>
    public string TypeString => TypeStringConverter.ToTypeString(Type);

    /// <summary>
    /// Computes the number of bytes remaining in the atom, given the <see cref="Stream"/> position.
    /// </summary>
    public long BytesLeft => Size == 0 ? Stream.Length - Stream.Position : Size - (Stream.Position - StartPosition);

    /// <summary>
    /// Reads this atom's remaining contents as child atoms, allowing at most 100 levels of nested containers.
    /// </summary>
    /// <param name="handler">A callback function to handle each child atom.</param>
    /// <param name="errorHandler">Receives an error if the nesting limit is exceeded. The remaining contents are then skipped.</param>
    public void ProcessChildren(Action<AtomCallbackArgs> handler, Action<string> errorHandler)
    {
        if (Depth >= MaxAtomDepth)
        {
            errorHandler("Atom nesting too deep.");
            // BytesLeft accounts for both 8-byte and 16-byte atom headers.
            Reader.Skip(BytesLeft);
            return;
        }

        QuickTimeReader.ProcessAtoms(Stream, handler, BytesLeft, Depth + 1);
    }
}

/// <summary>
/// Static class for processing atoms the QuickTime container format.
/// </summary>
/// <remarks>
/// QuickTime file format specification: https://developer.apple.com/library/mac/documentation/QuickTime/QTFF/qtff.pdf
/// </remarks>
public static class QuickTimeReader
{
    /// <summary>
    /// Reads atom data from <paramref name="stream"/>, invoking <paramref name="handler"/> for each atom encountered.
    /// </summary>
    /// <remarks>
    /// Use <see cref="AtomCallbackArgs.ProcessChildren"/> within callbacks to process nested containers with bounded depth.
    /// </remarks>
    /// <param name="stream">The stream to read atoms from.</param>
    /// <param name="handler">A callback function to handle each atom.</param>
    /// <param name="stopByBytes">The maximum number of bytes to process before discontinuing.</param>
    public static void ProcessAtoms(Stream stream, Action<AtomCallbackArgs> handler, long stopByBytes = -1)
    {
        ProcessAtoms(stream, handler, stopByBytes, depth: 0);
    }

    internal static void ProcessAtoms(Stream stream, Action<AtomCallbackArgs> handler, long stopByBytes, int depth)
    {
        var reader = new SequentialStreamReader(stream);

        var seriesStartPos = stream.Position;

        while (stopByBytes == -1 || stream.Position - seriesStartPos < stopByBytes)
        {
            var atomStartPos = stream.Position;

            try
            {
                var remaining = stopByBytes == -1
                    ? stream.Length - atomStartPos
                    : Math.Min(stream.Length - atomStartPos, stopByBytes - (atomStartPos - seriesStartPos));

                if (remaining < 8)
                    return;

                // Length of the atom's data, in bytes, including size bytes
                long atomSize = reader.GetUInt32();

                // Typically four ASCII characters, but may be non-printable.
                // By convention, lowercase 4CCs are reserved by Apple.
                var atomType = reader.GetUInt32();

                var headerSize = 8;
                if (atomSize == 1)
                {
                    headerSize = 16;
                    if (remaining < headerSize)
                        return;

                    var extendedSize = reader.GetUInt64();
                    if (extendedSize > long.MaxValue)
                        return;
                    atomSize = (long)extendedSize;
                }

                var effectiveSize = atomSize == 0 && headerSize == 8 ? stream.Length - atomStartPos : atomSize;
                if (effectiveSize < headerSize || effectiveSize > remaining)
                    return;

                var args = new AtomCallbackArgs(atomType, atomSize, stream, atomStartPos, reader) { Depth = depth };

                handler(args);

                if (args.Cancel)
                    return;

                if (atomSize == 0)
                    return;

                var toSkip = effectiveSize - (stream.Position - atomStartPos);

                if (toSkip < 0)
                {
                    // Atoms are nested within each other. We have delegated to a sub-atom handler to
                    // process this atom's data, but it read more than it should have.
                    // TODO log this error somewhere (we don't have a directory available here)
                    return;
                }

                if (!reader.TrySkip(toSkip))
                    return;
            }
            catch (IOException)
            {
                // Exception trapping is used when stream doesn't support stream length method only
                return;
            }
        }
    }
}

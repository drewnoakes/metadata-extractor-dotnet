// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using static MetadataExtractor.Formats.Mkv.ElementIds;

namespace MetadataExtractor.Formats.Mkv;

/// <summary>Reads the supported metadata subset of Matroska and WebM without buffering media payloads.</summary>
/// <remarks>
/// Traversal is forward-only, uses 64-bit element sizes, and supports unknown-sized Segments and
/// Clusters, including concatenated documents. Truncation and unsupported EBML schemas are reported
/// as directory errors, retaining metadata already extracted.
/// Individual text and binary metadata values are limited to 16 MiB.
/// See RFC 8794 and https://www.matroska.org/technical/elements.html.
/// </remarks>
public sealed class MkvReader
{
    private enum ValueType { Integer, Float, String, Utf8, Binary }

    private static readonly Dictionary<int, ValueType> _elements = CreateElements();

    private static Dictionary<int, ValueType> CreateElements()
    {
        var elements = new Dictionary<int, ValueType>();
        Add(ValueType.Integer, EbmlVersion, EbmlReadVersion, EbmlMaxIdLength, EbmlMaxSizeLength,
            DocTypeVersion, DocTypeReadVersion, TimestampScale, TrackNumber, TrackUid, TrackType,
            FlagLacing, Channels, BitDepth, DefaultDuration, DisplayWidth, DisplayHeight, DisplayUnit,
            PixelWidth, PixelHeight, FlagInterlaced, TransferCharacteristics, MatrixCoefficients,
            Primaries, ColourRange, ChromaSitingHorz, ChromaSitingVert, CodecDelay, SeekPreRoll,
            TagTrackUid, TargetTypeValue, TagEditionUid, TagChapterUid, TagAttachmentUid, TagDefault);
        Add(ValueType.String, DocType, CodecId, Language, LanguageBcp47, TagLanguage, TagLanguageBcp47, TargetType);
        Add(ValueType.Utf8, MuxingApp, WritingApp, TagName, TagString);
        Add(ValueType.Float, Duration, SamplingFrequency);
        Add(ValueType.Binary, SegmentUuid, TagBinary);
        return elements;

        void Add(ValueType type, params int[] ids)
        {
            foreach (var id in ids)
                elements.Add(id, type);
        }
    }

    public IReadOnlyList<Directory> Extract(SequentialReader reader) => new Parser(reader, headerOnly: false).Extract();

    internal IReadOnlyList<Directory> ExtractHeader(SequentialReader reader) => new Parser(reader, headerOnly: true).Extract();

    private class Context(int id, long end, bool unknownSize, Directory? directory)
    {
        public int Id { get; } = id;
        public long End { get; } = end;
        public bool UnknownSize { get; } = unknownSize;
        public Directory? Directory { get; } = directory;
        public int TagUpdateCount { get; set; }
    }

    private sealed class TargetsContext(long end, Directory? directory) : Context(Targets, end, false, directory)
    {
        public Dictionary<int, List<ulong>> Uids { get; } = [];
    }

    private sealed class Parser
    {
        private const int MaximumScalarBytes = 16 * 1024 * 1024;

        private readonly SequentialReader _reader;
        private readonly bool _headerOnly;
        private readonly List<Directory> _directories = [];
        private readonly Stack<Context> _contexts = new();
        private EbmlDirectory? _header;
        private bool _headerComplete;
        private bool _validHeader;
        private ulong _maxSizeLength = 8;

        public Parser(SequentialReader reader, bool headerOnly)
        {
            _reader = reader.WithByteOrder(isMotorolaByteOrder: true);
            _headerOnly = headerOnly;
            _contexts.Push(new Context(0, -1, true, null));
        }

        public IReadOnlyList<Directory> Extract()
        {
            try
            {
                Process();
            }
            catch (IOException e)
            {
                var directory = _contexts.Select(context => context.Directory).FirstOrDefault(d => d is not null);
                if (directory is null)
                {
                    if (_header is null)
                    {
                        _header = new EbmlDirectory();
                        _directories.Add(_header);
                    }
                    directory = _header;
                }
                directory.AddError($"Error reading Matroska data: {e.Message}");
            }
            finally
            {
                while (_contexts.Count > 1)
                    CloseContext();
            }
            return _directories;
        }

        private void Process()
        {
            while (true)
            {
                while (_contexts.Count > 1 && _contexts.Peek().End == _reader.Position)
                    CloseContext();
                if (_headerOnly && _headerComplete)
                    return;

                long start = _reader.Position;
                int id;
                try
                {
                    id = DataParser.GetElementId(_reader);
                }
                catch (EndOfStreamException)
                {
                    if (_reader.Position != start)
                        throw;
                    if (_contexts.Any(context => context.End >= 0 && start < context.End))
                        throw new EndOfStreamException("Unexpected end of an EBML master element.");
                    return;
                }

                EndUnknownContexts(id);
                var parent = _contexts.Peek();
                if (parent.End >= 0 && _reader.Position >= parent.End)
                    throw new IOException("EBML element header exceeds its parent boundary.");
                long sizeStart = _reader.Position;
                long size = DataParser.GetElementSize(_reader);
                long position = _reader.Position;
                if (_headerComplete && id != EbmlHeader && (ulong)(position - sizeStart) > _maxSizeLength)
                    throw new IOException("EBML element size exceeds EBMLMaxSizeLength.");
                if (parent.End >= 0 && (position > parent.End || size > parent.End - position))
                    throw new IOException("EBML element exceeds its parent boundary.");
                if (size > long.MaxValue - position)
                    throw new IOException("EBML element size overflows its position.");
                if (size < 0 && id is not (Segment or Cluster))
                    throw new IOException("Unknown size is only supported for Segment and Cluster elements.");

                long end = size < 0 ? parent.End : position + size;
                var child = CreateContext(id, end, size < 0, parent);
                if (child is not null)
                {
                    _contexts.Push(child);
                    continue;
                }
                if (size < 0)
                    throw new IOException("Unexpected unknown-sized EBML element.");

                if (parent.Directory is { } directory && Accepts(parent, id) && _elements.TryGetValue(id, out var type))
                {
                    if (parent is TargetsContext targets && IsTargetUid(id))
                    {
                        if (!targets.Uids.TryGetValue(id, out var uids))
                            targets.Uids.Add(id, uids = []);
                        uids.Add(DataParser.GetUnsignedInteger(_reader, size));
                        continue;
                    }
                    object value = ReadValue(id, size, type);
                    if (directory.ContainsTag(id))
                        directory.AddError($"Duplicate EBML element 0x{id:X}.");
                    directory.Set(id, value);
                }
                else
                {
                    Skip(size);
                }
            }
        }

        private void EndUnknownContexts(int id)
        {
            var context = _contexts.Peek();
            if (context.Id == Cluster && context.UnknownSize && (IsSegmentChild(id) || id is EbmlHeader or Segment))
            {
                CloseContext();
                context = _contexts.Peek();
            }
            if (context.Id == Segment && context.UnknownSize && id is EbmlHeader or Segment)
                CloseContext();
        }

        private static bool IsSegmentChild(int id)
            => id is SeekHead or SegmentInfo or Tracks or Cluster or Cues or Tags or 0x1941A469 or 0x1043A770;

        private Context? CreateContext(int id, long end, bool unknownSize, Context parent)
        {
            Directory directory;
            if (parent.Id == 0 && id == EbmlHeader)
            {
                _header = new EbmlDirectory();
                _headerComplete = false;
                _validHeader = false;
                _directories.Add(_header);
                return new Context(id, end, unknownSize, _header);
            }
            if (parent.Id == 0 && id == Segment)
            {
                if (!_validHeader)
                    throw new IOException("A valid Matroska or WebM EBML header is required before Segment.");
                directory = new MkvSegmentDirectory { Parent = _header };
                _directories.Add(directory);
                return new Context(id, end, unknownSize, directory);
            }
            if ((parent.Id == Segment && id is SegmentInfo or Tracks)
                || (parent.Id == Segment && id == Cluster && unknownSize)
                || (parent.Id == Video && id == Colour))
                return new Context(id, end, unknownSize, parent.Directory);

            if (parent.Id == Segment && id == Tags)
            {
                directory = new MkvTagsDirectory();
                directory.Set(MkvTagsDirectory.TagUpdateNumber, ++parent.TagUpdateCount);
                _directories.Add(directory);
            }
            else if (parent.Id == Tags && id == TagElement)
            {
                directory = new MkvTagDirectory();
                _directories.Add(directory);
            }
            else if (parent.Id == TagElement && id == Targets)
            {
                return new TargetsContext(end, parent.Directory);
            }
            else if (parent.Id is TagElement or SimpleTag && id == SimpleTag)
            {
                directory = new MkvSimpleTagDirectory();
                _directories.Add(directory);
            }
            else if (parent.Id == Tracks && id == TrackEntry)
                directory = new MkvTrackDirectory();
            else if (parent.Id == TrackEntry && id == Video)
                directory = new MkvVideoDirectory();
            else if (parent.Id == TrackEntry && id == Audio)
                directory = new MkvAudioDirectory();
            else
                return null;

            directory.Parent = parent.Directory;
            return new Context(id, end, unknownSize, directory);
        }

        private static bool Accepts(Context parent, int id)
            => parent.Id is not (Segment or Tracks or Cluster or Tags or TagElement) && parent.Directory!.HasTagName(id);

        private static bool IsTargetUid(int id) => id is TagTrackUid or TagEditionUid or TagChapterUid or TagAttachmentUid;

        private void CloseContext()
        {
            var context = _contexts.Pop();
            var parent = _contexts.Peek();
            if (context.Directory is not { } directory)
                return;

            switch (context.Id)
            {
                case Targets:
                    foreach (var entry in ((TargetsContext)context).Uids)
                        directory.Set(entry.Key, entry.Value.ToArray());
                    break;
                case TagElement:
                    ApplyDefaults(directory, TargetTypeValue);
                    break;
                case SimpleTag:
                    ApplyDefaults(directory, TagLanguage, TagDefault);
                    if (!directory.ContainsTag(TagName))
                        directory.AddError("SimpleTag is missing its TagName.");
                    if (directory.ContainsTag(TagString) && directory.ContainsTag(TagBinary))
                        directory.AddError("SimpleTag must not contain both TagString and TagBinary.");
                    break;
                case EbmlHeader:
                    ApplyDefaults(directory, EbmlVersion, EbmlReadVersion, EbmlMaxIdLength,
                        EbmlMaxSizeLength, DocTypeVersion, DocTypeReadVersion);
                    ValidateHeader();
                    break;
                case SegmentInfo:
                case Segment:
                    ApplyDefaults(directory, TimestampScale);
                    if (context.Id == Segment && directory.TryGetDouble(Duration, out var duration)
                        && directory.TryGetDouble(TimestampScale, out var scale))
                        directory.Set(MkvSegmentDirectory.TagDurationSeconds, duration * scale / 1_000_000_000.0);
                    break;
                case Audio:
                    ApplyDefaults(directory, SamplingFrequency, Channels);
                    break;
                case Video:
                    ApplyDefaults(directory, FlagInterlaced, DisplayUnit);
                    break;
                case Colour:
                    ApplyDefaults(directory, MatrixCoefficients, TransferCharacteristics, Primaries,
                        ColourRange, ChromaSitingHorz, ChromaSitingVert);
                    break;
            }
            if (context.Id is Video or Audio && parent.Directory is { } trackDirectory)
                CopyTags(directory, trackDirectory);
            else if (context.Id == TrackEntry)
            {
                ApplyDefaults(directory, FlagLacing, Language, CodecDelay, SeekPreRoll);
                Directory track = directory.GetObject(TrackType) switch
                {
                    1UL => new MkvVideoDirectory(),
                    2UL => new MkvAudioDirectory(),
                    _ => directory
                };
                track.Parent = parent.Directory;
                CopyTags(directory, track);
                if (track is MkvVideoDirectory && track.TryGetDouble(DefaultDuration, out var duration) && duration > 0)
                    track.Set(MkvVideoDirectory.TagFrameRate, 1_000_000_000.0 / duration);
                _directories.Add(track);
            }
        }

        private static void CopyTags(Directory source, Directory destination)
        {
            if (ReferenceEquals(source, destination))
                return;
            foreach (var tag in source.Tags)
                destination.Set(tag.Type, source.GetObject(tag.Type)!);
            foreach (var error in source.Errors)
                destination.AddError(error);
        }

        private void ValidateHeader()
        {
            var header = _header!;
            _headerComplete = true;
            var docType = header.GetString(DocType);
            if (docType is not ("matroska" or "webm"))
                header.AddError($"Unsupported EBML document type: {docType ?? "null"}");
            bool hasEbmlRead = header.TryGetUInt64(EbmlReadVersion, out var ebmlRead);
            bool hasDocRead = header.TryGetUInt64(DocTypeReadVersion, out var docRead);
            if (!hasEbmlRead || ebmlRead != 1)
                header.AddError("Unsupported EBML read version.");
            if (!hasDocRead || docRead is < 1 or > 4)
                header.AddError("Unsupported Matroska document read version.");
            if (!header.TryGetUInt64(EbmlMaxIdLength, out var maxId) || maxId != 4)
                header.AddError("Matroska requires EBMLMaxIDLength to be four.");
            if (!header.TryGetUInt64(EbmlMaxSizeLength, out var maxSize) || maxSize is < 1 or > 8)
                header.AddError("Unsupported EBMLMaxSizeLength.");
            else
                _maxSizeLength = maxSize;
            if (!hasEbmlRead || !hasDocRead
                || !header.TryGetUInt64(EbmlVersion, out var ebmlVersion) || ebmlVersion < ebmlRead
                || !header.TryGetUInt64(DocTypeVersion, out var docVersion) || docVersion < docRead)
                header.AddError("EBML version must not be lower than its read version.");
            _validHeader = !header.HasError;
        }

        private object ReadValue(int id, long size, ValueType type)
        {
            if (size == 0 && GetDefaultValue(id) is { } value)
                return value;
            if (type is ValueType.String or ValueType.Utf8 or ValueType.Binary)
            {
                if (size > MaximumScalarBytes)
                    throw new IOException("EBML value is too large to store in memory: the limit is 16 MiB.");
                if (id == SegmentUuid && size != 16)
                    throw new IOException("SegmentUUID must contain exactly 16 bytes.");
                if (_reader.IsCloserToEnd(size))
                    throw new EndOfStreamException("EBML value exceeds the remaining input.");
            }
            return type switch
            {
                ValueType.Integer => DataParser.GetUnsignedInteger(_reader, size),
                ValueType.Float => DataParser.GetDouble(_reader, size),
                ValueType.String => _reader.GetNullTerminatedString(DataParser.GetByteCount(size), Encoding.ASCII, moveToMaxLength: true),
                ValueType.Utf8 => _reader.GetNullTerminatedString(DataParser.GetByteCount(size), Encoding.UTF8, moveToMaxLength: true),
                ValueType.Binary => _reader.GetBytes(DataParser.GetByteCount(size)),
                _ => throw new InvalidOperationException("Unexpected EBML value type.")
            };
        }

        private static void ApplyDefaults(Directory directory, params int[] ids)
        {
            foreach (var id in ids)
            {
                if (!directory.ContainsTag(id))
                    directory.Set(id, GetDefaultValue(id) ?? throw new InvalidOperationException($"No default is defined for EBML element {id}."));
            }
        }

        private static object? GetDefaultValue(int id) => id switch
        {
            EbmlVersion or EbmlReadVersion or DocTypeVersion or DocTypeReadVersion or FlagLacing or Channels or TagDefault => 1UL,
            TargetTypeValue => 50UL,
            EbmlMaxIdLength => 4UL,
            EbmlMaxSizeLength => 8UL,
            TimestampScale => 1_000_000UL,
            SamplingFrequency => 8000.0,
            Language => "eng",
            TagLanguage => "und",
            MatrixCoefficients or TransferCharacteristics or Primaries => 2UL,
            FlagInterlaced or DisplayUnit or CodecDelay or SeekPreRoll or ColourRange or ChromaSitingHorz or ChromaSitingVert => 0UL,
            _ => null
        };

        private void Skip(long size)
        {
            if (size == 0)
                return;
            // Seeking can succeed past EOF. Reserve the final byte for an actual read.
            size--;
            if (size > 0)
            {
                long start = _reader.Position;
                if (!_reader.TrySkip(size))
                {
                    size -= _reader.Position - start;
                    Span<byte> buffer = stackalloc byte[4096];
                    while (size > 0)
                    {
                        int count = (int)Math.Min(size, buffer.Length);
                        _reader.GetBytes(buffer.Slice(0, count));
                        size -= count;
                    }
                }
            }
            _reader.GetByte();
        }
    }
}

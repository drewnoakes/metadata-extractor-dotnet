// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MetadataExtractor.Formats.Tiff;
using MetadataExtractor.IO;
using XmpCore;
using XmpCore.Impl;
using XmpCore.Options;

namespace MetadataExtractor.NewApi;

public interface IDirectory : IReadOnlyCollection<IEntry>
{
    string Name { get; }

    IEnumerable<IDirectory> SubDirectories { get; }
}

/// <summary>
/// Base class for directories whose contents are stored by key.
/// </summary>
/// <typeparam name="TKey">The type of identifier used for each property.</typeparam>
/// <typeparam name="TEntry"></typeparam>
public abstract class KeyedDirectory<TKey, TEntry>
    : IDirectory,
      IEnumerable<TEntry>
      where TEntry : notnull, IEntry
      where TKey : notnull
{
    // TODO need to maintain order of values if we are to write data again

    private readonly Dictionary<TKey, TEntry> _entryByKey; 

    protected KeyedDirectory(IEqualityComparer<TKey>? comparator = null)
    {
        _entryByKey = new Dictionary<TKey, TEntry>(comparator);
    }

    public abstract string Name { get; }

    public int Count => _entryByKey.Count;

    // TODO can we store IDirectory as IEntry too? A directory may have different entry metadata based upon how it's embedded in outer data. something like ILinkedDirectoryEntry?
    public IEnumerable<IDirectory> SubDirectories => _entryByKey.Select(entry => entry.Value.Value).OfType<IDirectory>();

    public bool TryGet(TKey key, [NotNullWhen(returnValue: true)] out TEntry? entry)
    {
        return _entryByKey.TryGetValue(key, out entry);
    }

    public virtual void Add(TKey key, TEntry entry)
    {
        _entryByKey.Add(key, entry);
    }

    public virtual TEntry this[TKey key]
    {
        get => _entryByKey[key];
        set => _entryByKey[key] = value;
    }

    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<IEntry> IEnumerable<IEntry>.GetEnumerator() => _entryByKey.Values.Cast<IEntry>().GetEnumerator();
    
    public IEnumerator<TEntry> GetEnumerator() => ((IEnumerable<TEntry>)_entryByKey.Values).GetEnumerator();

    #endregion
}

/// <summary>
/// Models those properties common to all kinds of  <see cref="Directory"/> entry.
/// </summary>
public interface IEntry
{
    /// <summary>
    /// Gets the entry's value.
    /// </summary>
    object Value { get; }
    
    /// <summary>
    /// Gets the entry's name.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets a description of the entry's value.
    /// </summary>
    string? Description { get; }
}

public readonly struct TiffValue : IEntry
{
    public TiffDataFormat Format { get; }
    public int ComponentCount { get; }
    public object Value { get; }
    public TiffTag Tag { get; }

    private TiffValue(TiffDataFormat format, int componentCount, object value, TiffTag tag)
    {
        Format = format;
        ComponentCount = componentCount;
        Value = value;
        Tag = tag;
    }

    string IEntry.Name => Tag is KnownTiffTag knownTag ? knownTag.Name : $"Unknown ({Tag.Id})";

    string? IEntry.Description => Tag.Describe(this);

    public static TiffValue CreateInt8U         (byte        value, TiffTag tag) => new(TiffDataFormat.Int8U,     1, value, tag);
    public static TiffValue CreateInt16U        (ushort      value, TiffTag tag) => new(TiffDataFormat.Int16U,    1, value, tag);
    public static TiffValue CreateInt32U        (uint        value, TiffTag tag) => new(TiffDataFormat.Int32U,    1, value, tag);
    public static TiffValue CreateRationalU     (URational   value, TiffTag tag) => new(TiffDataFormat.RationalU, 1, value, tag);
    public static TiffValue CreateInt8S         (sbyte       value, TiffTag tag) => new(TiffDataFormat.Int8S,     1, value, tag);
    public static TiffValue CreateInt16S        (short       value, TiffTag tag) => new(TiffDataFormat.Int16S,    1, value, tag);
    public static TiffValue CreateInt32S        (int         value, TiffTag tag) => new(TiffDataFormat.Int32S,    1, value, tag);
    public static TiffValue CreateRationalS     (Rational    value, TiffTag tag) => new(TiffDataFormat.RationalS, 1, value, tag);
    public static TiffValue CreateSingle        (float       value, TiffTag tag) => new(TiffDataFormat.Single,    1, value, tag);
    public static TiffValue CreateDouble        (double      value, TiffTag tag) => new(TiffDataFormat.Double,    1, value, tag);

    public static TiffValue CreateString        (byte[]      value, TiffTag tag) => new(TiffDataFormat.String,    value.Length, value, tag);
    public static TiffValue CreateUndefined     (byte[]      value, TiffTag tag) => new(TiffDataFormat.Undefined, value.Length, value, tag);

    public static TiffValue CreateInt8UArray    (byte[]      value, TiffTag tag) => new(TiffDataFormat.Int8U,     value.Length, value, tag);
    public static TiffValue CreateInt16UArray   (ushort[]    value, TiffTag tag) => new(TiffDataFormat.Int16U,    value.Length, value, tag);
    public static TiffValue CreateInt32UArray   (uint[]      value, TiffTag tag) => new(TiffDataFormat.Int32U,    value.Length, value, tag);
    public static TiffValue CreateRationalUArray(URational[] value, TiffTag tag) => new(TiffDataFormat.RationalU, value.Length, value, tag);
    public static TiffValue CreateInt8SArray    (sbyte[]     value, TiffTag tag) => new(TiffDataFormat.Int8S,     value.Length, value, tag);
    public static TiffValue CreateInt16SArray   (short[]     value, TiffTag tag) => new(TiffDataFormat.Int16S,    value.Length, value, tag);
    public static TiffValue CreateInt32SArray   (int[]       value, TiffTag tag) => new(TiffDataFormat.Int32S,    value.Length, value, tag);
    public static TiffValue CreateRationalSArray(Rational[]  value, TiffTag tag) => new(TiffDataFormat.RationalS, value.Length, value, tag);
    public static TiffValue CreateSingleArray   (float[]     value, TiffTag tag) => new(TiffDataFormat.Single,    value.Length, value, tag);
    public static TiffValue CreateDoubleArray   (double[]    value, TiffTag tag) => new(TiffDataFormat.Double,    value.Length, value, tag);

    public static TiffValue CreateUndefinedArray(byte[][]    value, TiffTag tag) => new(TiffDataFormat.Undefined, value.Length, value, tag);

    private bool TryGetInt32(int max, out int number)
    {
        int? nu = Value switch
        {
            int n => n,
            short n => n,
            ushort n => n,
            uint n and <= int.MaxValue => (int)n,
            byte n => n,
            sbyte n => n,
            long n and <= int.MaxValue and >= int.MinValue => (int)n,
            ulong n and <= int.MaxValue => (int)n,
            _ => null
        };

        if (nu <= max)
        {
            number = nu.Value;
            return true;
        }

        number = default;
        return false;
    }

    public bool TryGetByte(out byte b)
    {
        if (TryGetInt32(max: byte.MaxValue, out int n))
        {
            b = (byte)n;
            return true;
        }
        
        b = default;
        return false;
    }

    // TODO implement all these
    
    public bool TryGetUInt16(out ushort value) => false;
    public bool TryGetUInt32(out uint value) => false;
    public bool TryGetURational(out URational value) => false;
    public bool TryGetSByte(out byte value) => false;
    public bool TryGetInt16(out short value) => false;
    public bool TryGetInt32(out int value) => false;
    public bool TryGetRational(out Rational value) => false;
    public bool TryGetSingle(out float value) => false;
    public bool TryGetDouble(out double value) => false;
    public bool TryGetString(out string value) => false;
    public bool TryGetUndefined(out byte[] value) => false;

    public bool TryGetByteArray(out byte[] b) => false;
    public bool TryGetUInt16Array(out ushort[] value) => false;
    public bool TryGetUInt32Array(out uint[] value) => false;
    public bool TryGetURationalArray(out URational[] value) => false;
    public bool TryGetSByteArray(out byte[] value) => false;
    public bool TryGetInt16Array(out short[] value) => false;
    public bool TryGetInt32Array(out int[] value) => false;
    public bool TryGetRationalArray(out Rational[] value) => false;
    public bool TryGetSingleArray(out float[] value) => false;
    public bool TryGetDoubleArray(out double[] value) => false;
    public bool TryGetStringArray(out string[] value) => false;
    public bool TryGetUndefinedArray(out byte[] value) => false;

    public override string? ToString() => ToString(provider: null);
    
    public string? ToString(IFormatProvider? provider) => Tag.Describe(this, provider);
}

/// <summary>
/// Base class for metadata about TIFF tags, whether known or unknown.
/// </summary>
public abstract class TiffTag
{
    public abstract bool IsKnown { get; }

    public abstract string Name { get; }

    public int Id { get; } // TODO should this be int?
    
    protected TiffTag(int id) => Id = id;

    public abstract string? Describe(TiffValue value, IFormatProvider? format = null);
}

public sealed class TiffTagIdComparator : IEqualityComparer<TiffTag>
{
    public static TiffTagIdComparator Instance { get; } = new();

    public bool Equals(TiffTag? x, TiffTag? y) => x?.Id == y?.Id;

    public int GetHashCode(TiffTag obj) => obj.Id;
}

/// <summary>
/// Base class for TIFF tags known to exist within some TIFF-compliant format.
/// </summary>
public abstract class KnownTiffTag : TiffTag
{
    public override bool IsKnown => true;

    // TODO why model expected format/count? if it's for validation, consider a more general Validate method

    /// <summary>
    /// Gets the data format specified for this tag by its standard.
    /// </summary>
    public abstract TiffDataFormat ExpectedFormat { get; }

    /// <summary>
    /// Gets the number of items expected for this tag by its standard.
    /// </summary>
    public int ExpectedCount { get; } // TODO do all tags have a single value? should this be a range? nullable?

    /// <summary>
    /// Gets the display name for this tag.
    /// </summary>
    public override string Name { get; }

    protected KnownTiffTag(int id, string name, int expectedCount = 1)
        : base(id)
    {
        Name = name;
        ExpectedCount = expectedCount;
    }
}

public class TiffUInt8Tag : KnownTiffTag
{
    private readonly Func<byte, IFormatProvider?, string?>? _describer;

    public TiffUInt8Tag(int id, string name, Func<byte, IFormatProvider?, string?>? describer = null)
        : base(id, name)
    {
        _describer = describer;
    }

    public override string? Describe(TiffValue value, IFormatProvider? format = null)
    {
        if (value.TryGetByte(out byte b))
        {
            return _describer?.Invoke(b, format) ?? b.ToString(format);
        }
        
        return null;
    }

    public override TiffDataFormat ExpectedFormat => TiffDataFormat.Int8U;
}

public class TiffUInt16Tag : KnownTiffTag
{
    private readonly Func<ushort, IFormatProvider?, string?>? _describer;

    public TiffUInt16Tag(int id, string name, Func<ushort, IFormatProvider?, string?>? describer = null)
        : base(id, name)
    {
        _describer = describer;
    }

    public override string? Describe(TiffValue value, IFormatProvider? format = null)
    {
        if (value.TryGetUInt16(out ushort i))
        {
            return _describer?.Invoke(i, format) ?? i.ToString(format);
        }
        
        return null;
    }

    public override TiffDataFormat ExpectedFormat => TiffDataFormat.Int16U;
}

public class TiffUInt32Tag : KnownTiffTag
{
    private readonly Func<int, IFormatProvider?, string?>? _describer;

    public TiffUInt32Tag(int id, string name, Func<int, IFormatProvider?, string?>? describer = null)
        : base(id, name)
    {
        _describer = describer;
    }

    public override string? Describe(TiffValue value, IFormatProvider? format = null)
    {
        if (value.TryGetInt32(out int i))
        {
            return _describer?.Invoke(i, format) ?? i.ToString(format);
        }
        
        return null;
    }

    public override TiffDataFormat ExpectedFormat => TiffDataFormat.Int32U;
}

public class TiffSingleTag : KnownTiffTag
{
    private readonly Func<float, IFormatProvider?, string?>? _describer;

    public TiffSingleTag(int id, string name, Func<float, IFormatProvider?, string?>? describer = null)
        : base(id, name)
    {
        _describer = describer;
    }

    public override string? Describe(TiffValue value, IFormatProvider? format = null)
    {
        if (value.TryGetInt32(out int i))
        {
            return _describer?.Invoke(i, format) ?? i.ToString(format);
        }
        
        return null;
    }

    public override TiffDataFormat ExpectedFormat => TiffDataFormat.Single;
}

public class TiffRationalTag : KnownTiffTag
{
    private readonly Func<Rational, IFormatProvider?, string?>? _describer;

    public TiffRationalTag(int id, string name, Func<Rational, IFormatProvider?, string?>? describer = null)
        : base(id, name)
    {
        _describer = describer;
    }

    public override string? Describe(TiffValue value, IFormatProvider? format = null)
    {
        if (value.TryGetRational(out Rational i))
        {
            return _describer?.Invoke(i, format) ?? i.ToString(format);
        }
        
        return null;
    }

    public override TiffDataFormat ExpectedFormat => TiffDataFormat.RationalS;
}

public class TiffURationalTag : KnownTiffTag
{
    private readonly Func<URational, IFormatProvider?, string?>? _describer;

    public TiffURationalTag(int id, string name, Func<URational, IFormatProvider?, string?>? describer = null)
        : base(id, name)
    {
        _describer = describer;
    }

    public override string? Describe(TiffValue value, IFormatProvider? format = null)
    {
        if (value.TryGetURational(out URational i))
        {
            return _describer?.Invoke(i, format) ?? i.ToString(format);
        }
        
        return null;
    }

    public override TiffDataFormat ExpectedFormat => TiffDataFormat.RationalU;
}

public class TiffStringTag : KnownTiffTag
{
    private readonly Func<byte[], IFormatProvider?, string?>? _describer;

    public Encoding ExpectedEncoding { get; }

    public TiffStringTag(int id, string name, Func<byte[], IFormatProvider?, string?>? describer = null, Encoding? expectedEncoding = null)
        : base(id, name)
    {
        _describer = describer;
        ExpectedEncoding = expectedEncoding ?? Encoding.UTF8;
    }

    public override string? Describe(TiffValue value, IFormatProvider? format = null)
    {
        if (!value.TryGetByteArray(out byte[] bytes))
            return null;

        try
        {
            if (_describer == null)
            {
                // Decode the Unicode string and trim the Unicode zero "\0" from the end.
                // TODO remove trailing zeroes before conversion to reduce allocations
                return ExpectedEncoding.GetString(bytes, 0, bytes.Length).TrimEnd('\0');
            }

            return _describer(bytes, format);
        }
        catch
        {
            return null;
        }
    }

    public override TiffDataFormat ExpectedFormat => TiffDataFormat.String;
}

public class TiffUInt8ArrayTag : KnownTiffTag
{
    private readonly Func<byte[], IFormatProvider?, string?>? _describer;

    public TiffUInt8ArrayTag(int id, string name, Func<byte[], IFormatProvider?, string?>? describer = null)
        : base(id, name)
    {
        _describer = describer;
    }

    public override string? Describe(TiffValue value, IFormatProvider? format = null)
    {
        if (value.TryGetByteArray(out byte[] b))
        {
            return _describer?.Invoke(b, format) ?? b.ToString(format);
        }
        
        return null;
    }

    public override TiffDataFormat ExpectedFormat => TiffDataFormat.Int8U;
}

public class TiffUInt16ArrayTag : KnownTiffTag
{
    private readonly Func<int[], IFormatProvider?, string?>? _describer;

    public TiffUInt16ArrayTag(int id, string name, int expectedCount, Func<int[], IFormatProvider?, string?>? describer = null)
        : base(id, name, expectedCount)
    {
        _describer = describer;
    }

    public override string? Describe(TiffValue value, IFormatProvider? format = null)
    {
        if (value.TryGetInt32Array(out int[] i))
        {
            return _describer?.Invoke(i, format) ?? i.ToString(format); // TODO write central array formatting code and reuse
        }
        
        return null;
    }

    public override TiffDataFormat ExpectedFormat => TiffDataFormat.Int16U;
}

public class TiffURationalArrayTag : KnownTiffTag
{
    private readonly Func<URational[], IFormatProvider?, string?>? _describer;

    public TiffURationalArrayTag(int id, string name, int expectedCount, Func<URational[], IFormatProvider?, string?>? describer = null)
        : base(id, name, expectedCount)
    {
        _describer = describer;
    }

    public override string? Describe(TiffValue value, IFormatProvider? format = null)
    {
        if (value.TryGetURationalArray(out URational[] i))
        {
            return _describer?.Invoke(i, format) ?? i.ToString(format); // TODO write central array formatting code and reuse 
        }
        
        return null;
    }

    public override TiffDataFormat ExpectedFormat => TiffDataFormat.RationalU;
}

public class TiffIndexedUInt16Tag : TiffUInt16Tag
{
    public TiffIndexedUInt16Tag(int id, string name, int baseIndex, string?[] descriptions)
        : base(id, name, (i, _) => DecodeIndex(baseIndex, descriptions, i))
    {}

    private static string? DecodeIndex(int baseIndex, string?[] descriptions, int index)
    {
        int arrayIndex = index - baseIndex;

        if (arrayIndex < 0 || arrayIndex >= descriptions.Length)
            return null;
        
        return descriptions[arrayIndex];
    }
}

public class TiffMappedUInt16Tag : TiffUInt16Tag
{
    private readonly IReadOnlyDictionary<int, string> _descriptions;

    public TiffMappedUInt16Tag(int id, string name, IReadOnlyDictionary<int, string> descriptions)
        : base(id, name)
    {
        _descriptions = descriptions;
    }

    public override string? Describe(TiffValue value, IFormatProvider? format = null)
    {
        if (value.TryGetInt32(out int i))
        {
            return _descriptions.TryGetValue(i, out string? description) ? description : i.ToString(format);
        }
        
        return null;
    }
}

public class UnknownTiffTag : TiffTag
{
    public override bool IsKnown => false;

    public override string Name => $"Unknown ({Id})"; // TODO Hex display?

    public UnknownTiffTag(int id) : base(id)
    {
    }

    public override string? Describe(TiffValue value, IFormatProvider? format = null)
    {
        return value.ToString(format);
    }
}

public abstract class TiffDirectory : KeyedDirectory<TiffTag, TiffValue> 
{
    protected TiffDirectory() : base(TiffTagIdComparator.Instance)
    {
    }

    public bool TryGetInt32(TiffTag tag, out int value) // TODO should this be an override of the base?
    {
        if (TryGet(tag, out TiffValue? tiffValue) &&
            tiffValue.TryGetInt32(out value))
        {
            return true;
        }

        value = default;
        return false;
    }
}

public sealed class ExifIfd0Directory : TiffDirectory
{
    public override string Name => "Exif IFD0";

    // TODO generate these properties with a source generator?
    public int? Width => TryGetInt32(ExifTags.ImageWidth, out int value) ? value : default;
    public int? Height => TryGetInt32(ExifTags.ImageHeight, out int value) ? value : default;
}

/////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////

public readonly struct XmpName : IEquatable<XmpName>
{
    public string Namespace { get; }
    public string Name { get; }

    public XmpName(string @namespace, string name)
    {
        Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public override string ToString() => $"{Namespace}.{Name}"; // TODO review this formatting

    public bool Equals(XmpName other) => Namespace == other.Namespace && Name == other.Name;

    public override bool Equals(object? obj) => obj is XmpName other && Equals(other);

    public override int GetHashCode() => Namespace == null ? 0 : unchecked((Namespace.GetHashCode() * 397) ^ Name.GetHashCode());
}

public sealed class XmpValue : IEntry
{
    public XmpName XmpName { get; }
    public IXmpProperty XmpProperty { get; }

    public XmpValue(XmpName xmpName, IXmpProperty xmpProperty)
    {
        XmpName = xmpName;
        XmpProperty = xmpProperty;
    }

    public object Value => XmpProperty.Value;

    public string Name => XmpName.ToString();
    
    public string Description => XmpProperty.Value;
}

public sealed class XmpDirectory : KeyedDirectory<XmpName, XmpValue>
{
    private static readonly IteratorOptions _iteratorOptions = new IteratorOptions { IsJustLeafNodes = true };

    public override string Name => "XMP";

    public XmpDirectory(XmpMeta xmpMeta)
    {
        try
        {
            var i = new XmpIterator(xmpMeta, null, null, _iteratorOptions);
            
            while (i.HasNext())
            {
                var prop = (IXmpPropertyInfo)i.Next();
                string @namespace = prop.Namespace;
                string path = prop.Path;
                string value = prop.Value;
                if (@namespace != null && path != null && value != null)
                {
                    var name = new XmpName(@namespace, path);
                    Add(name, new XmpValue(name, prop));
                }
            }
        }
        catch (XmpException) { } // ignored
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////

// An example of a directory whose data is a fixed sequence of consecutive values, representable as a byte array.
// Entry objects are constructed lazily when requested. Individual values may be read/written via properties
// that update backing memory directly.

public enum PcxProperty
{
    Version,
}

/// <summary>
/// Base class for directories whose contents are stored at fixed offsets in a byte buffer.
/// </summary>
public abstract class BufferBackedDirectory : IDirectory
{
    // TODO methods for reading/writing data at specific offsets
    // TODO ability to 'freeze' object
    // TODO ability to 'clone' object
    // TODO support both II/MM data somehow
    
    public abstract int Count { get; }
    public abstract string Name { get; }
    public abstract IEnumerable<IDirectory> SubDirectories { get; }
    public abstract IEnumerator<IEntry> GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public sealed class PcxDirectory : BufferBackedDirectory
{
    public const int HeaderSizeBytes = 74;

    // Little-endian backing data
    private readonly byte[] _bytes;

    // NOTE this directory must be strict about the values it can receive for properties.
    // Version, for example, must be a byte. Other types are invalid and cannot be written.
    // This is different to TIFF, for example, where a value has an expected type, but may
    // actually be stored and written as something else.
    //
    // Would be good to have validation for this at the entry level.
    // 1) IEntryKey has "ValidateValue" method, or
    // 2) IEntryValue validates its value at construction time (using info from assigned key)
    
    // Alternative design: This directory keeps a fixed-length byte array in memory, and get/set
    // operations work directly on that byte array. Maybe all read/write operations provide the
    // directory instance. This would also help with Exif tags that need to read multiple values
    // as part of their description, but will make the API uglier I think.
    
    private static readonly IndexedTable _pcxVersionTable = new(new[]
    {
        "2.5 with fixed EGA palette information",
        null,
        "2.8 with modifiable EGA palette information",
        "2.8 without palette information (default palette)",
        "PC Paintbrush for Windows",
        "3.0 or better"
    });
    
    private static readonly IndexedTable _pcxColorPlanesTable = new(new[] { "24-bit color", "16 colors" }, baseIndex: 3);
    private static readonly IndexedTable _pcxPaletteTypeTable = new(new[] { "Color or B&W", "Grayscale" }, baseIndex: 1);

    // TODO use Memory<byte> instead?
    public PcxDirectory(byte[] bytes)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));
        if (bytes.Length != HeaderSizeBytes)
            throw new ArgumentException($"Must contain {HeaderSizeBytes} bytes", nameof(bytes));
        _bytes = bytes;
    }

    public override int Count => 14; // TODO previous implementation 
    
    public override string Name => "PCX";
    
    public override IEnumerable<IDirectory> SubDirectories => Enumerable.Empty<IDirectory>();
    
    // Bytes 0 and 2 have fixed values, validated during read time

    #region Properties
    
    // BUG this approach of Read/Write won't work, as it requires the host machine to use the same endianness as the data

    private T Read<T>(int index, int length) where T : struct
    {
        Span<byte> span = _bytes.AsSpan().Slice(index, length);
        return MemoryMarshal.Read<T>(span);
    }

    private void Write<T>(int index, int length, ref T value) where T : struct
    {
        Span<byte> span = _bytes.AsSpan().Slice(index, length);
        MemoryMarshal.Write(span, ref value);
    }

    public byte Version
    {
        get => _bytes[1];
        set => _bytes[1] = value;
    }

    public byte BitsPerPixel
    {
        get => _bytes[3];
        set => _bytes[3] = value;
    }

    public ushort XMin
    {
        get => Read<ushort>(4, 2);
        set => Write(4, 2, ref value);
    }

    public ushort YMin
    {
        get => Read<ushort>(6, 2);
        set => Write(6, 2, ref value);
    }

    public ushort XMax
    {
        get => Read<ushort>(8, 2);
        set => Write(8, 2, ref value);
    }

    public ushort YMax
    {
        get => Read<ushort>(10, 2);
        set => Write(10, 2, ref value);
    }

    public ushort HorizontalDpi
    {
        get => Read<ushort>(12, 2);
        set => Write(12, 2, ref value);
    }

    public ushort VerticalDpi
    {
        get => Read<ushort>(14, 2);
        set => Write(14, 2, ref value);
    }

    public byte[] Palette
    {
        get => _bytes[16..63];
        set => Array.Copy(value, sourceIndex: 0, _bytes, destinationIndex: 16, value.Length);
    }

    public byte ColorPlanes
    {
        get => _bytes[65];
        set => _bytes[65] = value;
    }

    public ushort BytesPerLine
    {
        get => Read<ushort>(66, 2);
        set => Write(66, 2, ref value);
    }

    public ushort PaletteType
    {
        get => Read<ushort>(68, 2);
        set => Write(68, 2, ref value);
    }

    public ushort HScrSize
    {
        get => Read<ushort>(70, 2);
        set => Write(70, 2, ref value);
    }

    public ushort VScrSize
    {
        get => Read<ushort>(72, 2);
        set => Write(72, 2, ref value);
    }
    
    #endregion

    public override IEnumerator<IEntry> GetEnumerator()
    {
        // Spike of a simpler API
        
        yield return new Entry<byte>("Version", Version, index => _pcxVersionTable.LookUp(index));
        yield return new Entry<byte>("Bits Per Pixel", BitsPerPixel);
        yield return new Entry<ushort>("X Min", XMin);
        yield return new Entry<ushort>("Y Min", YMin);
        yield return new Entry<ushort>("X Max", XMax);
        yield return new Entry<ushort>("Y Max", YMax);
        yield return new Entry<ushort>("Horizontal DPI", HorizontalDpi);
        yield return new Entry<ushort>("Vertical DPI", VerticalDpi);
        yield return new Entry<byte[]>("Palette", Palette);
        // TODO the old code would only return these if they were non-zero
        yield return new Entry<byte>("Color Planes", ColorPlanes, static index => _pcxColorPlanesTable.LookUp(index));
        yield return new Entry<ushort>("Bytes Per Line", BytesPerLine);
        yield return new Entry<ushort>("Palette Type", PaletteType, static index => _pcxPaletteTypeTable.LookUp(index));
        // TODO not all the above properties are exported here
    }
}

internal sealed class IndexedTable
{
    public IndexedTable(string?[] strings, int baseIndex = 0)
    {
    }

    public string? LookUp(int index)
    {
        asdf
    }
}

public class Entry<T> : IEntry where T : notnull
{
    private readonly Func<T, string?>? _descriptor;

    public Entry(string name, T value, Func<T, string?>? descriptor = null)
    {
        _descriptor = descriptor;
        Name = name;
        Value = value;
    }

    private T Value { get; }
    object IEntry.Value => Value;
    public string Name { get; }
    public string? Description => _descriptor == null ? Value.ToString() : _descriptor(Value);
}

public sealed class PcxReader
{
    public PcxDirectory Extract(SequentialReader reader)
    {
        byte[] bytes;
        try
        {
            bytes = reader.GetBytes(PcxDirectory.HeaderSizeBytes);
        }
        catch (Exception ex)
        {
            return WithError("Exception reading PCX metadata: " + ex.Message);
        }
        
        if (bytes[0] != 0x0A)
            return WithError("Invalid PCX identifier byte");
        if (bytes[2] != 0x01)
            return WithError("Invalid PCX encoding byte");

//            reader = reader.WithByteOrder(isMotorolaByteOrder: false);
        
        return new PcxDirectory(bytes);

        static PcxDirectory WithError(string errorMessage)
        {
            var directory = new PcxDirectory();
            directory.AddError(errorMessage);
            return directory;
        }
    }
}

/////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////

public class PngHeaderDirectory : KeyedDirectory<PngTag, PngEntry>
{
    
}

/////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////

public static class MetadataReader
{
    public static IReadOnlyList<IDirectory> Read(string path)
    {
        // TODO
    }
}

internal static class Program
{
    private static void Main(string[] args)
    {
        IReadOnlyList<IDirectory> directories = MetadataReader.Read(args[0]);

        foreach (IDirectory directory in directories)
        foreach (IEntry entry in directory)
            Console.Out.WriteLine($"{directory.Name} - {entry.Name} = {entry.Description}");

        var ifd0 = directories.OfType<ExifIfd0Directory>().SingleOrDefault();

        if (ifd0 != null)
        {
            Console.Out.WriteLine($"Image dimensions: {ifd0.Width} x {ifd0.Height}");

            foreach (TiffValue tiffValue in ifd0)
            {
                var tag = tiffValue.Tag;
                Console.Out.WriteLine($"Tag {tag.Name} ({tag.Id}) {tiffValue.Format} {tiffValue.ComponentCount} component(s) = {tiffValue.Value}");
            }
            
            // TODO printing errors
            // TODO recurring through sub-directories, vs flat-structure (IsTopLevel flag on IDirectory?)
        }

        // TODO some directories are flexible with types of values (eg. TIFF _can_ store unexpected types of values for tags) and some aren't (eg. PCX version _must_ be a byte) which becomes important if we want to write metadata
        
        // TODO sketch out an index-based directory type (eg. fixed offsets and all fields present)
        // TODO sketch out an enum-based directory type (simple key)
        
        // TODO sketch out multi-value getter, for tags whose values come from more than one field
        // - TiffTags.XResolution/YResolution + ResolutionUnits
    }
}

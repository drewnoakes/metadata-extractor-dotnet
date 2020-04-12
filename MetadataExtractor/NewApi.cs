// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

#nullable enable

// TODO remove this
// ReSharper disable CheckNamespace
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MetadataExtractor.Formats.Tiff;
using MetadataExtractor.IO;
using XmpCore;
using XmpCore.Impl;
using XmpCore.Options;

namespace MetadataExtractor.NewApi
{
    public interface IDirectory : IReadOnlyCollection<IEntry>
    {
        string Name { get; }

        IEnumerable<IDirectory> SubDirectories { get; }
    }

//    /// <summary>
//    /// Base class for directories whose contents are stored by index.
//    /// </summary>
//    /// <typeparam name="TEntry"></typeparam>
//    public abstract class Directory<TEntry> : IDirectory, IEnumerable<TEntry> where TEntry : IEntry
//    {
//        // TODO need to maintain order of values if we are to write data again
//
//        private readonly List<TEntry> _entries = new List<TEntry>(); 
//
//        public abstract string Name { get; }
//
//        public int Count => _entries.Count;
//
//        // TODO can we store IDirectory as IEntry too? A directory may have different entry metadata based upon how it's embedded in outer data. something like ILinkedDirectoryEntry?
//        public IEnumerable<IDirectory> SubDirectories => _entries.Select(entry => entry.Value).OfType<IDirectory>();
//
//        public bool TryGetValue(int index, out TEntry entry)
//        {
//            if (index >= 0 && index < _entries.Count)
//            {
//                entry = _entries[index];
//                return true;
//            }
//
//            entry = default;
//            return false;
//        }
//
//        #region IEnumerable
//
//        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//
//        IEnumerator<IEntry> IEnumerable<IEntry>.GetEnumerator() => GetEnumerator();
//        
//        public IEnumerator<TEntry> GetEnumerator() => _entries.GetEnumerator();
//
//        #endregion
//    }

    /// <summary>
    /// Based class for directories whose contents are stored by key.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TEntry"></typeparam>
    public abstract class Directory<TKey, TEntry> : IDirectory, IEnumerable<TEntry> where TEntry : IEntry
    {
        // TODO need to maintain order of values if we are to write data again

        private readonly Dictionary<TKey, TEntry> _entryByKey; 

        protected Directory(IEqualityComparer<TKey> comparator = null)
        {
            _entryByKey = new Dictionary<TKey, TEntry>(comparator);
        }

        public abstract string Name { get; }

        public int Count => _entryByKey.Count;

        // TODO can we store IDirectory as IEntry too? A directory may have different entry metadata based upon how it's embedded in outer data. something like ILinkedDirectoryEntry?
        public IEnumerable<IDirectory> SubDirectories => _entryByKey.Select(entry => entry.Value.Value).OfType<IDirectory>();

        public bool TryGetValue(TKey key, out TEntry entry)
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

        IEnumerator<IEntry> IEnumerable<IEntry>.GetEnumerator() => GetEnumerator();
        
        public IEnumerator<TEntry> GetEnumerator() => ((IEnumerable<TEntry>)_entryByKey.Values).GetEnumerator();

        #endregion
    }

    public interface IEntry
    {
        object Value { get; }
        string Name { get; }
        string? Description { get; }
    }

    [StructLayout(LayoutKind.Auto)]
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

        public static TiffValue CreateInt8U    (byte      value, TiffTag tag) => new TiffValue(TiffDataFormat.Int8U,     1, value, tag);
        public static TiffValue CreateInt16U   (ushort    value, TiffTag tag) => new TiffValue(TiffDataFormat.Int16U,    1, value, tag);
        public static TiffValue CreateInt32U   (uint      value, TiffTag tag) => new TiffValue(TiffDataFormat.Int32U,    1, value, tag);
        public static TiffValue CreateRationalU(URational value, TiffTag tag) => new TiffValue(TiffDataFormat.RationalU, 1, value, tag);
        public static TiffValue CreateInt8S    (sbyte     value, TiffTag tag) => new TiffValue(TiffDataFormat.Int8S,     1, value, tag);
        public static TiffValue CreateInt16S   (short     value, TiffTag tag) => new TiffValue(TiffDataFormat.Int16S,    1, value, tag);
        public static TiffValue CreateInt32S   (int       value, TiffTag tag) => new TiffValue(TiffDataFormat.Int32S,    1, value, tag);
        public static TiffValue CreateRationalS(Rational  value, TiffTag tag) => new TiffValue(TiffDataFormat.RationalS, 1, value, tag);
        public static TiffValue CreateSingle   (float     value, TiffTag tag) => new TiffValue(TiffDataFormat.Single,    1, value, tag);
        public static TiffValue CreateDouble   (double    value, TiffTag tag) => new TiffValue(TiffDataFormat.Double,    1, value, tag);

        public static TiffValue CreateString   (byte[] value,    TiffTag tag) => new TiffValue(TiffDataFormat.String,    value.Length, value, tag);
        public static TiffValue CreateUndefined(byte[] value,    TiffTag tag) => new TiffValue(TiffDataFormat.Undefined, value.Length, value, tag);

        public static TiffValue CreateInt8UArray    (byte[]      value, TiffTag tag) => new TiffValue(TiffDataFormat.Int8U,     value.Length, value, tag);
        public static TiffValue CreateInt16UArray   (ushort[]    value, TiffTag tag) => new TiffValue(TiffDataFormat.Int16U,    value.Length, value, tag);
        public static TiffValue CreateInt32UArray   (uint[]      value, TiffTag tag) => new TiffValue(TiffDataFormat.Int32U,    value.Length, value, tag);
        public static TiffValue CreateRationalUArray(URational[] value, TiffTag tag) => new TiffValue(TiffDataFormat.RationalU, value.Length, value, tag);
        public static TiffValue CreateInt8SArray    (sbyte[]     value, TiffTag tag) => new TiffValue(TiffDataFormat.Int8S,     value.Length, value, tag);
        public static TiffValue CreateUndefinedArray(byte[][]    value, TiffTag tag) => new TiffValue(TiffDataFormat.Undefined, value.Length, value, tag);
        public static TiffValue CreateInt16SArray   (short[]     value, TiffTag tag) => new TiffValue(TiffDataFormat.Int16S,    value.Length, value, tag);
        public static TiffValue CreateInt32SArray   (int[]       value, TiffTag tag) => new TiffValue(TiffDataFormat.Int32S,    value.Length, value, tag);
        public static TiffValue CreateRationalSArray(Rational[]  value, TiffTag tag) => new TiffValue(TiffDataFormat.RationalS, value.Length, value, tag);
        public static TiffValue CreateSingleArray   (float[]     value, TiffTag tag) => new TiffValue(TiffDataFormat.Single,    value.Length, value, tag);
        public static TiffValue CreateDoubleArray   (double[]    value, TiffTag tag) => new TiffValue(TiffDataFormat.Double,    value.Length, value, tag);

        public bool TryGetByte(out byte b)
        {
            object value = Value;
            
            if (value is byte int8)
            {
                b = int8;
                return true;
            }
            
            // TODO coercion could call other TryGet* methods to reduce logic duplication
            if (value is sbyte int8u && int8u >= byte.MinValue)
            {
                b = (byte)int8u;
                return true;
            }

            if (value is short int16 && int16 >= byte.MinValue && int16 <= byte.MaxValue)
            {
                b = (byte) int16;
                return true;
            }

            // TODO further coercions
            
            b = default;
            return false;
        }

        public bool TryGetInt32(out int value)
        {
        }

        public bool TryGetInt32Array(out int[] ints)
        {
        }

        public bool TryGetSingle(out float f)
        {
        }

        public bool TryGetString(out string s)
        {
        }

        public bool TryGetRational(out Rational rational)
        {
        }

        public bool TryGetURational(out URational uRational)
        {
        }

        public bool TryGetByteArray(out byte[] bytes)
        {
        }

        public bool TryGetURationalArray(out URational[] uRationals)
        {
        }

        public override string ToString() => ToString(provider: null);
        
        public string ToString(IFormatProvider? provider) => Tag.Describe(this, provider);
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

        public abstract string? Describe(TiffValue value, IFormatProvider? provider = null);
    }

    public sealed class TiffTagIdComparator : IEqualityComparer<TiffTag>
    {
        public static TiffTagIdComparator Instance { get; } = new TiffTagIdComparator();

        public bool Equals(TiffTag x, TiffTag y) => x?.Id == y?.Id;

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
        private Func<TiffValue, IFormatProvider?, string?> Describer { get; }

        public TiffUInt8Tag(int id, string name, Func<byte, IFormatProvider?, string>? describer = null)
            : base(id, name)
        {
            // TODO store describer delegate in base class if all subclasses end up using it
            Describer = (value, format) => value.TryGetByte(out byte b)
                ? describer?.Invoke(b, format) ?? b.ToString(format)
                : null;
        }

        public override string? Describe(TiffValue value, IFormatProvider? provider = null)
            => Describer(value, provider);

        public override TiffDataFormat ExpectedFormat => TiffDataFormat.Int8U;
    }

    public class TiffUInt16Tag : KnownTiffTag
    {
        private Func<TiffValue, IFormatProvider?, string?> Describer { get; }

        public TiffUInt16Tag(int id, string name, Func<int, IFormatProvider?, string?>? describer = null)
            : base(id, name)
        {
            // TODO store describer delegate in base class if all subclasses end up using it
            Describer = (value, format) => value.TryGetInt32(out var i)
                ? describer?.Invoke(i, format) ?? i.ToString(format)
                : null;
        }

        public override string? Describe(TiffValue value, IFormatProvider? provider = null)
            => Describer(value, provider);

        public override TiffDataFormat ExpectedFormat => TiffDataFormat.Int16U;
    }

    public class TiffUInt32Tag : KnownTiffTag
    {
        private Func<TiffValue, IFormatProvider?, string?> Describer { get; }

        public TiffUInt32Tag(int id, string name, Func<int, IFormatProvider?, string>? describer = null)
            : base(id, name)
        {
            // TODO store describer delegate in base class if all subclasses end up using it
            Describer = (value, format) => value.TryGetInt32(out var i)
                ? describer?.Invoke(i, format) ?? i.ToString(format)
                : null;
        }

        public override string? Describe(TiffValue value, IFormatProvider? provider = null)
            => Describer(value, provider);

        public override TiffDataFormat ExpectedFormat => TiffDataFormat.Int32U;
    }

    public class TiffSingleTag : KnownTiffTag
    {
        private Func<TiffValue, IFormatProvider?, string?> Describer { get; }

        public TiffSingleTag(int id, string name, Func<float, IFormatProvider?, string?>? describer = null)
            : base(id, name)
        {
            // TODO store describer delegate in base class if all subclasses end up using it
            Describer = (value, format) => value.TryGetSingle(out float f)
                ? describer?.Invoke(f, format) ?? f.ToString(format)
                : null;
        }

        public override string? Describe(TiffValue value, IFormatProvider? provider = null)
            => Describer(value, provider);

        public override TiffDataFormat ExpectedFormat => TiffDataFormat.Single;
    }

    public class TiffRationalTag : KnownTiffTag
    {
        private Func<TiffValue, IFormatProvider?, string?> Describer { get; }

        public TiffRationalTag(int id, string name, Func<Rational, IFormatProvider?, string>? describer = null)
            : base(id, name)
        {
            // TODO store describer delegate in base class if all subclasses end up using it
            Describer = (value, format) =>
            {
                return value.TryGetRational(out Rational r)
                    ? describer?.Invoke(r, format) ?? r.ToString(format)
                    : null;
            };
        }

        public override string? Describe(TiffValue value, IFormatProvider? provider = null)
            => Describer(value, provider);

        public override TiffDataFormat ExpectedFormat => TiffDataFormat.RationalS;
    }

    public class TiffURationalTag : KnownTiffTag
    {
        private Func<TiffValue, IFormatProvider?, string?> Describer { get; }

        public TiffURationalTag(int id, string name, Func<URational, IFormatProvider?, string>? describer = null)
            : base(id, name)
        {
            // TODO store describer delegate in base class if all subclasses end up using it
            Describer = (value, format) =>
            {
                return value.TryGetURational(out URational r)
                    ? describer?.Invoke(r, format) ?? r.ToString(format)
                    : null;
            };
        }

        public override string? Describe(TiffValue value, IFormatProvider? provider = null)
            => Describer(value, provider);

        public override TiffDataFormat ExpectedFormat => TiffDataFormat.RationalU;
    }

    public class TiffStringTag : KnownTiffTag
    {
        private readonly Func<byte[], IFormatProvider?, string?>? _describer;

        public Encoding ExpectedEncoding { get; }

        public TiffStringTag(int id, string name, Encoding? expectedEncoding = null)
            : base(id, name)
        {
            ExpectedEncoding = expectedEncoding ?? Encoding.UTF8;
        }
        
        public TiffStringTag(int id, string name, Func<byte[], IFormatProvider?, string> describer)
            : base(id, name)
        {
            _describer = describer;
            ExpectedEncoding = Encoding.UTF8;
        }

        public override string? Describe(TiffValue value, IFormatProvider? provider = null)
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

                return _describer(bytes, provider);
            }
            catch
            {
                return null;
            }
        }

        public override TiffDataFormat ExpectedFormat => TiffDataFormat.String;
    }

    public class TiffIndexedUInt16Tag : TiffUInt16Tag
    {
        public TiffIndexedUInt16Tag(int id, string name, int baseIndex, string[] descriptions)
            : base(id, name, (i, provider) => DecodeIndex(baseIndex, descriptions, i))
        {}

        private static string DecodeIndex(int baseIndex, string[] descriptions, int index)
        {
            int arrayIndex = index - baseIndex;

            if (arrayIndex < 0 || arrayIndex >= descriptions.Length)
                return null;
            
            return descriptions[arrayIndex];
        }
    }

    public class TiffMappedUInt16Tag : TiffUInt16Tag
    {
        public TiffMappedUInt16Tag(int id, string name, IReadOnlyDictionary<int, string> descriptions)
            : base(id, name, (i, provider) => DecodeIndex(descriptions, i))
        {}

        private static string? DecodeIndex(IReadOnlyDictionary<int, string> readOnlyDictionary, int value)
        {
            return !readOnlyDictionary.TryGetValue(value, out var description) ? null : description;
        }
    }

    public class TiffUInt16ArrayTag : KnownTiffTag
    {
        private Func<TiffValue, IFormatProvider?, string?> Describer { get; }

        public TiffUInt16ArrayTag(int id, string name, int expectedCount, Func<int[], IFormatProvider?, string?>? describer = null)
            : base(id, name, expectedCount)
        {
            // TODO store describer delegate in base class if all subclasses end up using it
            Describer = (value, format) => value.TryGetInt32Array(out var i) // TODO int16[] here?
                ? describer?.Invoke(i, format) ?? i.ToString()
                : null;
        }

        public override string? Describe(TiffValue value, IFormatProvider? provider = null)
            => Describer(value, provider);

        public override TiffDataFormat ExpectedFormat => TiffDataFormat.Int16U;
    }

    public class TiffURationalArrayTag : KnownTiffTag
    {
        private Func<TiffValue, IFormatProvider?, string?> Describer { get; }

        public TiffURationalArrayTag(int id, string name, int expectedCount, Func<URational[], IFormatProvider?, string?>? describer = null)
            : base(id, name, expectedCount)
        {
            // TODO store describer delegate in base class if all subclasses end up using it
            Describer = (value, format) => value.TryGetURationalArray(out URational[] i)
                ? describer?.Invoke(i, format) ?? i.ToString()
                : null;
        }

        public override string? Describe(TiffValue value, IFormatProvider? provider = null)
            => Describer(value, provider);

        public override TiffDataFormat ExpectedFormat => TiffDataFormat.RationalU;
    }

    public class UnknownTiffTag : TiffTag
    {
        public override bool IsKnown => false;

        public override string Name => $"Unknown ({Id})"; // TODO Hex display?

        public UnknownTiffTag(int id) : base(id)
        {
        }

        public override string? Describe(TiffValue value, IFormatProvider? provider = null)
        {
            return value.ToString(provider);
        }
    }

    public abstract class TiffDirectory : Directory<TiffTag, TiffValue> 
    {
        protected TiffDirectory() : base(TiffTagIdComparator.Instance)
        {
        }

        public bool TryGetInt32(TiffTag tag, out int value) // TODO should this be an override of the base?
        {
            if (TryGetValue(tag, out TiffValue tiffValue) &&
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

        public int? Width => TryGetInt32(ExifTags.ImageWidth, out int value) ? value : default;
        public int? Height => TryGetInt32(ExifTags.ImageHeight, out int value) ? value : default;
    }

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

        public override bool Equals(object obj) => obj is XmpName other && Equals(other);

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
    
    public sealed class XmpDirectory : Directory<XmpName, XmpValue>
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

    public enum PcxProperty
    {
        Version,
    }
    
    public sealed class PcxDirectory : Directory<IEntry>
    {
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
    }

    public sealed class PcxReader
    {
        public PcxDirectory Extract(SequentialReader reader)
        {
            reader = reader.WithByteOrder(isMotorolaByteOrder: false);

            var directory = new PcxDirectory();

            try
            {
                var identifier = reader.GetSByte();

                if (identifier != 0x0A)
                    throw new ImageProcessingException("Invalid PCX identifier byte");

                directory.Set(PcxDirectory.TagVersion, reader.GetSByte());

                var encoding = reader.GetSByte();
                if (encoding != 0x01)
                    throw new ImageProcessingException("Invalid PCX encoding byte");

                directory.Set(PcxDirectory.TagBitsPerPixel, reader.GetByte());
                directory.Set(PcxDirectory.TagXMin, reader.GetUInt16());
                directory.Set(PcxDirectory.TagYMin, reader.GetUInt16());
                directory.Set(PcxDirectory.TagXMax, reader.GetUInt16());
                directory.Set(PcxDirectory.TagYMax, reader.GetUInt16());
                directory.Set(PcxDirectory.TagHorizontalDpi, reader.GetUInt16());
                directory.Set(PcxDirectory.TagVerticalDpi, reader.GetUInt16());
                directory.Set(PcxDirectory.TagPalette, reader.GetBytes(48));
                reader.Skip(1);
                directory.Set(PcxDirectory.TagColorPlanes, reader.GetByte());
                directory.Set(PcxDirectory.TagBytesPerLine, reader.GetUInt16());

                var paletteType = reader.GetUInt16();
                if (paletteType != 0)
                    directory.Set(PcxDirectory.TagPaletteType, paletteType);

                var hScrSize = reader.GetUInt16();
                if (hScrSize != 0)
                    directory.Set(PcxDirectory.TagHScrSize, hScrSize);

                var vScrSize = reader.GetUInt16();
                if (vScrSize != 0)
                    directory.Set(PcxDirectory.TagVScrSize, vScrSize);
            }
            catch (Exception ex)
            {
                directory.AddError("Exception reading PCX file metadata: " + ex.Message);
            }

            return directory;
        }
    }

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
            var directories = MetadataReader.Read(args[0]);

            foreach (var directory in directories)
            foreach (var entry in directory)
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

            // TODO some directories are flexible (eg. TIFF _can_ store unexpected types of values for tags) and some aren't (eg. PCX version _must_ be a byte) which becomes important if we want to write metadata
            
            // TODO sketch out an index-based directory type (eg. fixed offsets and all fields present)
            // TODO sketch out an enum-based directory type (simple key)
            
            // TODO sketch out multi-value getter, for tags whose values come from more than one field
            // - TiffTags.XResolution/YResolution + ResolutionUnits
        }
    }
}

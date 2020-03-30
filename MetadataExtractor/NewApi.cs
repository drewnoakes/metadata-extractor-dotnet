#region License
//
// Copyright 2002-2016 Drew Noakes
// Ported from Java to C# by Yakov Danilov for Imazen LLC in 2014
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
// More information about this project is available at:
//
//    https://github.com/drewnoakes/metadata-extractor-dotnet
//    https://drewnoakes.com/code/exif/
//
#endregion

// ReSharper disable CheckNamespace
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MetadataExtractor.Formats.Tiff;

namespace MetadataExtractor.NewApi
{
    public interface IDirectory : IReadOnlyCollection<IEntry>
    {
        string Name { get; }

        IEnumerable<IDirectory> SubDirectories { get; }
    }

    public abstract class Directory<TEntry> : IDirectory where TEntry : IEntry
    {
        // TODO need to maintain order of values if we are to write data again

        private readonly List<TEntry> _entries = new List<TEntry>();

        public abstract string Name { get; }

        public int Count => _entries.Count;

        public IEnumerable<IDirectory> SubDirectories => _entries.Select(entry => entry.Value).OfType<IDirectory>();

//        public bool TryGetValue(TKey key, out TEntry entry)
//        {
//            return _entryByKey.TryGetValue(key, out entry);
//        }

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<IEntry> GetEnumerator() => _entries.Cast<IEntry>().GetEnumerator();

        #endregion
    }

    public interface IEntry
    {
        object Value { get; }
        string Name { get; }
        string? Description { get; }
    }

//    public static class ExifTags
//    {
//        public static readonly TiffTag Width = new TiffUInt16Tag(0x1234, "Width", TiffDataFormat.Int32, v => $"{v} pixel{v==0?"":"s"}");
//    }

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
            throw new NotImplementedException();
        }

        public bool TryGetInt(out int value)
        {

        }

        public bool TryGetIntArray(out int[] ints)
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
            throw new NotImplementedException();
        }

        public bool TryGetByteArray(out byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public bool TryGetURationalArray(out URational[] uRationals)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider? provider)
        {

        }
    }

    /// <summary>
    /// Base class for metadata about TIFF tags, whether known or unknown.
    /// </summary>
    public abstract class TiffTag
    {
        public abstract bool IsKnown { get; }

        public int Id { get; } // TODO should this be int?

        protected TiffTag(int id) => Id = id;

        public abstract string? Describe(TiffValue value, IFormatProvider? provider = null);
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
        public string Name { get; }

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
            Describer = (value, format) => value.TryGetInt(out var i)
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
            Describer = (value, format) => value.TryGetInt(out var i)
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

    public class RationalTag : KnownTiffTag
    {
        private Func<TiffValue, IFormatProvider?, string?> Describer { get; }

        public RationalTag(int id, string name, Func<Rational, IFormatProvider?, string>? describer = null)
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
        private readonly Func<string, IFormatProvider?, string?>? _describer;

        public Encoding ExpectedEncoding { get; }

        public TiffStringTag(int id, string name, Encoding? expectedEncoding = null, Func<string, IFormatProvider?, string>? describer = null)
            : base(id, name)
        {
            _describer = describer;
            ExpectedEncoding = expectedEncoding ?? Encoding.UTF8;
        }

        public override string? Describe(TiffValue value, IFormatProvider? provider = null)
        {
            if (!value.TryGetByteArray(out byte[] bytes))
                return null;

            try
            {
                // Decode the Unicode string and trim the Unicode zero "\0" from the end.
                var s = ExpectedEncoding.GetString(bytes, 0, bytes.Length).TrimEnd('\0');
                return _describer?.Invoke(s, provider) ?? s;
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
        public int BaseIndex { get; }
        public string[] Descriptions { get; }

        public TiffIndexedUInt16Tag(int id, string name, int baseIndex, string[] descriptions)
            : base(id, name, DecodeIndex)
        {
            BaseIndex = baseIndex;
            Descriptions = descriptions;
        }

        private string? DecodeIndex(int index, IFormatProvider? provider)
        {
            var arrayIndex = index - BaseIndex;

            if (arrayIndex >= 0 && arrayIndex < Descriptions.Length)
            {
                var description = Descriptions[arrayIndex];
                if (description != null)
                    return description;
            }

            return null;
        }
    }

    public class TiffMappedUInt16Tag : TiffUInt16Tag
    {
        public IReadOnlyDictionary<int, string> Descriptions { get; }

        public TiffMappedUInt16Tag(int id, string name, IReadOnlyDictionary<int, string> descriptions)
            : base(id, name, DecodeIndex)
        {
            Descriptions = descriptions;
        }

        private string? DecodeIndex(int value, IFormatProvider provider)
        {
            return !Descriptions.TryGetValue(value, out var description) ? null : description;
        }
    }

    public class TiffUInt16ArrayTag : KnownTiffTag
    {
        private Func<TiffValue, IFormatProvider?, string?> Describer { get; }

        public TiffUInt16ArrayTag(int id, string name, int expectedCount, Func<int[], IFormatProvider?, string?>? describer = null)
            : base(id, name, expectedCount)
        {
            // TODO store describer delegate in base class if all subclasses end up using it
            Describer = (value, format) => value.TryGetIntArray(out var i)
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

        public UnknownTiffTag(int id) : base(id)
        {
        }

        public override string? Describe(TiffValue value, IFormatProvider? provider = null)
        {
            return value.ToString(provider);
        }
    }

    public abstract class TiffDirectory : Directory<TiffValue>
    {
        protected TiffDirectory(string name) : base(name)
        {}

        // TODO store values -- Dictionary<TiffTag, TiffValue>
    }

    public class ExifIfd0Directory : TiffDirectory
    {
        public ExifIfd0Directory()
            : base("Exif IFD0")
        {}

        public int? Width
        {
            get { this.GetInt32 }
        }
    }

    public static class MetadataReader
    {
        public static IReadOnlyList<IDirectory> Read(string path)
        {}
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

            ifd0.Width
        }
    }
}

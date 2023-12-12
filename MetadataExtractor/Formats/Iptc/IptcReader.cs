// Copyright (c) Drew Noakes and contributors. All Rights Reserved. Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MetadataExtractor.Formats.Jpeg;

namespace MetadataExtractor.Formats.Iptc;

/// <summary>Reads IPTC data.</summary>
/// <remarks>
/// Extracted values are returned from <see cref="Extract"/> in an <see cref="IptcDirectory"/>.
/// <para />
/// See the IPTC specification: http://www.iptc.org/std/IIM/4.1/specification/IIMV4.1.pdf
/// </remarks>
/// <author>Drew Noakes https://drewnoakes.com</author>
public sealed class IptcReader : IJpegSegmentMetadataReader
{
    // TODO consider storing each IPTC record in a separate directory

    /*
    public static final int DIRECTORY_IPTC = 2;

    public static final int ENVELOPE_RECORD = 1;
    public static final int APPLICATION_RECORD_2 = 2;
    public static final int APPLICATION_RECORD_3 = 3;
    public static final int APPLICATION_RECORD_4 = 4;
    public static final int APPLICATION_RECORD_5 = 5;
    public static final int APPLICATION_RECORD_6 = 6;
    public static final int PRE_DATA_RECORD = 7;
    public static final int DATA_RECORD = 8;
    public static final int POST_DATA_RECORD = 9;
    */

    internal const byte IptcMarkerByte = 0x1c;

    ICollection<JpegSegmentType> IJpegSegmentMetadataReader.SegmentTypes { get; } = new[] { JpegSegmentType.AppD };

    public IEnumerable<Directory> ReadJpegSegments(IEnumerable<JpegSegment> segments)
    {
        // Ensure data starts with the IPTC marker byte
        return segments
            .Where(segment => segment.Bytes.Length != 0 && segment.Bytes[0] == IptcMarkerByte)
            .Select(segment => (Directory)Extract(new SequentialByteArrayReader(segment.Bytes), segment.Bytes.Length));
    }

    /// <summary>Reads IPTC values and returns them in an <see cref="IptcDirectory"/>.</summary>
    /// <remarks>
    /// Note that IPTC data does not describe its own length, hence <paramref name="length"/> is required.
    /// </remarks>
    public IptcDirectory Extract(SequentialReader reader, long length)
    {
        var directory = new IptcDirectory();

        var offset = 0;

        // for each tag
        while (offset < length)
        {
            // identifies start of a tag
            byte startByte;
            try
            {
                startByte = reader.GetByte();
                offset++;
            }
            catch (IOException)
            {
                directory.AddError("Unable to read starting byte of IPTC tag");
                break;
            }

            if (startByte != IptcMarkerByte)
            {
                // NOTE have seen images where there was one extra byte at the end, giving
                // offset==length at this point, which is not worth logging as an error.
                if (offset != length)
                    directory.AddError($"Invalid IPTC tag marker at offset {offset - 1}. Expected '0x{IptcMarkerByte:x2}' but got '0x{startByte:x}'.");
                break;
            }

            // we need at least four bytes left to read a tag
            if (offset + 4 > length)
            {
                directory.AddError("Too few bytes remain for a valid IPTC tag");
                break;
            }

            int directoryType;
            int tagType;
            int tagByteCount;
            try
            {
                directoryType = reader.GetByte();
                tagType = reader.GetByte();
                tagByteCount = reader.GetUInt16();
                if (tagByteCount > 0x7FFF)
                {
                    // Extended DataSet Tag (see 1.5(c), p14, IPTC-IIMV4.2.pdf)
                    tagByteCount = ((tagByteCount & 0x7FFF) << 16) | reader.GetUInt16();
                    offset += 2;
                }
                offset += 4;
            }
            catch (IOException)
            {
                directory.AddError("IPTC data segment ended mid-way through tag descriptor");
                break;
            }

            if (offset + tagByteCount > length)
            {
                directory.AddError("Data for tag extends beyond end of IPTC segment");
                break;
            }

            try
            {
                ProcessTag(reader, directory, directoryType, tagType, tagByteCount);
            }
            catch (IOException)
            {
                directory.AddError("Error processing IPTC tag");
                break;
            }

            offset += tagByteCount;
        }

        return directory;
    }

    private static void ProcessTag(SequentialReader reader, Directory directory, int directoryType, int tagType, int tagByteCount)
    {
        var tagIdentifier = tagType | (directoryType << 8);

        // Some images have been seen that specify a zero byte tag, which cannot be of much use.
        // We elect here to completely ignore the tag. The IPTC specification doesn't mention
        // anything about the interpretation of this situation.
        // https://raw.githubusercontent.com/wiki/drewnoakes/metadata-extractor/docs/IPTC-IIMV4.2.pdf
        if (tagByteCount == 0)
        {
            directory.Set(tagIdentifier, string.Empty);
            return;
        }

        switch (tagIdentifier)
        {
            case IptcDirectory.TagCodedCharacterSet:
            {
                var bytes = reader.GetBytes(tagByteCount);
                var charset = Iso2022Converter.ConvertEscapeSequenceToEncodingName(bytes);
                if (charset is null)
                {
                    // Unable to determine the charset, so fall through and treat tag as a regular string
                    charset = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                }
                directory.Set(tagIdentifier, charset);
                return;
            }

            case IptcDirectory.TagEnvelopeRecordVersion:
            case IptcDirectory.TagApplicationRecordVersion:
            case IptcDirectory.TagFileVersion:
            case IptcDirectory.TagArmVersion:
            case IptcDirectory.TagProgramVersion:
            {
                // short
                if (tagByteCount == 2)
                {
                    var shortValue = reader.GetUInt16();
                    reader.Skip(tagByteCount - 2);
                    directory.Set(tagIdentifier, shortValue);
                    return;
                }
                break;
            }

            case IptcDirectory.TagUrgency:
            {
                // byte
                directory.Set(tagIdentifier, reader.GetByte());
                reader.Skip(tagByteCount - 1);
                return;
            }
        }

        // If we haven't returned yet, treat it as a string
        // NOTE that there's a chance we've already loaded the value as a string above, but failed to parse the value
        var encodingName = directory.GetString(IptcDirectory.TagCodedCharacterSet);
        Encoding? encoding = null;
        if (encodingName is not null)
        {
            try
            {
                encoding = Encoding.GetEncoding(encodingName);
            }
            catch (ArgumentException)
            { }
        }

        StringValue str;
        if (encoding is not null)
            str = reader.GetStringValue(tagByteCount, encoding);
        else
        {
            var bytes = reader.GetBytes(tagByteCount);
            encoding = Iso2022Converter.GuessEncoding(bytes);
            str = new StringValue(bytes, encoding);
        }

        if (directory.ContainsTag(tagIdentifier))
        {
            // this fancy string[] business avoids using an ArrayList for performance reasons
            var oldStrings = directory.GetStringValueArray(tagIdentifier);

            StringValue[] newStrings;
            if (oldStrings is null)
            {
                // TODO hitting this block means any prior value(s) are discarded
                newStrings = new StringValue[1];
            }
            else
            {
                newStrings = new StringValue[oldStrings.Length + 1];
                Array.Copy(oldStrings, 0, newStrings, 0, oldStrings.Length);
            }
            newStrings[newStrings.Length - 1] = str;
            directory.Set(tagIdentifier, newStrings);
        }
        else
        {
            directory.Set(tagIdentifier, str);
        }
    }
}

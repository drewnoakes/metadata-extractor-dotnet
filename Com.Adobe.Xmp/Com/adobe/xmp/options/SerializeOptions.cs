// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

namespace Com.Adobe.Xmp.Options
{
    /// <summary>
    /// Options for <see cref="XmpMetaFactory.SerializeToBuffer(IXmpMeta, SerializeOptions)"/>.
    /// </summary>
    /// <since>24.01.2006</since>
    public sealed class SerializeOptions : Options
    {
        /// <summary>Omit the XML packet wrapper.</summary>
        public const int OmitPacketWrapper = unchecked((int)(0x0010));

        /// <summary>Mark packet as read-only.</summary>
        /// <remarks>Mark packet as read-only. Default is a writeable packet.</remarks>
        public const int ReadonlyPacket = unchecked((int)(0x0020));

        /// <summary>Use a compact form of RDF.</summary>
        /// <remarks>
        /// Use a compact form of RDF.
        /// The compact form is the default serialization format (this flag is technically ignored).
        /// To serialize to the canonical form, set the flag USE_CANONICAL_FORMAT.
        /// If both flags &quot;compact&quot; and &quot;canonical&quot; are set, canonical is used.
        /// </remarks>
        public const int UseCompactFormat = unchecked((int)(0x0040));

        /// <summary>Use the canonical form of RDF if set.</summary>
        /// <remarks>Use the canonical form of RDF if set. By default the compact form is used</remarks>
        public const int UseCanonicalFormat = unchecked((int)(0x0080));

        /// <summary>Include a padding allowance for a thumbnail image.</summary>
        /// <remarks>
        /// Include a padding allowance for a thumbnail image. If no <tt>xmp:Thumbnails</tt> property
        /// is present, the typical space for a JPEG thumbnail is used.
        /// </remarks>
        public const int IncludeThumbnailPad = unchecked((int)(0x0100));

        /// <summary>The padding parameter provides the overall packet length.</summary>
        /// <remarks>
        /// The padding parameter provides the overall packet length. The actual amount of padding is
        /// computed. An exception is thrown if the packet exceeds this length with no padding.
        /// </remarks>
        public const int ExactPacketLength = unchecked((int)(0x0200));

        /// <summary>Omit the &lt;x:xmpmeta&bt;-tag</summary>
        public const int OmitXmpmetaElement = unchecked((int)(0x1000));

        /// <summary>Sort the struct properties and qualifier before serializing</summary>
        public const int Sort = unchecked((int)(0x2000));

        /// <summary>Bit indicating little endian encoding, unset is big endian</summary>
        private const int LittleendianBit = unchecked((int)(0x0001));

        /// <summary>Bit indication UTF16 encoding.</summary>
        private const int Utf16Bit = unchecked((int)(0x0002));

        /// <summary>UTF8 encoding; this is the default</summary>
        public const int EncodeUtf8 = 0;

        /// <summary>UTF16BE encoding</summary>
        public const int EncodeUtf16Be = Utf16Bit;

        /// <summary>UTF16LE encoding</summary>
        public const int EncodeUtf16Le = Utf16Bit | LittleendianBit;

        private const int EncodingMask = Utf16Bit | LittleendianBit;

        /// <summary>The amount of padding to be added if a writeable XML packet is created.</summary>
        /// <remarks>
        /// The amount of padding to be added if a writeable XML packet is created. If zero is passed
        /// (the default) an appropriate amount of padding is computed.
        /// </remarks>
        private int _padding = 2048;

        /// <summary>The string to be used as a line terminator.</summary>
        /// <remarks>
        /// The string to be used as a line terminator. If empty it defaults to; linefeed, U+000A, the
        /// standard XML newline.
        /// </remarks>
        private string _newline = "\n";

        /// <summary>
        /// The string to be used for each level of indentation in the serialized
        /// RDF.
        /// </summary>
        /// <remarks>
        /// The string to be used for each level of indentation in the serialized
        /// RDF. If empty it defaults to two ASCII spaces, U+0020.
        /// </remarks>
        private string _indent = "  ";

        /// <summary>
        /// The number of levels of indentation to be used for the outermost XML element in the
        /// serialized RDF.
        /// </summary>
        /// <remarks>
        /// The number of levels of indentation to be used for the outermost XML element in the
        /// serialized RDF. This is convenient when embedding the RDF in other text, defaults to 0.
        /// </remarks>
        private int _baseIndent = 0;

        /// <summary>Omits the Toolkit version attribute, not published, only used for Unit tests.</summary>
        private readonly bool _omitVersionAttribute = false;

        /// <summary>Default constructor.</summary>
        public SerializeOptions()
        {
        }

        /// <summary>Constructor using inital options</summary>
        /// <param name="options">the inital options</param>
        /// <exception cref="XmpException">Thrown if options are not consistant.</exception>
        public SerializeOptions(int options)
            : base(options)
        {
        }

        // ---------------------------------------------------------------------------------------------
        // encoding bit constants
        // reveal default constructor
        /// <returns>Returns the option.</returns>
        public bool GetOmitPacketWrapper()
        {
            return GetOption(OmitPacketWrapper);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public SerializeOptions SetOmitPacketWrapper(bool value)
        {
            SetOption(OmitPacketWrapper, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool GetOmitXmpMetaElement()
        {
            return GetOption(OmitXmpmetaElement);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public SerializeOptions SetOmitXmpMetaElement(bool value)
        {
            SetOption(OmitXmpmetaElement, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool GetReadOnlyPacket()
        {
            return GetOption(ReadonlyPacket);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public SerializeOptions SetReadOnlyPacket(bool value)
        {
            SetOption(ReadonlyPacket, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool GetUseCompactFormat()
        {
            return GetOption(UseCompactFormat);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public SerializeOptions SetUseCompactFormat(bool value)
        {
            SetOption(UseCompactFormat, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool GetUseCanonicalFormat()
        {
            return GetOption(UseCanonicalFormat);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public SerializeOptions SetUseCanonicalFormat(bool value)
        {
            SetOption(UseCanonicalFormat, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool GetIncludeThumbnailPad()
        {
            return GetOption(IncludeThumbnailPad);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public SerializeOptions SetIncludeThumbnailPad(bool value)
        {
            SetOption(IncludeThumbnailPad, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool GetExactPacketLength()
        {
            return GetOption(ExactPacketLength);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public SerializeOptions SetExactPacketLength(bool value)
        {
            SetOption(ExactPacketLength, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool GetSort()
        {
            return GetOption(Sort);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public SerializeOptions SetSort(bool value)
        {
            SetOption(Sort, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool GetEncodeUtf16Be()
        {
            return (GetOptions() & EncodingMask) == EncodeUtf16Be;
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public SerializeOptions SetEncodeUtf16Be(bool value)
        {
            // clear unicode bits
            SetOption(Utf16Bit | LittleendianBit, false);
            SetOption(EncodeUtf16Be, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool GetEncodeUtf16Le()
        {
            return (GetOptions() & EncodingMask) == EncodeUtf16Le;
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public SerializeOptions SetEncodeUtf16Le(bool value)
        {
            // clear unicode bits
            SetOption(Utf16Bit | LittleendianBit, false);
            SetOption(EncodeUtf16Le, value);
            return this;
        }

        /// <returns>Returns the baseIndent.</returns>
        public int GetBaseIndent()
        {
            return _baseIndent;
        }

        /// <param name="baseIndent">The baseIndent to set.</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public SerializeOptions SetBaseIndent(int baseIndent)
        {
            this._baseIndent = baseIndent;
            return this;
        }

        /// <returns>Returns the indent.</returns>
        public string GetIndent()
        {
            return _indent;
        }

        /// <param name="indent">The indent to set.</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public SerializeOptions SetIndent(string indent)
        {
            this._indent = indent;
            return this;
        }

        /// <returns>Returns the newline.</returns>
        public string GetNewline()
        {
            return _newline;
        }

        /// <param name="newline">The newline to set.</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public SerializeOptions SetNewline(string newline)
        {
            this._newline = newline;
            return this;
        }

        /// <returns>Returns the padding.</returns>
        public int GetPadding()
        {
            return _padding;
        }

        /// <param name="padding">The padding to set.</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public SerializeOptions SetPadding(int padding)
        {
            this._padding = padding;
            return this;
        }

        /// <returns>
        /// Returns whether the Toolkit version attribute shall be omitted.
        /// <em>Note:</em> This options can only be set by unit tests.
        /// </returns>
        public bool GetOmitVersionAttribute()
        {
            return _omitVersionAttribute;
        }

        /// <returns>Returns the encoding as Java encoding String.</returns>
        public string GetEncoding()
        {
            if (GetEncodeUtf16Be())
            {
                return "UTF-16BE";
            }
            else
            {
                if (GetEncodeUtf16Le())
                {
                    return "UTF-16LE";
                }
                else
                {
                    return "UTF-8";
                }
            }
        }

        /// <returns>Returns clone of this SerializeOptions-object with the same options set.</returns>
        public object Clone()
        {
            SerializeOptions clone;
            try
            {
                clone = new SerializeOptions(GetOptions());
                clone.SetBaseIndent(_baseIndent);
                clone.SetIndent(_indent);
                clone.SetNewline(_newline);
                clone.SetPadding(_padding);
                return clone;
            }
            catch (XmpException)
            {
                // This cannot happen, the options are already checked in "this" object.
                return null;
            }
        }

        /// <seealso cref="Options.DefineOptionName(int)"/>
        protected internal override string DefineOptionName(int option)
        {
            switch (option)
            {
                case OmitPacketWrapper:
                {
                    return "OMIT_PACKET_WRAPPER";
                }

                case ReadonlyPacket:
                {
                    return "READONLY_PACKET";
                }

                case UseCompactFormat:
                {
                    return "USE_COMPACT_FORMAT";
                }

                case IncludeThumbnailPad:
                {
                    //            case USE_CANONICAL_FORMAT :        return "USE_CANONICAL_FORMAT";
                    return "INCLUDE_THUMBNAIL_PAD";
                }

                case ExactPacketLength:
                {
                    return "EXACT_PACKET_LENGTH";
                }

                case OmitXmpmetaElement:
                {
                    return "OMIT_XMPMETA_ELEMENT";
                }

                case Sort:
                {
                    return "NORMALIZED";
                }

                default:
                {
                    return null;
                }
            }
        }

        /// <seealso cref="Options.GetValidOptions()"/>
        protected internal override int GetValidOptions()
        {
            return OmitPacketWrapper | ReadonlyPacket | UseCompactFormat | IncludeThumbnailPad | OmitXmpmetaElement | ExactPacketLength | Sort;
        }
        //        USE_CANONICAL_FORMAT |
    }
}

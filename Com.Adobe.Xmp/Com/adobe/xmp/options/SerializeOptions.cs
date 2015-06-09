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
        private const int OmitPacketWrapperFlag = unchecked(0x0010);
        private const int ReadonlyPacketFlag = unchecked(0x0020);
        private const int UseCompactFormatFlag = unchecked(0x0040);
        private const int UseCanonicalFormatFlag = unchecked(0x0080);
        private const int IncludeThumbnailPadFlag = unchecked(0x0100);
        private const int ExactPacketLengthFlag = unchecked(0x0200);
        private const int OmitXmpmetaElementFlag = unchecked(0x1000);
        private const int SortFlag = unchecked(0x2000);

        /// <summary>Bit indicating little endian encoding, unset is big endian</summary>
        private const int LittleendianBit = unchecked(0x0001);
        /// <summary>Bit indication UTF16 encoding.</summary>
        private const int Utf16Bit = unchecked(0x0002);
        /// <summary>UTF8 encoding; this is the default</summary>
        public const int EncodeUtf8 = 0;
        public const int EncodeUtf16BeFlag = Utf16Bit;
        public const int EncodeUtf16LeFlag = Utf16Bit | LittleendianBit;
        private const int EncodingMask = Utf16Bit | LittleendianBit;

        /// <summary>Default constructor.</summary>
        public SerializeOptions()
        {
            Padding = 2048;
            Newline = "\n";
            Indent = "  ";
        }

        /// <summary>Constructor using inital options</summary>
        /// <param name="options">the inital options</param>
        /// <exception cref="XmpException">Thrown if options are not consistant.</exception>
        public SerializeOptions(int options)
            : base(options)
        {
            Padding = 2048;
            Newline = "\n";
            Indent = "  ";
        }

        // ---------------------------------------------------------------------------------------------
        // encoding bit constants

        // reveal default constructor

        /// <summary>Omit the XML packet wrapper.</summary>
        public bool OmitPacketWrapper
        {
            get { return GetOption(OmitPacketWrapperFlag); }
            set { SetOption(OmitPacketWrapperFlag, value); }
        }

        /// <summary>Omit the &lt;x:xmpmeta&gt; tag.</summary>
        public bool OmitXmpMetaElement
        {
            get { return GetOption(OmitXmpmetaElementFlag); }
            set { SetOption(OmitXmpmetaElementFlag, value); }
        }

        /// <summary>Mark packet as read-only.</summary>
        /// <remarks>Default is a writeable packet.</remarks>
        public bool ReadOnlyPacket
        {
            get { return GetOption(ReadonlyPacketFlag); }
            set { SetOption(ReadonlyPacketFlag, value); }
        }

        /// <summary>Use a compact form of RDF.</summary>
        /// <remarks>
        /// Use a compact form of RDF.
        /// The compact form is the default serialization format (this flag is technically ignored).
        /// To serialize to the canonical form, set the flag USE_CANONICAL_FORMAT.
        /// If both flags &quot;compact&quot; and &quot;canonical&quot; are set, canonical is used.
        /// </remarks>
        public bool UseCompactFormat
        {
            get { return GetOption(UseCompactFormatFlag); }
            set { SetOption(UseCompactFormatFlag, value); }
        }

        /// <summary>Use the canonical form of RDF if set.</summary>
        /// <remarks>By default the compact form is used.</remarks>
        public bool UseCanonicalFormat
        {
            get { return GetOption(UseCanonicalFormatFlag); }
            set { SetOption(UseCanonicalFormatFlag, value); }
        }

        /// <summary>Include a padding allowance for a thumbnail image.</summary>
        /// <remarks>
        /// Include a padding allowance for a thumbnail image. If no <tt>xmp:Thumbnails</tt> property
        /// is present, the typical space for a JPEG thumbnail is used.
        /// </remarks>
        public bool IncludeThumbnailPad
        {
            get { return GetOption(IncludeThumbnailPadFlag); }
            set { SetOption(IncludeThumbnailPadFlag, value); }
        }

        /// <summary>The padding parameter provides the overall packet length.</summary>
        /// <remarks>
        /// The padding parameter provides the overall packet length. The actual amount of padding is
        /// computed. An exception is thrown if the packet exceeds this length with no padding.
        /// </remarks>
        public bool ExactPacketLength
        {
            get { return GetOption(ExactPacketLengthFlag); }
            set { SetOption(ExactPacketLengthFlag, value); }
        }

        /// <summary>Sort the struct properties and qualifier before serializing</summary>
        public bool Sort
        {
            get { return GetOption(SortFlag); }
            set { SetOption(SortFlag, value); }
        }


        /// <summary>UTF16BE encoding</summary>
        public bool EncodeUtf16Be
        {
            get { return (GetOptions() & EncodingMask) == EncodeUtf16BeFlag; }
            set
            {
                // clear unicode bits
                SetOption(Utf16Bit | LittleendianBit, false);
                SetOption(EncodeUtf16BeFlag, value);
            }
        }

        /// <summary>UTF16LE encoding</summary>
        public bool EncodeUtf16Le
        {
            get { return (GetOptions() & EncodingMask) == EncodeUtf16LeFlag; }
            set
            {
                // clear unicode bits
                SetOption(Utf16Bit | LittleendianBit, false);
                SetOption(EncodeUtf16LeFlag, value);
            }
        }

        /// <summary>
        /// The number of levels of indentation to be used for the outermost XML element in the
        /// serialized RDF.
        /// </summary>
        /// <remarks>
        /// The number of levels of indentation to be used for the outermost XML element in the
        /// serialized RDF. This is convenient when embedding the RDF in other text, defaults to 0.
        /// </remarks>
        public int BaseIndent { set; get; }

        /// <summary>
        /// The string to be used for each level of indentation in the serialized
        /// RDF.
        /// </summary>
        /// <remarks>
        /// The string to be used for each level of indentation in the serialized
        /// RDF. If empty it defaults to two ASCII spaces, U+0020.
        /// </remarks>
        public string Indent { set; get; }

        /// <summary>The string to be used as a line terminator.</summary>
        /// <remarks>
        /// The string to be used as a line terminator. If empty it defaults to; linefeed, U+000A, the
        /// standard XML newline.
        /// </remarks>
        public string Newline { get; set; }

        /// <summary>The amount of padding to be added if a writeable XML packet is created.</summary>
        /// <remarks>
        /// The amount of padding to be added if a writeable XML packet is created. If zero is passed
        /// (the default) an appropriate amount of padding is computed.
        /// </remarks>
        public int Padding { get; set; }

        /// <returns>Returns the encoding as Java encoding String.</returns>
        public string GetEncoding()
        {
            if (EncodeUtf16Be)
            {
                return "UTF-16BE";
            }
            if (EncodeUtf16Le)
            {
                return "UTF-16LE";
            }
            return "UTF-8";
        }

        /// <returns>Returns clone of this SerializeOptions-object with the same options set.</returns>
        public object Clone()
        {
            SerializeOptions clone;
            try
            {
                clone = new SerializeOptions(GetOptions());
                clone.BaseIndent = BaseIndent;
                clone.Indent = Indent;
                clone.Newline = Newline;
                clone.Padding = Padding;
                return clone;
            }
            catch (XmpException)
            {
                // This cannot happen, the options are already checked in "this" object.
                return null;
            }
        }

        /// <seealso cref="Options.DefineOptionName(int)"/>
        protected override string DefineOptionName(int option)
        {
            switch (option)
            {
                case OmitPacketWrapperFlag:
                    return "OMIT_PACKET_WRAPPER";
                case ReadonlyPacketFlag:
                    return "READONLY_PACKET";
                case UseCompactFormatFlag:
                    return "USE_COMPACT_FORMAT";
                case IncludeThumbnailPadFlag:
                    return "INCLUDE_THUMBNAIL_PAD";
//              case USE_CANONICAL_FORMAT:        return "USE_CANONICAL_FORMAT";
                case ExactPacketLengthFlag:
                    return "EXACT_PACKET_LENGTH";
                case OmitXmpmetaElementFlag:
                    return "OMIT_XMPMETA_ELEMENT";
                case SortFlag:
                    return "NORMALIZED";
                default:
                    return null;
            }
        }

        /// <seealso cref="Options.GetValidOptions()"/>
        protected override int GetValidOptions()
        {
            return OmitPacketWrapperFlag | ReadonlyPacketFlag | UseCompactFormatFlag | IncludeThumbnailPadFlag | OmitXmpmetaElementFlag | ExactPacketLengthFlag | SortFlag;
            //        USE_CANONICAL_FORMAT |
        }
    }
}

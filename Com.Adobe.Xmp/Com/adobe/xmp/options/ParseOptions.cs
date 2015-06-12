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
    /// Options for <see cref="XmpMetaFactory.Parse(System.IO.Stream, ParseOptions)"/>.
    /// </summary>
    /// <since>24.01.2006</since>
    public sealed class ParseOptions : Options
    {
        /// <summary>Require a surrounding &quot;x:xmpmeta&quot; element in the xml-document.</summary>
        private const int RequireXmpMetaFlag = unchecked(0x0001);

        /// <summary>Do not reconcile alias differences, throw an exception instead.</summary>
        private const int StrictAliasingFlag = unchecked(0x0004);

        /// <summary>Convert ASCII control characters 0x01 - 0x1F (except tab, cr, and lf) to spaces.</summary>
        private const int FixControlCharsFlag = unchecked(0x0008);

        /// <summary>If the input is not unicode, try to parse it as ISO-8859-1.</summary>
        private const int AcceptLatin1Flag = unchecked(0x0010);

        /// <summary>Do not carry run the XMPNormalizer on a packet, leave it as it is.</summary>
        private const int OmitNormalizationFlag = unchecked(0x0020);

        /// <summary>Sets the options to the default values.</summary>
        public ParseOptions()
        {
            SetOption(FixControlCharsFlag | AcceptLatin1Flag, true);
        }

        public bool RequireXmpMeta
        {
            get { return GetOption(RequireXmpMetaFlag); }
            set { SetOption(RequireXmpMetaFlag, value); }
        }

        public bool StrictAliasing
        {
            get { return GetOption(StrictAliasingFlag); }
            set { SetOption(StrictAliasingFlag, value); }
        }

        public bool FixControlChars
        {
            get { return GetOption(FixControlCharsFlag); }
            set { SetOption(FixControlCharsFlag, value); }
        }

        public bool AcceptLatin1
        {
            get { return GetOption(AcceptLatin1Flag); }
            set { SetOption(AcceptLatin1Flag, value); }
        }

        public bool OmitNormalization
        {
            set { SetOption(OmitNormalizationFlag, value); }
            get { return GetOption(OmitNormalizationFlag); }
        }

        protected override string DefineOptionName(int option)
        {
            switch (option)
            {
                case RequireXmpMetaFlag:
                    return "REQUIRE_XMP_META";
                case StrictAliasingFlag:
                    return "STRICT_ALIASING";
                case FixControlCharsFlag:
                    return "FIX_CONTROL_CHARS";
                case AcceptLatin1Flag:
                    return "ACCEPT_LATIN_1";
                case OmitNormalizationFlag:
                    return "OMIT_NORMALIZATION";
                default:
                    return null;
            }
        }

        protected override int GetValidOptions()
        {
            return RequireXmpMetaFlag | StrictAliasingFlag | FixControlCharsFlag | AcceptLatin1Flag | OmitNormalizationFlag;
        }
    }
}

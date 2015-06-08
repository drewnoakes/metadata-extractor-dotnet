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
    /// Options for
    /// <see cref="Com.Adobe.Xmp.XMPMetaFactory.Parse(System.IO.InputStream, ParseOptions)"/>
    /// .
    /// </summary>
    /// <since>24.01.2006</since>
    public sealed class ParseOptions : Options
    {
        /// <summary>Require a surrounding &quot;x:xmpmeta&quot; element in the xml-document.</summary>
        public const int RequireXmpMeta = unchecked((int)(0x0001));

        /// <summary>Do not reconcile alias differences, throw an exception instead.</summary>
        public const int StrictAliasing = unchecked((int)(0x0004));

        /// <summary>Convert ASCII control characters 0x01 - 0x1F (except tab, cr, and lf) to spaces.</summary>
        public const int FixControlChars = unchecked((int)(0x0008));

        /// <summary>If the input is not unicode, try to parse it as ISO-8859-1.</summary>
        public const int AcceptLatin1 = unchecked((int)(0x0010));

        /// <summary>Do not carry run the XMPNormalizer on a packet, leave it as it is.</summary>
        public const int OmitNormalization = unchecked((int)(0x0020));

        /// <summary>Sets the options to the default values.</summary>
        public ParseOptions()
        {
            SetOption(FixControlChars | AcceptLatin1, true);
        }

        /// <returns>Returns the requireXMPMeta.</returns>
        public bool GetRequireXMPMeta()
        {
            return GetOption(RequireXmpMeta);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public ParseOptions SetRequireXMPMeta(bool value)
        {
            SetOption(RequireXmpMeta, value);
            return this;
        }

        /// <returns>Returns the strictAliasing.</returns>
        public bool GetStrictAliasing()
        {
            return GetOption(StrictAliasing);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public ParseOptions SetStrictAliasing(bool value)
        {
            SetOption(StrictAliasing, value);
            return this;
        }

        /// <returns>Returns the strictAliasing.</returns>
        public bool GetFixControlChars()
        {
            return GetOption(FixControlChars);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public ParseOptions SetFixControlChars(bool value)
        {
            SetOption(FixControlChars, value);
            return this;
        }

        /// <returns>Returns the strictAliasing.</returns>
        public bool GetAcceptLatin1()
        {
            return GetOption(AcceptLatin1);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public ParseOptions SetOmitNormalization(bool value)
        {
            SetOption(OmitNormalization, value);
            return this;
        }

        /// <returns>Returns the option "omit normalization".</returns>
        public bool GetOmitNormalization()
        {
            return GetOption(OmitNormalization);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public ParseOptions SetAcceptLatin1(bool value)
        {
            SetOption(AcceptLatin1, value);
            return this;
        }

        /// <seealso cref="Options.DefineOptionName(int)"/>
        protected internal override string DefineOptionName(int option)
        {
            switch (option)
            {
                case RequireXmpMeta:
                {
                    return "REQUIRE_XMP_META";
                }

                case StrictAliasing:
                {
                    return "STRICT_ALIASING";
                }

                case FixControlChars:
                {
                    return "FIX_CONTROL_CHARS";
                }

                case AcceptLatin1:
                {
                    return "ACCEPT_LATIN_1";
                }

                case OmitNormalization:
                {
                    return "OMIT_NORMALIZATION";
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
            return RequireXmpMeta | StrictAliasing | FixControlChars | AcceptLatin1 | OmitNormalization;
        }
    }
}

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
    /// <summary>Options for <code>XMPIterator</code> construction.</summary>
    /// <since>24.01.2006</since>
    public sealed class IteratorOptions : Options
    {
        /// <summary>Just do the immediate children of the root, default is subtree.</summary>
        public const int JustChildren = unchecked((int)(0x0100));

        /// <summary>Just do the leaf nodes, default is all nodes in the subtree.</summary>
        /// <remarks>
        /// Just do the leaf nodes, default is all nodes in the subtree.
        /// Bugfix #2658965: If this option is set the Iterator returns the namespace
        /// of the leaf instead of the namespace of the base property.
        /// </remarks>
        public const int JustLeafnodes = unchecked((int)(0x0200));

        /// <summary>Return just the leaf part of the path, default is the full path.</summary>
        public const int JustLeafname = unchecked((int)(0x0400));

        /// <summary>Omit all qualifiers.</summary>
        public const int OmitQualifiers = unchecked((int)(0x1000));

        //    /** Include aliases, default is just actual properties. <em>Note:</em> Not supported.
        //     *  @deprecated it is commonly preferred to work with the base properties */
        //    public static final int INCLUDE_ALIASES = 0x0800;
        /// <returns>Returns whether the option is set.</returns>
        public bool IsJustChildren()
        {
            return GetOption(JustChildren);
        }

        /// <returns>Returns whether the option is set.</returns>
        public bool IsJustLeafname()
        {
            return GetOption(JustLeafname);
        }

        /// <returns>Returns whether the option is set.</returns>
        public bool IsJustLeafnodes()
        {
            return GetOption(JustLeafnodes);
        }

        /// <returns>Returns whether the option is set.</returns>
        public bool IsOmitQualifiers()
        {
            return GetOption(OmitQualifiers);
        }

        /// <summary>Sets the option and returns the instance.</summary>
        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public IteratorOptions SetJustChildren(bool value)
        {
            SetOption(JustChildren, value);
            return this;
        }

        /// <summary>Sets the option and returns the instance.</summary>
        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public IteratorOptions SetJustLeafname(bool value)
        {
            SetOption(JustLeafname, value);
            return this;
        }

        /// <summary>Sets the option and returns the instance.</summary>
        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public IteratorOptions SetJustLeafnodes(bool value)
        {
            SetOption(JustLeafnodes, value);
            return this;
        }

        /// <summary>Sets the option and returns the instance.</summary>
        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public IteratorOptions SetOmitQualifiers(bool value)
        {
            SetOption(OmitQualifiers, value);
            return this;
        }

        /// <seealso cref="Options.DefineOptionName(int)"/>
        protected internal override string DefineOptionName(int option)
        {
            switch (option)
            {
                case JustChildren:
                {
                    return "JUST_CHILDREN";
                }

                case JustLeafnodes:
                {
                    return "JUST_LEAFNODES";
                }

                case JustLeafname:
                {
                    return "JUST_LEAFNAME";
                }

                case OmitQualifiers:
                {
                    return "OMIT_QUALIFIERS";
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
            return JustChildren | JustLeafnodes | JustLeafname | OmitQualifiers;
        }
    }
}

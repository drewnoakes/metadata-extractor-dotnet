// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

namespace XmpCore.Options
{
    /// <summary>Options for <c>XMPIterator</c> construction.</summary>
    /// <since>24.01.2006</since>
    public sealed class IteratorOptions : Options
    {
        /// <summary>Just do the immediate children of the root, default is subtree.</summary>
        public const int JustChildren = 0x0100;

        /// <summary>Just do the leaf nodes, default is all nodes in the subtree.</summary>
        /// <remarks>
        /// Just do the leaf nodes, default is all nodes in the subtree.
        /// Bugfix #2658965: If this option is set the Iterator returns the namespace
        /// of the leaf instead of the namespace of the base property.
        /// </remarks>
        public const int JustLeafNodes = 0x0200;

        /// <summary>Return just the leaf part of the path, default is the full path.</summary>
        public const int JustLeafName = 0x0400;

        /// <summary>Omit all qualifiers.</summary>
        public const int OmitQualifiers = 0x1000;

//      /** Include aliases, default is just actual properties. <em>Note:</em> Not supported.
//        *  @deprecated it is commonly preferred to work with the base properties */
//      public static final int INCLUDE_ALIASES = 0x0800;

        public bool IsJustChildren
        {
            get { return GetOption(JustChildren); }
            set { SetOption(JustChildren, value); }
        }

        public bool IsJustLeafName
        {
            get { return GetOption(JustLeafName); }
            set { SetOption(JustLeafName, value); }
        }

        public bool IsJustLeafNodes
        {
            get { return GetOption(JustLeafNodes); }
            set { SetOption(JustLeafNodes, value); }
        }

        public bool IsOmitQualifiers
        {
            get { return GetOption(OmitQualifiers); }
            set { SetOption(OmitQualifiers, value); }
        }

        protected override string DefineOptionName(int option)
        {
            switch (option)
            {
                case JustChildren:
                    return "JUST_CHILDREN";
                case JustLeafNodes:
                    return "JUST_LEAFNODES";
                case JustLeafName:
                    return "JUST_LEAFNAME";
                case OmitQualifiers:
                    return "OMIT_QUALIFIERS";
                default:
                    return null;
            }
        }

        protected override int GetValidOptions()
        {
            return JustChildren | JustLeafNodes | JustLeafName | OmitQualifiers;
        }
    }
}

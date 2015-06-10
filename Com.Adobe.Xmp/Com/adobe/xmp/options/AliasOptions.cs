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
    /// <summary>Options for XMPSchemaRegistryImpl#registerAlias.</summary>
    /// <since>20.02.2006</since>
    public sealed class AliasOptions : Options
    {
        /// <summary>This is a direct mapping.</summary>
        /// <remarks>This is a direct mapping. The actual data type does not matter.</remarks>
        public const int PropDirect = 0;

        /// <summary>The actual is an unordered array, the alias is to the first element of the array.</summary>
        public const int PropArray = PropertyOptions.ArrayFlag;

        /// <summary>The actual is an ordered array, the alias is to the first element of the array.</summary>
        public const int PropArrayOrdered = PropertyOptions.ArrayOrderedFlag;

        /// <summary>The actual is an alternate array, the alias is to the first element of the array.</summary>
        public const int PropArrayAlternate = PropertyOptions.ArrayAlternateFlag;

        /// <summary>The actual is an alternate text array, the alias is to the 'x-default' element of the array.</summary>
        public const int PropArrayAltText = PropertyOptions.ArrayAltTextFlag;

        public AliasOptions()
        {
        }

        /// <param name="options">the options to init with</param>
        /// <exception cref="XmpException">If options are not consistant</exception>
        public AliasOptions(int options)
            : base(options)
        {
        }

        /// <returns>Returns if the alias is of the simple form.</returns>
        public bool IsSimple()
        {
            return GetOptions() == PropDirect;
        }

        public bool IsArray
        {
            get { return GetOption(PropArray); }
            set { SetOption(PropArray, value); }
        }

        public bool IsArrayOrdered
        {
            get { return GetOption(PropArrayOrdered); }
            set { SetOption(PropArray | PropArrayOrdered, value); }
        }

        public bool IsArrayAlternate
        {
            get { return GetOption(PropArrayAlternate); }
            set { SetOption(PropArray | PropArrayOrdered | PropArrayAlternate, value); }
        }

        public bool IsArrayAltText
        {
            get { return GetOption(PropArrayAltText); }
            set { SetOption(PropArray | PropArrayOrdered | PropArrayAlternate | PropArrayAltText, value); }
        }

        /// <returns>
        /// Returns a <see cref="PropertyOptions"/> object
        /// </returns>
        /// <exception cref="XmpException">If the options are not consistant.</exception>
        public PropertyOptions ToPropertyOptions()
        {
            return new PropertyOptions(GetOptions());
        }

        protected override string DefineOptionName(int option)
        {
            switch (option)
            {
                case PropDirect:
                    return "PROP_DIRECT";
                case PropArray:
                    return "ARRAY";
                case PropArrayOrdered:
                    return "ARRAY_ORDERED";
                case PropArrayAlternate:
                    return "ARRAY_ALTERNATE";
                case PropArrayAltText:
                    return "ARRAY_ALT_TEXT";
                default:
                    return null;
            }
        }

        protected override int GetValidOptions()
        {
            return PropDirect | PropArray | PropArrayOrdered | PropArrayAlternate | PropArrayAltText;
        }
    }
}

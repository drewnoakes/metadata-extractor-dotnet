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
        public const int PropArray = PropertyOptions.Array;

        /// <summary>The actual is an ordered array, the alias is to the first element of the array.</summary>
        public const int PropArrayOrdered = PropertyOptions.ArrayOrdered;

        /// <summary>The actual is an alternate array, the alias is to the first element of the array.</summary>
        public const int PropArrayAlternate = PropertyOptions.ArrayAlternate;

        /// <summary>The actual is an alternate text array, the alias is to the 'x-default' element of the array.</summary>
        public const int PropArrayAltText = PropertyOptions.ArrayAltText;

        /// <seealso cref="Options.Options()"/>
        public AliasOptions()
        {
        }

        /// <param name="options">the options to init with</param>
        /// <exception cref="Com.Adobe.Xmp.XMPException">If options are not consistant</exception>
        public AliasOptions(int options)
            : base(options)
        {
        }

        // EMPTY
        /// <returns>Returns if the alias is of the simple form.</returns>
        public bool IsSimple()
        {
            return GetOptions() == PropDirect;
        }

        /// <returns>Returns the option.</returns>
        public bool IsArray()
        {
            return GetOption(PropArray);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public AliasOptions SetArray(bool value)
        {
            SetOption(PropArray, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool IsArrayOrdered()
        {
            return GetOption(PropArrayOrdered);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public AliasOptions SetArrayOrdered(bool value)
        {
            SetOption(PropArray | PropArrayOrdered, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool IsArrayAlternate()
        {
            return GetOption(PropArrayAlternate);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public AliasOptions SetArrayAlternate(bool value)
        {
            SetOption(PropArray | PropArrayOrdered | PropArrayAlternate, value);
            return this;
        }

        /// <returns>Returns the option.</returns>
        public bool IsArrayAltText()
        {
            return GetOption(PropArrayAltText);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns the instance to call more set-methods.</returns>
        public AliasOptions SetArrayAltText(bool value)
        {
            SetOption(PropArray | PropArrayOrdered | PropArrayAlternate | PropArrayAltText, value);
            return this;
        }

        /// <returns>
        /// returns a
        /// <see cref="PropertyOptions"/>
        /// s object
        /// </returns>
        /// <exception cref="Com.Adobe.Xmp.XMPException">If the options are not consistant.</exception>
        public PropertyOptions ToPropertyOptions()
        {
            return new PropertyOptions(GetOptions());
        }

        /// <seealso cref="Options.DefineOptionName(int)"/>
        protected internal override string DefineOptionName(int option)
        {
            switch (option)
            {
                case PropDirect:
                {
                    return "PROP_DIRECT";
                }

                case PropArray:
                {
                    return "ARRAY";
                }

                case PropArrayOrdered:
                {
                    return "ARRAY_ORDERED";
                }

                case PropArrayAlternate:
                {
                    return "ARRAY_ALTERNATE";
                }

                case PropArrayAltText:
                {
                    return "ARRAY_ALT_TEXT";
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
            return PropDirect | PropArray | PropArrayOrdered | PropArrayAlternate | PropArrayAltText;
        }
    }
}

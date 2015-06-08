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
    /// The property flags are used when properties are fetched from the <code>XMPMeta</code>-object
    /// and provide more detailed information about the property.
    /// </summary>
    /// <since>03.07.2006</since>
    public sealed class PropertyOptions : Options
    {
        public const int NoOptions = unchecked((int)(0x00000000));

        public const int Uri = unchecked((int)(0x00000002));

        public const int HasQualifiers = unchecked((int)(0x00000010));

        public const int Qualifier = unchecked((int)(0x00000020));

        public const int HasLanguage = unchecked((int)(0x00000040));

        public const int HasType = unchecked((int)(0x00000080));

        public const int Struct = unchecked((int)(0x00000100));

        public const int Array = unchecked((int)(0x00000200));

        public const int ArrayOrdered = unchecked((int)(0x00000400));

        public const int ArrayAlternate = unchecked((int)(0x00000800));

        public const int ArrayAltText = unchecked((int)(0x00001000));

        public const int SchemaNode = unchecked((int)(0x80000000));

        /// <summary>may be used in the future</summary>
        public const int DeleteExisting = unchecked((int)(0x20000000));

        /// <summary>Default constructor</summary>
        public PropertyOptions()
        {
        }

        /// <summary>Intialization constructor</summary>
        /// <param name="options">the initialization options</param>
        /// <exception cref="XmpException">If the options are not valid</exception>
        public PropertyOptions(int options)
            : base(options)
        {
        }

        // reveal default constructor
        /// <returns>
        /// Return whether the property value is a URI. It is serialized to RDF using the
        /// <tt>rdf:resource</tt> attribute. Not mandatory for URIs, but considered RDF-savvy.
        /// </returns>
        public bool IsUri()
        {
            return GetOption(Uri);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns this to enable cascaded options.</returns>
        public PropertyOptions SetUri(bool value)
        {
            SetOption(Uri, value);
            return this;
        }

        /// <returns>
        /// Return whether the property has qualifiers. These could be an <tt>xml:lang</tt>
        /// attribute, an <tt>rdf:type</tt> property, or a general qualifier. See the
        /// introductory discussion of qualified properties for more information.
        /// </returns>
        public bool GetHasQualifiers()
        {
            return GetOption(HasQualifiers);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns this to enable cascaded options.</returns>
        public PropertyOptions SetHasQualifiers(bool value)
        {
            SetOption(HasQualifiers, value);
            return this;
        }

        /// <returns>
        /// Return whether this property is a qualifier for some other property. Note that if the
        /// qualifier itself has a structured value, this flag is only set for the top node of
        /// the qualifier's subtree. Qualifiers may have arbitrary structure, and may even have
        /// qualifiers.
        /// </returns>
        public bool IsQualifier()
        {
            return GetOption(Qualifier);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns this to enable cascaded options.</returns>
        public PropertyOptions SetQualifier(bool value)
        {
            SetOption(Qualifier, value);
            return this;
        }

        /// <returns>Return whether this property has an <tt>xml:lang</tt> qualifier.</returns>
        public bool GetHasLanguage()
        {
            return GetOption(HasLanguage);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns this to enable cascaded options.</returns>
        public PropertyOptions SetHasLanguage(bool value)
        {
            SetOption(HasLanguage, value);
            return this;
        }

        /// <returns>Return whether this property has an <tt>rdf:type</tt> qualifier.</returns>
        public bool GetHasType()
        {
            return GetOption(HasType);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns this to enable cascaded options.</returns>
        public PropertyOptions SetHasType(bool value)
        {
            SetOption(HasType, value);
            return this;
        }

        /// <returns>Return whether this property contains nested fields.</returns>
        public bool IsStruct()
        {
            return GetOption(Struct);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns this to enable cascaded options.</returns>
        public PropertyOptions SetStruct(bool value)
        {
            SetOption(Struct, value);
            return this;
        }

        /// <returns>
        /// Return whether this property is an array. By itself this indicates a general
        /// unordered array. It is serialized using an <tt>rdf:Bag</tt> container.
        /// </returns>
        public bool IsArray()
        {
            return GetOption(Array);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns this to enable cascaded options.</returns>
        public PropertyOptions SetArray(bool value)
        {
            SetOption(Array, value);
            return this;
        }

        /// <returns>
        /// Return whether this property is an ordered array. Appears in conjunction with
        /// getPropValueIsArray(). It is serialized using an <tt>rdf:Seq</tt> container.
        /// </returns>
        public bool IsArrayOrdered()
        {
            return GetOption(ArrayOrdered);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns this to enable cascaded options.</returns>
        public PropertyOptions SetArrayOrdered(bool value)
        {
            SetOption(ArrayOrdered, value);
            return this;
        }

        /// <returns>
        /// Return whether this property is an alternative array. Appears in conjunction with
        /// getPropValueIsArray(). It is serialized using an <tt>rdf:Alt</tt> container.
        /// </returns>
        public bool IsArrayAlternate()
        {
            return GetOption(ArrayAlternate);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns this to enable cascaded options.</returns>
        public PropertyOptions SetArrayAlternate(bool value)
        {
            SetOption(ArrayAlternate, value);
            return this;
        }

        /// <returns>
        /// Return whether this property is an alt-text array. Appears in conjunction with
        /// getPropArrayIsAlternate(). It is serialized using an <tt>rdf:Alt</tt> container.
        /// Each array element is a simple property with an <tt>xml:lang</tt> attribute.
        /// </returns>
        public bool IsArrayAltText()
        {
            return GetOption(ArrayAltText);
        }

        /// <param name="value">the value to set</param>
        /// <returns>Returns this to enable cascaded options.</returns>
        public PropertyOptions SetArrayAltText(bool value)
        {
            SetOption(ArrayAltText, value);
            return this;
        }

        /// <returns>Returns whether the SCHEMA_NODE option is set.</returns>
        public bool IsSchemaNode()
        {
            return GetOption(SchemaNode);
        }

        /// <param name="value">the option DELETE_EXISTING to set</param>
        /// <returns>Returns this to enable cascaded options.</returns>
        public PropertyOptions SetSchemaNode(bool value)
        {
            SetOption(SchemaNode, value);
            return this;
        }

        //-------------------------------------------------------------------------- convenience methods
        /// <returns>Returns whether the property is of composite type - an array or a struct.</returns>
        public bool IsCompositeProperty()
        {
            return (GetOptions() & (Array | Struct)) > 0;
        }

        /// <returns>Returns whether the property is of composite type - an array or a struct.</returns>
        public bool IsSimple()
        {
            return (GetOptions() & (Array | Struct)) == 0;
        }

        /// <summary>Compares two options set for array compatibility.</summary>
        /// <param name="options">other options</param>
        /// <returns>Returns true if the array options of the sets are equal.</returns>
        public bool EqualArrayTypes(PropertyOptions options)
        {
            return IsArray() == options.IsArray() && IsArrayOrdered() == options.IsArrayOrdered() && IsArrayAlternate() == options.IsArrayAlternate() && IsArrayAltText() == options.IsArrayAltText();
        }

        /// <summary>Merges the set options of a another options object with this.</summary>
        /// <remarks>
        /// Merges the set options of a another options object with this.
        /// If the other options set is null, this objects stays the same.
        /// </remarks>
        /// <param name="options">other options</param>
        /// <exception cref="XmpException">If illegal options are provided</exception>
        public void MergeWith(PropertyOptions options)
        {
            if (options != null)
            {
                SetOptions(GetOptions() | options.GetOptions());
            }
        }

        /// <returns>Returns true if only array options are set.</returns>
        public bool IsOnlyArrayOptions()
        {
            return (GetOptions() & ~(Array | ArrayOrdered | ArrayAlternate | ArrayAltText)) == 0;
        }

        /// <seealso cref="Options.GetValidOptions()"/>
        protected internal override int GetValidOptions()
        {
            return Uri | HasQualifiers | Qualifier | HasLanguage | HasType | Struct | Array | ArrayOrdered | ArrayAlternate | ArrayAltText | SchemaNode;
        }

        /// <seealso cref="Options.DefineOptionName(int)"/>
        protected internal override string DefineOptionName(int option)
        {
            switch (option)
            {
                case Uri:
                {
                    return "URI";
                }

                case HasQualifiers:
                {
                    return "HAS_QUALIFIER";
                }

                case Qualifier:
                {
                    return "QUALIFIER";
                }

                case HasLanguage:
                {
                    return "HAS_LANGUAGE";
                }

                case HasType:
                {
                    return "HAS_TYPE";
                }

                case Struct:
                {
                    return "STRUCT";
                }

                case Array:
                {
                    return "ARRAY";
                }

                case ArrayOrdered:
                {
                    return "ARRAY_ORDERED";
                }

                case ArrayAlternate:
                {
                    return "ARRAY_ALTERNATE";
                }

                case ArrayAltText:
                {
                    return "ARRAY_ALT_TEXT";
                }

                case SchemaNode:
                {
                    return "SCHEMA_NODE";
                }

                default:
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Checks that a node not a struct and array at the same time;
        /// and URI cannot be a struct.
        /// </summary>
        /// <param name="options">the bitmask to check.</param>
        /// <exception cref="XmpException">Thrown if the options are not consistent.</exception>
        protected internal override void AssertConsistency(int options)
        {
            if ((options & Struct) > 0 && (options & Array) > 0)
            {
                throw new XmpException("IsStruct and IsArray options are mutually exclusive", XmpErrorCode.Badoptions);
            }
            if ((options & Uri) > 0 && (options & (Array | Struct)) > 0)
            {
                throw new XmpException("Structs and arrays can't have \"value\" options", XmpErrorCode.Badoptions);
            }
        }
    }
}

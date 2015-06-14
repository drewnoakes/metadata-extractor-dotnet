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
    /// The property flags are used when properties are fetched from the <c>XMPMeta</c>-object
    /// and provide more detailed information about the property.
    /// </summary>
    /// <since>03.07.2006</since>
    public sealed class PropertyOptions : Options
    {
        internal const int IsUriFlag = 0x00000002;
        internal const int HasQualifiersFlag = 0x00000010;
        internal const int QualifierFlag = 0x00000020;
        internal const int HasLanguageFlag = 0x00000040;
        internal const int HasTypeFlag = 0x00000080;
        internal const int StructFlag = 0x00000100;
        internal const int ArrayFlag = 0x00000200;
        internal const int ArrayOrderedFlag = 0x00000400;
        internal const int ArrayAlternateFlag = 0x00000800;
        internal const int ArrayAltTextFlag = 0x00001000;
        internal const int SchemaNodeFlag = unchecked((int)(0x80000000));

        /// <summary>may be used in the future</summary>
        internal const int DeleteExisting = 0x20000000;

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

        /// <summary>
        /// Get and set whether the property value is a URI. It is serialized to RDF using the
        /// <c>rdf:resource</c> attribute. Not mandatory for URIs, but considered RDF-savvy.
        /// </summary>
        public bool IsUri
        {
            get { return GetOption(IsUriFlag); }
            set { SetOption(IsUriFlag, value); }
        }

        /// <value>
        ///   Return whether the property has qualifiers. These could be an <tt>xml:lang</tt>
        ///   attribute, an <tt>rdf:type</tt> property, or a general qualifier. See the
        ///   introductory discussion of qualified properties for more information.
        /// </value>
        public bool HasQualifiers
        {
            get { return GetOption(HasQualifiersFlag); }
            set { SetOption(HasQualifiersFlag, value); }
        }

        /// <value>
        ///   Return whether this property is a qualifier for some other property. Note that if the
        ///   qualifier itself has a structured value, this flag is only set for the top node of
        ///   the qualifier's subtree. Qualifiers may have arbitrary structure, and may even have
        ///   qualifiers.
        /// </value>
        public bool IsQualifier
        {
            get { return GetOption(QualifierFlag); }
            set { SetOption(QualifierFlag, value); }
        }

        /// <value>Return whether this property has an <tt>xml:lang</tt> qualifier.</value>
        public bool HasLanguage
        {
            get { return GetOption(HasLanguageFlag); }
            set { SetOption(HasLanguageFlag, value); }
        }

        /// <value>Return whether this property has an <tt>rdf:type</tt> qualifier.</value>
        public bool HasType
        {
            get { return GetOption(HasTypeFlag); }
            set { SetOption(HasTypeFlag, value); }
        }

        /// <value>Return whether this property contains nested fields.</value>
        public bool IsStruct
        {
            get { return GetOption(StructFlag); }
            set { SetOption(StructFlag, value); }
        }

        /// <value>
        ///   Return whether this property is an array. By itself this indicates a general
        ///   unordered array. It is serialized using an <tt>rdf:Bag</tt> container.
        /// </value>
        public bool IsArray
        {
            get { return GetOption(ArrayFlag); }
            set { SetOption(ArrayFlag, value); }
        }

        /// <value>
        ///   Return whether this property is an ordered array. Appears in conjunction with
        ///   getPropValueIsArray(). It is serialized using an <tt>rdf:Seq</tt> container.
        /// </value>
        public bool IsArrayOrdered
        {
            get { return GetOption(ArrayOrderedFlag); }
            set { SetOption(ArrayOrderedFlag, value); }
        }

        /// <value>
        ///   Return whether this property is an alternative array. Appears in conjunction with
        ///   getPropValueIsArray(). It is serialized using an <tt>rdf:Alt</tt> container.
        /// </value>
        public bool IsArrayAlternate
        {
            get { return GetOption(ArrayAlternateFlag); }
            set { SetOption(ArrayAlternateFlag, value); }
        }

        /// <value>
        ///   Return whether this property is an alt-text array. Appears in conjunction with
        ///   getPropArrayIsAlternate(). It is serialized using an <tt>rdf:Alt</tt> container.
        ///   Each array element is a simple property with an <tt>xml:lang</tt> attribute.
        /// </value>
        public bool IsArrayAltText
        {
            get { return GetOption(ArrayAltTextFlag); }
            set { SetOption(ArrayAltTextFlag, value); }
        }

        /// <value>Returns whether the SCHEMA_NODE option is set.</value>
        public bool IsSchemaNode
        {
            get { return GetOption(SchemaNodeFlag); }
            set { SetOption(SchemaNodeFlag, value); }
        }

        /// <value>Returns whether the property is of composite type - an array or a struct.</value>
        public bool IsCompositeProperty
        {
            get { return (GetOptions() & (ArrayFlag | StructFlag)) > 0; }
        }

        /// <value>Returns whether the property is of composite type - an array or a struct.</value>
        public bool IsSimple
        {
            get { return !IsCompositeProperty; }
        }

        /// <summary>Compares two options set for array compatibility.</summary>
        /// <param name="options">other options</param>
        /// <returns>Returns true if the array options of the sets are equal.</returns>
        public bool EqualArrayTypes(PropertyOptions options)
        {
            return IsArray == options.IsArray && IsArrayOrdered == options.IsArrayOrdered && IsArrayAlternate == options.IsArrayAlternate && IsArrayAltText == options.IsArrayAltText;
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

        /// <value>Returns true if only array options are set.</value>
        public bool IsOnlyArrayOptions
        {
            get { return (GetOptions() & ~(ArrayFlag | ArrayOrderedFlag | ArrayAlternateFlag | ArrayAltTextFlag)) == 0; }
        }

        protected override int GetValidOptions()
        {
            return IsUriFlag | HasQualifiersFlag | QualifierFlag | HasLanguageFlag | HasTypeFlag | StructFlag | ArrayFlag | ArrayOrderedFlag | ArrayAlternateFlag | ArrayAltTextFlag | SchemaNodeFlag;
        }

        protected override string DefineOptionName(int option)
        {
            switch (option)
            {
                case IsUriFlag:
                    return "URI";
                case HasQualifiersFlag:
                    return "HAS_QUALIFIER";
                case QualifierFlag:
                    return "QUALIFIER";
                case HasLanguageFlag:
                    return "HAS_LANGUAGE";
                case HasTypeFlag:
                    return "HAS_TYPE";
                case StructFlag:
                    return "STRUCT";
                case ArrayFlag:
                    return "ARRAY";
                case ArrayOrderedFlag:
                    return "ARRAY_ORDERED";
                case ArrayAlternateFlag:
                    return "ARRAY_ALTERNATE";
                case ArrayAltTextFlag:
                    return "ARRAY_ALT_TEXT";
                case SchemaNodeFlag:
                    return "SCHEMA_NODE";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Checks that a node not a struct and array at the same time;
        /// and URI cannot be a struct.
        /// </summary>
        /// <param name="options">the bitmask to check.</param>
        /// <exception cref="XmpException">Thrown if the options are not consistent.</exception>
        internal override void AssertConsistency(int options)
        {
            if ((options & StructFlag) > 0 && (options & ArrayFlag) > 0)
            {
                throw new XmpException("IsStruct and IsArray options are mutually exclusive", XmpErrorCode.BadOptions);
            }
            if ((options & IsUriFlag) > 0 && (options & (ArrayFlag | StructFlag)) > 0)
            {
                throw new XmpException("Structs and arrays can't have \"value\" options", XmpErrorCode.BadOptions);
            }
        }
    }
}

// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using System.Collections;
using System.Text.RegularExpressions;
using Com.Adobe.Xmp.Options;
using Com.Adobe.Xmp.Properties;
using Sharpen;

namespace Com.Adobe.Xmp.Impl
{
    /// <summary>The schema registry handles the namespaces, aliases and global options for the XMP Toolkit.</summary>
    /// <remarks>
    /// The schema registry handles the namespaces, aliases and global options for the XMP Toolkit. There
    /// is only one single instance used by the toolkit.
    /// </remarks>
    /// <since>27.01.2006</since>
    public sealed class XmpSchemaRegistry : IXmpSchemaRegistry
    {
        /// <summary>a map from a namespace URI to its registered prefix</summary>
        private readonly IDictionary _namespaceToPrefixMap = new Hashtable();

        /// <summary>a map from a prefix to the associated namespace URI</summary>
        private readonly IDictionary _prefixToNamespaceMap = new Hashtable();

        /// <summary>a map of all registered aliases.</summary>
        /// <remarks>
        /// a map of all registered aliases.
        /// The map is a relationship from a qname to an <code>XMPAliasInfo</code>-object.
        /// </remarks>
        private readonly IDictionary _aliasMap = new Hashtable();

        /// <summary>The pattern that must not be contained in simple properties</summary>
        private readonly Regex _p = new Regex ("[/*?\\[\\]]", RegexOptions.Compiled);

        /// <summary>
        /// Performs the initialisation of the registry with the default namespaces, aliases and global
        /// options.
        /// </summary>
        public XmpSchemaRegistry()
        {
            try
            {
                RegisterStandardNamespaces();
                RegisterStandardAliases();
            }
            catch (XmpException)
            {
                throw new RuntimeException("The XMPSchemaRegistry cannot be initialized!");
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Namespace Functions
        /// <seealso cref="IXmpSchemaRegistry.RegisterNamespace(string, string)"/>
        /// <exception cref="XmpException"/>
        public string RegisterNamespace(string namespaceUri, string suggestedPrefix)
        {
            lock (this)
            {
                ParameterAsserts.AssertSchemaNs(namespaceUri);
                ParameterAsserts.AssertPrefix(suggestedPrefix);
                if (suggestedPrefix[suggestedPrefix.Length - 1] != ':')
                {
                    suggestedPrefix += ':';
                }
                if (!Utils.IsXmlNameNs(Runtime.Substring(suggestedPrefix, 0, suggestedPrefix.Length - 1)))
                {
                    throw new XmpException("The prefix is a bad XML name", XmpErrorConstants.Badxml);
                }
                string registeredPrefix = (string)_namespaceToPrefixMap.Get(namespaceUri);
                string registeredNs = (string)_prefixToNamespaceMap.Get(suggestedPrefix);
                if (registeredPrefix != null)
                {
                    // Return the actual prefix
                    return registeredPrefix;
                }
                else
                {
                    if (registeredNs != null)
                    {
                        // the namespace is new, but the prefix is already engaged,
                        // we generate a new prefix out of the suggested
                        string generatedPrefix = suggestedPrefix;
                        for (int i = 1; _prefixToNamespaceMap.ContainsKey(generatedPrefix); i++)
                        {
                            generatedPrefix = Runtime.Substring(suggestedPrefix, 0, suggestedPrefix.Length - 1) + "_" + i + "_:";
                        }
                        suggestedPrefix = generatedPrefix;
                    }
                    _prefixToNamespaceMap.Put(suggestedPrefix, namespaceUri);
                    _namespaceToPrefixMap.Put(namespaceUri, suggestedPrefix);
                    // Return the suggested prefix
                    return suggestedPrefix;
                }
            }
        }

        /// <seealso cref="IXmpSchemaRegistry.DeleteNamespace(string)"/>
        public void DeleteNamespace(string namespaceUri)
        {
            lock (this)
            {
                string prefixToDelete = GetNamespacePrefix(namespaceUri);
                if (prefixToDelete != null)
                {
                    Collections.Remove(_namespaceToPrefixMap, namespaceUri);
                    Collections.Remove(_prefixToNamespaceMap, prefixToDelete);
                }
            }
        }

        /// <seealso cref="IXmpSchemaRegistry.GetNamespacePrefix(string)"/>
        public string GetNamespacePrefix(string namespaceUri)
        {
            lock (this)
            {
                return (string)_namespaceToPrefixMap.Get(namespaceUri);
            }
        }

        /// <sIXmpSchemaRegistry.GetNamespaceUriceURI(string)"/>
        public string GetNamespaceUri(string namespacePrefix)
        {
            lock (this)
            {
                if (namespacePrefix != null && !namespacePrefix.EndsWith(":"))
                {
                    namespacePrefix += ":";
                }
                return (string)_prefixToNamespaceMap.Get(namespacePrefix);
            }
        }

        /// <seealso cref="IXmpSchemaRegistry.GetNamespaces()"/>
        public IDictionary GetNamespaces()
        {
            lock (this)
            {
                return Collections.UnmodifiableMap(new SortedList(_namespaceToPrefixMap));
            }
        }

        /// <seealso cref="IXmpSchemaRegistry.GetPrefixes()"/>
        public IDictionary GetPrefixes()
        {
            lock (this)
            {
                return Collections.UnmodifiableMap(new SortedList(_prefixToNamespaceMap));
            }
        }

        /// <summary>
        /// Register the standard namespaces of schemas and types that are included in the XMP
        /// Specification and some other Adobe private namespaces.
        /// </summary>
        /// <remarks>
        /// Register the standard namespaces of schemas and types that are included in the XMP
        /// Specification and some other Adobe private namespaces.
        /// Note: This method is not lock because only called by the constructor.
        /// </remarks>
        /// <exception cref="XmpException">Forwards processing exceptions</exception>
        private void RegisterStandardNamespaces()
        {
            // register standard namespaces
            RegisterNamespace(XmpConstConstants.NsXml, "xml");
            RegisterNamespace(XmpConstConstants.NsRdf, "rdf");
            RegisterNamespace(XmpConstConstants.NsDc, "dc");
            RegisterNamespace(XmpConstConstants.NsIptccore, "Iptc4xmpCore");
            RegisterNamespace(XmpConstConstants.NsIptcext, "Iptc4xmpExt");
            RegisterNamespace(XmpConstConstants.NsDicom, "DICOM");
            RegisterNamespace(XmpConstConstants.NsPlus, "plus");
            // register Adobe standard namespaces
            RegisterNamespace(XmpConstConstants.NsX, "x");
            RegisterNamespace(XmpConstConstants.NsIx, "iX");
            RegisterNamespace(XmpConstConstants.NsXmp, "xmp");
            RegisterNamespace(XmpConstConstants.NsXmpRights, "xmpRights");
            RegisterNamespace(XmpConstConstants.NsXmpMm, "xmpMM");
            RegisterNamespace(XmpConstConstants.NsXmpBj, "xmpBJ");
            RegisterNamespace(XmpConstConstants.NsXmpNote, "xmpNote");
            RegisterNamespace(XmpConstConstants.NsPdf, "pdf");
            RegisterNamespace(XmpConstConstants.NsPdfx, "pdfx");
            RegisterNamespace(XmpConstConstants.NsPdfxId, "pdfxid");
            RegisterNamespace(XmpConstConstants.NsPdfaSchema, "pdfaSchema");
            RegisterNamespace(XmpConstConstants.NsPdfaProperty, "pdfaProperty");
            RegisterNamespace(XmpConstConstants.NsPdfaType, "pdfaType");
            RegisterNamespace(XmpConstConstants.NsPdfaField, "pdfaField");
            RegisterNamespace(XmpConstConstants.NsPdfaId, "pdfaid");
            RegisterNamespace(XmpConstConstants.NsPdfaExtension, "pdfaExtension");
            RegisterNamespace(XmpConstConstants.NsPhotoshop, "photoshop");
            RegisterNamespace(XmpConstConstants.NsPsalbum, "album");
            RegisterNamespace(XmpConstConstants.NsExif, "exif");
            RegisterNamespace(XmpConstConstants.NsExifx, "exifEX");
            RegisterNamespace(XmpConstConstants.NsExifAux, "aux");
            RegisterNamespace(XmpConstConstants.NsTiff, "tiff");
            RegisterNamespace(XmpConstConstants.NsPng, "png");
            RegisterNamespace(XmpConstConstants.NsJpeg, "jpeg");
            RegisterNamespace(XmpConstConstants.NsJp2K, "jp2k");
            RegisterNamespace(XmpConstConstants.NsCameraraw, "crs");
            RegisterNamespace(XmpConstConstants.NsAdobestockphoto, "bmsp");
            RegisterNamespace(XmpConstConstants.NsCreatorAtom, "creatorAtom");
            RegisterNamespace(XmpConstConstants.NsAsf, "asf");
            RegisterNamespace(XmpConstConstants.NsWav, "wav");
            RegisterNamespace(XmpConstConstants.NsBwf, "bext");
            RegisterNamespace(XmpConstConstants.NsRiffinfo, "riffinfo");
            RegisterNamespace(XmpConstConstants.NsScript, "xmpScript");
            RegisterNamespace(XmpConstConstants.NsTxmp, "txmp");
            RegisterNamespace(XmpConstConstants.NsSwf, "swf");
            // register Adobe private namespaces
            RegisterNamespace(XmpConstConstants.NsDm, "xmpDM");
            RegisterNamespace(XmpConstConstants.NsTransient, "xmpx");
            // register Adobe standard type namespaces
            RegisterNamespace(XmpConstConstants.TypeText, "xmpT");
            RegisterNamespace(XmpConstConstants.TypePagedfile, "xmpTPg");
            RegisterNamespace(XmpConstConstants.TypeGraphics, "xmpG");
            RegisterNamespace(XmpConstConstants.TypeImage, "xmpGImg");
            RegisterNamespace(XmpConstConstants.TypeFont, "stFnt");
            RegisterNamespace(XmpConstConstants.TypeDimensions, "stDim");
            RegisterNamespace(XmpConstConstants.TypeResourceevent, "stEvt");
            RegisterNamespace(XmpConstConstants.TypeResourceref, "stRef");
            RegisterNamespace(XmpConstConstants.TypeStVersion, "stVer");
            RegisterNamespace(XmpConstConstants.TypeStJob, "stJob");
            RegisterNamespace(XmpConstConstants.TypeManifestitem, "stMfs");
            RegisterNamespace(XmpConstConstants.TypeIdentifierqual, "xmpidq");
        }

        // ---------------------------------------------------------------------------------------------
        // Alias Functions
        /// <seealso cref="IXmpSchemaRegistry.ResolveAlias(string, string)"/>
        public IXmpAliasInfo ResolveAlias(string aliasNs, string aliasProp)
        {
            lock (this)
            {
                string aliasPrefix = GetNamespacePrefix(aliasNs);
                if (aliasPrefix == null)
                {
                    return null;
                }
                return (IXmpAliasInfo)_aliasMap.Get(aliasPrefix + aliasProp);
            }
        }

        /// <seealso cref="IXmpSchemaRegistry.FindAlias(string)"/>
        public IXmpAliasInfo FindAlias(string qname)
        {
            lock (this)
            {
                return (IXmpAliasInfo)_aliasMap.Get(qname);
            }
        }

        /// <seealso cref="IXmpSchemaRegistry.FindAliases(string)"/>
        public IXmpAliasInfo[] FindAliases(string aliasNs)
        {
            lock (this)
            {
                string prefix = GetNamespacePrefix(aliasNs);
                IList result = new ArrayList();
                if (prefix != null)
                {
                    for (IIterator it = _aliasMap.Keys.Iterator(); it.HasNext(); )
                    {
                        string qname = (string)it.Next();
                        if (qname.StartsWith(prefix))
                        {
                            result.Add(FindAlias(qname));
                        }
                    }
                }
                return (IXmpAliasInfo[])Collections.ToArray(result, new IXmpAliasInfo[result.Count]);
            }
        }

        /// <summary>Associates an alias name with an actual name.</summary>
        /// <remarks>
        /// Associates an alias name with an actual name.
        /// <p>
        /// Define a alias mapping from one namespace/property to another. Both
        /// property names must be simple names. An alias can be a direct mapping,
        /// where the alias and actual have the same data type. It is also possible
        /// to map a simple alias to an item in an array. This can either be to the
        /// first item in the array, or to the 'x-default' item in an alt-text array.
        /// Multiple alias names may map to the same actual, as long as the forms
        /// match. It is a no-op to reregister an alias in an identical fashion.
        /// Note: This method is not locking because only called by registerStandardAliases
        /// which is only called by the constructor.
        /// Note2: The method is only package-private so that it can be tested with unittests
        /// </remarks>
        /// <param name="aliasNs">
        /// The namespace URI for the alias. Must not be null or the empty
        /// string.
        /// </param>
        /// <param name="aliasProp">
        /// The name of the alias. Must be a simple name, not null or the
        /// empty string and not a general path expression.
        /// </param>
        /// <param name="actualNs">
        /// The namespace URI for the actual. Must not be null or the
        /// empty string.
        /// </param>
        /// <param name="actualProp">
        /// The name of the actual. Must be a simple name, not null or the
        /// empty string and not a general path expression.
        /// </param>
        /// <param name="aliasForm">
        /// Provides options for aliases for simple aliases to array
        /// items. This is needed to know what kind of array to create if
        /// set for the first time via the simple alias. Pass
        /// <code>XMP_NoOptions</code>, the default value, for all
        /// direct aliases regardless of whether the actual data type is
        /// an array or not (see
        /// <see cref="Com.Adobe.Xmp.Options.AliasOptions"/>
        /// ).
        /// </param>
        /// <exception cref="XmpException">for inconsistant aliases.</exception>
        internal void RegisterAlias(string aliasNs, string aliasProp, string actualNs, string actualProp, AliasOptions aliasForm)
        {
            lock (this)
            {
                ParameterAsserts.AssertSchemaNs(aliasNs);
                ParameterAsserts.AssertPropName(aliasProp);
                ParameterAsserts.AssertSchemaNs(actualNs);
                ParameterAsserts.AssertPropName(actualProp);
                // Fix the alias options
                AliasOptions aliasOpts = aliasForm != null ? new AliasOptions(XmpNodeUtils.VerifySetOptions(aliasForm.ToPropertyOptions(), null).GetOptions()) : new AliasOptions();
                if (new Matcher(_p, aliasProp).Find() || new Matcher(_p, actualProp).Find())
                {
                    throw new XmpException("Alias and actual property names must be simple", XmpErrorConstants.Badxpath);
                }
                // check if both namespaces are registered
                string aliasPrefix = GetNamespacePrefix(aliasNs);
                string actualPrefix = GetNamespacePrefix(actualNs);
                if (aliasPrefix == null)
                {
                    throw new XmpException("Alias namespace is not registered", XmpErrorConstants.Badschema);
                }
                else
                {
                    if (actualPrefix == null)
                    {
                        throw new XmpException("Actual namespace is not registered", XmpErrorConstants.Badschema);
                    }
                }
                string key = aliasPrefix + aliasProp;
                // check if alias is already existing
                if (_aliasMap.ContainsKey(key))
                {
                    throw new XmpException("Alias is already existing", XmpErrorConstants.Badparam);
                }
                else
                {
                    if (_aliasMap.ContainsKey(actualPrefix + actualProp))
                    {
                        throw new XmpException("Actual property is already an alias, use the base property", XmpErrorConstants.Badparam);
                    }
                }
                IXmpAliasInfo aliasInfo = new XmpAliasInfo390(actualNs, actualPrefix, actualProp, aliasOpts);
                _aliasMap.Put(key, aliasInfo);
            }
        }

        private sealed class XmpAliasInfo390 : IXmpAliasInfo
        {
            public XmpAliasInfo390(string actualNs, string actualPrefix, string actualProp, AliasOptions aliasOpts)
            {
                this._actualNs = actualNs;
                this._actualPrefix = actualPrefix;
                this._actualProp = actualProp;
                this._aliasOpts = aliasOpts;
            }

            /// <seealso cref="IXmpAliasInfo.GetNamespace()"/>
            public string GetNamespace()
            {
                return _actualNs;
            }

            /// <seealso cref="IXmpAliasInfo.GetPrefix()"/>
            public string GetPrefix()
            {
                return _actualPrefix;
            }

            /// <seealso cref="IXmpAliasInfo.GetPropName()"/>
            public string GetPropName()
            {
                return _actualProp;
            }

            /// <seealso cref="IXmpAliasInfo.GetAliasForm()"/>
            public AliasOptions GetAliasForm()
            {
                return _aliasOpts;
            }

            public override string ToString()
            {
                return _actualPrefix + _actualProp + " NS(" + _actualNs + "), FORM (" + this.GetAliasForm() + ")";
            }

            private readonly string _actualNs;

            private readonly string _actualPrefix;

            private readonly string _actualProp;

            private readonly AliasOptions _aliasOpts;
        }

        /// <seealso cref="IXmpSchemaRegistry.GetAliases()"/>
        public IDictionary GetAliases()
        {
            lock (this)
            {
                return Collections.UnmodifiableMap(new SortedList(_aliasMap));
            }
        }

        /// <summary>Register the standard aliases.</summary>
        /// <remarks>
        /// Register the standard aliases.
        /// Note: This method is not lock because only called by the constructor.
        /// </remarks>
        /// <exception cref="XmpException">If the registrations of at least one alias fails.</exception>
        private void RegisterStandardAliases()
        {
            AliasOptions aliasToArrayOrdered = new AliasOptions().SetArrayOrdered(true);
            AliasOptions aliasToArrayAltText = new AliasOptions().SetArrayAltText(true);
            // Aliases from XMP to DC.
            RegisterAlias(XmpConstConstants.NsXmp, "Author", XmpConstConstants.NsDc, "creator", aliasToArrayOrdered);
            RegisterAlias(XmpConstConstants.NsXmp, "Authors", XmpConstConstants.NsDc, "creator", null);
            RegisterAlias(XmpConstConstants.NsXmp, "Description", XmpConstConstants.NsDc, "description", null);
            RegisterAlias(XmpConstConstants.NsXmp, "Format", XmpConstConstants.NsDc, "format", null);
            RegisterAlias(XmpConstConstants.NsXmp, "Keywords", XmpConstConstants.NsDc, "subject", null);
            RegisterAlias(XmpConstConstants.NsXmp, "Locale", XmpConstConstants.NsDc, "language", null);
            RegisterAlias(XmpConstConstants.NsXmp, "Title", XmpConstConstants.NsDc, "title", null);
            RegisterAlias(XmpConstConstants.NsXmpRights, "Copyright", XmpConstConstants.NsDc, "rights", null);
            // Aliases from PDF to DC and XMP.
            RegisterAlias(XmpConstConstants.NsPdf, "Author", XmpConstConstants.NsDc, "creator", aliasToArrayOrdered);
            RegisterAlias(XmpConstConstants.NsPdf, "BaseURL", XmpConstConstants.NsXmp, "BaseURL", null);
            RegisterAlias(XmpConstConstants.NsPdf, "CreationDate", XmpConstConstants.NsXmp, "CreateDate", null);
            RegisterAlias(XmpConstConstants.NsPdf, "Creator", XmpConstConstants.NsXmp, "CreatorTool", null);
            RegisterAlias(XmpConstConstants.NsPdf, "ModDate", XmpConstConstants.NsXmp, "ModifyDate", null);
            RegisterAlias(XmpConstConstants.NsPdf, "Subject", XmpConstConstants.NsDc, "description", aliasToArrayAltText);
            RegisterAlias(XmpConstConstants.NsPdf, "Title", XmpConstConstants.NsDc, "title", aliasToArrayAltText);
            // Aliases from PHOTOSHOP to DC and XMP.
            RegisterAlias(XmpConstConstants.NsPhotoshop, "Author", XmpConstConstants.NsDc, "creator", aliasToArrayOrdered);
            RegisterAlias(XmpConstConstants.NsPhotoshop, "Caption", XmpConstConstants.NsDc, "description", aliasToArrayAltText);
            RegisterAlias(XmpConstConstants.NsPhotoshop, "Copyright", XmpConstConstants.NsDc, "rights", aliasToArrayAltText);
            RegisterAlias(XmpConstConstants.NsPhotoshop, "Keywords", XmpConstConstants.NsDc, "subject", null);
            RegisterAlias(XmpConstConstants.NsPhotoshop, "Marked", XmpConstConstants.NsXmpRights, "Marked", null);
            RegisterAlias(XmpConstConstants.NsPhotoshop, "Title", XmpConstConstants.NsDc, "title", aliasToArrayAltText);
            RegisterAlias(XmpConstConstants.NsPhotoshop, "WebStatement", XmpConstConstants.NsXmpRights, "WebStatement", null);
            // Aliases from TIFF and EXIF to DC and XMP.
            RegisterAlias(XmpConstConstants.NsTiff, "Artist", XmpConstConstants.NsDc, "creator", aliasToArrayOrdered);
            RegisterAlias(XmpConstConstants.NsTiff, "Copyright", XmpConstConstants.NsDc, "rights", null);
            RegisterAlias(XmpConstConstants.NsTiff, "DateTime", XmpConstConstants.NsXmp, "ModifyDate", null);
            RegisterAlias(XmpConstConstants.NsTiff, "ImageDescription", XmpConstConstants.NsDc, "description", null);
            RegisterAlias(XmpConstConstants.NsTiff, "Software", XmpConstConstants.NsXmp, "CreatorTool", null);
            // Aliases from PNG (Acrobat ImageCapture) to DC and XMP.
            RegisterAlias(XmpConstConstants.NsPng, "Author", XmpConstConstants.NsDc, "creator", aliasToArrayOrdered);
            RegisterAlias(XmpConstConstants.NsPng, "Copyright", XmpConstConstants.NsDc, "rights", aliasToArrayAltText);
            RegisterAlias(XmpConstConstants.NsPng, "CreationTime", XmpConstConstants.NsXmp, "CreateDate", null);
            RegisterAlias(XmpConstConstants.NsPng, "Description", XmpConstConstants.NsDc, "description", aliasToArrayAltText);
            RegisterAlias(XmpConstConstants.NsPng, "ModificationTime", XmpConstConstants.NsXmp, "ModifyDate", null);
            RegisterAlias(XmpConstConstants.NsPng, "Software", XmpConstConstants.NsXmp, "CreatorTool", null);
            RegisterAlias(XmpConstConstants.NsPng, "Title", XmpConstConstants.NsDc, "title", aliasToArrayAltText);
        }
    }
}

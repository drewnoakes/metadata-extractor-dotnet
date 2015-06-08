// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using System.Collections;
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
    public sealed class XMPSchemaRegistryImpl : XMPSchemaRegistry, XMPConst
    {
        /// <summary>a map from a namespace URI to its registered prefix</summary>
        private readonly IDictionary namespaceToPrefixMap = new Hashtable();

        /// <summary>a map from a prefix to the associated namespace URI</summary>
        private readonly IDictionary prefixToNamespaceMap = new Hashtable();

        /// <summary>a map of all registered aliases.</summary>
        /// <remarks>
        /// a map of all registered aliases.
        /// The map is a relationship from a qname to an <code>XMPAliasInfo</code>-object.
        /// </remarks>
        private readonly IDictionary aliasMap = new Hashtable();

        /// <summary>The pattern that must not be contained in simple properties</summary>
        private readonly Pattern p = Pattern.Compile("[/*?\\[\\]]");

        /// <summary>
        /// Performs the initialisation of the registry with the default namespaces, aliases and global
        /// options.
        /// </summary>
        public XMPSchemaRegistryImpl()
        {
            try
            {
                RegisterStandardNamespaces();
                RegisterStandardAliases();
            }
            catch (XMPException)
            {
                throw new RuntimeException("The XMPSchemaRegistry cannot be initialized!");
            }
        }

        // ---------------------------------------------------------------------------------------------
        // Namespace Functions
        /// <seealso cref="Com.Adobe.Xmp.XMPSchemaRegistry.RegisterNamespace(string, string)"/>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public string RegisterNamespace(string namespaceURI, string suggestedPrefix)
        {
            lock (this)
            {
                ParameterAsserts.AssertSchemaNS(namespaceURI);
                ParameterAsserts.AssertPrefix(suggestedPrefix);
                if (suggestedPrefix[suggestedPrefix.Length - 1] != ':')
                {
                    suggestedPrefix += ':';
                }
                if (!Utils.IsXMLNameNS(Runtime.Substring(suggestedPrefix, 0, suggestedPrefix.Length - 1)))
                {
                    throw new XMPException("The prefix is a bad XML name", XMPErrorConstants.Badxml);
                }
                string registeredPrefix = (string)namespaceToPrefixMap.Get(namespaceURI);
                string registeredNS = (string)prefixToNamespaceMap.Get(suggestedPrefix);
                if (registeredPrefix != null)
                {
                    // Return the actual prefix
                    return registeredPrefix;
                }
                else
                {
                    if (registeredNS != null)
                    {
                        // the namespace is new, but the prefix is already engaged,
                        // we generate a new prefix out of the suggested
                        string generatedPrefix = suggestedPrefix;
                        for (int i = 1; prefixToNamespaceMap.ContainsKey(generatedPrefix); i++)
                        {
                            generatedPrefix = Runtime.Substring(suggestedPrefix, 0, suggestedPrefix.Length - 1) + "_" + i + "_:";
                        }
                        suggestedPrefix = generatedPrefix;
                    }
                    prefixToNamespaceMap.Put(suggestedPrefix, namespaceURI);
                    namespaceToPrefixMap.Put(namespaceURI, suggestedPrefix);
                    // Return the suggested prefix
                    return suggestedPrefix;
                }
            }
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPSchemaRegistry.DeleteNamespace(string)"/>
        public void DeleteNamespace(string namespaceURI)
        {
            lock (this)
            {
                string prefixToDelete = GetNamespacePrefix(namespaceURI);
                if (prefixToDelete != null)
                {
                    Collections.Remove(namespaceToPrefixMap, namespaceURI);
                    Collections.Remove(prefixToNamespaceMap, prefixToDelete);
                }
            }
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPSchemaRegistry.GetNamespacePrefix(string)"/>
        public string GetNamespacePrefix(string namespaceURI)
        {
            lock (this)
            {
                return (string)namespaceToPrefixMap.Get(namespaceURI);
            }
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPSchemaRegistry.GetNamespaceURI(string)"/>
        public string GetNamespaceURI(string namespacePrefix)
        {
            lock (this)
            {
                if (namespacePrefix != null && !namespacePrefix.EndsWith(":"))
                {
                    namespacePrefix += ":";
                }
                return (string)prefixToNamespaceMap.Get(namespacePrefix);
            }
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPSchemaRegistry.GetNamespaces()"/>
        public IDictionary GetNamespaces()
        {
            lock (this)
            {
                return Collections.UnmodifiableMap(new SortedList(namespaceToPrefixMap));
            }
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPSchemaRegistry.GetPrefixes()"/>
        public IDictionary GetPrefixes()
        {
            lock (this)
            {
                return Collections.UnmodifiableMap(new SortedList(prefixToNamespaceMap));
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
        /// <exception cref="Com.Adobe.Xmp.XMPException">Forwards processing exceptions</exception>
        private void RegisterStandardNamespaces()
        {
            // register standard namespaces
            RegisterNamespace(XMPConstConstants.NsXml, "xml");
            RegisterNamespace(XMPConstConstants.NsRdf, "rdf");
            RegisterNamespace(XMPConstConstants.NsDc, "dc");
            RegisterNamespace(XMPConstConstants.NsIptccore, "Iptc4xmpCore");
            RegisterNamespace(XMPConstConstants.NsIptcext, "Iptc4xmpExt");
            RegisterNamespace(XMPConstConstants.NsDicom, "DICOM");
            RegisterNamespace(XMPConstConstants.NsPlus, "plus");
            // register Adobe standard namespaces
            RegisterNamespace(XMPConstConstants.NsX, "x");
            RegisterNamespace(XMPConstConstants.NsIx, "iX");
            RegisterNamespace(XMPConstConstants.NsXmp, "xmp");
            RegisterNamespace(XMPConstConstants.NsXmpRights, "xmpRights");
            RegisterNamespace(XMPConstConstants.NsXmpMm, "xmpMM");
            RegisterNamespace(XMPConstConstants.NsXmpBj, "xmpBJ");
            RegisterNamespace(XMPConstConstants.NsXmpNote, "xmpNote");
            RegisterNamespace(XMPConstConstants.NsPdf, "pdf");
            RegisterNamespace(XMPConstConstants.NsPdfx, "pdfx");
            RegisterNamespace(XMPConstConstants.NsPdfxId, "pdfxid");
            RegisterNamespace(XMPConstConstants.NsPdfaSchema, "pdfaSchema");
            RegisterNamespace(XMPConstConstants.NsPdfaProperty, "pdfaProperty");
            RegisterNamespace(XMPConstConstants.NsPdfaType, "pdfaType");
            RegisterNamespace(XMPConstConstants.NsPdfaField, "pdfaField");
            RegisterNamespace(XMPConstConstants.NsPdfaId, "pdfaid");
            RegisterNamespace(XMPConstConstants.NsPdfaExtension, "pdfaExtension");
            RegisterNamespace(XMPConstConstants.NsPhotoshop, "photoshop");
            RegisterNamespace(XMPConstConstants.NsPsalbum, "album");
            RegisterNamespace(XMPConstConstants.NsExif, "exif");
            RegisterNamespace(XMPConstConstants.NsExifx, "exifEX");
            RegisterNamespace(XMPConstConstants.NsExifAux, "aux");
            RegisterNamespace(XMPConstConstants.NsTiff, "tiff");
            RegisterNamespace(XMPConstConstants.NsPng, "png");
            RegisterNamespace(XMPConstConstants.NsJpeg, "jpeg");
            RegisterNamespace(XMPConstConstants.NsJp2k, "jp2k");
            RegisterNamespace(XMPConstConstants.NsCameraraw, "crs");
            RegisterNamespace(XMPConstConstants.NsAdobestockphoto, "bmsp");
            RegisterNamespace(XMPConstConstants.NsCreatorAtom, "creatorAtom");
            RegisterNamespace(XMPConstConstants.NsAsf, "asf");
            RegisterNamespace(XMPConstConstants.NsWav, "wav");
            RegisterNamespace(XMPConstConstants.NsBwf, "bext");
            RegisterNamespace(XMPConstConstants.NsRiffinfo, "riffinfo");
            RegisterNamespace(XMPConstConstants.NsScript, "xmpScript");
            RegisterNamespace(XMPConstConstants.NsTxmp, "txmp");
            RegisterNamespace(XMPConstConstants.NsSwf, "swf");
            // register Adobe private namespaces
            RegisterNamespace(XMPConstConstants.NsDm, "xmpDM");
            RegisterNamespace(XMPConstConstants.NsTransient, "xmpx");
            // register Adobe standard type namespaces
            RegisterNamespace(XMPConstConstants.TypeText, "xmpT");
            RegisterNamespace(XMPConstConstants.TypePagedfile, "xmpTPg");
            RegisterNamespace(XMPConstConstants.TypeGraphics, "xmpG");
            RegisterNamespace(XMPConstConstants.TypeImage, "xmpGImg");
            RegisterNamespace(XMPConstConstants.TypeFont, "stFnt");
            RegisterNamespace(XMPConstConstants.TypeDimensions, "stDim");
            RegisterNamespace(XMPConstConstants.TypeResourceevent, "stEvt");
            RegisterNamespace(XMPConstConstants.TypeResourceref, "stRef");
            RegisterNamespace(XMPConstConstants.TypeStVersion, "stVer");
            RegisterNamespace(XMPConstConstants.TypeStJob, "stJob");
            RegisterNamespace(XMPConstConstants.TypeManifestitem, "stMfs");
            RegisterNamespace(XMPConstConstants.TypeIdentifierqual, "xmpidq");
        }

        // ---------------------------------------------------------------------------------------------
        // Alias Functions
        /// <seealso cref="Com.Adobe.Xmp.XMPSchemaRegistry.ResolveAlias(string, string)"/>
        public XMPAliasInfo ResolveAlias(string aliasNS, string aliasProp)
        {
            lock (this)
            {
                string aliasPrefix = GetNamespacePrefix(aliasNS);
                if (aliasPrefix == null)
                {
                    return null;
                }
                return (XMPAliasInfo)aliasMap.Get(aliasPrefix + aliasProp);
            }
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPSchemaRegistry.FindAlias(string)"/>
        public XMPAliasInfo FindAlias(string qname)
        {
            lock (this)
            {
                return (XMPAliasInfo)aliasMap.Get(qname);
            }
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPSchemaRegistry.FindAliases(string)"/>
        public XMPAliasInfo[] FindAliases(string aliasNS)
        {
            lock (this)
            {
                string prefix = GetNamespacePrefix(aliasNS);
                IList result = new ArrayList();
                if (prefix != null)
                {
                    for (Iterator it = aliasMap.Keys.Iterator(); it.HasNext(); )
                    {
                        string qname = (string)it.Next();
                        if (qname.StartsWith(prefix))
                        {
                            result.Add(FindAlias(qname));
                        }
                    }
                }
                return (XMPAliasInfo[])Collections.ToArray(result, new XMPAliasInfo[result.Count]);
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
        /// <param name="aliasNS">
        /// The namespace URI for the alias. Must not be null or the empty
        /// string.
        /// </param>
        /// <param name="aliasProp">
        /// The name of the alias. Must be a simple name, not null or the
        /// empty string and not a general path expression.
        /// </param>
        /// <param name="actualNS">
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
        /// <exception cref="Com.Adobe.Xmp.XMPException">for inconsistant aliases.</exception>
        internal void RegisterAlias(string aliasNS, string aliasProp, string actualNS, string actualProp, AliasOptions aliasForm)
        {
            lock (this)
            {
                ParameterAsserts.AssertSchemaNS(aliasNS);
                ParameterAsserts.AssertPropName(aliasProp);
                ParameterAsserts.AssertSchemaNS(actualNS);
                ParameterAsserts.AssertPropName(actualProp);
                // Fix the alias options
                AliasOptions aliasOpts = aliasForm != null ? new AliasOptions(XMPNodeUtils.VerifySetOptions(aliasForm.ToPropertyOptions(), null).GetOptions()) : new AliasOptions();
                if (p.Matcher(aliasProp).Find() || p.Matcher(actualProp).Find())
                {
                    throw new XMPException("Alias and actual property names must be simple", XMPErrorConstants.Badxpath);
                }
                // check if both namespaces are registered
                string aliasPrefix = GetNamespacePrefix(aliasNS);
                string actualPrefix = GetNamespacePrefix(actualNS);
                if (aliasPrefix == null)
                {
                    throw new XMPException("Alias namespace is not registered", XMPErrorConstants.Badschema);
                }
                else
                {
                    if (actualPrefix == null)
                    {
                        throw new XMPException("Actual namespace is not registered", XMPErrorConstants.Badschema);
                    }
                }
                string key = aliasPrefix + aliasProp;
                // check if alias is already existing
                if (aliasMap.ContainsKey(key))
                {
                    throw new XMPException("Alias is already existing", XMPErrorConstants.Badparam);
                }
                else
                {
                    if (aliasMap.ContainsKey(actualPrefix + actualProp))
                    {
                        throw new XMPException("Actual property is already an alias, use the base property", XMPErrorConstants.Badparam);
                    }
                }
                XMPAliasInfo aliasInfo = new _XMPAliasInfo_390(actualNS, actualPrefix, actualProp, aliasOpts);
                aliasMap.Put(key, aliasInfo);
            }
        }

        private sealed class _XMPAliasInfo_390 : XMPAliasInfo
        {
            public _XMPAliasInfo_390(string actualNS, string actualPrefix, string actualProp, AliasOptions aliasOpts)
            {
                this.actualNS = actualNS;
                this.actualPrefix = actualPrefix;
                this.actualProp = actualProp;
                this.aliasOpts = aliasOpts;
            }

            /// <seealso cref="Com.Adobe.Xmp.Properties.XMPAliasInfo.GetNamespace()"/>
            public string GetNamespace()
            {
                return actualNS;
            }

            /// <seealso cref="Com.Adobe.Xmp.Properties.XMPAliasInfo.GetPrefix()"/>
            public string GetPrefix()
            {
                return actualPrefix;
            }

            /// <seealso cref="Com.Adobe.Xmp.Properties.XMPAliasInfo.GetPropName()"/>
            public string GetPropName()
            {
                return actualProp;
            }

            /// <seealso cref="Com.Adobe.Xmp.Properties.XMPAliasInfo.GetAliasForm()"/>
            public AliasOptions GetAliasForm()
            {
                return aliasOpts;
            }

            public override string ToString()
            {
                return actualPrefix + actualProp + " NS(" + actualNS + "), FORM (" + this.GetAliasForm() + ")";
            }

            private readonly string actualNS;

            private readonly string actualPrefix;

            private readonly string actualProp;

            private readonly AliasOptions aliasOpts;
        }

        /// <seealso cref="Com.Adobe.Xmp.XMPSchemaRegistry.GetAliases()"/>
        public IDictionary GetAliases()
        {
            lock (this)
            {
                return Collections.UnmodifiableMap(new SortedList(aliasMap));
            }
        }

        /// <summary>Register the standard aliases.</summary>
        /// <remarks>
        /// Register the standard aliases.
        /// Note: This method is not lock because only called by the constructor.
        /// </remarks>
        /// <exception cref="Com.Adobe.Xmp.XMPException">If the registrations of at least one alias fails.</exception>
        private void RegisterStandardAliases()
        {
            AliasOptions aliasToArrayOrdered = new AliasOptions().SetArrayOrdered(true);
            AliasOptions aliasToArrayAltText = new AliasOptions().SetArrayAltText(true);
            // Aliases from XMP to DC.
            RegisterAlias(XMPConstConstants.NsXmp, "Author", XMPConstConstants.NsDc, "creator", aliasToArrayOrdered);
            RegisterAlias(XMPConstConstants.NsXmp, "Authors", XMPConstConstants.NsDc, "creator", null);
            RegisterAlias(XMPConstConstants.NsXmp, "Description", XMPConstConstants.NsDc, "description", null);
            RegisterAlias(XMPConstConstants.NsXmp, "Format", XMPConstConstants.NsDc, "format", null);
            RegisterAlias(XMPConstConstants.NsXmp, "Keywords", XMPConstConstants.NsDc, "subject", null);
            RegisterAlias(XMPConstConstants.NsXmp, "Locale", XMPConstConstants.NsDc, "language", null);
            RegisterAlias(XMPConstConstants.NsXmp, "Title", XMPConstConstants.NsDc, "title", null);
            RegisterAlias(XMPConstConstants.NsXmpRights, "Copyright", XMPConstConstants.NsDc, "rights", null);
            // Aliases from PDF to DC and XMP.
            RegisterAlias(XMPConstConstants.NsPdf, "Author", XMPConstConstants.NsDc, "creator", aliasToArrayOrdered);
            RegisterAlias(XMPConstConstants.NsPdf, "BaseURL", XMPConstConstants.NsXmp, "BaseURL", null);
            RegisterAlias(XMPConstConstants.NsPdf, "CreationDate", XMPConstConstants.NsXmp, "CreateDate", null);
            RegisterAlias(XMPConstConstants.NsPdf, "Creator", XMPConstConstants.NsXmp, "CreatorTool", null);
            RegisterAlias(XMPConstConstants.NsPdf, "ModDate", XMPConstConstants.NsXmp, "ModifyDate", null);
            RegisterAlias(XMPConstConstants.NsPdf, "Subject", XMPConstConstants.NsDc, "description", aliasToArrayAltText);
            RegisterAlias(XMPConstConstants.NsPdf, "Title", XMPConstConstants.NsDc, "title", aliasToArrayAltText);
            // Aliases from PHOTOSHOP to DC and XMP.
            RegisterAlias(XMPConstConstants.NsPhotoshop, "Author", XMPConstConstants.NsDc, "creator", aliasToArrayOrdered);
            RegisterAlias(XMPConstConstants.NsPhotoshop, "Caption", XMPConstConstants.NsDc, "description", aliasToArrayAltText);
            RegisterAlias(XMPConstConstants.NsPhotoshop, "Copyright", XMPConstConstants.NsDc, "rights", aliasToArrayAltText);
            RegisterAlias(XMPConstConstants.NsPhotoshop, "Keywords", XMPConstConstants.NsDc, "subject", null);
            RegisterAlias(XMPConstConstants.NsPhotoshop, "Marked", XMPConstConstants.NsXmpRights, "Marked", null);
            RegisterAlias(XMPConstConstants.NsPhotoshop, "Title", XMPConstConstants.NsDc, "title", aliasToArrayAltText);
            RegisterAlias(XMPConstConstants.NsPhotoshop, "WebStatement", XMPConstConstants.NsXmpRights, "WebStatement", null);
            // Aliases from TIFF and EXIF to DC and XMP.
            RegisterAlias(XMPConstConstants.NsTiff, "Artist", XMPConstConstants.NsDc, "creator", aliasToArrayOrdered);
            RegisterAlias(XMPConstConstants.NsTiff, "Copyright", XMPConstConstants.NsDc, "rights", null);
            RegisterAlias(XMPConstConstants.NsTiff, "DateTime", XMPConstConstants.NsXmp, "ModifyDate", null);
            RegisterAlias(XMPConstConstants.NsTiff, "ImageDescription", XMPConstConstants.NsDc, "description", null);
            RegisterAlias(XMPConstConstants.NsTiff, "Software", XMPConstConstants.NsXmp, "CreatorTool", null);
            // Aliases from PNG (Acrobat ImageCapture) to DC and XMP.
            RegisterAlias(XMPConstConstants.NsPng, "Author", XMPConstConstants.NsDc, "creator", aliasToArrayOrdered);
            RegisterAlias(XMPConstConstants.NsPng, "Copyright", XMPConstConstants.NsDc, "rights", aliasToArrayAltText);
            RegisterAlias(XMPConstConstants.NsPng, "CreationTime", XMPConstConstants.NsXmp, "CreateDate", null);
            RegisterAlias(XMPConstConstants.NsPng, "Description", XMPConstConstants.NsDc, "description", aliasToArrayAltText);
            RegisterAlias(XMPConstConstants.NsPng, "ModificationTime", XMPConstConstants.NsXmp, "ModifyDate", null);
            RegisterAlias(XMPConstConstants.NsPng, "Software", XMPConstConstants.NsXmp, "CreatorTool", null);
            RegisterAlias(XMPConstConstants.NsPng, "Title", XMPConstConstants.NsDc, "title", aliasToArrayAltText);
        }
    }
}

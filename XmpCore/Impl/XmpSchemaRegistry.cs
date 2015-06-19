// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sharpen;
using XmpCore.Options;
using XmpCore;

namespace XmpCore.Impl
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
        private readonly Dictionary<string, string> _namespaceToPrefixMap = new Dictionary<string, string>();

        /// <summary>a map from a prefix to the associated namespace URI</summary>
        private readonly IDictionary<string, string> _prefixToNamespaceMap = new Dictionary<string, string>();

        /// <summary>a map of all registered aliases.</summary>
        /// <remarks>
        /// a map of all registered aliases.
        /// The map is a relationship from a qname to an <c>XMPAliasInfo</c>-object.
        /// </remarks>
        private readonly IDictionary<string, IXmpAliasInfo> _aliasMap = new Dictionary<string, IXmpAliasInfo>();

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
            catch (XmpException e)
            {
                throw new Exception("The XMPSchemaRegistry cannot be initialized!", e);
            }
        }

        #region Namespaces

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
                if (!Utils.IsXmlNameNs(suggestedPrefix.Substring(0, suggestedPrefix.Length - 1 - 0)))
                {
                    throw new XmpException("The prefix is a bad XML name", XmpErrorCode.BadXml);
                }
                var registeredPrefix = _namespaceToPrefixMap[namespaceUri];
                var registeredNs = _prefixToNamespaceMap[suggestedPrefix];
                if (registeredPrefix != null)
                {
                    // Return the actual prefix
                    return registeredPrefix;
                }
                if (registeredNs != null)
                {
                    // the namespace is new, but the prefix is already engaged,
                    // we generate a new prefix out of the suggested
                    var generatedPrefix = suggestedPrefix;
                    for (var i = 1; _prefixToNamespaceMap.ContainsKey(generatedPrefix); i++)
                    {
                        generatedPrefix = suggestedPrefix.Substring(0, suggestedPrefix.Length - 1 - 0) + "_" + i + "_:";
                    }
                    suggestedPrefix = generatedPrefix;
                }
                _prefixToNamespaceMap[suggestedPrefix] = namespaceUri;
                _namespaceToPrefixMap[namespaceUri] = suggestedPrefix;
                // Return the suggested prefix
                return suggestedPrefix;
            }
        }

        public void DeleteNamespace(string namespaceUri)
        {
            lock (this)
            {
                var prefixToDelete = GetNamespacePrefix(namespaceUri);
                if (prefixToDelete != null)
                {
                    _namespaceToPrefixMap.Remove(namespaceUri);
                    _prefixToNamespaceMap.Remove(prefixToDelete);
                }
            }
        }

        public string GetNamespacePrefix(string namespaceUri)
        {
            lock (this)
            {
                return _namespaceToPrefixMap[namespaceUri];
            }
        }

        public string GetNamespaceUri(string namespacePrefix)
        {
            lock (this)
            {
                if (namespacePrefix != null && !namespacePrefix.EndsWith(":"))
                {
                    namespacePrefix += ":";
                }
                return _prefixToNamespaceMap[namespacePrefix];
            }
        }

        public IDictionary GetNamespaces()
        {
            lock (this)
            {
                return new Dictionary<string, string>(_namespaceToPrefixMap);
            }
        }

        public IDictionary GetPrefixes()
        {
            lock (this)
            {
                return new Dictionary<string, string>(_prefixToNamespaceMap);
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

        #endregion

        #region Aliases

        public IXmpAliasInfo ResolveAlias(string aliasNs, string aliasProp)
        {
            lock (this)
            {
                var aliasPrefix = GetNamespacePrefix(aliasNs);
                if (aliasPrefix == null)
                {
                    return null;
                }
                return _aliasMap[aliasPrefix + aliasProp];
            }
        }

        public IXmpAliasInfo FindAlias(string qname)
        {
            lock (this)
            {
                return _aliasMap[qname];
            }
        }

        public IReadOnlyList<IXmpAliasInfo> FindAliases(string aliasNs)
        {
            lock (this)
            {
                var prefix = GetNamespacePrefix(aliasNs);
                var result = new List<IXmpAliasInfo>();
                if (prefix != null)
                {
                    for (var it = _aliasMap.Keys.Iterator(); it.HasNext();)
                    {
                        var qname = it.Next();
                        if (qname.StartsWith(prefix))
                        {
                            result.Add(FindAlias(qname));
                        }
                    }
                }
                return result;
            }
        }

        /// <summary>Associates an alias name with an actual name.</summary>
        /// <remarks>
        /// Associates an alias name with an actual name.
        /// <para />
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
        /// <c>XMP_NoOptions</c>, the default value, for all
        /// direct aliases regardless of whether the actual data type is
        /// an array or not (see
        /// <see cref="AliasOptions"/>
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
                var aliasOpts = aliasForm != null ? new AliasOptions(XmpNodeUtils.VerifySetOptions(aliasForm.ToPropertyOptions(), null).GetOptions()) : new AliasOptions();
                if (_p.IsMatch(aliasProp) || _p.IsMatch(actualProp))
                {
                    throw new XmpException("Alias and actual property names must be simple", XmpErrorCode.BadXPath);
                }
                // check if both namespaces are registered
                var aliasPrefix = GetNamespacePrefix(aliasNs);
                var actualPrefix = GetNamespacePrefix(actualNs);
                if (aliasPrefix == null)
                {
                    throw new XmpException("Alias namespace is not registered", XmpErrorCode.BadSchema);
                }
                if (actualPrefix == null)
                {
                    throw new XmpException("Actual namespace is not registered", XmpErrorCode.BadSchema);
                }
                var key = aliasPrefix + aliasProp;
                // check if alias is already existing
                if (_aliasMap.ContainsKey(key))
                {
                    throw new XmpException("Alias is already existing", XmpErrorCode.BadParam);
                }
                if (_aliasMap.ContainsKey(actualPrefix + actualProp))
                {
                    throw new XmpException("Actual property is already an alias, use the base property", XmpErrorCode.BadParam);
                }
                IXmpAliasInfo aliasInfo = new XmpAliasInfo390(actualNs, actualPrefix, actualProp, aliasOpts);
                _aliasMap[key] = aliasInfo;
            }
        }

        private sealed class XmpAliasInfo390 : IXmpAliasInfo
        {
            public XmpAliasInfo390(string actualNs, string actualPrefix, string actualProp, AliasOptions aliasOpts)
            {
                _actualNs = actualNs;
                _actualPrefix = actualPrefix;
                _actualProp = actualProp;
                _aliasOpts = aliasOpts;
            }

            public string GetNamespace()
            {
                return _actualNs;
            }

            public string GetPrefix()
            {
                return _actualPrefix;
            }

            public string GetPropName()
            {
                return _actualProp;
            }

            public AliasOptions GetAliasForm()
            {
                return _aliasOpts;
            }

            public override string ToString()
            {
                return _actualPrefix + _actualProp + " NS(" + _actualNs + "), FORM (" + GetAliasForm() + ")";
            }

            private readonly string _actualNs;
            private readonly string _actualPrefix;
            private readonly string _actualProp;
            private readonly AliasOptions _aliasOpts;
        }

        public IDictionary GetAliases()
        {
            lock (this)
            {
                return new Dictionary<string, IXmpAliasInfo>(_aliasMap);
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
            var aliasToArrayOrdered = new AliasOptions { IsArrayOrdered = true };
            var aliasToArrayAltText = new AliasOptions { IsArrayAltText = true };
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

        #endregion
    }
}

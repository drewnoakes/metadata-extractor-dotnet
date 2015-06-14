// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

namespace Com.Adobe.Xmp.Impl.Xpath
{
    /// <summary>Parser for XMP XPaths.</summary>
    /// <since>01.03.2006</since>
    public static class XmpPathParser
    {
        /// <summary>
        /// Split an XMPPath expression apart at the conceptual steps, adding the
        /// root namespace prefix to the first property component.
        /// </summary>
        /// <remarks>
        /// Split an XMPPath expression apart at the conceptual steps, adding the
        /// root namespace prefix to the first property component. The schema URI is
        /// put in the first (0th) slot in the expanded XMPPath. Check if the top
        /// level component is an alias, but don't resolve it.
        /// <para />
        /// In the most verbose case steps are separated by '/', and each step can be
        /// of these forms:
        /// <dl>
        /// <dt>prefix:name
        /// <dd> A top level property or struct field.
        /// <dt>[index]
        /// <dd> An element of an array.
        /// <dt>[last()]
        /// <dd> The last element of an array.
        /// <dt>[fieldName=&quot;value&quot;]
        /// <dd> An element in an array of structs, chosen by a field value.
        /// <dt>[@xml:lang=&quot;value&quot;]
        /// <dd> An element in an alt-text array, chosen by the xml:lang qualifier.
        /// <dt>[?qualName=&quot;value&quot;]
        /// <dd> An element in an array, chosen by a qualifier value.
        /// <dt>@xml:lang
        /// <dd> An xml:lang qualifier.
        /// <dt>?qualName
        /// <dd> A general qualifier.
        /// </dl>
        /// <para />
        /// The logic is complicated though by shorthand for arrays, the separating
        /// '/' and leading '*' are optional. These are all equivalent: array/*[2]
        /// array/[2] array*[2] array[2] All of these are broken into the 2 steps
        /// "array" and "[2]".
        /// <para />
        /// The value portion in the array selector forms is a string quoted by '''
        /// or '"'. The value may contain any character including a doubled quoting
        /// character. The value may be empty.
        /// <para />
        /// The syntax isn't checked, but an XML name begins with a letter or '_',
        /// and contains letters, digits, '.', '-', '_', and a bunch of special
        /// non-ASCII Unicode characters. An XML qualified name is a pair of names
        /// separated by a colon.
        /// </remarks>
        /// <param name="schemaNs">schema namespace</param>
        /// <param name="path">property name</param>
        /// <returns>Returns the expandet XMPPath.</returns>
        /// <exception cref="XmpException">Thrown if the format is not correct somehow.</exception>
        public static XmpPath ExpandXPath(string schemaNs, string path)
        {
            if (schemaNs == null || path == null)
            {
                throw new XmpException("Parameter must not be null", XmpErrorCode.BadParam);
            }
            var expandedXPath = new XmpPath();
            var pos = new PathPosition { Path = path };
            // Pull out the first component and do some special processing on it: add the schema
            // namespace prefix and and see if it is an alias. The start must be a "qualName".
            ParseRootNode(schemaNs, pos, expandedXPath);
            // Now continue to process the rest of the XMPPath string.
            while (pos.StepEnd < path.Length)
            {
                pos.StepBegin = pos.StepEnd;
                SkipPathDelimiter(path, pos);
                pos.StepEnd = pos.StepBegin;
                var segment = path[pos.StepBegin] != '['
                    ? ParseStructSegment(pos)
                    : ParseIndexSegment(pos);
                if (segment.GetKind() == XmpPath.StructFieldStep)
                {
                    if (segment.GetName()[0] == '@')
                    {
                        segment.SetName("?" + segment.GetName().Substring (1));
                        if (!"?xml:lang".Equals(segment.GetName()))
                        {
                            throw new XmpException("Only xml:lang allowed with '@'", XmpErrorCode.BadXPath);
                        }
                    }
                    if (segment.GetName()[0] == '?')
                    {
                        pos.NameStart++;
                        segment.SetKind(XmpPath.QualifierStep);
                    }
                    VerifyQualName(pos.Path.Substring (pos.NameStart, pos.NameEnd - pos.NameStart));
                }
                else
                {
                    if (segment.GetKind() == XmpPath.FieldSelectorStep)
                    {
                        if (segment.GetName()[1] == '@')
                        {
                            segment.SetName("[?" + segment.GetName().Substring (2));
                            if (!segment.GetName().StartsWith("[?xml:lang="))
                            {
                                throw new XmpException("Only xml:lang allowed with '@'", XmpErrorCode.BadXPath);
                            }
                        }
                        if (segment.GetName()[1] == '?')
                        {
                            pos.NameStart++;
                            segment.SetKind(XmpPath.QualSelectorStep);
                            VerifyQualName(pos.Path.Substring (pos.NameStart, pos.NameEnd - pos.NameStart));
                        }
                    }
                }
                expandedXPath.Add(segment);
            }
            return expandedXPath;
        }

        /// <param name="path"/>
        /// <param name="pos"/>
        /// <exception cref="XmpException"/>
        private static void SkipPathDelimiter(string path, PathPosition pos)
        {
            if (path[pos.StepBegin] == '/')
            {
                // skip slash
                pos.StepBegin++;
                // added for Java
                if (pos.StepBegin >= path.Length)
                {
                    throw new XmpException("Empty XMPPath segment", XmpErrorCode.BadXPath);
                }
            }
            if (path[pos.StepBegin] == '*')
            {
                // skip asterisk
                pos.StepBegin++;
                if (pos.StepBegin >= path.Length || path[pos.StepBegin] != '[')
                {
                    throw new XmpException("Missing '[' after '*'", XmpErrorCode.BadXPath);
                }
            }
        }

        /// <summary>Parses a struct segment</summary>
        /// <param name="pos">the current position in the path</param>
        /// <returns>Retusn the segment or an errror</returns>
        /// <exception cref="XmpException">If the sement is empty</exception>
        private static XmpPathSegment ParseStructSegment(PathPosition pos)
        {
            pos.NameStart = pos.StepBegin;
            while (pos.StepEnd < pos.Path.Length && "/[*".IndexOf(pos.Path[pos.StepEnd]) < 0)
            {
                pos.StepEnd++;
            }
            pos.NameEnd = pos.StepEnd;
            if (pos.StepEnd == pos.StepBegin)
            {
                throw new XmpException("Empty XMPPath segment", XmpErrorCode.BadXPath);
            }
            // ! Touch up later, also changing '@' to '?'.
            var segment = new XmpPathSegment(pos.Path.Substring (pos.StepBegin, pos.StepEnd - pos.StepBegin), XmpPath.StructFieldStep);
            return segment;
        }

        /// <summary>Parses an array index segment.</summary>
        /// <param name="pos">the xmp path</param>
        /// <returns>Returns the segment or an error</returns>
        /// <exception cref="XmpException">thrown on xmp path errors</exception>
        private static XmpPathSegment ParseIndexSegment(PathPosition pos)
        {
            XmpPathSegment segment;
            pos.StepEnd++;
            // Look at the character after the leading '['.
            if ('0' <= pos.Path[pos.StepEnd] && pos.Path[pos.StepEnd] <= '9')
            {
                // A numeric (decimal integer) array index.
                while (pos.StepEnd < pos.Path.Length && '0' <= pos.Path[pos.StepEnd] && pos.Path[pos.StepEnd] <= '9')
                {
                    pos.StepEnd++;
                }
                segment = new XmpPathSegment(null, XmpPath.ArrayIndexStep);
            }
            else
            {
                // Could be "[last()]" or one of the selector forms. Find the ']' or '='.
                while (pos.StepEnd < pos.Path.Length && pos.Path[pos.StepEnd] != ']' && pos.Path[pos.StepEnd] != '=')
                {
                    pos.StepEnd++;
                }
                if (pos.StepEnd >= pos.Path.Length)
                {
                    throw new XmpException("Missing ']' or '=' for array index", XmpErrorCode.BadXPath);
                }
                if (pos.Path[pos.StepEnd] == ']')
                {
                    if (!"[last()".Equals(pos.Path.Substring (pos.StepBegin, pos.StepEnd - pos.StepBegin)))
                    {
                        throw new XmpException("Invalid non-numeric array index", XmpErrorCode.BadXPath);
                    }
                    segment = new XmpPathSegment(null, XmpPath.ArrayLastStep);
                }
                else
                {
                    pos.NameStart = pos.StepBegin + 1;
                    pos.NameEnd = pos.StepEnd;
                    pos.StepEnd++;
                    // Absorb the '=', remember the quote.
                    var quote = pos.Path[pos.StepEnd];
                    if (quote != '\'' && quote != '"')
                    {
                        throw new XmpException("Invalid quote in array selector", XmpErrorCode.BadXPath);
                    }
                    pos.StepEnd++;
                    // Absorb the leading quote.
                    while (pos.StepEnd < pos.Path.Length)
                    {
                        if (pos.Path[pos.StepEnd] == quote)
                        {
                            // check for escaped quote
                            if (pos.StepEnd + 1 >= pos.Path.Length || pos.Path[pos.StepEnd + 1] != quote)
                            {
                                break;
                            }
                            pos.StepEnd++;
                        }
                        pos.StepEnd++;
                    }
                    if (pos.StepEnd >= pos.Path.Length)
                    {
                        throw new XmpException("No terminating quote for array selector", XmpErrorCode.BadXPath);
                    }
                    pos.StepEnd++;
                    // Absorb the trailing quote.
                    // ! Touch up later, also changing '@' to '?'.
                    segment = new XmpPathSegment(null, XmpPath.FieldSelectorStep);
                }
            }
            if (pos.StepEnd >= pos.Path.Length || pos.Path[pos.StepEnd] != ']')
            {
                throw new XmpException("Missing ']' for array index", XmpErrorCode.BadXPath);
            }
            pos.StepEnd++;
            segment.SetName(pos.Path.Substring (pos.StepBegin, pos.StepEnd - pos.StepBegin));
            return segment;
        }

        /// <summary>
        /// Parses the root node of an XMP Path, checks if namespace and prefix fit together
        /// and resolve the property to the base property if it is an alias.
        /// </summary>
        /// <param name="schemaNs">the root namespace</param>
        /// <param name="pos">the parsing position helper</param>
        /// <param name="expandedXPath">the path to contribute to</param>
        /// <exception cref="XmpException">If the path is not valid.</exception>
        private static void ParseRootNode(string schemaNs, PathPosition pos, XmpPath expandedXPath)
        {
            while (pos.StepEnd < pos.Path.Length && "/[*".IndexOf(pos.Path[pos.StepEnd]) < 0)
            {
                pos.StepEnd++;
            }
            if (pos.StepEnd == pos.StepBegin)
            {
                throw new XmpException("Empty initial XMPPath step", XmpErrorCode.BadXPath);
            }
            var rootProp = VerifyXPathRoot(schemaNs, pos.Path.Substring (pos.StepBegin, pos.StepEnd - pos.StepBegin));
            var aliasInfo = XmpMetaFactory.GetSchemaRegistry().FindAlias(rootProp);
            if (aliasInfo == null)
            {
                // add schema xpath step
                expandedXPath.Add(new XmpPathSegment(schemaNs, XmpPath.SchemaNode));
                var rootStep = new XmpPathSegment(rootProp, XmpPath.StructFieldStep);
                expandedXPath.Add(rootStep);
            }
            else
            {
                // add schema xpath step and base step of alias
                expandedXPath.Add(new XmpPathSegment(aliasInfo.GetNamespace(), XmpPath.SchemaNode));
                var rootStep = new XmpPathSegment(VerifyXPathRoot(aliasInfo.GetNamespace(), aliasInfo.GetPropName()), XmpPath.StructFieldStep);
                rootStep.SetAlias(true);
                rootStep.SetAliasForm(aliasInfo.GetAliasForm().GetOptions());
                expandedXPath.Add(rootStep);
                if (aliasInfo.GetAliasForm().IsArrayAltText)
                {
                    var qualSelectorStep = new XmpPathSegment("[?xml:lang='x-default']", XmpPath.QualSelectorStep);
                    qualSelectorStep.SetAlias(true);
                    qualSelectorStep.SetAliasForm(aliasInfo.GetAliasForm().GetOptions());
                    expandedXPath.Add(qualSelectorStep);
                }
                else
                {
                    if (aliasInfo.GetAliasForm().IsArray)
                    {
                        var indexStep = new XmpPathSegment("[1]", XmpPath.ArrayIndexStep);
                        indexStep.SetAlias(true);
                        indexStep.SetAliasForm(aliasInfo.GetAliasForm().GetOptions());
                        expandedXPath.Add(indexStep);
                    }
                }
            }
        }

        /// <summary>
        /// Verifies whether the qualifier name is not XML conformant or the
        /// namespace prefix has not been registered.
        /// </summary>
        /// <param name="qualName">a qualifier name</param>
        /// <exception cref="XmpException">If the name is not conformant</exception>
        private static void VerifyQualName(string qualName)
        {
            var colonPos = qualName.IndexOf(':');
            if (colonPos > 0)
            {
                var prefix = qualName.Substring (0, colonPos - 0);
                if (Utils.IsXmlNameNs(prefix))
                {
                    var regUri = XmpMetaFactory.GetSchemaRegistry().GetNamespaceUri(prefix);
                    if (regUri != null)
                    {
                        return;
                    }
                    throw new XmpException("Unknown namespace prefix for qualified name", XmpErrorCode.BadXPath);
                }
            }
            throw new XmpException("Ill-formed qualified name", XmpErrorCode.BadXPath);
        }

        /// <summary>Verify if an XML name is conformant.</summary>
        /// <param name="name">an XML name</param>
        /// <exception cref="XmpException">When the name is not XML conformant</exception>
        private static void VerifySimpleXmlName(string name)
        {
            if (!Utils.IsXmlName(name))
            {
                throw new XmpException("Bad XML name", XmpErrorCode.BadXPath);
            }
        }

        /// <summary>Set up the first 2 components of the expanded XMPPath.</summary>
        /// <remarks>
        /// Set up the first 2 components of the expanded XMPPath. Normalizes the various cases of using
        /// the full schema URI and/or a qualified root property name. Returns true for normal
        /// processing. If allowUnknownSchemaNS is true and the schema namespace is not registered, false
        /// is returned. If allowUnknownSchemaNS is false and the schema namespace is not registered, an
        /// exception is thrown
        /// <para />
        /// (Should someday check the full syntax:)
        /// </remarks>
        /// <param name="schemaNs">schema namespace</param>
        /// <param name="rootProp">the root xpath segment</param>
        /// <returns>Returns root QName.</returns>
        /// <exception cref="XmpException">Thrown if the format is not correct somehow.</exception>
        private static string VerifyXPathRoot(string schemaNs, string rootProp)
        {
            // Do some basic checks on the URI and name. Try to lookup the URI. See if the name is
            // qualified.
            if (string.IsNullOrEmpty(schemaNs))
            {
                throw new XmpException("Schema namespace URI is required", XmpErrorCode.BadSchema);
            }
            if ((rootProp[0] == '?') || (rootProp[0] == '@'))
            {
                throw new XmpException("Top level name must not be a qualifier", XmpErrorCode.BadXPath);
            }
            if (rootProp.IndexOf('/') >= 0 || rootProp.IndexOf('[') >= 0)
            {
                throw new XmpException("Top level name must be simple", XmpErrorCode.BadXPath);
            }
            var prefix = XmpMetaFactory.GetSchemaRegistry().GetNamespacePrefix(schemaNs);
            if (prefix == null)
            {
                throw new XmpException("Unregistered schema namespace URI", XmpErrorCode.BadSchema);
            }
            // Verify the various URI and prefix combinations. Initialize the
            // expanded XMPPath.
            var colonPos = rootProp.IndexOf(':');
            if (colonPos < 0)
            {
                // The propName is unqualified, use the schemaURI and associated
                // prefix.
                VerifySimpleXmlName(rootProp);
                // Verify the part before any colon
                return prefix + rootProp;
            }
            // The propName is qualified. Make sure the prefix is legit. Use the associated URI and
            // qualified name.
            // Verify the part before any colon
            VerifySimpleXmlName(rootProp.Substring (0, colonPos - 0));
            VerifySimpleXmlName(rootProp.Substring (colonPos));
            prefix = rootProp.Substring (0, colonPos + 1 - 0);
            var regPrefix = XmpMetaFactory.GetSchemaRegistry().GetNamespacePrefix(schemaNs);
            if (regPrefix == null)
            {
                throw new XmpException("Unknown schema namespace prefix", XmpErrorCode.BadSchema);
            }
            if (!prefix.Equals(regPrefix))
            {
                throw new XmpException("Schema namespace URI and prefix mismatch", XmpErrorCode.BadSchema);
            }
            return rootProp;
        }
    }

    /// <summary>This objects contains all needed char positions to parse.</summary>
    internal class PathPosition
    {
        /// <summary>the complete path</summary>
        public string Path;

        /// <summary>the start of a segment name</summary>
        internal int NameStart;

        /// <summary>the end of a segment name</summary>
        internal int NameEnd;

        /// <summary>the begin of a step</summary>
        internal int StepBegin;

        /// <summary>the end of a step</summary>
        internal int StepEnd;
    }
}

// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using System.Collections.Generic;
using System.IO;
using Com.Adobe.Xmp.Options;

namespace Com.Adobe.Xmp.Impl
{
    /// <summary>Serializes the <c>XMPMeta</c>-object using the standard RDF serialization format.</summary>
    /// <remarks>
    /// Serializes the <c>XMPMeta</c>-object using the standard RDF serialization format.
    /// The output is written to an <c>OutputStream</c>
    /// according to the <c>SerializeOptions</c>.
    /// </remarks>
    /// <since>11.07.2006</since>
    public sealed class XmpSerializerRdf
    {
        /// <summary>default padding</summary>
        private const int DefaultPad = 2048;

        private const string PacketHeader = "<?xpacket begin=\"\uFEFF\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?>";

        /// <summary>The w/r is missing inbetween</summary>
        private const string PacketTrailer = "<?xpacket end=\"";

        private const string PacketTrailer2 = "\"?>";

        private const string RdfXmpmetaStart = "<x:xmpmeta xmlns:x=\"adobe:ns:meta/\" x:xmptk=\"";

        private const string RdfXmpmetaEnd = "</x:xmpmeta>";

        private const string RdfRdfStart = "<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">";

        private const string RdfRdfEnd = "</rdf:RDF>";

        private const string RdfSchemaStart = "<rdf:Description rdf:about=";

        private const string RdfSchemaEnd = "</rdf:Description>";

        private const string RdfStructStart = "<rdf:Description";

        private const string RdfStructEnd = "</rdf:Description>";

        private const string RdfEmptyStruct = "<rdf:Description/>";

        /// <summary>a set of all rdf attribute qualifier</summary>
        internal static readonly ICollection<object> RdfAttrQualifier = new HashSet<object>(new[] { XmpConstConstants.XmlLang, "rdf:resource", "rdf:ID", "rdf:bagID", "rdf:nodeID" });

        /// <summary>the metadata object to be serialized.</summary>
        private XmpMeta _xmp;

        /// <summary>the output stream to serialize to</summary>
        private Stream _stream;

        /// <summary>this writer is used to do the actual serialization</summary>
        private StreamWriter _writer;

        /// <summary>the stored serialization options</summary>
        private SerializeOptions _options;

        /// <summary>
        /// the size of one unicode char, for UTF-8 set to 1
        /// (Note: only valid for ASCII chars lower than 0x80),
        /// set to 2 in case of UTF-16
        /// </summary>
        private int _unicodeSize = 1;

        /// <summary>
        /// the padding in the XMP Packet, or the length of the complete packet in
        /// case of option <em>exactPacketLength</em>.
        /// </summary>
        private int _padding;

        private long _startPos;

        // UTF-8
        /// <summary>The actual serialization.</summary>
        /// <param name="xmp">the metadata object to be serialized</param>
        /// <param name="stream">outputStream the output stream to serialize to</param>
        /// <param name="options">the serialization options</param>
        /// <exception cref="XmpException">If case of wrong options or any other serialization error.</exception>
        public void Serialize(IXmpMeta xmp, Stream stream, SerializeOptions options)
        {
            try
            {
                _stream = stream;
                _startPos = _stream.Position;
                _writer = new StreamWriter(_stream, options.GetEncoding());
                _xmp = (XmpMeta)xmp;
                _options = options;
                _padding = options.Padding;
                _writer = new StreamWriter(_stream, options.GetEncoding());
                CheckOptionsConsistence();
                // serializes the whole packet, but don't write the tail yet
                // and flush to make sure that the written bytes are calculated correctly
                var tailStr = SerializeAsRdf();
                _writer.Flush();
                // adds padding
                AddPadding(tailStr.Length);
                // writes the tail
                Write(tailStr);
                _writer.Flush();
                _stream.Close();
            }
            catch (IOException)
            {
                throw new XmpException("Error writing to the OutputStream", XmpErrorCode.Unknown);
            }
        }

        /// <summary>Calculates the padding according to the options and write it to the stream.</summary>
        /// <param name="tailLength">the length of the tail string</param>
        /// <exception cref="XmpException">thrown if packet size is to small to fit the padding</exception>
        /// <exception cref="System.IO.IOException">forwards writer errors</exception>
        private void AddPadding(int tailLength)
        {
            if (_options.ExactPacketLength)
            {
                // the string length is equal to the length of the UTF-8 encoding
                var minSize = checked((int)(_stream.Position - _startPos)) + tailLength * _unicodeSize;
                if (minSize > _padding)
                {
                    throw new XmpException("Can't fit into specified packet size", XmpErrorCode.BadSerialize);
                }
                _padding -= minSize;
            }
            // Now the actual amount of padding to add.
            // fix rest of the padding according to Unicode unit size.
            _padding /= _unicodeSize;
            var newlineLen = _options.Newline.Length;
            if (_padding >= newlineLen)
            {
                _padding -= newlineLen;
                // Write this newline last.
                while (_padding >= (100 + newlineLen))
                {
                    WriteChars(100, ' ');
                    WriteNewline();
                    _padding -= (100 + newlineLen);
                }
                WriteChars(_padding, ' ');
                WriteNewline();
            }
            else
            {
                WriteChars(_padding, ' ');
            }
        }

        /// <summary>Checks if the supplied options are consistent.</summary>
        /// <exception cref="XmpException">Thrown if options are conflicting</exception>
        private void CheckOptionsConsistence()
        {
            if (_options.EncodeUtf16Be | _options.EncodeUtf16Le)
            {
                _unicodeSize = 2;
            }
            if (_options.ExactPacketLength)
            {
                if (_options.OmitPacketWrapper | _options.IncludeThumbnailPad)
                {
                    throw new XmpException("Inconsistent options for exact size serialize", XmpErrorCode.BadOptions);
                }
                if ((_options.Padding & (_unicodeSize - 1)) != 0)
                {
                    throw new XmpException("Exact size must be a multiple of the Unicode element", XmpErrorCode.BadOptions);
                }
            }
            else
            {
                if (_options.ReadOnlyPacket)
                {
                    if (_options.OmitPacketWrapper | _options.IncludeThumbnailPad)
                    {
                        throw new XmpException("Inconsistent options for read-only packet", XmpErrorCode.BadOptions);
                    }
                    _padding = 0;
                }
                else
                {
                    if (_options.OmitPacketWrapper)
                    {
                        if (_options.IncludeThumbnailPad)
                        {
                            throw new XmpException("Inconsistent options for non-packet serialize", XmpErrorCode.BadOptions);
                        }
                        _padding = 0;
                    }
                    else
                    {
                        if (_padding == 0)
                        {
                            _padding = DefaultPad * _unicodeSize;
                        }
                        if (_options.IncludeThumbnailPad)
                        {
                            if (!_xmp.DoesPropertyExist(XmpConstConstants.NsXmp, "Thumbnails"))
                            {
                                _padding += 10000 * _unicodeSize;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Writes the (optional) packet header and the outer rdf-tags.</summary>
        /// <returns>Returns the packet end processing instraction to be written after the padding.</returns>
        /// <exception cref="System.IO.IOException">Forwarded writer exceptions.</exception>
        /// <exception cref="XmpException"></exception>
        private string SerializeAsRdf()
        {
            var level = 0;
            // Write the packet header PI.
            if (!_options.OmitPacketWrapper)
            {
                WriteIndent(level);
                Write(PacketHeader);
                WriteNewline();
            }
            // Write the x:xmpmeta element's start tag.
            if (!_options.OmitXmpMetaElement)
            {
                WriteIndent(level);
                Write(RdfXmpmetaStart);
                Write(XmpMetaFactory.GetVersionInfo().Message);
                Write("\">");
                WriteNewline();
                level++;
            }
            // Write the rdf:RDF start tag.
            WriteIndent(level);
            Write(RdfRdfStart);
            WriteNewline();
            // Write all of the properties.
            if (_options.UseCanonicalFormat)
            {
                SerializeCanonicalRdfSchemas(level);
            }
            else
            {
                SerializeCompactRdfSchemas(level);
            }
            // Write the rdf:RDF end tag.
            WriteIndent(level);
            Write(RdfRdfEnd);
            WriteNewline();
            // Write the xmpmeta end tag.
            if (!_options.OmitXmpMetaElement)
            {
                level--;
                WriteIndent(level);
                Write(RdfXmpmetaEnd);
                WriteNewline();
            }
            // Write the packet trailer PI into the tail string as UTF-8.
            var tailStr = string.Empty;
            if (!_options.OmitPacketWrapper)
            {
                for (level = _options.BaseIndent; level > 0; level--)
                {
                    tailStr += _options.Indent;
                }
                tailStr += PacketTrailer;
                tailStr += _options.ReadOnlyPacket ? 'r' : 'w';
                tailStr += PacketTrailer2;
            }
            return tailStr;
        }

        /// <summary>Serializes the metadata in pretty-printed manner.</summary>
        /// <param name="level">indent level</param>
        /// <exception cref="System.IO.IOException">Forwarded writer exceptions</exception>
        /// <exception cref="XmpException"></exception>
        private void SerializeCanonicalRdfSchemas(int level)
        {
            if (_xmp.GetRoot().GetChildrenLength() > 0)
            {
                StartOuterRdfDescription(_xmp.GetRoot(), level);
                for (var it = _xmp.GetRoot().IterateChildren(); it.HasNext(); )
                {
                    var currSchema = (XmpNode)it.Next();
                    SerializeCanonicalRdfSchema(currSchema, level);
                }
                EndOuterRdfDescription(level);
            }
            else
            {
                WriteIndent(level + 1);
                Write(RdfSchemaStart);
                // Special case an empty XMP object.
                WriteTreeName();
                Write("/>");
                WriteNewline();
            }
        }

        /// <exception cref="System.IO.IOException"/>
        private void WriteTreeName()
        {
            Write('"');
            var name = _xmp.GetRoot().Name;
            if (name != null)
            {
                AppendNodeValue(name, true);
            }
            Write('"');
        }

        /// <summary>Serializes the metadata in compact manner.</summary>
        /// <param name="level">indent level to start with</param>
        /// <exception cref="System.IO.IOException">Forwarded writer exceptions</exception>
        /// <exception cref="XmpException"></exception>
        private void SerializeCompactRdfSchemas(int level)
        {
            // Begin the rdf:Description start tag.
            WriteIndent(level + 1);
            Write(RdfSchemaStart);
            WriteTreeName();
            // Write all necessary xmlns attributes.
            ICollection<object> usedPrefixes = new HashSet<object>();
            usedPrefixes.Add("xml");
            usedPrefixes.Add("rdf");
            for (var it = _xmp.GetRoot().IterateChildren(); it.HasNext(); )
            {
                var schema = (XmpNode)it.Next();
                DeclareUsedNamespaces(schema, usedPrefixes, level + 3);
            }
            // Write the top level "attrProps" and close the rdf:Description start tag.
            var allAreAttrs = true;
            for (var it1 = _xmp.GetRoot().IterateChildren(); it1.HasNext(); )
            {
                var schema = (XmpNode)it1.Next();
                allAreAttrs &= SerializeCompactRdfAttrProps(schema, level + 2);
            }
            if (!allAreAttrs)
            {
                Write('>');
                WriteNewline();
            }
            else
            {
                Write("/>");
                WriteNewline();
                return;
            }
            // ! Done if all properties in all schema are written as attributes.
            // Write the remaining properties for each schema.
            for (var it2 = _xmp.GetRoot().IterateChildren(); it2.HasNext(); )
            {
                var schema = (XmpNode)it2.Next();
                SerializeCompactRdfElementProps(schema, level + 2);
            }
            // Write the rdf:Description end tag.
            WriteIndent(level + 1);
            Write(RdfSchemaEnd);
            WriteNewline();
        }

        /// <summary>Write each of the parent's simple unqualified properties as an attribute.</summary>
        /// <remarks>
        /// Write each of the parent's simple unqualified properties as an attribute. Returns true if all
        /// of the properties are written as attributes.
        /// </remarks>
        /// <param name="parentNode">the parent property node</param>
        /// <param name="indent">the current indent level</param>
        /// <returns>Returns true if all properties can be rendered as RDF attribute.</returns>
        /// <exception cref="System.IO.IOException"/>
        private bool SerializeCompactRdfAttrProps(XmpNode parentNode, int indent)
        {
            var allAreAttrs = true;
            for (var it = parentNode.IterateChildren(); it.HasNext(); )
            {
                var prop = (XmpNode)it.Next();
                if (CanBeRdfAttrProp(prop))
                {
                    WriteNewline();
                    WriteIndent(indent);
                    Write(prop.Name);
                    Write("=\"");
                    AppendNodeValue(prop.Value, true);
                    Write('"');
                }
                else
                {
                    allAreAttrs = false;
                }
            }
            return allAreAttrs;
        }

        /// <summary>
        /// Recursively handles the "value" for a node that must be written as an RDF
        /// property element.
        /// </summary>
        /// <remarks>
        /// Recursively handles the "value" for a node that must be written as an RDF
        /// property element. It does not matter if it is a top level property, a
        /// field of a struct, or an item of an array. The indent is that for the
        /// property element. The patterns bwlow ignore attribute qualifiers such as
        /// xml:lang, they don't affect the output form.
        /// <blockquote>
        /// <pre>
        /// &lt;ns:UnqualifiedStructProperty-1
        /// ... The fields as attributes, if all are simple and unqualified
        /// /&gt;
        /// &lt;ns:UnqualifiedStructProperty-2 rdf:parseType=&quot;Resource&quot;&gt;
        /// ... The fields as elements, if none are simple and unqualified
        /// &lt;/ns:UnqualifiedStructProperty-2&gt;
        /// &lt;ns:UnqualifiedStructProperty-3&gt;
        /// &lt;rdf:Description
        /// ... The simple and unqualified fields as attributes
        /// &gt;
        /// ... The compound or qualified fields as elements
        /// &lt;/rdf:Description&gt;
        /// &lt;/ns:UnqualifiedStructProperty-3&gt;
        /// &lt;ns:UnqualifiedArrayProperty&gt;
        /// &lt;rdf:Bag&gt; or Seq or Alt
        /// ... Array items as rdf:li elements, same forms as top level properties
        /// &lt;/rdf:Bag&gt;
        /// &lt;/ns:UnqualifiedArrayProperty&gt;
        /// &lt;ns:QualifiedProperty rdf:parseType=&quot;Resource&quot;&gt;
        /// &lt;rdf:value&gt; ... Property &quot;value&quot;
        /// following the unqualified forms ... &lt;/rdf:value&gt;
        /// ... Qualifiers looking like named struct fields
        /// &lt;/ns:QualifiedProperty&gt;
        /// </pre>
        /// </blockquote>
        /// *** Consider numbered array items, but has compatibility problems.
        /// Consider qualified form with rdf:Description and attributes.
        /// </remarks>
        /// <param name="parentNode">the parent node</param>
        /// <param name="indent">the current indent level</param>
        /// <exception cref="System.IO.IOException">Forwards writer exceptions</exception>
        /// <exception cref="XmpException">If qualifier and element fields are mixed.</exception>
        private void SerializeCompactRdfElementProps(XmpNode parentNode, int indent)
        {
            for (var it = parentNode.IterateChildren(); it.HasNext(); )
            {
                var node = (XmpNode)it.Next();
                if (CanBeRdfAttrProp(node))
                {
                    continue;
                }
                var emitEndTag = true;
                var indentEndTag = true;
                // Determine the XML element name, write the name part of the start tag. Look over the
                // qualifiers to decide on "normal" versus "rdf:value" form. Emit the attribute
                // qualifiers at the same time.
                var elemName = node.Name;
                if (XmpConstConstants.ArrayItemName.Equals(elemName))
                {
                    elemName = "rdf:li";
                }
                WriteIndent(indent);
                Write('<');
                Write(elemName);
                var hasGeneralQualifiers = false;
                var hasRdfResourceQual = false;
                for (var iq = node.IterateQualifier(); iq.HasNext(); )
                {
                    var qualifier = (XmpNode)iq.Next();
                    if (!RdfAttrQualifier.Contains(qualifier.Name))
                    {
                        hasGeneralQualifiers = true;
                    }
                    else
                    {
                        hasRdfResourceQual = "rdf:resource".Equals(qualifier.Name);
                        Write(' ');
                        Write(qualifier.Name);
                        Write("=\"");
                        AppendNodeValue(qualifier.Value, true);
                        Write('"');
                    }
                }
                // Process the property according to the standard patterns.
                if (hasGeneralQualifiers)
                {
                    SerializeCompactRdfGeneralQualifier(indent, node);
                }
                else
                {
                    // This node has only attribute qualifiers. Emit as a property element.
                    if (!node.Options.IsCompositeProperty)
                    {
                        var result = SerializeCompactRdfSimpleProp(node);
                        emitEndTag = ((bool)result[0]);
                        indentEndTag = ((bool)result[1]);
                    }
                    else
                    {
                        if (node.Options.IsArray)
                        {
                            SerializeCompactRdfArrayProp(node, indent);
                        }
                        else
                        {
                            emitEndTag = SerializeCompactRdfStructProp(node, indent, hasRdfResourceQual);
                        }
                    }
                }
                // Emit the property element end tag.
                if (emitEndTag)
                {
                    if (indentEndTag)
                    {
                        WriteIndent(indent);
                    }
                    Write("</");
                    Write(elemName);
                    Write('>');
                    WriteNewline();
                }
            }
        }

        /// <summary>Serializes a simple property.</summary>
        /// <param name="node">an XMPNode</param>
        /// <returns>Returns an array containing the flags emitEndTag and indentEndTag.</returns>
        /// <exception cref="System.IO.IOException">Forwards the writer exceptions.</exception>
        private object[] SerializeCompactRdfSimpleProp(XmpNode node)
        {
            // This is a simple property.
            var emitEndTag = true;
            var indentEndTag = true;
            if (node.Options.IsUri)
            {
                Write(" rdf:resource=\"");
                AppendNodeValue(node.Value, true);
                Write("\"/>");
                WriteNewline();
                emitEndTag = false;
            }
            else
            {
                if (string.IsNullOrEmpty(node.Value))
                {
                    Write("/>");
                    WriteNewline();
                    emitEndTag = false;
                }
                else
                {
                    Write('>');
                    AppendNodeValue(node.Value, false);
                    indentEndTag = false;
                }
            }
            return new object[] { emitEndTag, indentEndTag };
        }

        /// <summary>Serializes an array property.</summary>
        /// <param name="node">an XMPNode</param>
        /// <param name="indent">the current indent level</param>
        /// <exception cref="System.IO.IOException">Forwards the writer exceptions.</exception>
        /// <exception cref="XmpException">If qualifier and element fields are mixed.</exception>
        private void SerializeCompactRdfArrayProp(XmpNode node, int indent)
        {
            // This is an array.
            Write('>');
            WriteNewline();
            EmitRdfArrayTag(node, true, indent + 1);
            if (node.Options.IsArrayAltText)
            {
                XmpNodeUtils.NormalizeLangArray(node);
            }
            SerializeCompactRdfElementProps(node, indent + 2);
            EmitRdfArrayTag(node, false, indent + 1);
        }

        /// <summary>Serializes a struct property.</summary>
        /// <param name="node">an XMPNode</param>
        /// <param name="indent">the current indent level</param>
        /// <param name="hasRdfResourceQual">Flag if the element has resource qualifier</param>
        /// <returns>Returns true if an end flag shall be emitted.</returns>
        /// <exception cref="System.IO.IOException">Forwards the writer exceptions.</exception>
        /// <exception cref="XmpException">If qualifier and element fields are mixed.</exception>
        private bool SerializeCompactRdfStructProp(XmpNode node, int indent, bool hasRdfResourceQual)
        {
            // This must be a struct.
            var hasAttrFields = false;
            var hasElemFields = false;
            var emitEndTag = true;
            for (var ic = node.IterateChildren(); ic.HasNext(); )
            {
                var field = (XmpNode)ic.Next();
                if (CanBeRdfAttrProp(field))
                {
                    hasAttrFields = true;
                }
                else
                {
                    hasElemFields = true;
                }
                if (hasAttrFields && hasElemFields)
                {
                    break;
                }
            }
            // No sense looking further.
            if (hasRdfResourceQual && hasElemFields)
            {
                throw new XmpException("Can't mix rdf:resource qualifier and element fields", XmpErrorCode.BadRdf);
            }
            if (!node.HasChildren)
            {
                // Catch an empty struct as a special case. The case
                // below would emit an empty
                // XML element, which gets reparsed as a simple property
                // with an empty value.
                Write(" rdf:parseType=\"Resource\"/>");
                WriteNewline();
                emitEndTag = false;
            }
            else
            {
                if (!hasElemFields)
                {
                    // All fields can be attributes, use the
                    // emptyPropertyElt form.
                    SerializeCompactRdfAttrProps(node, indent + 1);
                    Write("/>");
                    WriteNewline();
                    emitEndTag = false;
                }
                else
                {
                    if (!hasAttrFields)
                    {
                        // All fields must be elements, use the
                        // parseTypeResourcePropertyElt form.
                        Write(" rdf:parseType=\"Resource\">");
                        WriteNewline();
                        SerializeCompactRdfElementProps(node, indent + 1);
                    }
                    else
                    {
                        // Have a mix of attributes and elements, use an inner rdf:Description.
                        Write('>');
                        WriteNewline();
                        WriteIndent(indent + 1);
                        Write(RdfStructStart);
                        SerializeCompactRdfAttrProps(node, indent + 2);
                        Write(">");
                        WriteNewline();
                        SerializeCompactRdfElementProps(node, indent + 1);
                        WriteIndent(indent + 1);
                        Write(RdfStructEnd);
                        WriteNewline();
                    }
                }
            }
            return emitEndTag;
        }

        /// <summary>Serializes the general qualifier.</summary>
        /// <param name="node">the root node of the subtree</param>
        /// <param name="indent">the current indent level</param>
        /// <exception cref="System.IO.IOException">Forwards all writer exceptions.</exception>
        /// <exception cref="XmpException">If qualifier and element fields are mixed.</exception>
        private void SerializeCompactRdfGeneralQualifier(int indent, XmpNode node)
        {
            // The node has general qualifiers, ones that can't be
            // attributes on a property element.
            // Emit using the qualified property pseudo-struct form. The
            // value is output by a call
            // to SerializePrettyRDFProperty with emitAsRDFValue set.
            Write(" rdf:parseType=\"Resource\">");
            WriteNewline();
            SerializeCanonicalRdfProperty(node, false, true, indent + 1);
            for (var iq = node.IterateQualifier(); iq.HasNext(); )
            {
                var qualifier = (XmpNode)iq.Next();
                SerializeCanonicalRdfProperty(qualifier, false, false, indent + 1);
            }
        }

        /// <summary>
        /// Serializes one schema with all contained properties in pretty-printed
        /// manner.<br />
        /// Each schema's properties are written to a single
        /// rdf:Description element.
        /// </summary>
        /// <remarks>
        /// Serializes one schema with all contained properties in pretty-printed
        /// manner.<br />
        /// Each schema's properties are written to a single
        /// rdf:Description element. All of the necessary namespaces are declared in
        /// the rdf:Description element. The baseIndent is the base level for the
        /// entire serialization, that of the x:xmpmeta element. An xml:lang
        /// qualifier is written as an attribute of the property start tag, not by
        /// itself forcing the qualified property form.
        /// <blockquote>
        /// <pre>
        /// &lt;rdf:Description rdf:about=&quot;TreeName&quot; xmlns:ns=&quot;URI&quot; ... &gt;
        /// ... The actual properties of the schema, see SerializePrettyRDFProperty
        /// &lt;!-- ns1:Alias is aliased to ns2:Actual --&gt;  ... If alias comments are wanted
        /// &lt;/rdf:Description&gt;
        /// </pre>
        /// </blockquote>
        /// </remarks>
        /// <param name="schemaNode">a schema node</param>
        /// <param name="level"></param>
        /// <exception cref="System.IO.IOException">Forwarded writer exceptions</exception>
        /// <exception cref="XmpException"></exception>
        private void SerializeCanonicalRdfSchema(XmpNode schemaNode, int level)
        {
            // Write each of the schema's actual properties.
            for (var it = schemaNode.IterateChildren(); it.HasNext(); )
            {
                var propNode = (XmpNode)it.Next();
                SerializeCanonicalRdfProperty(propNode, _options.UseCanonicalFormat, false, level + 2);
            }
        }

        /// <summary>Writes all used namespaces of the subtree in node to the output.</summary>
        /// <remarks>
        /// Writes all used namespaces of the subtree in node to the output.
        /// The subtree is recursivly traversed.
        /// </remarks>
        /// <param name="node">the root node of the subtree</param>
        /// <param name="usedPrefixes">a set containing currently used prefixes</param>
        /// <param name="indent">the current indent level</param>
        /// <exception cref="System.IO.IOException">Forwards all writer exceptions.</exception>
        private void DeclareUsedNamespaces(XmpNode node, ICollection<object> usedPrefixes, int indent)
        {
            if (node.Options.IsSchemaNode)
            {
                // The schema node name is the URI, the value is the prefix.
                var prefix = node.Value.Substring (0, node.Value.Length - 1 - 0);
                DeclareNamespace(prefix, node.Name, usedPrefixes, indent);
            }
            else
            {
                if (node.Options.IsStruct)
                {
                    for (var it = node.IterateChildren(); it.HasNext(); )
                    {
                        var field = (XmpNode)it.Next();
                        DeclareNamespace(field.Name, null, usedPrefixes, indent);
                    }
                }
            }
            for (var it1 = node.IterateChildren(); it1.HasNext(); )
            {
                var child = (XmpNode)it1.Next();
                DeclareUsedNamespaces(child, usedPrefixes, indent);
            }
            for (var it2 = node.IterateQualifier(); it2.HasNext(); )
            {
                var qualifier = (XmpNode)it2.Next();
                DeclareNamespace(qualifier.Name, null, usedPrefixes, indent);
                DeclareUsedNamespaces(qualifier, usedPrefixes, indent);
            }
        }

        /// <summary>Writes one namespace declaration to the output.</summary>
        /// <param name="prefix">a namespace prefix (without colon) or a complete qname (when namespace == null)</param>
        /// <param name="namespace">the a namespace</param>
        /// <param name="usedPrefixes">a set containing currently used prefixes</param>
        /// <param name="indent">the current indent level</param>
        /// <exception cref="System.IO.IOException">Forwards all writer exceptions.</exception>
        private void DeclareNamespace(string prefix, string @namespace, ICollection<object> usedPrefixes, int indent)
        {
            if (@namespace == null)
            {
                // prefix contains qname, extract prefix and lookup namespace with prefix
                var qname = new QName(prefix);
                if (qname.HasPrefix())
                {
                    prefix = qname.GetPrefix();
                    // add colon for lookup
                    @namespace = XmpMetaFactory.GetSchemaRegistry().GetNamespaceUri(prefix + ":");
                    // prefix w/o colon
                    DeclareNamespace(prefix, @namespace, usedPrefixes, indent);
                }
                else
                {
                    return;
                }
            }
            if (!usedPrefixes.Contains(prefix))
            {
                WriteNewline();
                WriteIndent(indent);
                Write("xmlns:");
                Write(prefix);
                Write("=\"");
                Write(@namespace);
                Write('"');
                usedPrefixes.Add(prefix);
            }
        }

        /// <summary>Start the outer rdf:Description element, including all needed xmlns attributes.</summary>
        /// <remarks>
        /// Start the outer rdf:Description element, including all needed xmlns attributes.
        /// Leave the element open so that the compact form can add property attributes.
        /// </remarks>
        /// <exception cref="System.IO.IOException">If the writing to</exception>
        private void StartOuterRdfDescription(XmpNode schemaNode, int level)
        {
            WriteIndent(level + 1);
            Write(RdfSchemaStart);
            WriteTreeName();
            ICollection<object> usedPrefixes = new HashSet<object>();
            usedPrefixes.Add("xml");
            usedPrefixes.Add("rdf");
            DeclareUsedNamespaces(schemaNode, usedPrefixes, level + 3);
            Write('>');
            WriteNewline();
        }

        /// <summary>Write the </rdf:Description> end tag.</summary>
        /// <exception cref="System.IO.IOException"/>
        private void EndOuterRdfDescription(int level)
        {
            WriteIndent(level + 1);
            Write(RdfSchemaEnd);
            WriteNewline();
        }

        /// <summary>Recursively handles the "value" for a node.</summary>
        /// <remarks>
        /// Recursively handles the "value" for a node. It does not matter if it is a
        /// top level property, a field of a struct, or an item of an array. The
        /// indent is that for the property element. An xml:lang qualifier is written
        /// as an attribute of the property start tag, not by itself forcing the
        /// qualified property form. The patterns below mostly ignore attribute
        /// qualifiers like xml:lang. Except for the one struct case, attribute
        /// qualifiers don't affect the output form.
        /// <blockquote>
        /// <pre>
        /// &lt;ns:UnqualifiedSimpleProperty&gt;value&lt;/ns:UnqualifiedSimpleProperty&gt;
        /// &lt;ns:UnqualifiedStructProperty&gt; (If no rdf:resource qualifier)
        /// &lt;rdf:Description&gt;
        /// ... Fields, same forms as top level properties
        /// &lt;/rdf:Description&gt;
        /// &lt;/ns:UnqualifiedStructProperty&gt;
        /// &lt;ns:ResourceStructProperty rdf:resource=&quot;URI&quot;
        /// ... Fields as attributes
        /// &gt;
        /// &lt;ns:UnqualifiedArrayProperty&gt;
        /// &lt;rdf:Bag&gt; or Seq or Alt
        /// ... Array items as rdf:li elements, same forms as top level properties
        /// &lt;/rdf:Bag&gt;
        /// &lt;/ns:UnqualifiedArrayProperty&gt;
        /// &lt;ns:QualifiedProperty&gt;
        /// &lt;rdf:Description&gt;
        /// &lt;rdf:value&gt; ... Property &quot;value&quot; following the unqualified
        /// forms ... &lt;/rdf:value&gt;
        /// ... Qualifiers looking like named struct fields
        /// &lt;/rdf:Description&gt;
        /// &lt;/ns:QualifiedProperty&gt;
        /// </pre>
        /// </blockquote>
        /// </remarks>
        /// <param name="node">the property node</param>
        /// <param name="emitAsRdfValue">property shall be rendered as attribute rather than tag</param>
        /// <param name="useCanonicalRdf">
        /// use canonical form with inner description tag or
        /// the compact form with rdf:ParseType=&quot;resource&quot; attribute.
        /// </param>
        /// <param name="indent">the current indent level</param>
        /// <exception cref="System.IO.IOException">Forwards all writer exceptions.</exception>
        /// <exception cref="XmpException">If &quot;rdf:resource&quot; and general qualifiers are mixed.</exception>
        private void SerializeCanonicalRdfProperty(XmpNode node, bool useCanonicalRdf, bool emitAsRdfValue, int indent)
        {
            var emitEndTag = true;
            var indentEndTag = true;
            // Determine the XML element name. Open the start tag with the name and
            // attribute qualifiers.
            var elemName = node.Name;
            if (emitAsRdfValue)
            {
                elemName = "rdf:value";
            }
            else
            {
                if (XmpConstConstants.ArrayItemName.Equals(elemName))
                {
                    elemName = "rdf:li";
                }
            }
            WriteIndent(indent);
            Write('<');
            Write(elemName);
            var hasGeneralQualifiers = false;
            var hasRdfResourceQual = false;
            for (var it = node.IterateQualifier(); it.HasNext(); )
            {
                var qualifier = (XmpNode)it.Next();
                if (!RdfAttrQualifier.Contains(qualifier.Name))
                {
                    hasGeneralQualifiers = true;
                }
                else
                {
                    hasRdfResourceQual = "rdf:resource".Equals(qualifier.Name);
                    if (!emitAsRdfValue)
                    {
                        Write(' ');
                        Write(qualifier.Name);
                        Write("=\"");
                        AppendNodeValue(qualifier.Value, true);
                        Write('"');
                    }
                }
            }
            // Process the property according to the standard patterns.
            if (hasGeneralQualifiers && !emitAsRdfValue)
            {
                // This node has general, non-attribute, qualifiers. Emit using the
                // qualified property form.
                // ! The value is output by a recursive call ON THE SAME NODE with
                // emitAsRDFValue set.
                if (hasRdfResourceQual)
                {
                    throw new XmpException("Can't mix rdf:resource and general qualifiers", XmpErrorCode.BadRdf);
                }
                // Change serialization to canonical format with inner rdf:Description-tag
                // depending on option
                if (useCanonicalRdf)
                {
                    Write(">");
                    WriteNewline();
                    indent++;
                    WriteIndent(indent);
                    Write(RdfStructStart);
                    Write(">");
                }
                else
                {
                    Write(" rdf:parseType=\"Resource\">");
                }
                WriteNewline();
                SerializeCanonicalRdfProperty(node, useCanonicalRdf, true, indent + 1);
                for (var it1 = node.IterateQualifier(); it1.HasNext(); )
                {
                    var qualifier = (XmpNode)it1.Next();
                    if (!RdfAttrQualifier.Contains(qualifier.Name))
                    {
                        SerializeCanonicalRdfProperty(qualifier, useCanonicalRdf, false, indent + 1);
                    }
                }
                if (useCanonicalRdf)
                {
                    WriteIndent(indent);
                    Write(RdfStructEnd);
                    WriteNewline();
                    indent--;
                }
            }
            else
            {
                // This node has no general qualifiers. Emit using an unqualified form.
                if (!node.Options.IsCompositeProperty)
                {
                    // This is a simple property.
                    if (node.Options.IsUri)
                    {
                        Write(" rdf:resource=\"");
                        AppendNodeValue(node.Value, true);
                        Write("\"/>");
                        WriteNewline();
                        emitEndTag = false;
                    }
                    else
                    {
                        if (node.Value == null || string.Empty.Equals(node.Value))
                        {
                            Write("/>");
                            WriteNewline();
                            emitEndTag = false;
                        }
                        else
                        {
                            Write('>');
                            AppendNodeValue(node.Value, false);
                            indentEndTag = false;
                        }
                    }
                }
                else
                {
                    if (node.Options.IsArray)
                    {
                        // This is an array.
                        Write('>');
                        WriteNewline();
                        EmitRdfArrayTag(node, true, indent + 1);
                        if (node.Options.IsArrayAltText)
                        {
                            XmpNodeUtils.NormalizeLangArray(node);
                        }
                        for (var it1 = node.IterateChildren(); it1.HasNext(); )
                        {
                            var child = (XmpNode)it1.Next();
                            SerializeCanonicalRdfProperty(child, useCanonicalRdf, false, indent + 2);
                        }
                        EmitRdfArrayTag(node, false, indent + 1);
                    }
                    else
                    {
                        if (!hasRdfResourceQual)
                        {
                            // This is a "normal" struct, use the rdf:parseType="Resource" form.
                            if (!node.HasChildren)
                            {
                                // Change serialization to canonical format with inner rdf:Description-tag
                                // if option is set
                                if (useCanonicalRdf)
                                {
                                    Write(">");
                                    WriteNewline();
                                    WriteIndent(indent + 1);
                                    Write(RdfEmptyStruct);
                                }
                                else
                                {
                                    Write(" rdf:parseType=\"Resource\"/>");
                                    emitEndTag = false;
                                }
                                WriteNewline();
                            }
                            else
                            {
                                // Change serialization to canonical format with inner rdf:Description-tag
                                // if option is set
                                if (useCanonicalRdf)
                                {
                                    Write(">");
                                    WriteNewline();
                                    indent++;
                                    WriteIndent(indent);
                                    Write(RdfStructStart);
                                    Write(">");
                                }
                                else
                                {
                                    Write(" rdf:parseType=\"Resource\">");
                                }
                                WriteNewline();
                                for (var it1 = node.IterateChildren(); it1.HasNext(); )
                                {
                                    var child = (XmpNode)it1.Next();
                                    SerializeCanonicalRdfProperty(child, useCanonicalRdf, false, indent + 1);
                                }
                                if (useCanonicalRdf)
                                {
                                    WriteIndent(indent);
                                    Write(RdfStructEnd);
                                    WriteNewline();
                                    indent--;
                                }
                            }
                        }
                        else
                        {
                            // This is a struct with an rdf:resource attribute, use the
                            // "empty property element" form.
                            for (var it1 = node.IterateChildren(); it1.HasNext(); )
                            {
                                var child = (XmpNode)it1.Next();
                                if (!CanBeRdfAttrProp(child))
                                {
                                    throw new XmpException("Can't mix rdf:resource and complex fields", XmpErrorCode.BadRdf);
                                }
                                WriteNewline();
                                WriteIndent(indent + 1);
                                Write(' ');
                                Write(child.Name);
                                Write("=\"");
                                AppendNodeValue(child.Value, true);
                                Write('"');
                            }
                            Write("/>");
                            WriteNewline();
                            emitEndTag = false;
                        }
                    }
                }
            }
            // Emit the property element end tag.
            if (emitEndTag)
            {
                if (indentEndTag)
                {
                    WriteIndent(indent);
                }
                Write("</");
                Write(elemName);
                Write('>');
                WriteNewline();
            }
        }

        /// <summary>Writes the array start and end tags.</summary>
        /// <param name="arrayNode">an array node</param>
        /// <param name="isStartTag">flag if its the start or end tag</param>
        /// <param name="indent">the current indent level</param>
        /// <exception cref="System.IO.IOException">forwards writer exceptions</exception>
        private void EmitRdfArrayTag(XmpNode arrayNode, bool isStartTag, int indent)
        {
            if (isStartTag || arrayNode.HasChildren)
            {
                WriteIndent(indent);
                Write(isStartTag ? "<rdf:" : "</rdf:");

                if (arrayNode.Options.IsArrayAlternate)
                {
                    Write("Alt");
                }
                else
                {
                    Write(arrayNode.Options.IsArrayOrdered ? "Seq" : "Bag");
                }

                if (isStartTag && !arrayNode.HasChildren)
                {
                    Write("/>");
                }
                else
                {
                    Write(">");
                }
                WriteNewline();
            }
        }

        /// <summary>Serializes the node value in XML encoding.</summary>
        /// <remarks>
        /// Serializes the node value in XML encoding. Its used for tag bodies and
        /// attributes. <em>Note:</em> The attribute is always limited by quotes,
        /// thats why <c>&amp;apos;</c> is never serialized. <em>Note:</em>
        /// Control chars are written unescaped, but if the user uses others than tab, LF
        /// and CR the resulting XML will become invalid.
        /// </remarks>
        /// <param name="value">the value of the node</param>
        /// <param name="forAttribute">flag if value is an attribute value</param>
        /// <exception cref="System.IO.IOException"/>
        private void AppendNodeValue(string value, bool forAttribute)
        {
            if (value == null)
            {
                value = string.Empty;
            }
            Write(Utils.EscapeXml(value, forAttribute, true));
        }

        /// <summary>
        /// A node can be serialized as RDF-Attribute, if it meets the following conditions:
        /// <list type="bullet">
        /// <item>is not array item</item>
        /// <item>don't has qualifier</item>
        /// <item>is no URI</item>
        /// <item>is no composite property</item>
        /// </list>
        /// </summary>
        /// <param name="node">an XMPNode</param>
        /// <returns>Returns true if the node serialized as RDF-Attribute</returns>
        private static bool CanBeRdfAttrProp(XmpNode node)
        {
            return !node.HasQualifier && !node.Options.IsUri && !node.Options.IsCompositeProperty && !XmpConstConstants.ArrayItemName.Equals(node.Name);
        }

        /// <summary>Writes indents and automatically includes the baseindend from the options.</summary>
        /// <param name="times">number of indents to write</param>
        /// <exception cref="System.IO.IOException">forwards exception</exception>
        private void WriteIndent(int times)
        {
            for (var i = _options.BaseIndent + times; i > 0; i--)
            {
                _writer.Write(_options.Indent);
            }
        }

        /// <summary>Writes a char to the output.</summary>
        /// <param name="c">a char</param>
        /// <exception cref="System.IO.IOException">forwards writer exceptions</exception>
        private void Write(int c)
        {
            _writer.Write(c);
        }

        /// <summary>Writes a String to the output.</summary>
        /// <param name="str">a String</param>
        /// <exception cref="System.IO.IOException">forwards writer exceptions</exception>
        private void Write(string str)
        {
            _writer.Write(str);
        }

        /// <summary>Writes an amount of chars, mostly spaces</summary>
        /// <param name="number">number of chars</param>
        /// <param name="c">a char</param>
        /// <exception cref="System.IO.IOException"/>
        private void WriteChars(int number, char c)
        {
            for (; number > 0; number--)
            {
                _writer.Write(c);
            }
        }

        /// <summary>Writes a newline according to the options.</summary>
        /// <exception cref="System.IO.IOException">Forwards exception</exception>
        private void WriteNewline()
        {
            _writer.Write(_options.Newline);
        }
    }
}

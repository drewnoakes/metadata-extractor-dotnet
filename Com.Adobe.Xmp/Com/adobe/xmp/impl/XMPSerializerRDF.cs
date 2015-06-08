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
using Sharpen;

namespace Com.Adobe.Xmp.Impl
{
    /// <summary>Serializes the <code>XMPMeta</code>-object using the standard RDF serialization format.</summary>
    /// <remarks>
    /// Serializes the <code>XMPMeta</code>-object using the standard RDF serialization format.
    /// The output is written to an <code>OutputStream</code>
    /// according to the <code>SerializeOptions</code>.
    /// </remarks>
    /// <since>11.07.2006</since>
    public class XMPSerializerRDF
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
        internal static readonly ICollection<object> RdfAttrQualifier = new HashSet<object>(Arrays.AsList(new string[] { XMPConstConstants.XmlLang, "rdf:resource", "rdf:ID", "rdf:bagID", "rdf:nodeID" }));

        /// <summary>the metadata object to be serialized.</summary>
        private XMPMetaImpl xmp;

        /// <summary>the output stream to serialize to</summary>
        private CountOutputStream outputStream;

        /// <summary>this writer is used to do the actual serialization</summary>
        private OutputStreamWriter writer;

        /// <summary>the stored serialization options</summary>
        private SerializeOptions options;

        /// <summary>
        /// the size of one unicode char, for UTF-8 set to 1
        /// (Note: only valid for ASCII chars lower than 0x80),
        /// set to 2 in case of UTF-16
        /// </summary>
        private int unicodeSize = 1;

        /// <summary>
        /// the padding in the XMP Packet, or the length of the complete packet in
        /// case of option <em>exactPacketLength</em>.
        /// </summary>
        private int padding;

        // UTF-8
        /// <summary>The actual serialization.</summary>
        /// <param name="xmp">the metadata object to be serialized</param>
        /// <param name="out">outputStream the output stream to serialize to</param>
        /// <param name="options">the serialization options</param>
        /// <exception cref="Com.Adobe.Xmp.XMPException">If case of wrong options or any other serialization error.</exception>
        public virtual void Serialize(XMPMeta xmp, OutputStream @out, SerializeOptions options)
        {
            try
            {
                outputStream = new CountOutputStream(@out);
                writer = new OutputStreamWriter(outputStream, options.GetEncoding());
                this.xmp = (XMPMetaImpl)xmp;
                this.options = options;
                this.padding = options.GetPadding();
                writer = new OutputStreamWriter(outputStream, options.GetEncoding());
                CheckOptionsConsistence();
                // serializes the whole packet, but don't write the tail yet
                // and flush to make sure that the written bytes are calculated correctly
                string tailStr = SerializeAsRDF();
                writer.Flush();
                // adds padding
                AddPadding(tailStr.Length);
                // writes the tail
                Write(tailStr);
                writer.Flush();
                outputStream.Close();
            }
            catch (IOException)
            {
                throw new XMPException("Error writing to the OutputStream", XMPErrorConstants.Unknown);
            }
        }

        /// <summary>Calculates the padding according to the options and write it to the stream.</summary>
        /// <param name="tailLength">the length of the tail string</param>
        /// <exception cref="Com.Adobe.Xmp.XMPException">thrown if packet size is to small to fit the padding</exception>
        /// <exception cref="System.IO.IOException">forwards writer errors</exception>
        private void AddPadding(int tailLength)
        {
            if (options.GetExactPacketLength())
            {
                // the string length is equal to the length of the UTF-8 encoding
                int minSize = outputStream.GetBytesWritten() + tailLength * unicodeSize;
                if (minSize > padding)
                {
                    throw new XMPException("Can't fit into specified packet size", XMPErrorConstants.Badserialize);
                }
                padding -= minSize;
            }
            // Now the actual amount of padding to add.
            // fix rest of the padding according to Unicode unit size.
            padding /= unicodeSize;
            int newlineLen = options.GetNewline().Length;
            if (padding >= newlineLen)
            {
                padding -= newlineLen;
                // Write this newline last.
                while (padding >= (100 + newlineLen))
                {
                    WriteChars(100, ' ');
                    WriteNewline();
                    padding -= (100 + newlineLen);
                }
                WriteChars(padding, ' ');
                WriteNewline();
            }
            else
            {
                WriteChars(padding, ' ');
            }
        }

        /// <summary>Checks if the supplied options are consistent.</summary>
        /// <exception cref="Com.Adobe.Xmp.XMPException">Thrown if options are conflicting</exception>
        protected internal virtual void CheckOptionsConsistence()
        {
            if (options.GetEncodeUTF16BE() | options.GetEncodeUTF16LE())
            {
                unicodeSize = 2;
            }
            if (options.GetExactPacketLength())
            {
                if (options.GetOmitPacketWrapper() | options.GetIncludeThumbnailPad())
                {
                    throw new XMPException("Inconsistent options for exact size serialize", XMPErrorConstants.Badoptions);
                }
                if ((options.GetPadding() & (unicodeSize - 1)) != 0)
                {
                    throw new XMPException("Exact size must be a multiple of the Unicode element", XMPErrorConstants.Badoptions);
                }
            }
            else
            {
                if (options.GetReadOnlyPacket())
                {
                    if (options.GetOmitPacketWrapper() | options.GetIncludeThumbnailPad())
                    {
                        throw new XMPException("Inconsistent options for read-only packet", XMPErrorConstants.Badoptions);
                    }
                    padding = 0;
                }
                else
                {
                    if (options.GetOmitPacketWrapper())
                    {
                        if (options.GetIncludeThumbnailPad())
                        {
                            throw new XMPException("Inconsistent options for non-packet serialize", XMPErrorConstants.Badoptions);
                        }
                        padding = 0;
                    }
                    else
                    {
                        if (padding == 0)
                        {
                            padding = DefaultPad * unicodeSize;
                        }
                        if (options.GetIncludeThumbnailPad())
                        {
                            if (!xmp.DoesPropertyExist(XMPConstConstants.NsXmp, "Thumbnails"))
                            {
                                padding += 10000 * unicodeSize;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Writes the (optional) packet header and the outer rdf-tags.</summary>
        /// <returns>Returns the packet end processing instraction to be written after the padding.</returns>
        /// <exception cref="System.IO.IOException">Forwarded writer exceptions.</exception>
        /// <exception cref="Com.Adobe.Xmp.XMPException"></exception>
        private string SerializeAsRDF()
        {
            int level = 0;
            // Write the packet header PI.
            if (!options.GetOmitPacketWrapper())
            {
                WriteIndent(level);
                Write(PacketHeader);
                WriteNewline();
            }
            // Write the x:xmpmeta element's start tag.
            if (!options.GetOmitXmpMetaElement())
            {
                WriteIndent(level);
                Write(RdfXmpmetaStart);
                // Note: this flag can only be set by unit tests
                if (!options.GetOmitVersionAttribute())
                {
                    Write(XMPMetaFactory.GetVersionInfo().GetMessage());
                }
                Write("\">");
                WriteNewline();
                level++;
            }
            // Write the rdf:RDF start tag.
            WriteIndent(level);
            Write(RdfRdfStart);
            WriteNewline();
            // Write all of the properties.
            if (options.GetUseCanonicalFormat())
            {
                SerializeCanonicalRDFSchemas(level);
            }
            else
            {
                SerializeCompactRDFSchemas(level);
            }
            // Write the rdf:RDF end tag.
            WriteIndent(level);
            Write(RdfRdfEnd);
            WriteNewline();
            // Write the xmpmeta end tag.
            if (!options.GetOmitXmpMetaElement())
            {
                level--;
                WriteIndent(level);
                Write(RdfXmpmetaEnd);
                WriteNewline();
            }
            // Write the packet trailer PI into the tail string as UTF-8.
            string tailStr = string.Empty;
            if (!options.GetOmitPacketWrapper())
            {
                for (level = options.GetBaseIndent(); level > 0; level--)
                {
                    tailStr += options.GetIndent();
                }
                tailStr += PacketTrailer;
                tailStr += options.GetReadOnlyPacket() ? 'r' : 'w';
                tailStr += PacketTrailer2;
            }
            return tailStr;
        }

        /// <summary>Serializes the metadata in pretty-printed manner.</summary>
        /// <param name="level">indent level</param>
        /// <exception cref="System.IO.IOException">Forwarded writer exceptions</exception>
        /// <exception cref="Com.Adobe.Xmp.XMPException"></exception>
        private void SerializeCanonicalRDFSchemas(int level)
        {
            if (xmp.GetRoot().GetChildrenLength() > 0)
            {
                StartOuterRDFDescription(xmp.GetRoot(), level);
                for (Iterator it = xmp.GetRoot().IterateChildren(); it.HasNext(); )
                {
                    XMPNode currSchema = (XMPNode)it.Next();
                    SerializeCanonicalRDFSchema(currSchema, level);
                }
                EndOuterRDFDescription(level);
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
            string name = xmp.GetRoot().GetName();
            if (name != null)
            {
                AppendNodeValue(name, true);
            }
            Write('"');
        }

        /// <summary>Serializes the metadata in compact manner.</summary>
        /// <param name="level">indent level to start with</param>
        /// <exception cref="System.IO.IOException">Forwarded writer exceptions</exception>
        /// <exception cref="Com.Adobe.Xmp.XMPException"></exception>
        private void SerializeCompactRDFSchemas(int level)
        {
            // Begin the rdf:Description start tag.
            WriteIndent(level + 1);
            Write(RdfSchemaStart);
            WriteTreeName();
            // Write all necessary xmlns attributes.
            ICollection<object> usedPrefixes = new HashSet<object>();
            usedPrefixes.Add("xml");
            usedPrefixes.Add("rdf");
            for (Iterator it = xmp.GetRoot().IterateChildren(); it.HasNext(); )
            {
                XMPNode schema = (XMPNode)it.Next();
                DeclareUsedNamespaces(schema, usedPrefixes, level + 3);
            }
            // Write the top level "attrProps" and close the rdf:Description start tag.
            bool allAreAttrs = true;
            for (Iterator it_1 = xmp.GetRoot().IterateChildren(); it_1.HasNext(); )
            {
                XMPNode schema = (XMPNode)it_1.Next();
                allAreAttrs &= SerializeCompactRDFAttrProps(schema, level + 2);
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
            for (Iterator it_2 = xmp.GetRoot().IterateChildren(); it_2.HasNext(); )
            {
                XMPNode schema = (XMPNode)it_2.Next();
                SerializeCompactRDFElementProps(schema, level + 2);
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
        private bool SerializeCompactRDFAttrProps(XMPNode parentNode, int indent)
        {
            bool allAreAttrs = true;
            for (Iterator it = parentNode.IterateChildren(); it.HasNext(); )
            {
                XMPNode prop = (XMPNode)it.Next();
                if (CanBeRDFAttrProp(prop))
                {
                    WriteNewline();
                    WriteIndent(indent);
                    Write(prop.GetName());
                    Write("=\"");
                    AppendNodeValue(prop.GetValue(), true);
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
        /// <exception cref="Com.Adobe.Xmp.XMPException">If qualifier and element fields are mixed.</exception>
        private void SerializeCompactRDFElementProps(XMPNode parentNode, int indent)
        {
            for (Iterator it = parentNode.IterateChildren(); it.HasNext(); )
            {
                XMPNode node = (XMPNode)it.Next();
                if (CanBeRDFAttrProp(node))
                {
                    continue;
                }
                bool emitEndTag = true;
                bool indentEndTag = true;
                // Determine the XML element name, write the name part of the start tag. Look over the
                // qualifiers to decide on "normal" versus "rdf:value" form. Emit the attribute
                // qualifiers at the same time.
                string elemName = node.GetName();
                if (XMPConstConstants.ArrayItemName.Equals(elemName))
                {
                    elemName = "rdf:li";
                }
                WriteIndent(indent);
                Write('<');
                Write(elemName);
                bool hasGeneralQualifiers = false;
                bool hasRDFResourceQual = false;
                for (Iterator iq = node.IterateQualifier(); iq.HasNext(); )
                {
                    XMPNode qualifier = (XMPNode)iq.Next();
                    if (!RdfAttrQualifier.Contains(qualifier.GetName()))
                    {
                        hasGeneralQualifiers = true;
                    }
                    else
                    {
                        hasRDFResourceQual = "rdf:resource".Equals(qualifier.GetName());
                        Write(' ');
                        Write(qualifier.GetName());
                        Write("=\"");
                        AppendNodeValue(qualifier.GetValue(), true);
                        Write('"');
                    }
                }
                // Process the property according to the standard patterns.
                if (hasGeneralQualifiers)
                {
                    SerializeCompactRDFGeneralQualifier(indent, node);
                }
                else
                {
                    // This node has only attribute qualifiers. Emit as a property element.
                    if (!node.GetOptions().IsCompositeProperty())
                    {
                        object[] result = SerializeCompactRDFSimpleProp(node);
                        emitEndTag = ((bool)result[0]);
                        indentEndTag = ((bool)result[1]);
                    }
                    else
                    {
                        if (node.GetOptions().IsArray())
                        {
                            SerializeCompactRDFArrayProp(node, indent);
                        }
                        else
                        {
                            emitEndTag = SerializeCompactRDFStructProp(node, indent, hasRDFResourceQual);
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
        private object[] SerializeCompactRDFSimpleProp(XMPNode node)
        {
            // This is a simple property.
            bool emitEndTag = true;
            bool indentEndTag = true;
            if (node.GetOptions().IsURI())
            {
                Write(" rdf:resource=\"");
                AppendNodeValue(node.GetValue(), true);
                Write("\"/>");
                WriteNewline();
                emitEndTag = false;
            }
            else
            {
                if (node.GetValue() == null || node.GetValue().Length == 0)
                {
                    Write("/>");
                    WriteNewline();
                    emitEndTag = false;
                }
                else
                {
                    Write('>');
                    AppendNodeValue(node.GetValue(), false);
                    indentEndTag = false;
                }
            }
            return new object[] { emitEndTag, indentEndTag };
        }

        /// <summary>Serializes an array property.</summary>
        /// <param name="node">an XMPNode</param>
        /// <param name="indent">the current indent level</param>
        /// <exception cref="System.IO.IOException">Forwards the writer exceptions.</exception>
        /// <exception cref="Com.Adobe.Xmp.XMPException">If qualifier and element fields are mixed.</exception>
        private void SerializeCompactRDFArrayProp(XMPNode node, int indent)
        {
            // This is an array.
            Write('>');
            WriteNewline();
            EmitRDFArrayTag(node, true, indent + 1);
            if (node.GetOptions().IsArrayAltText())
            {
                XMPNodeUtils.NormalizeLangArray(node);
            }
            SerializeCompactRDFElementProps(node, indent + 2);
            EmitRDFArrayTag(node, false, indent + 1);
        }

        /// <summary>Serializes a struct property.</summary>
        /// <param name="node">an XMPNode</param>
        /// <param name="indent">the current indent level</param>
        /// <param name="hasRDFResourceQual">Flag if the element has resource qualifier</param>
        /// <returns>Returns true if an end flag shall be emitted.</returns>
        /// <exception cref="System.IO.IOException">Forwards the writer exceptions.</exception>
        /// <exception cref="Com.Adobe.Xmp.XMPException">If qualifier and element fields are mixed.</exception>
        private bool SerializeCompactRDFStructProp(XMPNode node, int indent, bool hasRDFResourceQual)
        {
            // This must be a struct.
            bool hasAttrFields = false;
            bool hasElemFields = false;
            bool emitEndTag = true;
            for (Iterator ic = node.IterateChildren(); ic.HasNext(); )
            {
                XMPNode field = (XMPNode)ic.Next();
                if (CanBeRDFAttrProp(field))
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
            if (hasRDFResourceQual && hasElemFields)
            {
                throw new XMPException("Can't mix rdf:resource qualifier and element fields", XMPErrorConstants.Badrdf);
            }
            if (!node.HasChildren())
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
                    SerializeCompactRDFAttrProps(node, indent + 1);
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
                        SerializeCompactRDFElementProps(node, indent + 1);
                    }
                    else
                    {
                        // Have a mix of attributes and elements, use an inner rdf:Description.
                        Write('>');
                        WriteNewline();
                        WriteIndent(indent + 1);
                        Write(RdfStructStart);
                        SerializeCompactRDFAttrProps(node, indent + 2);
                        Write(">");
                        WriteNewline();
                        SerializeCompactRDFElementProps(node, indent + 1);
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
        /// <exception cref="Com.Adobe.Xmp.XMPException">If qualifier and element fields are mixed.</exception>
        private void SerializeCompactRDFGeneralQualifier(int indent, XMPNode node)
        {
            // The node has general qualifiers, ones that can't be
            // attributes on a property element.
            // Emit using the qualified property pseudo-struct form. The
            // value is output by a call
            // to SerializePrettyRDFProperty with emitAsRDFValue set.
            Write(" rdf:parseType=\"Resource\">");
            WriteNewline();
            SerializeCanonicalRDFProperty(node, false, true, indent + 1);
            for (Iterator iq = node.IterateQualifier(); iq.HasNext(); )
            {
                XMPNode qualifier = (XMPNode)iq.Next();
                SerializeCanonicalRDFProperty(qualifier, false, false, indent + 1);
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
        /// <exception cref="Com.Adobe.Xmp.XMPException"></exception>
        private void SerializeCanonicalRDFSchema(XMPNode schemaNode, int level)
        {
            // Write each of the schema's actual properties.
            for (Iterator it = schemaNode.IterateChildren(); it.HasNext(); )
            {
                XMPNode propNode = (XMPNode)it.Next();
                SerializeCanonicalRDFProperty(propNode, options.GetUseCanonicalFormat(), false, level + 2);
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
        private void DeclareUsedNamespaces(XMPNode node, ICollection<object> usedPrefixes, int indent)
        {
            if (node.GetOptions().IsSchemaNode())
            {
                // The schema node name is the URI, the value is the prefix.
                string prefix = Runtime.Substring(node.GetValue(), 0, node.GetValue().Length - 1);
                DeclareNamespace(prefix, node.GetName(), usedPrefixes, indent);
            }
            else
            {
                if (node.GetOptions().IsStruct())
                {
                    for (Iterator it = node.IterateChildren(); it.HasNext(); )
                    {
                        XMPNode field = (XMPNode)it.Next();
                        DeclareNamespace(field.GetName(), null, usedPrefixes, indent);
                    }
                }
            }
            for (Iterator it_1 = node.IterateChildren(); it_1.HasNext(); )
            {
                XMPNode child = (XMPNode)it_1.Next();
                DeclareUsedNamespaces(child, usedPrefixes, indent);
            }
            for (Iterator it_2 = node.IterateQualifier(); it_2.HasNext(); )
            {
                XMPNode qualifier = (XMPNode)it_2.Next();
                DeclareNamespace(qualifier.GetName(), null, usedPrefixes, indent);
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
                QName qname = new QName(prefix);
                if (qname.HasPrefix())
                {
                    prefix = qname.GetPrefix();
                    // add colon for lookup
                    @namespace = XMPMetaFactory.GetSchemaRegistry().GetNamespaceURI(prefix + ":");
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
        private void StartOuterRDFDescription(XMPNode schemaNode, int level)
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
        private void EndOuterRDFDescription(int level)
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
        /// <param name="emitAsRDFValue">property shall be rendered as attribute rather than tag</param>
        /// <param name="useCanonicalRDF">
        /// use canonical form with inner description tag or
        /// the compact form with rdf:ParseType=&quot;resource&quot; attribute.
        /// </param>
        /// <param name="indent">the current indent level</param>
        /// <exception cref="System.IO.IOException">Forwards all writer exceptions.</exception>
        /// <exception cref="Com.Adobe.Xmp.XMPException">If &quot;rdf:resource&quot; and general qualifiers are mixed.</exception>
        private void SerializeCanonicalRDFProperty(XMPNode node, bool useCanonicalRDF, bool emitAsRDFValue, int indent)
        {
            bool emitEndTag = true;
            bool indentEndTag = true;
            // Determine the XML element name. Open the start tag with the name and
            // attribute qualifiers.
            string elemName = node.GetName();
            if (emitAsRDFValue)
            {
                elemName = "rdf:value";
            }
            else
            {
                if (XMPConstConstants.ArrayItemName.Equals(elemName))
                {
                    elemName = "rdf:li";
                }
            }
            WriteIndent(indent);
            Write('<');
            Write(elemName);
            bool hasGeneralQualifiers = false;
            bool hasRDFResourceQual = false;
            for (Iterator it = node.IterateQualifier(); it.HasNext(); )
            {
                XMPNode qualifier = (XMPNode)it.Next();
                if (!RdfAttrQualifier.Contains(qualifier.GetName()))
                {
                    hasGeneralQualifiers = true;
                }
                else
                {
                    hasRDFResourceQual = "rdf:resource".Equals(qualifier.GetName());
                    if (!emitAsRDFValue)
                    {
                        Write(' ');
                        Write(qualifier.GetName());
                        Write("=\"");
                        AppendNodeValue(qualifier.GetValue(), true);
                        Write('"');
                    }
                }
            }
            // Process the property according to the standard patterns.
            if (hasGeneralQualifiers && !emitAsRDFValue)
            {
                // This node has general, non-attribute, qualifiers. Emit using the
                // qualified property form.
                // ! The value is output by a recursive call ON THE SAME NODE with
                // emitAsRDFValue set.
                if (hasRDFResourceQual)
                {
                    throw new XMPException("Can't mix rdf:resource and general qualifiers", XMPErrorConstants.Badrdf);
                }
                // Change serialization to canonical format with inner rdf:Description-tag
                // depending on option
                if (useCanonicalRDF)
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
                SerializeCanonicalRDFProperty(node, useCanonicalRDF, true, indent + 1);
                for (Iterator it_1 = node.IterateQualifier(); it_1.HasNext(); )
                {
                    XMPNode qualifier = (XMPNode)it_1.Next();
                    if (!RdfAttrQualifier.Contains(qualifier.GetName()))
                    {
                        SerializeCanonicalRDFProperty(qualifier, useCanonicalRDF, false, indent + 1);
                    }
                }
                if (useCanonicalRDF)
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
                if (!node.GetOptions().IsCompositeProperty())
                {
                    // This is a simple property.
                    if (node.GetOptions().IsURI())
                    {
                        Write(" rdf:resource=\"");
                        AppendNodeValue(node.GetValue(), true);
                        Write("\"/>");
                        WriteNewline();
                        emitEndTag = false;
                    }
                    else
                    {
                        if (node.GetValue() == null || string.Empty.Equals(node.GetValue()))
                        {
                            Write("/>");
                            WriteNewline();
                            emitEndTag = false;
                        }
                        else
                        {
                            Write('>');
                            AppendNodeValue(node.GetValue(), false);
                            indentEndTag = false;
                        }
                    }
                }
                else
                {
                    if (node.GetOptions().IsArray())
                    {
                        // This is an array.
                        Write('>');
                        WriteNewline();
                        EmitRDFArrayTag(node, true, indent + 1);
                        if (node.GetOptions().IsArrayAltText())
                        {
                            XMPNodeUtils.NormalizeLangArray(node);
                        }
                        for (Iterator it_1 = node.IterateChildren(); it_1.HasNext(); )
                        {
                            XMPNode child = (XMPNode)it_1.Next();
                            SerializeCanonicalRDFProperty(child, useCanonicalRDF, false, indent + 2);
                        }
                        EmitRDFArrayTag(node, false, indent + 1);
                    }
                    else
                    {
                        if (!hasRDFResourceQual)
                        {
                            // This is a "normal" struct, use the rdf:parseType="Resource" form.
                            if (!node.HasChildren())
                            {
                                // Change serialization to canonical format with inner rdf:Description-tag
                                // if option is set
                                if (useCanonicalRDF)
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
                                if (useCanonicalRDF)
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
                                for (Iterator it_1 = node.IterateChildren(); it_1.HasNext(); )
                                {
                                    XMPNode child = (XMPNode)it_1.Next();
                                    SerializeCanonicalRDFProperty(child, useCanonicalRDF, false, indent + 1);
                                }
                                if (useCanonicalRDF)
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
                            for (Iterator it_1 = node.IterateChildren(); it_1.HasNext(); )
                            {
                                XMPNode child = (XMPNode)it_1.Next();
                                if (!CanBeRDFAttrProp(child))
                                {
                                    throw new XMPException("Can't mix rdf:resource and complex fields", XMPErrorConstants.Badrdf);
                                }
                                WriteNewline();
                                WriteIndent(indent + 1);
                                Write(' ');
                                Write(child.GetName());
                                Write("=\"");
                                AppendNodeValue(child.GetValue(), true);
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
        private void EmitRDFArrayTag(XMPNode arrayNode, bool isStartTag, int indent)
        {
            if (isStartTag || arrayNode.HasChildren())
            {
                WriteIndent(indent);
                Write(isStartTag ? "<rdf:" : "</rdf:");
                if (arrayNode.GetOptions().IsArrayAlternate())
                {
                    Write("Alt");
                }
                else
                {
                    if (arrayNode.GetOptions().IsArrayOrdered())
                    {
                        Write("Seq");
                    }
                    else
                    {
                        Write("Bag");
                    }
                }
                if (isStartTag && !arrayNode.HasChildren())
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
        /// thats why <code>&amp;apos;</code> is never serialized. <em>Note:</em>
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
            Write(Utils.EscapeXML(value, forAttribute, true));
        }

        /// <summary>
        /// A node can be serialized as RDF-Attribute, if it meets the following conditions:
        /// <ul>
        /// <li>is not array item
        /// <li>don't has qualifier
        /// <li>is no URI
        /// <li>is no composite property
        /// </ul>
        /// </summary>
        /// <param name="node">an XMPNode</param>
        /// <returns>Returns true if the node serialized as RDF-Attribute</returns>
        private static bool CanBeRDFAttrProp(XMPNode node)
        {
            return !node.HasQualifier() && !node.GetOptions().IsURI() && !node.GetOptions().IsCompositeProperty() && !XMPConstConstants.ArrayItemName.Equals(node.GetName());
        }

        /// <summary>Writes indents and automatically includes the baseindend from the options.</summary>
        /// <param name="times">number of indents to write</param>
        /// <exception cref="System.IO.IOException">forwards exception</exception>
        private void WriteIndent(int times)
        {
            for (int i = options.GetBaseIndent() + times; i > 0; i--)
            {
                writer.Write(options.GetIndent());
            }
        }

        /// <summary>Writes a char to the output.</summary>
        /// <param name="c">a char</param>
        /// <exception cref="System.IO.IOException">forwards writer exceptions</exception>
        private void Write(int c)
        {
            writer.Write(c);
        }

        /// <summary>Writes a String to the output.</summary>
        /// <param name="str">a String</param>
        /// <exception cref="System.IO.IOException">forwards writer exceptions</exception>
        private void Write(string str)
        {
            writer.Write(str);
        }

        /// <summary>Writes an amount of chars, mostly spaces</summary>
        /// <param name="number">number of chars</param>
        /// <param name="c">a char</param>
        /// <exception cref="System.IO.IOException"/>
        private void WriteChars(int number, char c)
        {
            for (; number > 0; number--)
            {
                writer.Write(c);
            }
        }

        /// <summary>Writes a newline according to the options.</summary>
        /// <exception cref="System.IO.IOException">Forwards exception</exception>
        private void WriteNewline()
        {
            writer.Write(options.GetNewline());
        }
    }
}

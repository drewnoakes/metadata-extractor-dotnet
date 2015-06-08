// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using System;
using System.IO;
using System.Xml;
using Com.Adobe.Xmp.Options;
using Sharpen;
using StringReader = Sharpen.StringReader;

namespace Com.Adobe.Xmp.Impl
{
    /// <summary>
    /// This class replaces the <c>ExpatAdapter.cpp</c> and does the
    /// XML-parsing and fixes the prefix.
    /// </summary>
    /// <remarks>
    /// This class replaces the <c>ExpatAdapter.cpp</c> and does the
    /// XML-parsing and fixes the prefix. After the parsing several normalisations
    /// are applied to the XMPTree.
    /// </remarks>
    /// <since>01.02.2006</since>
    public static class XmpMetaParser
    {
        private static readonly object XmpRdf = new object();

        // EMPTY
        /// <summary>
        /// Parses the input source into an XMP metadata object, including
        /// de-aliasing and normalisation.
        /// </summary>
        /// <param name="input">
        /// the input can be an <c>InputStream</c>, a <c>String</c> or
        /// a byte buffer containing the XMP packet.
        /// </param>
        /// <param name="options">the parse options</param>
        /// <returns>Returns the resulting XMP metadata object</returns>
        /// <exception cref="XmpException">Thrown if parsing or normalisation fails.</exception>
        public static IXmpMeta Parse(object input, ParseOptions options)
        {
            ParameterAsserts.AssertNotNull(input);
            options = options ?? new ParseOptions();
            XmlDocument document = ParseXml(input, options);
            bool xmpmetaRequired = options.GetRequireXmpMeta();
            object[] result = new object[3];
            result = FindRootNode(document, xmpmetaRequired, result);
            if (result != null && result[1] == XmpRdf)
            {
                XmpMeta xmp = ParseRdf.Parse((XmlNode)result[0]);
                xmp.SetPacketHeader((string)result[2]);
                // Check if the XMP object shall be normalized
                if (!options.GetOmitNormalization())
                {
                    return XmpNormalizer.Process(xmp, options);
                }
                return xmp;
            }
            // no appropriate root node found, return empty metadata object
            return new XmpMeta();
        }

        /// <summary>Parses the raw XML metadata packet considering the parsing options.</summary>
        /// <remarks>
        /// Parses the raw XML metadata packet considering the parsing options.
        /// Latin-1/ISO-8859-1 can be accepted when the input is a byte stream
        /// (some old toolkits versions such packets). The stream is
        /// then wrapped in another stream that converts Latin-1 to UTF-8.
        /// <para>
        /// If control characters shall be fixed, a reader is used that fixes the chars to spaces
        /// (if the input is a byte stream is has to be read as character stream).
        /// <para>
        /// Both options reduce the performance of the parser.
        /// </remarks>
        /// <param name="input">
        /// the input can be an <c>InputStream</c>, a <c>String</c> or
        /// a byte buffer containing the XMP packet.
        /// </param>
        /// <param name="options">the parsing options</param>
        /// <returns>Returns the parsed XML document or an exception.</returns>
        /// <exception cref="XmpException">Thrown if the parsing fails for different reasons</exception>
        private static XmlDocument ParseXml(object input, ParseOptions options)
        {
            var stream = input as InputStream;
            if (stream != null)
            {
                return ParseXmlFromInputStream(stream, options);
            }
            var sbytes = input as sbyte[];
            if (sbytes != null)
            {
                return ParseXmlFromBytebuffer(new ByteBuffer(sbytes), options);
            }
            return ParseXmlFromString((string)input, options);
        }

        /// <summary>
        /// Parses XML from an <see cref="InputStream"/>,
        /// fixing the encoding (Latin-1 to UTF-8) and illegal control character optionally.
        /// </summary>
        /// <param name="stream">an <c>InputStream</c></param>
        /// <param name="options">the parsing options</param>
        /// <returns>Returns an XML DOM-Document.</returns>
        /// <exception cref="XmpException">Thrown when the parsing fails.</exception>
        private static XmlDocument ParseXmlFromInputStream(InputStream stream, ParseOptions options)
        {
            if (!options.GetAcceptLatin1() && !options.GetFixControlChars())
            {
                return ParseInputSource(new InputSource(stream));
            }
            // load stream into bytebuffer
            try
            {
                ByteBuffer buffer = new ByteBuffer(stream);
                return ParseXmlFromBytebuffer(buffer, options);
            }
            catch (IOException e)
            {
                throw new XmpException("Error reading the XML-file", XmpErrorCode.Badstream, e);
            }
        }

        /// <summary>
        /// Parses XML from a byte buffer,
        /// fixing the encoding (Latin-1 to UTF-8) and illegal control character optionally.
        /// </summary>
        /// <param name="buffer">a byte buffer containing the XMP packet</param>
        /// <param name="options">the parsing options</param>
        /// <returns>Returns an XML DOM-Document.</returns>
        /// <exception cref="XmpException">Thrown when the parsing fails.</exception>
        private static XmlDocument ParseXmlFromBytebuffer(ByteBuffer buffer, ParseOptions options)
        {
            InputSource source = new InputSource(buffer.GetByteStream());
            try
            {
                return ParseInputSource(source);
            }
            catch (XmpException e)
            {
                if (e.GetErrorCode() == XmpErrorCode.Badxml || e.GetErrorCode() == XmpErrorCode.Badstream)
                {
                    if (options.GetAcceptLatin1())
                    {
                        buffer = Latin1Converter.Convert(buffer);
                    }
                    if (options.GetFixControlChars())
                    {
                        try
                        {
                            string encoding = buffer.GetEncoding();
                            StreamReader fixReader = new FixAsciiControlsReader(new InputStreamReader(buffer.GetByteStream(), encoding));
                            return ParseInputSource(new InputSource(fixReader));
                        }
                        catch (UnsupportedEncodingException)
                        {
                            // can normally not happen as the encoding is provided by a util function
                            throw new XmpException("Unsupported Encoding", XmpErrorCode.Internalfailure, e);
                        }
                    }
                    source = new InputSource(buffer.GetByteStream());
                    return ParseInputSource(source);
                }
                throw;
            }
        }

        /// <summary>
        /// Parses XML from a <see cref="string"/>, fixing the illegal control character optionally.
        /// </summary>
        /// <param name="input">a <c>String</c> containing the XMP packet</param>
        /// <param name="options">the parsing options</param>
        /// <returns>Returns an XML DOM-Document.</returns>
        /// <exception cref="XmpException">Thrown when the parsing fails.</exception>
        private static XmlDocument ParseXmlFromString(string input, ParseOptions options)
        {
            InputSource source = new InputSource(new StringReader(input));
            try
            {
                return ParseInputSource(source);
            }
            catch (XmpException e)
            {
                if (e.GetErrorCode() == XmpErrorCode.Badxml && options.GetFixControlChars())
                {
                    source = new InputSource(new FixAsciiControlsReader(new StringReader(input)));
                    return ParseInputSource(source);
                }
                throw;
            }
        }

        /// <summary>Runs the XML-Parser.</summary>
        /// <param name="source">an <c>InputSource</c></param>
        /// <returns>Returns an XML DOM-Document.</returns>
        /// <exception cref="XmpException">Wraps parsing and I/O-exceptions into an XMPException.</exception>
        private static XmlDocument ParseInputSource(InputSource source)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(source.GetStream());

                return doc;
            }
            catch (XmlException e)
            {
                throw new XmpException("XML parsing failure", XmpErrorCode.Badxml, e);
            }
            catch (IOException e)
            {
                throw new XmpException("Error reading the XML-file", XmpErrorCode.Badstream, e);
            }
            catch (Exception e)
            {
                throw new XmpException("XML Parser not correctly configured", XmpErrorCode.Unknown, e);
            }
        }

        /// <summary>Find the XML node that is the root of the XMP data tree.</summary>
        /// <remarks>
        /// Find the XML node that is the root of the XMP data tree. Generally this
        /// will be an outer node, but it could be anywhere if a general XML document
        /// is parsed (e.g. SVG). The XML parser counted all rdf:RDF and
        /// pxmp:XMP_Packet nodes, and kept a pointer to the last one. If there is
        /// more than one possible root use PickBestRoot to choose among them.
        /// <para>
        /// If there is a root node, try to extract the version of the previous XMP
        /// toolkit.
        /// <para>
        /// Pick the first x:xmpmeta among multiple root candidates. If there aren't
        /// any, pick the first bare rdf:RDF if that is allowed. The returned root is
        /// the rdf:RDF child if an x:xmpmeta element was chosen. The search is
        /// breadth first, so a higher level candiate is chosen over a lower level
        /// one that was textually earlier in the serialized XML.
        /// </remarks>
        /// <param name="root">the root of the xml document</param>
        /// <param name="xmpmetaRequired">
        /// flag if the xmpmeta-tag is still required, might be set
        /// initially to <c>true</c>, if the parse option "REQUIRE_XMP_META" is set
        /// </param>
        /// <param name="result">The result array that is filled during the recursive process.</param>
        /// <returns>
        /// Returns an array that contains the result or <c>null</c>.
        /// The array contains:
        /// <list type="bullet">
        /// <item>[0] - the rdf:RDF-node
        /// <item>[1] - an object that is either XMP_RDF or XMP_PLAIN (the latter is decrecated)
        /// <item>[2] - the body text of the xpacket-instruction.
        /// </list>
        /// </returns>
        private static object[] FindRootNode(XmlNode root, bool xmpmetaRequired, object[] result)
        {
            // Look among this parent's content for x:xapmeta or x:xmpmeta.
            // The recursion for x:xmpmeta is broader than the strictly defined choice,
            // but gives us smaller code.
            XmlNodeList children = root.ChildNodes;
            for (int i = 0; i < children.Count; i++)
            {
                root = children.Item(i);
                if (XmlNodeType.ProcessingInstruction == root.NodeType && XmpConstConstants.XmpPi.Equals(((XmlProcessingInstruction)root).Target))
                {
                    // Store the processing instructions content
                    if (result != null)
                    {
                        result[2] = ((XmlProcessingInstruction)root).Data;
                    }
                }
                else
                {
                    if (XmlNodeType.Text != root.NodeType && XmlNodeType.ProcessingInstruction != root.NodeType)
                    {
                        string rootNs = root.NamespaceURI;
                        string rootLocal = root.LocalName;
                        if ((XmpConstConstants.TagXmpmeta.Equals(rootLocal) || XmpConstConstants.TagXapmeta.Equals(rootLocal)) && XmpConstConstants.NsX.Equals(rootNs))
                        {
                            // by not passing the RequireXMPMeta-option, the rdf-Node will be valid
                            return FindRootNode(root, false, result);
                        }
                        if (!xmpmetaRequired && "RDF".Equals(rootLocal) && XmpConstConstants.NsRdf.Equals(rootNs))
                        {
                            if (result != null)
                            {
                                result[0] = root;
                                result[1] = XmpRdf;
                            }
                            return result;
                        }
                        // continue searching
                        object[] newResult = FindRootNode(root, xmpmetaRequired, result);
                        if (newResult != null)
                        {
                            return newResult;
                        }
                        continue;
                    }
                }
            }
            // no appropriate node has been found
            return null;
        }
    }
}

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
using System.Text;
using System.Xml;
using Com.Adobe.Xmp.Options;

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

        /// <summary>
        /// Parses an XMP metadata object from a stream, including de-aliasing and normalisation.
        /// </summary>
        /// <exception cref="XmpException">Thrown if parsing or normalisation fails.</exception>
        public static IXmpMeta Parse(Stream stream, ParseOptions options = null)
        {
            ParameterAsserts.AssertNotNull(stream);
            options = options ?? new ParseOptions();
            var doc = ParseXmlFromInputStream(stream, options);
            return ParseXmlDoc(doc, options);
        }

        /// <summary>
        /// Parses an XMP metadata object from a stream, including de-aliasing and normalisation.
        /// </summary>
        /// <exception cref="XmpException">Thrown if parsing or normalisation fails.</exception>
        public static IXmpMeta Parse(byte[] bytes, ParseOptions options = null)
        {
            ParameterAsserts.AssertNotNull(bytes);
            options = options ?? new ParseOptions();
            var doc = ParseXmlFromByteBuffer(new ByteBuffer(bytes), options);
            return ParseXmlDoc(doc, options);
        }

        /// <summary>
        /// Parses an XMP metadata object from a stream, including de-aliasing and normalisation.
        /// </summary>
        /// <exception cref="XmpException">Thrown if parsing or normalisation fails.</exception>
        public static IXmpMeta Parse(string xmlStr, ParseOptions options = null)
        {
            ParameterAsserts.AssertNotNull(xmlStr);
            options = options ?? new ParseOptions();
            var doc = ParseXmlString(xmlStr, options);
            return ParseXmlDoc(doc, options);
        }

        private static IXmpMeta ParseXmlDoc(XmlDocument document, ParseOptions options)
        {
            object[] result;
            if (FindRootNode(document, options.RequireXmpMeta, out result) && result[1] == XmpRdf)
            {
                var xmp = ParseRdf.Parse((XmlNode)result[0]);
                xmp.SetPacketHeader((string)result[2]);
                // Check if the XMP object shall be normalized
                if (!options.OmitNormalization)
                {
                    return XmpNormalizer.Process(xmp, options);
                }
                return xmp;
            }
            // no appropriate root node found, return empty metadata object
            return new XmpMeta();
        }

        /// <summary>
        /// Parses XML from an <see cref="Stream"/>,
        /// fixing the encoding (Latin-1 to UTF-8) and illegal control character optionally.
        /// </summary>
        /// <param name="stream">an <c>Stream</c></param>
        /// <param name="options">the parsing options</param>
        /// <returns>Returns an XML DOM-Document.</returns>
        /// <exception cref="XmpException">Thrown when the parsing fails.</exception>
        private static XmlDocument ParseXmlFromInputStream(Stream stream, ParseOptions options)
        {
            if (!options.AcceptLatin1 && !options.FixControlChars)
            {
                return ParseStream(stream);
            }
            // load stream into bytebuffer
            try
            {
                var buffer = new ByteBuffer(stream);
                return ParseXmlFromByteBuffer(buffer, options);
            }
            catch (IOException e)
            {
                throw new XmpException("Error reading the XML-file", XmpErrorCode.BadStream, e);
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
        private static XmlDocument ParseXmlFromByteBuffer(ByteBuffer buffer, ParseOptions options)
        {
            try
            {
                return ParseStream(buffer.GetByteStream());
            }
            catch (XmpException e)
            {
                if (e.GetErrorCode() == XmpErrorCode.BadXml || e.GetErrorCode() == XmpErrorCode.BadStream)
                {
                    if (options.AcceptLatin1)
                    {
                        buffer = Latin1Converter.Convert(buffer);
                    }
                    if (options.FixControlChars)
                    {
                        try
                        {
                            return ParseTextReader(new FixAsciiControlsReader(new StreamReader(buffer.GetByteStream(), buffer.GetEncoding())));
                        }
                        catch
                        {
                            // can normally not happen as the encoding is provided by a util function
                            throw new XmpException("Unsupported Encoding", XmpErrorCode.InternalFailure, e);
                        }
                    }
                    return ParseStream(buffer.GetByteStream());
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
        private static XmlDocument ParseXmlString(string input, ParseOptions options)
        {
            try
            {
                return ParseStream(new MemoryStream(Encoding.UTF8.GetBytes(input)));
            }
            catch (XmpException e)
            {
                if (e.GetErrorCode() == XmpErrorCode.BadXml && options.FixControlChars)
                    return ParseTextReader(new FixAsciiControlsReader(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(input)))));
                throw;
            }
        }

        /// <exception cref="XmpException">Wraps parsing and I/O-exceptions into an XMPException.</exception>
        private static XmlDocument ParseStream(Stream stream)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(stream);
                return doc;
            }
            catch (XmlException e)
            {
                throw new XmpException("XML parsing failure", XmpErrorCode.BadXml, e);
            }
            catch (IOException e)
            {
                throw new XmpException("Error reading the XML-file", XmpErrorCode.BadStream, e);
            }
            catch (Exception e)
            {
                throw new XmpException("XML Parser not correctly configured", XmpErrorCode.Unknown, e);
            }
        }
        /// <exception cref="XmpException">Wraps parsing and I/O-exceptions into an XMPException.</exception>
        private static XmlDocument ParseTextReader(TextReader reader)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(reader);
                return doc;
            }
            catch (XmlException e)
            {
                throw new XmpException("XML parsing failure", XmpErrorCode.BadXml, e);
            }
            catch (IOException e)
            {
                throw new XmpException("Error reading the XML-file", XmpErrorCode.BadStream, e);
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
        /// <para />
        /// If there is a root node, try to extract the version of the previous XMP
        /// toolkit.
        /// <para />
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
        /// <item>[0] - the rdf:RDF-node</item>
        /// <item>[1] - an object that is either XMP_RDF or XMP_PLAIN (the latter is decrecated)</item>
        /// <item>[2] - the body text of the xpacket-instruction.</item>
        /// </list>
        /// </returns>
        private static bool FindRootNode(XmlNode root, bool xmpmetaRequired, out object[] result)
        {
            result = new object[3];

            // Look among this parent's content for x:xapmeta or x:xmpmeta.
            // The recursion for x:xmpmeta is broader than the strictly defined choice,
            // but gives us smaller code.
            var children = root.ChildNodes;
            for (var i = 0; i < children.Count; i++)
            {
                root = children.Item(i);
                if (XmlNodeType.ProcessingInstruction == root.NodeType && XmpConstConstants.XmpPi.Equals(((XmlProcessingInstruction)root).Target))
                {
                    // Store the processing instructions content
                    result[2] = ((XmlProcessingInstruction)root).Data;
                }
                else
                {
                    if (XmlNodeType.Text != root.NodeType && XmlNodeType.ProcessingInstruction != root.NodeType)
                    {
                        var rootNs = root.NamespaceURI;
                        var rootLocal = root.LocalName;
                        if ((XmpConstConstants.TagXmpmeta.Equals(rootLocal) || XmpConstConstants.TagXapmeta.Equals(rootLocal)) && XmpConstConstants.NsX.Equals(rootNs))
                        {
                            // by not passing the RequireXMPMeta-option, the rdf-Node will be valid
                            return FindRootNode(root, false, out result);
                        }
                        if (!xmpmetaRequired && "RDF".Equals(rootLocal) && XmpConstConstants.NsRdf.Equals(rootNs))
                        {
                            if (result != null)
                            {
                                result[0] = root;
                                result[1] = XmpRdf;
                            }
                            return true;
                        }
                        // continue searching
                        if (FindRootNode(root, xmpmetaRequired, out result))
                            return true;
                    }
                }
            }
            // no appropriate node has been found
            result = null;
            return false;
        }
    }
}

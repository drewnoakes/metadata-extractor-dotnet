// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================
using System.IO;
using Com.Adobe.Xmp.Options;
using Sharpen;

namespace Com.Adobe.Xmp.Impl
{
    /// <summary>
    /// Serializes the <code>XMPMeta</code>-object to an <code>OutputStream</code> according to the
    /// <code>SerializeOptions</code>.
    /// </summary>
    /// <since>11.07.2006</since>
    public class XMPSerializerHelper
    {
        /// <summary>Static method to serialize the metadata object.</summary>
        /// <remarks>
        /// Static method to serialize the metadata object. For each serialisation, a new XMPSerializer
        /// instance is created, either XMPSerializerRDF or XMPSerializerPlain so thats its possible to
        /// serialialize the same XMPMeta objects in two threads.
        /// </remarks>
        /// <param name="xmp">a metadata implementation object</param>
        /// <param name="out">the output stream to serialize to</param>
        /// <param name="options">serialization options, can be <code>null</code> for default.</param>
        /// <exception cref="Com.Adobe.Xmp.XMPException"/>
        public static void Serialize(XMPMetaImpl xmp, OutputStream @out, SerializeOptions options)
        {
            options = options != null ? options : new SerializeOptions();
            // sort the internal data model on demand
            if (options.GetSort())
            {
                xmp.Sort();
            }
            new XMPSerializerRDF().Serialize(xmp, @out, options);
        }

        /// <summary>Serializes an <code>XMPMeta</code>-object as RDF into a string.</summary>
        /// <remarks>
        /// Serializes an <code>XMPMeta</code>-object as RDF into a string.
        /// <em>Note:</em> Encoding is forced to UTF-16 when serializing to a
        /// string to ensure the correctness of &quot;exact packet size&quot;.
        /// </remarks>
        /// <param name="xmp">a metadata implementation object</param>
        /// <param name="options">
        /// Options to control the serialization (see
        /// <see cref="Com.Adobe.Xmp.Options.SerializeOptions"/>
        /// ).
        /// </param>
        /// <returns>Returns a string containing the serialized RDF.</returns>
        /// <exception cref="Com.Adobe.Xmp.XMPException">on serializsation errors.</exception>
        public static string SerializeToString(XMPMetaImpl xmp, SerializeOptions options)
        {
            // forces the encoding to be UTF-16 to get the correct string length
            options = options != null ? options : new SerializeOptions();
            options.SetEncodeUTF16BE(true);
            ByteArrayOutputStream @out = new ByteArrayOutputStream(2048);
            Serialize(xmp, @out, options);
            try
            {
                return @out.ToString(options.GetEncoding());
            }
            catch (UnsupportedEncodingException)
            {
                // cannot happen as UTF-8/16LE/BE is required to be implemented in
                // Java
                return @out.ToString();
            }
        }

        /// <summary>Serializes an <code>XMPMeta</code>-object as RDF into a byte buffer.</summary>
        /// <param name="xmp">a metadata implementation object</param>
        /// <param name="options">
        /// Options to control the serialization (see
        /// <see cref="Com.Adobe.Xmp.Options.SerializeOptions"/>
        /// ).
        /// </param>
        /// <returns>Returns a byte buffer containing the serialized RDF.</returns>
        /// <exception cref="Com.Adobe.Xmp.XMPException">on serializsation errors.</exception>
        public static sbyte[] SerializeToBuffer(XMPMetaImpl xmp, SerializeOptions options)
        {
            ByteArrayOutputStream @out = new ByteArrayOutputStream(2048);
            Serialize(xmp, @out, options);
            return @out.ToByteArray();
        }
    }
}

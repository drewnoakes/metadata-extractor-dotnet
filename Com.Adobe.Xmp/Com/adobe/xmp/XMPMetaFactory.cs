//=================================================================================================
//ADOBE SYSTEMS INCORPORATED
//Copyright 2006-2007 Adobe Systems Incorporated
//All Rights Reserved
//
//NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
//of the Adobe license agreement accompanying it.
//=================================================================================================

using System;
using Com.Adobe.Xmp.Impl;
using Com.Adobe.Xmp.Options;
using Sharpen;

namespace Com.Adobe.Xmp
{
    /// <summary>Creates <code>XMPMeta</code>-instances from an <code>InputStream</code></summary>
    /// <since>30.01.2006</since>
    public sealed class XmpMetaFactory
    {
        /// <summary>The singleton instance of the <code>XMPSchemaRegistry</code>.</summary>
        private static IXmpSchemaRegistry _schema = new XmpSchemaRegistry();

        /// <summary>cache for version info</summary>
        private static IXmpVersionInfo _versionInfo = null;

        /// <summary>Hides public constructor</summary>
        private XmpMetaFactory()
        {
        }

        // EMPTY
        /// <returns>Returns the singleton instance of the <code>XMPSchemaRegistry</code>.</returns>
        public static IXmpSchemaRegistry GetSchemaRegistry()
        {
            return _schema;
        }

        /// <returns>Returns an empty <code>XMPMeta</code>-object.</returns>
        public static IXmpMeta Create()
        {
            return new XmpMeta();
        }

        /// <summary>
        /// These functions support parsing serialized RDF into an XMP object, and serailizing an XMP
        /// object into RDF.
        /// </summary>
        /// <remarks>
        /// These functions support parsing serialized RDF into an XMP object, and serailizing an XMP
        /// object into RDF. The input for parsing may be any valid Unicode
        /// encoding. ISO Latin-1 is also recognized, but its use is strongly discouraged. Serialization
        /// is always as UTF-8.
        /// <p>
        /// <code>parseFromBuffer()</code> parses RDF from an <code>InputStream</code>. The encoding
        /// is recognized automatically.
        /// </remarks>
        /// <param name="in">an <code>InputStream</code></param>
        /// <param name="options">
        /// Options controlling the parsing.<br />
        /// The available options are:
        /// <ul>
        /// <li> XMP_REQUIRE_XMPMETA - The &lt;x:xmpmeta&gt; XML element is required around
        /// <tt>&lt;rdf:RDF&gt;</tt>.
        /// <li> XMP_STRICT_ALIASING - Do not reconcile alias differences, throw an exception.
        /// </ul>
        /// <em>Note:</em>The XMP_STRICT_ALIASING option is not yet implemented.
        /// </param>
        /// <returns>Returns the <code>XMPMeta</code>-object created from the input.</returns>
        /// <exception cref="XmpException">If the file is not well-formed XML or if the parsing fails.</exception>
        /// <exception cref="XmpException"/>
        public static IXmpMeta Parse(InputStream @in, ParseOptions options = null)
        {
            return XmpMetaParser.Parse(@in, options);
        }

        /// <summary>Creates an <code>XMPMeta</code>-object from a string.</summary>
        /// <seealso cref="ParseFromString(string, Com.Adobe.Xmp.Options.ParseOptions)"/>
        /// <param name="packet">a String contain an XMP-file.</param>
        /// <param name="options">Options controlling the parsing.</param>
        /// <returns>Returns the <code>XMPMeta</code>-object created from the input.</returns>
        /// <exception cref="XmpException">If the file is not well-formed XML or if the parsing fails.</exception>
        /// <exception cref="XmpException"/>
        public static IXmpMeta ParseFromString(string packet, ParseOptions options = null)
        {
            return XmpMetaParser.Parse(packet, options);
        }

        /// <summary>Creates an <code>XMPMeta</code>-object from a byte-buffer.</summary>
        /// <seealso cref="Parse(System.IO.InputStream, Com.Adobe.Xmp.Options.ParseOptions)"/>
        /// <param name="buffer">a String contain an XMP-file.</param>
        /// <param name="options">Options controlling the parsing.</param>
        /// <returns>Returns the <code>XMPMeta</code>-object created from the input.</returns>
        /// <exception cref="XmpException">If the file is not well-formed XML or if the parsing fails.</exception>
        /// <exception cref="XmpException"/>
        public static IXmpMeta ParseFromBuffer(sbyte[] buffer, ParseOptions options = null)
        {
            return XmpMetaParser.Parse(buffer, options);
        }

        /// <summary>Serializes an <code>XMPMeta</code>-object as RDF into an <code>OutputStream</code>.</summary>
        /// <param name="xmp">a metadata object</param>
        /// <param name="options">
        /// Options to control the serialization (see
        /// <see cref="Com.Adobe.Xmp.Options.SerializeOptions"/>
        /// ).
        /// </param>
        /// <param name="out">an <code>OutputStream</code> to write the serialized RDF to.</param>
        /// <exception cref="XmpException">on serializsation errors.</exception>
        /// <exception cref="XmpException"/>
        public static void Serialize(IXmpMeta xmp, OutputStream @out, SerializeOptions options = null)
        {
            AssertImplementation(xmp);
            XmpSerializerHelper.Serialize((XmpMeta)xmp, @out, options);
        }

        /// <summary>Serializes an <code>XMPMeta</code>-object as RDF into a byte buffer.</summary>
        /// <param name="xmp">a metadata object</param>
        /// <param name="options">
        /// Options to control the serialization (see
        /// <see cref="Com.Adobe.Xmp.Options.SerializeOptions"/>
        /// ).
        /// </param>
        /// <returns>Returns a byte buffer containing the serialized RDF.</returns>
        /// <exception cref="XmpException">on serializsation errors.</exception>
        /// <exception cref="XmpException"/>
        public static sbyte[] SerializeToBuffer(IXmpMeta xmp, SerializeOptions options)
        {
            AssertImplementation(xmp);
            return XmpSerializerHelper.SerializeToBuffer((XmpMeta)xmp, options);
        }

        /// <summary>Serializes an <code>XMPMeta</code>-object as RDF into a string.</summary>
        /// <remarks>
        /// Serializes an <code>XMPMeta</code>-object as RDF into a string. <em>Note:</em> Encoding
        /// is ignored when serializing to a string.
        /// </remarks>
        /// <param name="xmp">a metadata object</param>
        /// <param name="options">
        /// Options to control the serialization (see
        /// <see cref="Com.Adobe.Xmp.Options.SerializeOptions"/>
        /// ).
        /// </param>
        /// <returns>Returns a string containing the serialized RDF.</returns>
        /// <exception cref="XmpException">on serializsation errors.</exception>
        /// <exception cref="XmpException"/>
        public static string SerializeToString(IXmpMeta xmp, SerializeOptions options)
        {
            AssertImplementation(xmp);
            return XmpSerializerHelper.SerializeToString((XmpMeta)xmp, options);
        }

        /// <param name="xmp">Asserts that xmp is compatible to <code>XMPMetaImpl</code>.s</param>
        private static void AssertImplementation(IXmpMeta xmp)
        {
            if (!(xmp is XmpMeta))
            {
                throw new NotSupportedException("The serializing service works only" + "with the XMPMeta implementation of this library");
            }
        }

        /// <summary>Resets the schema registry to its original state (creates a new one).</summary>
        /// <remarks>
        /// Resets the schema registry to its original state (creates a new one).
        /// Be careful this might break all existing XMPMeta-objects and should be used
        /// only for testing purpurses.
        /// </remarks>
        public static void Reset()
        {
            _schema = new XmpSchemaRegistry();
        }

        /// <summary>Obtain version information.</summary>
        /// <remarks>
        /// Obtain version information. The XMPVersionInfo singleton is created the first time
        /// its requested.
        /// </remarks>
        /// <returns>Returns the version information.</returns>
        public static IXmpVersionInfo GetVersionInfo()
        {
            lock (typeof(XmpMetaFactory))
            {
                if (_versionInfo == null)
                {
                    try
                    {
                        int major = 5;
                        int minor = 1;
                        int micro = 0;
                        int engBuild = 3;
                        bool debug = false;
                        // Adobe XMP Core 5.0-jc001 DEBUG-<branch>.<changelist>, 2009 Jan 28 15:22:38-CET
                        string message = "Adobe XMP Core 5.1.0-jc003";
                        _versionInfo = new XmpVersionInfo274(major, minor, micro, debug, engBuild, message);
                    }
                    catch (Exception e)
                    {
                        // EMTPY, severe error would be detected during the tests
                        Console.Out.Println(e);
                    }
                }
                return _versionInfo;
            }
        }

        private sealed class XmpVersionInfo274 : IXmpVersionInfo
        {
            public XmpVersionInfo274(int major, int minor, int micro, bool debug, int engBuild, string message)
            {
                this._major = major;
                this._minor = minor;
                this._micro = micro;
                this._debug = debug;
                this._engBuild = engBuild;
                this._message = message;
            }

            public int GetMajor()
            {
                return _major;
            }

            public int GetMinor()
            {
                return _minor;
            }

            public int GetMicro()
            {
                return _micro;
            }

            public bool IsDebug()
            {
                return _debug;
            }

            public int GetBuild()
            {
                return _engBuild;
            }

            public string GetMessage()
            {
                return _message;
            }

            public override string ToString()
            {
                return _message;
            }

            private readonly int _major;

            private readonly int _minor;

            private readonly int _micro;

            private readonly bool _debug;

            private readonly int _engBuild;

            private readonly string _message;
        }
    }
}

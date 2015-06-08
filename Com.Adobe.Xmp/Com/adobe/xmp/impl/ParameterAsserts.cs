// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

namespace Com.Adobe.Xmp.Impl
{
    /// <since>11.08.2006</since>
    internal static class ParameterAsserts
    {
        // EMPTY
        /// <summary>Asserts that an array name is set.</summary>
        /// <param name="arrayName">an array name</param>
        /// <exception cref="Com.Adobe.Xmp.XMPException">Array name is null or empty</exception>
        public static void AssertArrayName(string arrayName)
        {
            if (arrayName == null || arrayName.Length == 0)
            {
                throw new XMPException("Empty array name", XMPErrorConstants.Badparam);
            }
        }

        /// <summary>Asserts that a property name is set.</summary>
        /// <param name="propName">a property name or path</param>
        /// <exception cref="Com.Adobe.Xmp.XMPException">Property name is null or empty</exception>
        public static void AssertPropName(string propName)
        {
            if (propName == null || propName.Length == 0)
            {
                throw new XMPException("Empty property name", XMPErrorConstants.Badparam);
            }
        }

        /// <summary>Asserts that a schema namespace is set.</summary>
        /// <param name="schemaNS">a schema namespace</param>
        /// <exception cref="Com.Adobe.Xmp.XMPException">Schema is null or empty</exception>
        public static void AssertSchemaNS(string schemaNS)
        {
            if (schemaNS == null || schemaNS.Length == 0)
            {
                throw new XMPException("Empty schema namespace URI", XMPErrorConstants.Badparam);
            }
        }

        /// <summary>Asserts that a prefix is set.</summary>
        /// <param name="prefix">a prefix</param>
        /// <exception cref="Com.Adobe.Xmp.XMPException">Prefix is null or empty</exception>
        public static void AssertPrefix(string prefix)
        {
            if (prefix == null || prefix.Length == 0)
            {
                throw new XMPException("Empty prefix", XMPErrorConstants.Badparam);
            }
        }

        /// <summary>Asserts that a specific language is set.</summary>
        /// <param name="specificLang">a specific lang</param>
        /// <exception cref="Com.Adobe.Xmp.XMPException">Specific language is null or empty</exception>
        public static void AssertSpecificLang(string specificLang)
        {
            if (specificLang == null || specificLang.Length == 0)
            {
                throw new XMPException("Empty specific language", XMPErrorConstants.Badparam);
            }
        }

        /// <summary>Asserts that a struct name is set.</summary>
        /// <param name="structName">a struct name</param>
        /// <exception cref="Com.Adobe.Xmp.XMPException">Struct name is null or empty</exception>
        public static void AssertStructName(string structName)
        {
            if (structName == null || structName.Length == 0)
            {
                throw new XMPException("Empty array name", XMPErrorConstants.Badparam);
            }
        }

        /// <summary>Asserts that any string parameter is set.</summary>
        /// <param name="param">any string parameter</param>
        /// <exception cref="Com.Adobe.Xmp.XMPException">Thrown if the parameter is null or has length 0.</exception>
        public static void AssertNotNull(object param)
        {
            if (param == null)
            {
                throw new XMPException("Parameter must not be null", XMPErrorConstants.Badparam);
            }
            else
            {
                if ((param is string) && ((string)param).Length == 0)
                {
                    throw new XMPException("Parameter must not be null or empty", XMPErrorConstants.Badparam);
                }
            }
        }

        /// <summary>
        /// Asserts that the xmp object is of this implemention
        /// (
        /// <see cref="XMPMetaImpl"/>
        /// ).
        /// </summary>
        /// <param name="xmp">the XMP object</param>
        /// <exception cref="Com.Adobe.Xmp.XMPException">A wrong implentaion is used.</exception>
        public static void AssertImplementation(XMPMeta xmp)
        {
            if (xmp == null)
            {
                throw new XMPException("Parameter must not be null", XMPErrorConstants.Badparam);
            }
            else
            {
                if (!(xmp is XMPMetaImpl))
                {
                    throw new XMPException("The XMPMeta-object is not compatible with this implementation", XMPErrorConstants.Badparam);
                }
            }
        }
    }
}

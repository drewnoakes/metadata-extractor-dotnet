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
        /// <exception cref="XmpException">Array name is null or empty</exception>
        public static void AssertArrayName(string arrayName)
        {
            if (arrayName == null || arrayName.Length == 0)
            {
                throw new XmpException("Empty array name", XmpErrorConstants.Badparam);
            }
        }

        /// <summary>Asserts that a property name is set.</summary>
        /// <param name="propName">a property name or path</param>
        /// <exception cref="XmpException">Property name is null or empty</exception>
        public static void AssertPropName(string propName)
        {
            if (propName == null || propName.Length == 0)
            {
                throw new XmpException("Empty property name", XmpErrorConstants.Badparam);
            }
        }

        /// <summary>Asserts that a schema namespace is set.</summary>
        /// <param name="schemaNs">a schema namespace</param>
        /// <exception cref="XmpException">Schema is null or empty</exception>
        public static void AssertSchemaNs(string schemaNs)
        {
            if (schemaNs == null || schemaNs.Length == 0)
            {
                throw new XmpException("Empty schema namespace URI", XmpErrorConstants.Badparam);
            }
        }

        /// <summary>Asserts that a prefix is set.</summary>
        /// <param name="prefix">a prefix</param>
        /// <exception cref="XmpException">Prefix is null or empty</exception>
        public static void AssertPrefix(string prefix)
        {
            if (prefix == null || prefix.Length == 0)
            {
                throw new XmpException("Empty prefix", XmpErrorConstants.Badparam);
            }
        }

        /// <summary>Asserts that a specific language is set.</summary>
        /// <param name="specificLang">a specific lang</param>
        /// <exception cref="XmpException">Specific language is null or empty</exception>
        public static void AssertSpecificLang(string specificLang)
        {
            if (specificLang == null || specificLang.Length == 0)
            {
                throw new XmpException("Empty specific language", XmpErrorConstants.Badparam);
            }
        }

        /// <summary>Asserts that a struct name is set.</summary>
        /// <param name="structName">a struct name</param>
        /// <exception cref="XmpException">Struct name is null or empty</exception>
        public static void AssertStructName(string structName)
        {
            if (structName == null || structName.Length == 0)
            {
                throw new XmpException("Empty array name", XmpErrorConstants.Badparam);
            }
        }

        /// <summary>Asserts that any string parameter is set.</summary>
        /// <param name="param">any string parameter</param>
        /// <exception cref="XmpException">Thrown if the parameter is null or has length 0.</exception>
        public static void AssertNotNull(object param)
        {
            if (param == null)
            {
                throw new XmpException("Parameter must not be null", XmpErrorConstants.Badparam);
            }
            var s = param as string;
            if (s != null && s.Length == 0)
            {
                throw new XmpException("Parameter must not be null or empty", XmpErrorConstants.Badparam);
            }
        }

        /// <summary>
        /// Asserts that the xmp object is of this implemention
        /// (
        /// <see cref="XmpMeta"/>
        /// ).
        /// </summary>
        /// <param name="xmp">the XMP object</param>
        /// <exception cref="XmpException">A wrong implentaion is used.</exception>
        public static void AssertImplementation(IXmpMeta xmp)
        {
            if (xmp == null)
            {
                throw new XmpException("Parameter must not be null", XmpErrorConstants.Badparam);
            }
            if (!(xmp is XmpMeta))
            {
                throw new XmpException("The XMPMeta-object is not compatible with this implementation", XmpErrorConstants.Badparam);
            }
        }
    }
}

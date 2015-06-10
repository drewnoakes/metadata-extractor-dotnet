// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using Com.Adobe.Xmp.Impl;
using Com.Adobe.Xmp.Impl.Xpath;

namespace Com.Adobe.Xmp
{
    /// <summary>Utility services for the metadata object.</summary>
    /// <remarks>
    /// Utility services for the metadata object. It has only public static functions, you cannot create
    /// an object. These are all functions that layer cleanly on top of the core XMP toolkit.
    /// <para />
    /// These functions provide support for composing path expressions to deeply nested properties. The
    /// functions <c>XMPMeta</c> such as <c>getProperty()</c>,
    /// <c>getArrayItem()</c> and <c>getStructField()</c> provide easy access to top
    /// level simple properties, items in top level arrays, and fields of top level structs. They do not
    /// provide convenient access to more complex things like fields several levels deep in a complex
    /// struct, or fields within an array of structs, or items of an array that is a field of a struct.
    /// These functions can also be used to compose paths to top level array items or struct fields so
    /// that you can use the binary accessors like <c>getPropertyAsInteger()</c>.
    /// <para />
    /// You can use these functions is to compose a complete path expression, or all but the last
    /// component. Suppose you have a property that is an array of integers within a struct. You can
    /// access one of the array items like this:
    /// <para />
    /// <blockquote>
    /// <pre>
    /// String path = XMPPathFactory.composeStructFieldPath (schemaNS, &quot;Struct&quot;, fieldNS,
    /// &quot;Array&quot;);
    /// String path += XMPPathFactory.composeArrayItemPath (schemaNS, &quot;Array&quot; index);
    /// PropertyInteger result = xmpObj.getPropertyAsInteger(schemaNS, path);
    /// </pre>
    /// </blockquote> You could also use this code if you want the string form of the integer:
    /// <blockquote>
    /// <pre>
    /// String path = XMPPathFactory.composeStructFieldPath (schemaNS, &quot;Struct&quot;, fieldNS,
    /// &quot;Array&quot;);
    /// PropertyText xmpObj.getArrayItem (schemaNS, path, index);
    /// </pre>
    /// </blockquote>
    /// <para />
    /// <em>Note:</em> It might look confusing that the schemaNS is passed in all of the calls above.
    /// This is because the XMP toolkit keeps the top level &quot;schema&quot; namespace separate from
    /// the rest of the path expression.
    /// <em>Note:</em> These methods are much simpler than in the C++-API, they don't check the given
    /// path or array indices.
    /// </remarks>
    /// <since>25.01.2006</since>
    public static class XmpPathFactory
    {
        /// <summary>Compose the path expression for an item in an array.</summary>
        /// <param name="arrayName">
        /// The name of the array. May be a general path expression, must not be
        /// <c>null</c> or the empty string.
        /// </param>
        /// <param name="itemIndex">
        /// The index of the desired item. Arrays in XMP are indexed from 1.
        /// 0 and below means last array item and renders as <c>[last()]</c>.
        /// </param>
        /// <returns>
        /// Returns the composed path basing on fullPath. This will be of the form
        /// <tt>ns:arrayName[i]</tt>, where &quot;ns&quot; is the prefix for schemaNS and
        /// &quot;i&quot; is the decimal representation of itemIndex.
        /// </returns>
        /// <exception cref="XmpException">Throws exeption if index zero is used.</exception>
        /// <exception cref="XmpException"/>
        public static string ComposeArrayItemPath(string arrayName, int itemIndex)
        {
            if (itemIndex > 0)
            {
                return arrayName + '[' + itemIndex + ']';
            }
            if (itemIndex == XmpConstConstants.ArrayLastItem)
            {
                return arrayName + "[last()]";
            }
            throw new XmpException("Array index must be larger than zero", XmpErrorCode.BadIndex);
        }

        /// <summary>Compose the path expression for a field in a struct.</summary>
        /// <remarks>
        /// Compose the path expression for a field in a struct. The result can be added to the
        /// path of
        /// </remarks>
        /// <param name="fieldNs">
        /// The namespace URI for the field. Must not be <c>null</c> or the empty
        /// string.
        /// </param>
        /// <param name="fieldName">
        /// The name of the field. Must be a simple XML name, must not be
        /// <c>null</c> or the empty string.
        /// </param>
        /// <returns>
        /// Returns the composed path. This will be of the form
        /// <tt>ns:structName/fNS:fieldName</tt>, where &quot;ns&quot; is the prefix for
        /// schemaNS and &quot;fNS&quot; is the prefix for fieldNS.
        /// </returns>
        /// <exception cref="XmpException">Thrown if the path to create is not valid.</exception>
        /// <exception cref="XmpException"/>
        public static string ComposeStructFieldPath(string fieldNs, string fieldName)
        {
            AssertFieldNs(fieldNs);
            AssertFieldName(fieldName);
            var fieldPath = XmpPathParser.ExpandXPath(fieldNs, fieldName);
            if (fieldPath.Size() != 2)
            {
                throw new XmpException("The field name must be simple", XmpErrorCode.BadXPath);
            }
            return '/' + fieldPath.GetSegment(XmpPath.StepRootProp).GetName();
        }

        /// <summary>Compose the path expression for a qualifier.</summary>
        /// <param name="qualNs">
        /// The namespace URI for the qualifier. May be <c>null</c> or the empty
        /// string if the qualifier is in the XML empty namespace.
        /// </param>
        /// <param name="qualName">
        /// The name of the qualifier. Must be a simple XML name, must not be
        /// <c>null</c> or the empty string.
        /// </param>
        /// <returns>
        /// Returns the composed path. This will be of the form
        /// <tt>ns:propName/?qNS:qualName</tt>, where &quot;ns&quot; is the prefix for
        /// schemaNS and &quot;qNS&quot; is the prefix for qualNS.
        /// </returns>
        /// <exception cref="XmpException">Thrown if the path to create is not valid.</exception>
        /// <exception cref="XmpException"/>
        public static string ComposeQualifierPath(string qualNs, string qualName)
        {
            AssertQualNs(qualNs);
            AssertQualName(qualName);
            var qualPath = XmpPathParser.ExpandXPath(qualNs, qualName);
            if (qualPath.Size() != 2)
            {
                throw new XmpException("The qualifier name must be simple", XmpErrorCode.BadXPath);
            }
            return "/?" + qualPath.GetSegment(XmpPath.StepRootProp).GetName();
        }

        /// <summary>Compose the path expression to select an alternate item by language.</summary>
        /// <remarks>
        /// Compose the path expression to select an alternate item by language. The
        /// path syntax allows two forms of &quot;content addressing&quot; that may
        /// be used to select an item in an array of alternatives. The form used in
        /// ComposeLangSelector lets you select an item in an alt-text array based on
        /// the value of its <tt>xml:lang</tt> qualifier. The other form of content
        /// addressing is shown in ComposeFieldSelector. \note ComposeLangSelector
        /// does not supplant SetLocalizedText or GetLocalizedText. They should
        /// generally be used, as they provide extra logic to choose the appropriate
        /// language and maintain consistency with the 'x-default' value.
        /// ComposeLangSelector gives you an path expression that is explicitly and
        /// only for the language given in the langName parameter.
        /// </remarks>
        /// <param name="arrayName">
        /// The name of the array. May be a general path expression, must
        /// not be <c>null</c> or the empty string.
        /// </param>
        /// <param name="langName">The RFC 3066 code for the desired language.</param>
        /// <returns>
        /// Returns the composed path. This will be of the form
        /// <tt>ns:arrayName[@xml:lang='langName']</tt>, where
        /// &quot;ns&quot; is the prefix for schemaNS.
        /// </returns>
        public static string ComposeLangSelector(string arrayName, string langName)
        {
            return arrayName + "[?xml:lang=\"" + Utils.NormalizeLangValue(langName) + "\"]";
        }

        /// <summary>Compose the path expression to select an alternate item by a field's value.</summary>
        /// <remarks>
        /// Compose the path expression to select an alternate item by a field's value. The path syntax
        /// allows two forms of &quot;content addressing&quot; that may be used to select an item in an
        /// array of alternatives. The form used in ComposeFieldSelector lets you select an item in an
        /// array of structs based on the value of one of the fields in the structs. The other form of
        /// content addressing is shown in ComposeLangSelector. For example, consider a simple struct
        /// that has two fields, the name of a city and the URI of an FTP site in that city. Use this to
        /// create an array of download alternatives. You can show the user a popup built from the values
        /// of the city fields. You can then get the corresponding URI as follows:
        /// <para />
        /// <blockquote>
        /// <pre>
        /// String path = composeFieldSelector ( schemaNS, &quot;Downloads&quot;, fieldNS,
        /// &quot;City&quot;, chosenCity );
        /// XMPProperty prop = xmpObj.getStructField ( schemaNS, path, fieldNS, &quot;URI&quot; );
        /// </pre>
        /// </blockquote>
        /// </remarks>
        /// <param name="arrayName">
        /// The name of the array. May be a general path expression, must not be
        /// <c>null</c> or the empty string.
        /// </param>
        /// <param name="fieldNs">
        /// The namespace URI for the field used as the selector. Must not be
        /// <c>null</c> or the empty string.
        /// </param>
        /// <param name="fieldName">
        /// The name of the field used as the selector. Must be a simple XML name, must
        /// not be <c>null</c> or the empty string. It must be the name of a field that is
        /// itself simple.
        /// </param>
        /// <param name="fieldValue">The desired value of the field.</param>
        /// <returns>
        /// Returns the composed path. This will be of the form
        /// <tt>ns:arrayName[fNS:fieldName='fieldValue']</tt>, where &quot;ns&quot; is the
        /// prefix for schemaNS and &quot;fNS&quot; is the prefix for fieldNS.
        /// </returns>
        /// <exception cref="XmpException">Thrown if the path to create is not valid.</exception>
        /// <exception cref="XmpException"/>
        public static string ComposeFieldSelector(string arrayName, string fieldNs, string fieldName, string fieldValue)
        {
            var fieldPath = XmpPathParser.ExpandXPath(fieldNs, fieldName);
            if (fieldPath.Size() != 2)
            {
                throw new XmpException("The fieldName name must be simple", XmpErrorCode.BadXPath);
            }
            return arrayName + '[' + fieldPath.GetSegment(XmpPath.StepRootProp).GetName() + "=\"" + fieldValue + "\"]";
        }

        /// <summary>ParameterAsserts that a qualifier namespace is set.</summary>
        /// <param name="qualNs">a qualifier namespace</param>
        /// <exception cref="XmpException">Qualifier schema is null or empty</exception>
        /// <exception cref="XmpException"/>
        private static void AssertQualNs(string qualNs)
        {
            if (string.IsNullOrEmpty(qualNs))
            {
                throw new XmpException("Empty qualifier namespace URI", XmpErrorCode.BadSchema);
            }
        }

        /// <summary>ParameterAsserts that a qualifier name is set.</summary>
        /// <param name="qualName">a qualifier name or path</param>
        /// <exception cref="XmpException">Qualifier name is null or empty</exception>
        /// <exception cref="XmpException"/>
        private static void AssertQualName(string qualName)
        {
            if (string.IsNullOrEmpty(qualName))
            {
                throw new XmpException("Empty qualifier name", XmpErrorCode.BadXPath);
            }
        }

        /// <summary>ParameterAsserts that a struct field namespace is set.</summary>
        /// <param name="fieldNs">a struct field namespace</param>
        /// <exception cref="XmpException">Struct field schema is null or empty</exception>
        /// <exception cref="XmpException"/>
        private static void AssertFieldNs(string fieldNs)
        {
            if (string.IsNullOrEmpty(fieldNs))
            {
                throw new XmpException("Empty field namespace URI", XmpErrorCode.BadSchema);
            }
        }

        /// <summary>ParameterAsserts that a struct field name is set.</summary>
        /// <param name="fieldName">a struct field name or path</param>
        /// <exception cref="XmpException">Struct field name is null or empty</exception>
        /// <exception cref="XmpException"/>
        private static void AssertFieldName(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new XmpException("Empty f name", XmpErrorCode.BadXPath);
            }
        }
    }
}

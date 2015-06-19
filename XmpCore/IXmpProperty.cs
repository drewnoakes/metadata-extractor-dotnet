// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using XmpCore.Options;

namespace XmpCore
{
    /// <summary>This interface is used to return a text property together with its and options.</summary>
    /// <since>23.01.2006</since>
    public interface IXmpProperty
    {
        /// <value>Returns the value of the property.</value>
        string Value { get; }

        /// <value>Returns the options of the property.</value>
        PropertyOptions Options { get; }

        /// <summary>
        /// Only set by <see cref="IXmpMeta.GetLocalizedText(string, string, string, string)"/>.
        /// </summary>
        /// <value>Returns the language of the alt-text item.</value>
        string Language { get; }
    }
}

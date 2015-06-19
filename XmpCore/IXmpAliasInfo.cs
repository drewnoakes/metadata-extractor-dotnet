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
    /// <summary>This interface is used to return info about an alias.</summary>
    /// <since>27.01.2006</since>
    public interface IXmpAliasInfo
    {
        /// <returns>Returns Returns the namespace URI for the base property.</returns>
        string GetNamespace();

        /// <returns>Returns the default prefix for the given base property.</returns>
        string GetPrefix();

        /// <returns>Returns the path of the base property.</returns>
        string GetPropName();

        /// <returns>
        /// Returns the kind of the alias. This can be a direct alias
        /// (ARRAY), a simple property to an ordered array
        /// (ARRAY_ORDERED), to an alternate array
        /// (ARRAY_ALTERNATE) or to an alternate text array
        /// (ARRAY_ALT_TEXT).
        /// </returns>
        AliasOptions GetAliasForm();
    }
}

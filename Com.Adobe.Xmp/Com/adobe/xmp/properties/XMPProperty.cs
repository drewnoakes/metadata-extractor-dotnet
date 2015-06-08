// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using Com.Adobe.Xmp.Options;

namespace Com.Adobe.Xmp.Properties
{
    /// <summary>This interface is used to return a text property together with its and options.</summary>
    /// <since>23.01.2006</since>
    public interface XMPProperty
    {
        /// <returns>Returns the value of the property.</returns>
        string GetValue();

        /// <returns>Returns the options of the property.</returns>
        PropertyOptions GetOptions();

        /// <summary>
        /// Only set by
        /// <see cref="Com.Adobe.Xmp.XMPMeta.GetLocalizedText(string, string, string, string)"/>
        /// .
        /// </summary>
        /// <returns>Returns the language of the alt-text item.</returns>
        string GetLanguage();
    }
}

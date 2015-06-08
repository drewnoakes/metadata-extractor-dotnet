// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

namespace Com.Adobe.Xmp
{
    /// <summary>XMP Toolkit Version Information.</summary>
    /// <remarks>
    /// XMP Toolkit Version Information.
    /// <p>
    /// Version information for the XMP toolkit is stored in the jar-library and available through a
    /// runtime call, <see cref="XmpMetaFactory.GetVersionInfo()"/>, addition static version numbers are
    /// defined in "version.properties".
    /// </remarks>
    /// <since>23.01.2006</since>
    public interface IXmpVersionInfo
    {
        /// <returns>Returns the primary release number, the "1" in version "1.2.3".</returns>
        int GetMajor();

        /// <returns>Returns the secondary release number, the "2" in version "1.2.3".</returns>
        int GetMinor();

        /// <returns>Returns the tertiary release number, the "3" in version "1.2.3".</returns>
        int GetMicro();

        /// <returns>Returns a rolling build number, monotonically increasing in a release.</returns>
        int GetBuild();

        /// <returns>Returns true if this is a debug build.</returns>
        bool IsDebug();

        /// <returns>Returns a comprehensive version information string.</returns>
        string GetMessage();
    }
}

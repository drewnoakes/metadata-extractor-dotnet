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
    public enum XmpErrorCode
    {
        Unknown = 0,

        BadParam = 4,

        BadValue = 5,

        InternalFailure = 9,

        BadSchema = 101,

        BadXPath = 102,

        BadOptions = 103,

        BadIndex = 104,

        BadSerialize = 107,

        BadXml = 201,

        BadRdf = 202,

        BadXmp = 203,

        /// <summary>This code is introduced by Java.</summary>
        BadStream = 204
    }
}

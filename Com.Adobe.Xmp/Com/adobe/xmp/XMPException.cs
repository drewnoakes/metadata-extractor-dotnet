// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

using System;

namespace Com.Adobe.Xmp
{
    /// <summary>This exception wraps all errors that occur in the XMP Toolkit.</summary>
    /// <since>16.02.2006</since>
    [Serializable]
    public class XmpException : Exception
    {
        /// <summary>the errorCode of the XMP toolkit</summary>
        private readonly XmpErrorCode _errorCode;

        /// <summary>Constructs an exception with a message and an error code.</summary>
        /// <param name="message">the message</param>
        /// <param name="errorCode">the error code</param>
        public XmpException(string message, XmpErrorCode errorCode)
            : base(message)
        {
            _errorCode = errorCode;
        }

        /// <summary>Constructs an exception with a message, an error code and a <code>Throwable</code></summary>
        /// <param name="message">the error message.</param>
        /// <param name="errorCode">the error code</param>
        /// <param name="t">the exception source</param>
        public XmpException(string message, XmpErrorCode errorCode, Exception t)
            : base(message, t)
        {
            _errorCode = errorCode;
        }

        /// <returns>Returns the error code.</returns>
        public virtual XmpErrorCode GetErrorCode()
        {
            return _errorCode;
        }
    }
}

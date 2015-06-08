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
    public class XMPException : Exception
    {
        /// <summary>the errorCode of the XMP toolkit</summary>
        private readonly int errorCode;

        /// <summary>Constructs an exception with a message and an error code.</summary>
        /// <param name="message">the message</param>
        /// <param name="errorCode">the error code</param>
        public XMPException(string message, int errorCode)
            : base(message)
        {
            this.errorCode = errorCode;
        }

        /// <summary>Constructs an exception with a message, an error code and a <code>Throwable</code></summary>
        /// <param name="message">the error message.</param>
        /// <param name="errorCode">the error code</param>
        /// <param name="t">the exception source</param>
        public XMPException(string message, int errorCode, Exception t)
            : base(message, t)
        {
            this.errorCode = errorCode;
        }

        /// <returns>Returns the errorCode.</returns>
        public virtual int GetErrorCode()
        {
            return errorCode;
        }
    }
}

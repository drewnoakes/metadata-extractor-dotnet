// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================

namespace XmpCore.Impl
{
    /// <since>09.11.2006</since>
    public sealed class QName
    {
        /// <summary>XML namespace prefix</summary>
        private readonly string _prefix;

        /// <summary>XML localname</summary>
        private readonly string _localName;

        /// <summary>Splits a qname into prefix and localname.</summary>
        /// <param name="qname">a QName</param>
        public QName(string qname)
        {
            var colon = qname.IndexOf(':');
            if (colon >= 0)
            {
                _prefix = qname.Substring (0, colon - 0);
                _localName = qname.Substring (colon + 1);
            }
            else
            {
                _prefix = string.Empty;
                _localName = qname;
            }
        }

        /// <summary>Constructor that initializes the fields</summary>
        /// <param name="prefix">the prefix</param>
        /// <param name="localName">the name</param>
        public QName(string prefix, string localName)
        {
            _prefix = prefix;
            _localName = localName;
        }

        /// <returns>Returns whether the QName has a prefix.</returns>
        public bool HasPrefix()
        {
            return !string.IsNullOrEmpty(_prefix);
        }

        /// <returns>the localName</returns>
        public string GetLocalName()
        {
            return _localName;
        }

        /// <returns>the prefix</returns>
        public string GetPrefix()
        {
            return _prefix;
        }
    }
}

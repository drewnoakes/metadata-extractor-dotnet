// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================
using Sharpen;

namespace Com.Adobe.Xmp.Impl
{
    /// <since>09.11.2006</since>
    public class QName
    {
        /// <summary>XML namespace prefix</summary>
        private string prefix;

        /// <summary>XML localname</summary>
        private string localName;

        /// <summary>Splits a qname into prefix and localname.</summary>
        /// <param name="qname">a QName</param>
        public QName(string qname)
        {
            int colon = qname.IndexOf(':');
            if (colon >= 0)
            {
                prefix = Sharpen.Runtime.Substring(qname, 0, colon);
                localName = Sharpen.Runtime.Substring(qname, colon + 1);
            }
            else
            {
                prefix = string.Empty;
                localName = qname;
            }
        }

        /// <summary>Constructor that initializes the fields</summary>
        /// <param name="prefix">the prefix</param>
        /// <param name="localName">the name</param>
        public QName(string prefix, string localName)
        {
            this.prefix = prefix;
            this.localName = localName;
        }

        /// <returns>Returns whether the QName has a prefix.</returns>
        public virtual bool HasPrefix()
        {
            return prefix != null && prefix.Length > 0;
        }

        /// <returns>the localName</returns>
        public virtual string GetLocalName()
        {
            return localName;
        }

        /// <returns>the prefix</returns>
        public virtual string GetPrefix()
        {
            return prefix;
        }
    }
}

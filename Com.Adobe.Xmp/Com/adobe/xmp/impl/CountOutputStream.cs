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
    /// <summary>An <code>OutputStream</code> that counts the written bytes.</summary>
    /// <since>08.11.2006</since>
    public sealed class CountOutputStream : OutputStream
    {
        /// <summary>the decorated output stream</summary>
        private readonly OutputStream @out;

        /// <summary>the byte counter</summary>
        private int bytesWritten = 0;

        /// <summary>Constructor with providing the output stream to decorate.</summary>
        /// <param name="out">an <code>OutputStream</code></param>
        internal CountOutputStream(OutputStream @out)
        {
            this.@out = @out;
        }

        /// <summary>Counts the written bytes.</summary>
        /// <seealso cref="System.IO.OutputStream.Write(sbyte[], int, int)"/>
        /// <exception cref="System.IO.IOException"/>
        public override void Write(sbyte[] buf, int off, int len)
        {
            @out.Write(buf, off, len);
            bytesWritten += len;
        }

        /// <summary>Counts the written bytes.</summary>
        /// <seealso cref="System.IO.OutputStream.Write(sbyte[])"/>
        /// <exception cref="System.IO.IOException"/>
        public override void Write(sbyte[] buf)
        {
            @out.Write(buf);
            bytesWritten += buf.Length;
        }

        /// <summary>Counts the written bytes.</summary>
        /// <seealso cref="System.IO.OutputStream.Write(int)"/>
        /// <exception cref="System.IO.IOException"/>
        public override void Write(int b)
        {
            @out.Write(b);
            bytesWritten++;
        }

        /// <returns>the bytesWritten</returns>
        public int GetBytesWritten()
        {
            return bytesWritten;
        }
    }
}

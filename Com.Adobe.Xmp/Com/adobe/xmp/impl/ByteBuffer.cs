// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================
using System;
using System.IO;
using Sharpen;

namespace Com.Adobe.Xmp.Impl
{
    /// <summary>Byte buffer container including length of valid data.</summary>
    /// <since>11.10.2006</since>
    public class ByteBuffer
    {
        private sbyte[] buffer;

        private int length;

        private string encoding = null;

        /// <param name="initialCapacity">the initial capacity for this buffer</param>
        public ByteBuffer(int initialCapacity)
        {
            this.buffer = new sbyte[initialCapacity];
            this.length = 0;
        }

        /// <param name="buffer">a byte array that will be wrapped with <code>ByteBuffer</code>.</param>
        public ByteBuffer(sbyte[] buffer)
        {
            this.buffer = buffer;
            this.length = buffer.Length;
        }

        /// <param name="buffer">a byte array that will be wrapped with <code>ByteBuffer</code>.</param>
        /// <param name="length">the length of valid bytes in the array</param>
        public ByteBuffer(sbyte[] buffer, int length)
        {
            if (length > buffer.Length)
            {
                throw new IndexOutOfRangeException("Valid length exceeds the buffer length.");
            }
            this.buffer = buffer;
            this.length = length;
        }

        /// <summary>Loads the stream into a buffer.</summary>
        /// <param name="in">an InputStream</param>
        /// <exception cref="System.IO.IOException">If the stream cannot be read.</exception>
        public ByteBuffer(InputStream @in)
        {
            // load stream into buffer
            int chunk = 16384;
            this.length = 0;
            this.buffer = new sbyte[chunk];
            int read;
            while ((read = @in.Read(this.buffer, this.length, chunk)) > 0)
            {
                this.length += read;
                if (read == chunk)
                {
                    EnsureCapacity(length + chunk);
                }
                else
                {
                    break;
                }
            }
        }

        /// <param name="buffer">a byte array that will be wrapped with <code>ByteBuffer</code>.</param>
        /// <param name="offset">the offset of the provided buffer.</param>
        /// <param name="length">the length of valid bytes in the array</param>
        public ByteBuffer(sbyte[] buffer, int offset, int length)
        {
            if (length > buffer.Length - offset)
            {
                throw new IndexOutOfRangeException("Valid length exceeds the buffer length.");
            }
            this.buffer = new sbyte[length];
            System.Array.Copy(buffer, offset, this.buffer, 0, length);
            this.length = length;
        }

        /// <returns>Returns a byte stream that is limited to the valid amount of bytes.</returns>
        public virtual InputStream GetByteStream()
        {
            return new ByteArrayInputStream(buffer, 0, length);
        }

        /// <returns>
        /// Returns the length, that means the number of valid bytes, of the buffer;
        /// the inner byte array might be bigger than that.
        /// </returns>
        public virtual int Length()
        {
            return length;
        }

        //    /**
        //     * <em>Note:</em> Only the byte up to length are valid!
        //     * @return Returns the inner byte buffer.
        //     */
        //    public byte[] getBuffer()
        //    {
        //        return buffer;
        //    }
        /// <param name="index">the index to retrieve the byte from</param>
        /// <returns>Returns a byte from the buffer</returns>
        public virtual sbyte ByteAt(int index)
        {
            if (index < length)
            {
                return buffer[index];
            }
            else
            {
                throw new IndexOutOfRangeException("The index exceeds the valid buffer area");
            }
        }

        /// <param name="index">the index to retrieve a byte as int or char.</param>
        /// <returns>Returns a byte from the buffer</returns>
        public virtual int CharAt(int index)
        {
            if (index < length)
            {
                return buffer[index] & unchecked((int)(0xFF));
            }
            else
            {
                throw new IndexOutOfRangeException("The index exceeds the valid buffer area");
            }
        }

        /// <summary>Appends a byte to the buffer.</summary>
        /// <param name="b">a byte</param>
        public virtual void Append(sbyte b)
        {
            EnsureCapacity(length + 1);
            buffer[length++] = b;
        }

        /// <summary>Appends a byte array or part of to the buffer.</summary>
        /// <param name="bytes">a byte array</param>
        /// <param name="offset">an offset with</param>
        /// <param name="len"/>
        public virtual void Append(sbyte[] bytes, int offset, int len)
        {
            EnsureCapacity(length + len);
            System.Array.Copy(bytes, offset, buffer, length, len);
            length += len;
        }

        /// <summary>Append a byte array to the buffer</summary>
        /// <param name="bytes">a byte array</param>
        public virtual void Append(sbyte[] bytes)
        {
            Append(bytes, 0, bytes.Length);
        }

        /// <summary>Append another buffer to this buffer.</summary>
        /// <param name="anotherBuffer">another <code>ByteBuffer</code></param>
        public virtual void Append(Com.Adobe.Xmp.Impl.ByteBuffer anotherBuffer)
        {
            Append(anotherBuffer.buffer, 0, anotherBuffer.length);
        }

        /// <summary>Detects the encoding of the byte buffer, stores and returns it.</summary>
        /// <remarks>
        /// Detects the encoding of the byte buffer, stores and returns it.
        /// Only UTF-8, UTF-16LE/BE and UTF-32LE/BE are recognized.
        /// <em>Note:</em> UTF-32 flavors are not supported by Java, the XML-parser will complain.
        /// </remarks>
        /// <returns>Returns the encoding string.</returns>
        public virtual string GetEncoding()
        {
            if (encoding == null)
            {
                // needs four byte at maximum to determine encoding
                if (length < 2)
                {
                    // only one byte length must be UTF-8
                    encoding = "UTF-8";
                }
                else
                {
                    if (buffer[0] == 0)
                    {
                        // These cases are:
                        //   00 nn -- -- - Big endian UTF-16
                        //   00 00 00 nn - Big endian UTF-32
                        //   00 00 FE FF - Big endian UTF 32
                        if (length < 4 || buffer[1] != 0)
                        {
                            encoding = "UTF-16BE";
                        }
                        else
                        {
                            if ((buffer[2] & unchecked((int)(0xFF))) == unchecked((int)(0xFE)) && (buffer[3] & unchecked((int)(0xFF))) == unchecked((int)(0xFF)))
                            {
                                encoding = "UTF-32BE";
                            }
                            else
                            {
                                encoding = "UTF-32";
                            }
                        }
                    }
                    else
                    {
                        if ((buffer[0] & unchecked((int)(0xFF))) < unchecked((int)(0x80)))
                        {
                            // These cases are:
                            //   nn mm -- -- - UTF-8, includes EF BB BF case
                            //   nn 00 -- -- - Little endian UTF-16
                            if (buffer[1] != 0)
                            {
                                encoding = "UTF-8";
                            }
                            else
                            {
                                if (length < 4 || buffer[2] != 0)
                                {
                                    encoding = "UTF-16LE";
                                }
                                else
                                {
                                    encoding = "UTF-32LE";
                                }
                            }
                        }
                        else
                        {
                            // These cases are:
                            //   EF BB BF -- - UTF-8
                            //   FE FF -- -- - Big endian UTF-16
                            //   FF FE 00 00 - Little endian UTF-32
                            //   FF FE -- -- - Little endian UTF-16
                            if ((buffer[0] & unchecked((int)(0xFF))) == unchecked((int)(0xEF)))
                            {
                                encoding = "UTF-8";
                            }
                            else
                            {
                                if ((buffer[0] & unchecked((int)(0xFF))) == unchecked((int)(0xFE)))
                                {
                                    encoding = "UTF-16";
                                }
                                else
                                {
                                    // in fact BE 
                                    if (length < 4 || buffer[2] != 0)
                                    {
                                        encoding = "UTF-16";
                                    }
                                    else
                                    {
                                        // in fact LE
                                        encoding = "UTF-32";
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // in fact LE
            return encoding;
        }

        /// <summary>
        /// Ensures the requested capacity by increasing the buffer size when the
        /// current length is exceeded.
        /// </summary>
        /// <param name="requestedLength">requested new buffer length</param>
        private void EnsureCapacity(int requestedLength)
        {
            if (requestedLength > buffer.Length)
            {
                sbyte[] oldBuf = buffer;
                buffer = new sbyte[oldBuf.Length * 2];
                System.Array.Copy(oldBuf, 0, buffer, 0, oldBuf.Length);
            }
        }
    }
}

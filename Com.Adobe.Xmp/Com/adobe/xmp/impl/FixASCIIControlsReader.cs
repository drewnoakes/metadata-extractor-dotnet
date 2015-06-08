// =================================================================================================
// ADOBE SYSTEMS INCORPORATED
// Copyright 2006 Adobe Systems Incorporated
// All Rights Reserved
//
// NOTICE:  Adobe permits you to use, modify, and distribute this file in accordance with the terms
// of the Adobe license agreement accompanying it.
// =================================================================================================
using System.IO;
using Sharpen;

namespace Com.Adobe.Xmp.Impl
{
    /// <since>22.08.2006</since>
    public class FixASCIIControlsReader : PushbackReader
    {
        private const int StateStart = 0;

        private const int StateAmp = 1;

        private const int StateHash = 2;

        private const int StateHex = 3;

        private const int StateDig1 = 4;

        private const int StateError = 5;

        private const int BufferSize = 8;

        /// <summary>the state of the automaton</summary>
        private int state = StateStart;

        /// <summary>the result of the escaping sequence</summary>
        private int control = 0;

        /// <summary>count the digits of the sequence</summary>
        private int digits = 0;

        /// <summary>The look-ahead size is 6 at maximum (&amp;#xAB;)</summary>
        /// <seealso cref="System.IO.PushbackReader.PushbackReader(System.IO.StreamReader, int)"/>
        /// <param name="in">a Reader</param>
        public FixASCIIControlsReader(System.IO.StreamReader @in)
            : base(@in, BufferSize)
        {
        }

        /// <seealso cref="System.IO.StreamReader.Read(char[], int, int)"/>
        /// <exception cref="System.IO.IOException"/>
        public override int Read(char[] cbuf, int off, int len)
        {
            int readAhead = 0;
            int read = 0;
            int pos = off;
            char[] readAheadBuffer = new char[BufferSize];
            bool available = true;
            while (available && read < len)
            {
                available = base.Read(readAheadBuffer, readAhead, 1) == 1;
                if (available)
                {
                    char c = ProcessChar(readAheadBuffer[readAhead]);
                    if (state == StateStart)
                    {
                        // replace control chars with space
                        if (Utils.IsControlChar(c))
                        {
                            c = ' ';
                        }
                        cbuf[pos++] = c;
                        readAhead = 0;
                        read++;
                    }
                    else
                    {
                        if (state == StateError)
                        {
                            Unread(readAheadBuffer, 0, readAhead + 1);
                            readAhead = 0;
                        }
                        else
                        {
                            readAhead++;
                        }
                    }
                }
                else
                {
                    if (readAhead > 0)
                    {
                        // handles case when file ends within excaped sequence
                        Unread(readAheadBuffer, 0, readAhead);
                        state = StateError;
                        readAhead = 0;
                        available = true;
                    }
                }
            }
            return read > 0 || available ? read : -1;
        }

        /// <summary>Processes numeric escaped chars to find out if they are a control character.</summary>
        /// <param name="ch">a char</param>
        /// <returns>Returns the char directly or as replacement for the escaped sequence.</returns>
        private char ProcessChar(char ch)
        {
            switch (state)
            {
                case StateStart:
                {
                    if (ch == '&')
                    {
                        state = StateAmp;
                    }
                    return ch;
                }

                case StateAmp:
                {
                    if (ch == '#')
                    {
                        state = StateHash;
                    }
                    else
                    {
                        state = StateError;
                    }
                    return ch;
                }

                case StateHash:
                {
                    if (ch == 'x')
                    {
                        control = 0;
                        digits = 0;
                        state = StateHex;
                    }
                    else
                    {
                        if ('0' <= ch && ch <= '9')
                        {
                            control = Sharpen.Extensions.Digit(ch, 10);
                            digits = 1;
                            state = StateDig1;
                        }
                        else
                        {
                            state = StateError;
                        }
                    }
                    return ch;
                }

                case StateDig1:
                {
                    if ('0' <= ch && ch <= '9')
                    {
                        control = control * 10 + Sharpen.Extensions.Digit(ch, 10);
                        digits++;
                        if (digits <= 5)
                        {
                            state = StateDig1;
                        }
                        else
                        {
                            state = StateError;
                        }
                    }
                    else
                    {
                        // sequence too long
                        if (ch == ';' && Utils.IsControlChar((char)control))
                        {
                            state = StateStart;
                            return (char)control;
                        }
                        else
                        {
                            state = StateError;
                        }
                    }
                    return ch;
                }

                case StateHex:
                {
                    if (('0' <= ch && ch <= '9') || ('a' <= ch && ch <= 'f') || ('A' <= ch && ch <= 'F'))
                    {
                        control = control * 16 + Sharpen.Extensions.Digit(ch, 16);
                        digits++;
                        if (digits <= 4)
                        {
                            state = StateHex;
                        }
                        else
                        {
                            state = StateError;
                        }
                    }
                    else
                    {
                        // sequence too long
                        if (ch == ';' && Utils.IsControlChar((char)control))
                        {
                            state = StateStart;
                            return (char)control;
                        }
                        else
                        {
                            state = StateError;
                        }
                    }
                    return ch;
                }

                case StateError:
                {
                    state = StateStart;
                    return ch;
                }

                default:
                {
                    // not reachable
                    return ch;
                }
            }
        }
    }
}

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

namespace XmpCore.Impl
{
    /// <since>22.08.2006</since>
    public class FixAsciiControlsReader : PushbackReader
    {
        private const int StateStart = 0;

        private const int StateAmp = 1;

        private const int StateHash = 2;

        private const int StateHex = 3;

        private const int StateDig1 = 4;

        private const int StateError = 5;

        private const int BufferSize = 8;

        /// <summary>the state of the automaton</summary>
        private int _state = StateStart;

        /// <summary>the result of the escaping sequence</summary>
        private int _control;

        /// <summary>count the digits of the sequence</summary>
        private int _digits;

        /// <summary>The look-ahead size is 6 at maximum (&amp;#xAB;)</summary>
        /// <seealso cref="PushbackReader(System.IO.StreamReader, int)"/>
        /// <param name="in">a Reader</param>
        public FixAsciiControlsReader(StreamReader @in)
            : base(@in, BufferSize)
        {
        }

        /// <exception cref="System.IO.IOException"/>
        public override int Read(char[] cbuf, int off, int len)
        {
            var readAhead = 0;
            var read = 0;
            var pos = off;
            var readAheadBuffer = new char[BufferSize];
            var available = true;
            while (available && read < len)
            {
                available = base.Read(readAheadBuffer, readAhead, 1) == 1;
                if (available)
                {
                    var c = ProcessChar(readAheadBuffer[readAhead]);
                    if (_state == StateStart)
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
                        if (_state == StateError)
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
                        _state = StateError;
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
            switch (_state)
            {
                case StateStart:
                {
                    if (ch == '&')
                    {
                        _state = StateAmp;
                    }
                    return ch;
                }

                case StateAmp:
                {
                    _state = ch == '#' ? StateHash : StateError;
                    return ch;
                }

                case StateHash:
                {
                    if (ch == 'x')
                    {
                        _control = 0;
                        _digits = 0;
                        _state = StateHex;
                    }
                    else
                    {
                        if ('0' <= ch && ch <= '9')
                        {
                            _control = ch - '0';
                            _digits = 1;
                            _state = StateDig1;
                        }
                        else
                        {
                            _state = StateError;
                        }
                    }
                    return ch;
                }

                case StateDig1:
                {
                    if ('0' <= ch && ch <= '9')
                    {
                        _control = _control * 10 + (ch - '0');
                        _digits++;
                        _state = _digits <= 5 ? StateDig1 : StateError;
                    }
                    else
                    {
                        // sequence too long
                        if (ch == ';' && Utils.IsControlChar((char)_control))
                        {
                            _state = StateStart;
                            return (char)_control;
                        }
                        _state = StateError;
                    }
                    return ch;
                }

                case StateHex:
                {
                    if (('0' <= ch && ch <= '9') || ('a' <= ch && ch <= 'f') || ('A' <= ch && ch <= 'F'))
                    {
                        _control = _control * 16 + Convert.ToInt32(ch.ToString(), fromBase: 16);
                        _digits++;
                        _state = _digits <= 4 ? StateHex : StateError;
                    }
                    else
                    {
                        // sequence too long
                        if (ch == ';' && Utils.IsControlChar((char)_control))
                        {
                            _state = StateStart;
                            return (char)_control;
                        }
                        _state = StateError;
                    }
                    return ch;
                }

                case StateError:
                {
                    _state = StateStart;
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

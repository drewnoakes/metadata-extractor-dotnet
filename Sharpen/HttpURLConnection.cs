//
// HttpURLConnection.cs
//
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
//
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Net;

namespace Sharpen
{
    public abstract class UrlConnection
    {
        public abstract InputStream GetInputStream();
    }

    public class HttpsUrlConnection: HttpUrlConnection
    {
        public HttpsUrlConnection (Uri uri): base (uri)
        {
        }
    }

    public class HttpUrlConnection: UrlConnection
    {
        readonly HttpWebRequest _request;
        HttpWebResponse _reqResponse;

        public HttpUrlConnection (Uri uri)
        {
            _request = (HttpWebRequest) WebRequest.Create (uri);
        }

        HttpWebResponse Response {
            get {
                if (_reqResponse == null)
                {
                    try
                    {
                        _reqResponse = (HttpWebResponse) _request.GetResponse ();
                    }
                    catch (WebException ex)
                    {
                        _reqResponse = (HttpWebResponse) ex.Response;
                        if (_reqResponse == null) {
                            if (this is HttpsUrlConnection)
                                throw new WebException ("A secure connection could not be established", ex);
                            throw;
                        }
                    }
                }
                return _reqResponse;
            }
        }

        public override InputStream GetInputStream ()
        {
            return Response.GetResponseStream ();
        }
    }
}


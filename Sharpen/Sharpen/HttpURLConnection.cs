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
    public abstract class URLConnection
    {
        public abstract InputStream GetInputStream();
    }

    public class HttpsURLConnection: HttpURLConnection
    {
        public HttpsURLConnection (Uri uri): base (uri)
        {
        }
    }

    public class HttpURLConnection: URLConnection
    {
        readonly HttpWebRequest request;
        HttpWebResponse reqResponse;

        public HttpURLConnection (Uri uri)
        {
            request = (HttpWebRequest) HttpWebRequest.Create (uri);
        }

        HttpWebResponse Response {
            get {
                if (reqResponse == null)
                {
                    try
                    {
                        reqResponse = (HttpWebResponse) request.GetResponse ();
                    }
                    catch (WebException ex)
                    {
                        reqResponse = (HttpWebResponse) ex.Response;
                        if (reqResponse == null) {
                            if (this is HttpsURLConnection)
                                throw new WebException ("A secure connection could not be established", ex);
                            throw;
                        }
                    }
                }
                return reqResponse;
            }
        }

        public override InputStream GetInputStream ()
        {
            return Response.GetResponseStream ();
        }
    }
}


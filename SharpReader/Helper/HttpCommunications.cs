using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Reflection;

namespace ReaderSharp.Helper
{
    class HttpCommunications
    {
        //-----------------------------------------------------------------------
        // <copyright file="HttpCommunications.cs" company="lI' Ghun">
        // 
        //  Copyright (c) 2011, Sven Walther (sven@li-ghun.de)
        //  All rights reserved.
        //  
        //  Redistribution and use in source and binary forms, with or without modification, are 
        //  permitted provided that the following conditions are met:
        // 
        //  - Redistributions of source code must retain the above copyright notice, this list 
        //    of conditions and the following disclaimer.
        //  - Redistributions in binary form must reproduce the above copyright notice, this list 
        //    of conditions and the following disclaimer in the documentation and/or other 
        //    materials provided with the distribution.
        //  - Neither the name of the Nymphicus nor the names of its contributors may be 
        //    used to endorse or promote products derived from this software without specific 
        //    prior written permission.
        // 
        //  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
        //  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
        //  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
        //  IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
        //  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
        //  NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
        //  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
        //  WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
        //  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
        //  POSSIBILITY OF SUCH DAMAGE.
        // </copyright>
        // <author>Sven Walther</author>
        // <summary>Some helper methods for HTTP requests
        // This code is inspired by the great work found in Desktop Google Reader
        // (http://desktopgooglereader.codeplex.com)
        // </summary>
        //-----------------------------------------------------------------------

        #region Public

        public static string SendPostRequest(string url, object data, bool allowAutoRedirect, string userAgent)
        {
            try
            {
                string formData = string.Empty;
                HttpCommunications.GetProperties(data).ToList().ForEach(x =>
                {
                    formData += string.Format("{0}={1}&", x.Key, x.Value);
                });
                formData = formData.TrimEnd('&');

                //url = ProcessUrl(url);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "POST";
                request.AllowAutoRedirect = allowAutoRedirect;
                request.Accept = "*/*";
                request.UserAgent = userAgent;
                request.ContentType = "application/x-www-form-urlencoded";

                byte[] encodedData = Encoding.ASCII.GetBytes(formData);
                request.ContentLength = encodedData.Length;


                if (Properties.Settings.Default.authToken != "")
                {
                    request.Headers.Add("Authorization:GoogleLogin auth=" + Properties.Settings.Default.authToken);
                }

                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(encodedData, 0, encodedData.Length);
                }
                return GetResponse(request);
            }
            catch (System.Exception e)
            {
                return "";
            }
        }


        public static string SendGetRequest(string url, object data, bool allowAutoRedirect)
        {
            string paramData = string.Empty;
            HttpCommunications.GetProperties(data).ToList().ForEach(x =>
            {
                paramData += string.Format("{0}={1}&", x.Key, x.Value);
            });
            paramData = paramData.TrimEnd('&');

            url = ProcessUrl(url);

            HttpWebRequest request = paramData.Length > 0 ? (HttpWebRequest)WebRequest.Create(string.Format("{0}?{1}", url, paramData)) : (HttpWebRequest)WebRequest.Create(url);
            request.AllowAutoRedirect = allowAutoRedirect;
            if (Properties.Settings.Default.authToken != "")
            {
                request.Headers.Add("Authorization:GoogleLogin auth=" + Properties.Settings.Default.authToken);
            }
            return GetResponse(request);
        }

        #endregion

        #region Private

        private static string ProcessUrl(string url)
        {
            string questionMark = "?";
            if (url.Contains(questionMark))
            {
                url = url.Replace(questionMark, System.Web.HttpUtility.UrlEncode(questionMark));
            }
            return url;
        }

        private static string GetResponse(HttpWebRequest request)
        {

            HttpWebResponse response;
            try
            {
                WebResponse responseTemp = (HttpWebResponse)request.GetResponse();
                response = (HttpWebResponse)responseTemp;
            }
            catch (System.Exception e)
            {
                // some proxys have problems with Continue-100 headers
                request.ProtocolVersion = HttpVersion.Version10;
                request.ServicePoint.Expect100Continue = false;
                System.Net.ServicePointManager.Expect100Continue = false;
                HttpWebResponse responseTemp = (HttpWebResponse)request.GetResponse();
                response = responseTemp;
            }

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string result = reader.ReadToEnd();
                if (result.Contains(Resources.GoogleAuthErrorMessage) || result.Contains(Resources.GoogleRequiredFieldBlankErrorMessage))
                    throw new GoogleAuthenticationException();
                response.Close();

                return result;
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetProperties(object o)
        {
            foreach (var p in o.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                yield return new KeyValuePair<string, string>(p.Name.TrimStart('_'), System.Web.HttpUtility.UrlEncode(p.GetValue(o, null).ToString()));
            }
        }

        #endregion
    }
}

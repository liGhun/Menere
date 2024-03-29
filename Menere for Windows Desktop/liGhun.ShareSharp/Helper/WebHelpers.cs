﻿//-----------------------------------------------------------------------
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Reflection;
using ShareSharp;

namespace WebHelpers
{
    public static class WebHelpers
    {
        public static Encoding encoding = Encoding.ASCII;

        /// <summary>
        /// Post the data as a multipart form
        /// postParameters with a value of type byte[] will be passed in the form as a file, and value of type string will be
        /// passed as a name/value pair.
        /// </summary>
        public static HttpWebResponse MultipartFormDataPost(string UserAgent, string postUrl, Dictionary<string, object> postParameters, Dictionary<string, string> addtionalHeaders)
        {
            string formDataBoundary = "-----------------------------785636581884";
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = WebHelpers.GetMultipartFormData(postParameters, formDataBoundary);

            return WebHelpers.PostForm(UserAgent, postUrl, contentType, formData, addtionalHeaders);
        }

        /// <summary>
        /// Post a form
        /// Please change the UserAgent if you use this method!
        /// </summary>
        /// <param name="postUrl">The url the request shall be targeted at</param>
        /// <param name="contentType">the content type</param>
        /// <param name="formData">Form data as bytestream</param>
        /// <param name="account">The Twitter account</param>
        /// <param name="addtionalHeaders">Headers to be send as string,string dictionary</param>
        /// <returns></returns>
        private static HttpWebResponse PostForm(string UserAgent, string postUrl, string contentType, byte[] formData, Dictionary<string, string> addtionalHeaders)
        {
            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            // Add these, as we're doing a POST
            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = UserAgent;
            request.CookieContainer = new CookieContainer();
            foreach (KeyValuePair<string,string> additonalHeader in addtionalHeaders)
            {
                request.Headers.Add(additonalHeader.Key,additonalHeader.Value);
            }

            // We need to count how many bytes we're sending. 
            request.ContentLength = formData.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                // Push it out there
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;
        }

        /// <summary>
        /// Turn the key and value pairs into a multipart form.
        /// See http://www.ietf.org/rfc/rfc2388.txt for issues about file uploads
        /// </summary>
        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();

            foreach (var param in postParameters)
            {
                if (param.Value is byte[])
                {
                    byte[] fileData = param.Value as byte[];

                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: application/octet-stream\r\n\r\n", boundary, param.Key, param.Key);
                    formDataStream.Write(encoding.GetBytes(header), 0, header.Length);

                    // Write the file data directly to the Stream, rather than serializing it to a string.  This 
                    formDataStream.Write(fileData, 0, fileData.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", boundary, param.Key, param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, postData.Length);
                }
            }

            // Add the end of the request
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, footer.Length);

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }

        public static Response SendPostRequest(string UserAgent, string url, object data, bool allowAutoRedirect)
        {
            try
            {
                string formData = string.Empty;
                GetProperties(data).ToList().ForEach(x =>
                {
                    string key = x.Key;
                    if (x.Key == "newone")
                    {
                        // this is a workaround as new is a command in C#...
                        key = "new";
                    }
                    formData += string.Format("{0}={1}&", key, x.Value);
                });
                formData = formData.TrimEnd('&');

                url = ProcessUrl(url);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "POST";
                request.AllowAutoRedirect = allowAutoRedirect;
                request.Accept = "*/*";
                request.UserAgent = request.UserAgent = UserAgent;
                request.ContentType = "application/x-www-form-urlencoded";

                byte[] encodedData = new UTF8Encoding().GetBytes(formData);
                request.ContentLength = encodedData.Length;

                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(encodedData, 0, encodedData.Length);
                }

                Response returnValue = GetResponse(request);
                return returnValue;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                Response nullResponse = new Response();
                nullResponse.Success = false;
                nullResponse.Error = e.Message;
                return nullResponse;
            }
        }

        public static Response SendPostRequestStringDataOnly(string UserAgent, string url, string stringContent, Dictionary<string, string> addtionalHeaders, bool allowAutoRedirect)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "POST";
                request.AllowAutoRedirect = allowAutoRedirect;
                request.Accept = "*/*";
                request.UserAgent = request.UserAgent = UserAgent;

                request.CookieContainer = new CookieContainer();
                foreach (KeyValuePair<string, string> additonalHeader in addtionalHeaders)
                {
                    request.Headers.Add(additonalHeader.Key, additonalHeader.Value);
                }

                byte[] encodedData = new UTF8Encoding().GetBytes(stringContent);
                request.ContentLength = encodedData.Length;

                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(encodedData, 0, encodedData.Length);
                }

                Response returnValue = GetResponse(request);
                return returnValue;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                Response nullResponse = new Response();
                nullResponse.Success = false;
                nullResponse.Error = e.Message;
                return nullResponse;
            }
        }


        public static Response SendGetRequest(string UserAgent, string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                request.AllowAutoRedirect = true;
                request.Accept = "*/*";
                request.UserAgent = request.UserAgent = UserAgent;
                
                Response returnValue = GetResponse(request);
                return returnValue;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                Response nullResponse = new Response();
                nullResponse.Success = false;
                nullResponse.Error = e.Message;
                return nullResponse;
            }
        }

        public static Response SendGetRequest(string UserAgent, string url, Dictionary<string, string> additionalHeaders)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                request.AllowAutoRedirect = true;
                request.Accept = "*/*";
                request.UserAgent = UserAgent;

                foreach (KeyValuePair<string, string> additonalHeader in additionalHeaders)
                {
                    request.Headers.Add(additonalHeader.Key, additonalHeader.Value);
                }

                Response returnValue = GetResponse(request);
                return returnValue;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                Response nullResponse = new Response();
                nullResponse.Success = false;
                nullResponse.Error = e.Message;
                return nullResponse;
            }
        }

        public static Response SendPostRequestWithBasicAuth(string UserAgent, string url, object data, string username, string password, bool allowAutoRedirect)
        {
            try
            {
                string formData = string.Empty;
                GetProperties(data).ToList().ForEach(x =>
                {
                    string key = x.Key;
                    if (x.Key == "newone")
                    {
                        // this is a workaround as new is a command in C#...
                        key = "new";
                    }
                    formData += string.Format("{0}={1}&", key, x.Value);
                });
                formData = formData.TrimEnd('&');

                url = ProcessUrl(url);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "POST";
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password)));
                request.AllowAutoRedirect = allowAutoRedirect;
                request.Accept = "*/*";
                request.UserAgent = UserAgent;
                request.ContentType = "application/x-www-form-urlencoded";

                byte[] encodedData = new UTF8Encoding().GetBytes(formData);
                request.ContentLength = encodedData.Length;

                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(encodedData, 0, encodedData.Length);
                }

                Response returnValue = GetResponse(request);
                return returnValue;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                Response nullResponse = new Response();
                nullResponse.Success = false;
                nullResponse.Error = e.Message;
                return nullResponse;
            }
        }

        #region Private

        private static string ProcessUrl(string url)
        {
            if (url.Contains("?"))
            {
                url = url.Replace("?", System.Web.HttpUtility.UrlEncode("?"));
            }
            return url;
        }

        private static Response GetResponse(HttpWebRequest request)
        {

            HttpWebResponse response;
            try
            {
                HttpWebResponse responseTemp = (HttpWebResponse)request.GetResponse();
                response = responseTemp;
            }
            catch (System.Exception e)
            {
                // some proxys have problems with Continue-100 headers
                request.ProtocolVersion = HttpVersion.Version10;
                request.ServicePoint.Expect100Continue = false;
                System.Net.ServicePointManager.Expect100Continue = false;
                HttpWebResponse responseTemp = (HttpWebResponse)request.GetResponse();
                response = responseTemp;
                System.Console.WriteLine(e.Message);
            }

            Response returnValue = new Response();

            returnValue.FullHeaders = response.Headers;
            returnValue = parseHeaders(returnValue);

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                returnValue.Content = reader.ReadToEnd();
                return returnValue;
            }
        }

        private static Response parseHeaders(Response response)
        {
            for (int i = 0; i < response.FullHeaders.Count; i++)
            {
                KeyValuePair<string, string> header = new KeyValuePair<string, string>(response.FullHeaders.GetKey(i), response.FullHeaders.Get(i));
                switch (header.Key)
                {
                    case "Status":
                        response.Status = header.Value;
                        if (header.Value.StartsWith("200"))
                        {
                            response.Success = true;
                        }
                        else
                        {
                            response.Success = false;
                        }
                        break;

           
                    default:
                        // any other response we are not interested...
                        break;
                }
            }
            return response;
        }

        private static IEnumerable<KeyValuePair<string, string>> GetProperties(object o)
        {
            foreach (var p in o.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                yield return new KeyValuePair<string, string>(p.Name.TrimStart('_'), System.Web.HttpUtility.UrlEncode(p.GetValue(o, null).ToString()));
            }
        }

        #endregion

        public class Response
        {
            public bool Success { get; set; }
            public string Status { get; set; }
            public string Error { get; set; }
            public string Content { get; set; }
         
            public WebHeaderCollection FullHeaders { get; set; }

        }

    }
}

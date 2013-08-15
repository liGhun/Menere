using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareSharp
{
    public class Instapaper : IShareService
    {
        public string Name
        {
            get { return "Instapaper"; }
        }

        public string Description
        {
            get { return "Instapaper saves webpages for reading them later"; }
        }

        public string Homepage
        {
            get { return "http://www.instapaper.com/"; }
        }

        public bool UsesOAuth
        {
            get { return false; }
        }

        public string Username
        {
            get
            {
                return Properties.Settings.Default.InstapaperUsername;
            }

            set
            {
                Properties.Settings.Default.InstapaperUsername = value;
            }
        }

        public string Password
        {
            get
            {
                return Crypto.ToInsecureString(Crypto.DecryptString(Properties.Settings.Default.InstapaperPassword));
            }
            set
            {
                Properties.Settings.Default.InstapaperPassword = Crypto.EncryptString(Crypto.ToSecureString(value));
            }
        }

        public bool Verified
        {
            get;set;
        }

        public string LastError
        {
            get { return ""; }
        }

        public string ServiceIcon
        {
            get
            {
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase), "ShareSharpImages", "instapaper.png").Substring(6);
            }
        }

        public bool CanVerifyCredentials { get { return true; } }

        public VerificationResponse VerifyCredentials()
        {
            VerificationResponse verificationResponse = new VerificationResponse();
            if (!string.IsNullOrEmpty(this.Username) && !string.IsNullOrEmpty(this.Password))
            {
                WebHelpers.WebHelpers.Response result = WebHelpers.WebHelpers.SendPostRequest("Share#", @"https://www.instapaper.com/api/authenticate", new
                {
                    username = Username,
                    password = Password
                }, false);
                
                bool success = (!string.IsNullOrEmpty(result.Content) && result.Content.ToLowerInvariant().StartsWith("200"));
                if (!success)
                {
                    verificationResponse.Success = false;
                    verificationResponse.ErrorMessage = result.Content;
                }
                else
                {
                    verificationResponse.Success = true;
                }
              
            }
            else
            {
                verificationResponse.Success = false;
                verificationResponse.ErrorMessage = "Missing username or password";
                verificationResponse.UsernameOrPasswordMissing = true;
                Verified = false;
            }
            return verificationResponse;
        }

        public ShareResponse SendNow(string title, string description, string url)
        {
            ShareResponse shareResponse = new ShareResponse();
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    shareResponse.Success = false;
                    shareResponse.ErrorMessage = "No URL specified for sharing";
                }
                else 
                {
                    if (!string.IsNullOrEmpty(this.Username) && !string.IsNullOrEmpty(this.Password))
                    {
                        WebHelpers.WebHelpers.Response result = new WebHelpers.WebHelpers.Response();
                        if (string.IsNullOrEmpty(title))
                        {
                            title = url;
                        }
                        result = WebHelpers.WebHelpers.SendPostRequest("Share#", @"https://www.instapaper.com/api/add", new
                        {
                            username = this.Username,
                            password = Password,
                            url = url,
                            title = title

                        }, false);

                        bool success = (!string.IsNullOrEmpty(result.Content) && result.Content.ToLowerInvariant().StartsWith("201"));
                        if (!success)
                        {
                            shareResponse.ResponseText = result.Content;
                            shareResponse.Success = false;
                            shareResponse.UsernameOrPasswordMissing = true;
                        }
                        else
                        {
                            shareResponse.Success = true;
                        }
                    }
                    else
                    {
                        shareResponse.Success = false;
                        shareResponse.ErrorMessage = "Missing username or password";
                        shareResponse.UsernameOrPasswordMissing = true;
                        Verified = false;
                    }
                }
            }
            catch (Exception exp)
            {
                shareResponse.Success = false;
                shareResponse.ErrorMessage = exp.Message;
                shareResponse.ResponseText = exp.StackTrace;
            }
            return shareResponse;
        }


        public string ApiKey
        {
            get;
            set;
        }

        public bool NeedsApiKey
        {
            get { return false; }
        }
    }
}

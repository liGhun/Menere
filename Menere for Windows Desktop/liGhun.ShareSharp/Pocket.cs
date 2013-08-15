using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareSharp
{
    public class Pocket : IShareService
    {
        public string ApiKey { get; set; }
        
        private string lastError
        {
            get
            {
                return lastError;
            }
        }

        public string Name
        {
            get
            {
                return "Pocket";
            }

        }

        public string Description
        {
            get
            {
                return "Pocket is a service to remember websites. See http://getpocket.com/";
            }
        }

        public string Homepage
        {
            get
            {
                return "http://getpocket.com/";
            }
        }

        public string ServiceIcon 
        {
            get
            {
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase), "Images", "ExternalServices", "pocket.png").Substring(6);
            }
        }

        public string ServiceIconRelativePath
        {
            get
            {
                return "Images\\ExternalServices\\pocket.png";
            }
        }

        public bool UsesOAuth
        {
            get
            {
                return false;
            }
        }

        public string Username
        {
            get
            {
                return Properties.Settings.Default.PocketUsername;
            }

            set
            {
                Properties.Settings.Default.PocketUsername = value;
            }
        }

        public string Password
        {
            get
            {
                return Crypto.ToInsecureString(Crypto.DecryptString(Properties.Settings.Default.PocketPassword));
            }
            set
            {
                Properties.Settings.Default.PocketPassword = Crypto.EncryptString(Crypto.ToSecureString(value));
            }
        }

        public bool Verified { get; set; }

        public string LastError
        {
            get
            {
                return lastError;
            }
        }

        public bool CanVerifyCredentials { get { return true; } }

        public VerificationResponse VerifyCredentials()
        {
            VerificationResponse verificationResponse = new VerificationResponse();
            try
            {
                if (string.IsNullOrEmpty(ApiKey))
                {
                    verificationResponse.Success = false;
                    verificationResponse.ErrorMessage = "No API key for Pocket given";
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.Username) && !string.IsNullOrEmpty(this.Password))
                    {
                        WebHelpers.WebHelpers.Response result = WebHelpers.WebHelpers.SendPostRequest("Share#", @"https://readitlaterlist.com/v2/auth", new
                        {
                            username = this.Username,
                            password = this.Password,
                            apikey = ApiKey,

                        }, false);
                        result.Success = (!string.IsNullOrEmpty(result.Content) && result.Content.ToLowerInvariant() == "200 ok");
                        if (!result.Success)
                        {
                            verificationResponse.Success = false;
                            verificationResponse.ErrorMessage = result.Error;
                            Verified = false;
                        }
                        else
                        {
                            Verified = true;
                            verificationResponse.Success = true;
                        }
                    }
                    else
                    {
                        verificationResponse.ErrorMessage = "Missing username or password";
                        verificationResponse.Success = false;
                        Verified = false;
                    }
                }
            }
            catch (Exception exp)
            {
                verificationResponse.Success = false;
                verificationResponse.ErrorMessage = exp.Message;
            }
            return verificationResponse;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description">is used in this case to send the ref id</param>
        /// <param name="url"></param>
        /// <returns></returns>
        public ShareResponse SendNow(string title, string description, string url)
        {
            ShareResponse shareResponse = new ShareResponse();
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    shareResponse.Success = false;
                    shareResponse.ErrorMessage = "No URL specified";
                }
                else if (string.IsNullOrEmpty(ApiKey))
                {
                    shareResponse.Success = false;
                    shareResponse.ErrorMessage = "No API key for Pocket given";
                }
                {
                    if (!string.IsNullOrEmpty(this.Password) && !string.IsNullOrEmpty(this.Username))
                    {
                        WebHelpers.WebHelpers.Response result;
                        result = WebHelpers.WebHelpers.SendPostRequest("Share#", @"https://readitlaterlist.com/v2/add", new
                        {
                            username = this.Username,
                            password = this.Password,
                            apikey = ApiKey,
                            url = url,
                            title = title,
                            ref_id = description

                        }, false);

                        result.Success = (!string.IsNullOrEmpty(result.Content) && result.Content.ToLowerInvariant() == "200 ok");
                        if (!result.Success)
                        {
                            shareResponse.Success = false;
                            shareResponse.ErrorMessage = result.Content;
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


        public bool NeedsApiKey
        {
            get { return true; }
        }
    }
}

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ShareSharp
{
    public class Pinboard : IShareService
    {
        public string Name
        {
            get { return "Pinboard"; }
        }

        public string Description
        {
            get { return "Pinboard is a fast, low-noise bookmarking site."; }
        }

        public string Homepage
        {
            get { return "http://pinboard.in/"; }
        }

        public bool UsesOAuth
        {
            get { return false; }
        }

        public string Username
        {
            get
            {
                return Properties.Settings.Default.PinboardUsername;
            }

            set
            {
                Properties.Settings.Default.PinboardUsername = value;
            }
        }

        public string Password
        {
            get
            {
                return Crypto.ToInsecureString(Crypto.DecryptString(Properties.Settings.Default.PinboardPassword));
            }
            set
            {
                Properties.Settings.Default.PinboardPassword = Crypto.EncryptString(Crypto.ToSecureString(value));
            }
        }

        public bool Verified
        {
            get;
            set;
        }

        public string LastError
        {
            get { return ""; }
        }

        public string ServiceIcon
        {
            get
            {
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase), "ShareSharpImages", "pinboard.gif").Substring(6);
            }
        }

        public bool CanVerifyCredentials { get { return false; } }

        public bool VerifyCredentials()
        {
            throw new NotImplementedException();
        }

        public ShareResponse SendNow(string title, string description, string url)
        {
            ShareResponse shareResponse = new ShareResponse();
            try
            {
                if (!string.IsNullOrEmpty(this.Username) && !string.IsNullOrEmpty(this.Password))
                {
                    WebHelpers.WebHelpers.Response result;

                    result = WebHelpers.WebHelpers.SendPostRequestWithBasicAuth("Share#", @"https://api.pinboard.in/v1/posts/add", new
                    {
                        url = url,
                        description = description,
                        title = title
                    },
                    this.Username,
                    this.Password,
                    false);

                    bool success = (!string.IsNullOrEmpty(result.Content) && result.Content.ToLowerInvariant().Contains("<result code=\"done\" />"));
                    if (!success)
                    {
                        shareResponse.Success = false;
                        shareResponse.ResponseText = result.Content;
                        Regex objAlphaPattern = new Regex("^<result code=\"(?<errorMessage>.*)\".?/>$");
                        Match match = objAlphaPattern.Match(result.Content.ToLower());
                        if (match.Success)
                        {
                            shareResponse.ErrorMessage = match.Groups["errorMessage"].Value;
                        }
                        else
                        {
                            shareResponse.ErrorMessage = "Unknown error";
                        }
                    }
                    else
                    {
                        shareResponse.Success = true;
                    }
                }
                else
                {
                    shareResponse.ErrorMessage = "Missing username or password";
                    shareResponse.Success = false;
                    shareResponse.UsernameOrPasswordMissing = true;
                    Verified = false;
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

        VerificationResponse IShareService.VerifyCredentials()
        {
            throw new NotImplementedException();
        }
    }
}

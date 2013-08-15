using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ShareSharp
{
    public class Delicious : IShareService
    {
        public string Name
        {
            get { return "del.icio.us"; }
        }

        public string Description
        {
            get { return "Delicious is a Social Bookmarking service, which means you can save all your bookmarks online, share them with other people, and see what other people are bookmarking."; }
        }

        public string Homepage
        {
            get { return "http://del.icio.us/"; }
        }

        public bool UsesOAuth
        {
            get { return false; }
        }

        public string Username
        {
            get
            {
                return Properties.Settings.Default.DeliciousUsername;
            }

            set
            {
                Properties.Settings.Default.DeliciousPassword = value;
            }
        }

        public string Password
        {
            get
            {
                return Crypto.ToInsecureString(Crypto.DecryptString(Properties.Settings.Default.DeliciousPassword));
            }
            set
            {
                Properties.Settings.Default.DeliciousPassword = Crypto.EncryptString(Crypto.ToSecureString(value));
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
                return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase), "ShareSharpImages", "delicious.png").Substring(6);
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
                    if (!string.IsNullOrEmpty(url))
                    {

                        result = WebHelpers.WebHelpers.SendPostRequestWithBasicAuth("Share#", @"https://api.del.icio.us/v1/posts/add", new
                        {
                            url = url,
                            description = description,
                            title = title

                        },
                        this.Username,
                        this.Password,
                        false);
                    }
                    else
                    {
                        shareResponse.Success = false;
                        shareResponse.ErrorMessage = "URL is null or empty";
                        return shareResponse;
                    }

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
                    shareResponse.Success = false;
                    shareResponse.ErrorMessage = "Missing username or password";
                    shareResponse.UsernameOrPasswordMissing = true;
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

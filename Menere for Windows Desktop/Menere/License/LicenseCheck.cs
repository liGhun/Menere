using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Menere
{
    class LicenseChecker
    {
        public static bool checkLicense(string username, string licenseCode)
        {
            Properties.Settings.Default.isValidLicense = false;
            if(checkLicenseOffline(username,licenseCode)) 
            {
                return true;
            }
            else if(Properties.Settings.Default.checkOnlineForLicense) 
            {
                return(checkOnlineForLicense(username));
            }
            
            return false;
        }

        private static bool checkLicenseOffline(string username, string licenseCode)
        {

            if (licenseCode.Length != 10)
            {
                return false;
            }
            string expectedLicenseCode = "";
            string licenseToCheck = "Menere-ktb6ik5u5critdu6v5-" + username.ToLower();
            if (licenseToCheck.Length > 16)
            {
                expectedLicenseCode = GetMD5Hash(licenseToCheck).Substring(2, 10);
            }
            if (licenseCode == expectedLicenseCode)
            {
                Properties.Settings.Default.licenseCode = licenseCode;
                Properties.Settings.Default.isValidLicense = true;
                // AppController.Current.sendNotification("General info", "License found for @" + username, "Thank you for having bought Nymphicus.", AppController.Current.pathToIcon, null); 
                return true;
            }
            return false;
        }
        
        private static string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

        public static bool checkOnlineForLicense(string username)
        {
            if (Properties.Settings.Default.checkOnlineForLicense && !string.IsNullOrEmpty(username))
            {
                try
                {
                    string UserHash = GetMD5Hash(username.ToLower());
                    AppNetDotNet.Helper.Response response = AppNetDotNet.Helper.SendPostRequest("http://www.li-ghun.de/Nymphicus/api/checkLicense/",
                        new
                        {
                            userhash = UserHash,
                            data = "5ct3vw5wtg"
                        });
                    if (response.Content != null)
                    {
                        if (response.Content != "")
                        {

                            string licenseCode = "";

                            licenseCode = response.Content;

                            bool licenseValid = LicenseChecker.checkLicenseOffline(username, licenseCode);
                            if (licenseValid)
                            {
                                Properties.Settings.Default.licenseCode = licenseCode;
                                Properties.Settings.Default.isValidLicense = true;
                                Properties.Settings.Default.Save();
                                return true;
                            }
                        }
                    }
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

      
    }
}

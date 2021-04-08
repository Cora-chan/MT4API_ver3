using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MT4API.Model;

namespace MT4API.Utils
{
    public class SecretUtil
    {
        public static Secret GetLiveHostSecret()
        {

            var configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            var settings = configuration.AppSettings.Settings;
            string host = "103.105.58.181:443";
            int login = 538;
            string password = "hGBe0de";

            Secret secret = new Secret(login, password, host);

            return secret;
        }

        public static Secret GetDemoHostSecret()
        {

            var configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            var settings = configuration.AppSettings.Settings;
            var host = settings["mt_host_demo"].Value;
            var login = int.Parse(settings["mt_login"].Value);
            var password = settings["mt_password"].Value;

            Secret secret = new Secret(login, password, host);

            return secret;
        }


        //public Secret GetFilterUserAccount()
        //{
        //    var configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
        //    var settings = configuration.AppSettings.Settings;
        //    var host = settings["mt_host"].Value;
        //    var login = int.Parse(settings["mt_filtered_user_login"].Value);
        //    var password = settings["mt_filtered_user_password"].Value;

        //    Secret secret = new Secret(login, password, host);

        //    return secret;
        //}

        //public static string Protect(string stringToEncrypt, string optionalEntropy, DataProtectionScope scope)
        //{

        //    return Convert.ToBase64String(
        //        ProtectedData.Protect(
        //            Encoding.UTF8.GetBytes(stringToEncrypt)
        //            , optionalEntropy != null ? Encoding.UTF8.GetBytes(optionalEntropy) : null
        //            , scope));
        //}
        //public static string Unprotect(string encryptedString, string optionalEntropy, DataProtectionScope scope)
        //{
        //    return Encoding.UTF8.GetString(
        //        ProtectedData.Unprotect(
        //            Convert.FromBase64String(encryptedString)
        //            , optionalEntropy != null ? Encoding.UTF8.GetBytes(optionalEntropy) : null
        //            , scope));
        //}
        public static Secret GetRedisSecret()
        {
            var configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            var settings = configuration.AppSettings.Settings;
            var redisServer = settings["redis_server"].Value;

            Secret secret = new Secret(redisServer);

            return secret;
        }

    }
}

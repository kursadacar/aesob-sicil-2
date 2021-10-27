using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using CTD.Core.Constants;
using CTD.Utilities.Manager;
using CTD.Web.Framework.Enums;
using Newtonsoft.Json;

namespace System.Web.Mvc
{
    public static class Accesses
    {
        public static List<string> CookieKeys = new List<string> {"_adi", "_hak", "_ip", "_id", "_resim"};

        public static bool IsAdmin => Hak == "admin";

        public static bool HasAuthorization => Hak == "admin" || Hak == "mudur";
        public static string Adi
        {
            get
            {
                try
                {
                    return UtilityManager.Base64Decode(HttpContext.Current.Request.Cookies["_adi"].Value);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static string Hak
        {
            get
            {
                try
                {
                    return UtilityManager.Base64Decode(HttpContext.Current.Request.Cookies["_hak"].Value);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static string Ip
        {
            get
            {
                try
                {
                    return UtilityManager.Base64Decode(HttpContext.Current.Request.Cookies["_ip"].Value);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static int Id
        {
            get
            {
                try
                {
                    return int.Parse(UtilityManager.Base64Decode(HttpContext.Current.Request.Cookies["_id"].Value));
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public static string Resim
        {
            get
            {
                try
                {
                    return UtilityManager.Base64Decode(HttpContext.Current.Request.Cookies["_resim"].Value);
                }
                catch (Exception)
                {
                    return "user.png";
                }
            }
        }

        public static int KullaniciId
        {
            get
            {
                try
                {
                    return Convert.ToInt32(JsonConvert.DeserializeObject<string>(
                        EnDeCode.Decrypt(
                            FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies[".u"].Value).Name,
                            StaticParams.SifrelemeParametresi)));
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public static ForbiddenAccessTypes IsLogin()
        {
            try
            {
                var cookies = HttpContext.Current.Request.Cookies;
                if (CookieKeys.Any(p =>
                    cookies[p] == null || string.IsNullOrEmpty(cookies[p].Value) ||
                    string.IsNullOrEmpty(UtilityManager.Base64Decode(cookies[p].Value))))
                    return ForbiddenAccessTypes.IsLogout;
                var forbiddenType = ForbiddenAccessTypes.PersonId;
                try
                {
                    var personId = JsonConvert.DeserializeObject<string>(EnDeCode.Decrypt(
                        FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies[".u"].Value).Name,
                        StaticParams.SifrelemeParametresi));
                    forbiddenType = ForbiddenAccessTypes.UnForbidden;
                    return forbiddenType;
                }
                catch (Exception)
                {
                    return forbiddenType;
                }
            }
            catch (Exception)
            {
                return ForbiddenAccessTypes.IsLogout;
            }
        }
    }
}
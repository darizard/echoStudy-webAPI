using echoStudy_webAPI.Areas.Identity.Data;
using echoStudy_webAPI.Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace echoStudy_webAPI.Data
{
    public static class GravatarConfig
    {
        public static string GenerateGravatarURL(EchoUser user)
        {
            string s = Convert.ToHexString(MD5.HashData(Encoding.ASCII.GetBytes(user.Email.Trim().ToLower())));
            return "https://gravatar.com/avatar/" + s + "?d=retro";
        }


    }
}

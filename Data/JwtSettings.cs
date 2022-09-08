using Microsoft.Extensions.Configuration;

namespace echoStudy_webAPI.Data
{
    public class JwtSettings
    {
        public readonly string Key;

        public JwtSettings(IConfiguration config)
        {
            Key = config.GetValue<string>("JwtSettings:Key");
        }
    }
}

using Newtonsoft.Json.Linq;

namespace DikeErosion.IO
{
    public static class JTokenExtensions
    {
        public static double ToDouble(this JToken? token)
        {
            return token == null || token.Type == JTokenType.Null ? double.NaN : (double)token;
        }

        public static string ToString(this JToken? token)
        {
            return token == null || token.Type == JTokenType.Null ? string.Empty : token.ToString();
        }

        public static bool ToBool(this JToken? token)
        {
            return token != null && token.Type != JTokenType.Null && (bool)token;
        }
    }
}

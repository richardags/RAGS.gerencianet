using Newtonsoft.Json;

namespace gerencianet.PIX.JSON
{
    public class Token
    {
        [JsonProperty("access_token")]
        public string accessToken;
        
        [JsonProperty("expires_in")]
        public int expiresIn;
    }
}
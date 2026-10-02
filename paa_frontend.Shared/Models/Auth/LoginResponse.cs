using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace paa_frontend.Shared.Models.Auth
{
    // ─────────────────────────────────────────────────────────────
    //  LOGIN RESPONSE  (mirrors AuthResponse.java)
    //  Returned by POST /api/v1/auth/login
    // ─────────────────────────────────────────────────────────────

    public class LoginResponse
    {
        [JsonPropertyName("accessToken")]  public string        AccessToken  { get; set; } = string.Empty;
        [JsonPropertyName("refreshToken")] public string        RefreshToken { get; set; } = string.Empty;
        [JsonPropertyName("tokenType")]    public string        TokenType    { get; set; } = "Bearer";
        [JsonPropertyName("username")]     public string        Username     { get; set; } = string.Empty;
        [JsonPropertyName("roles")]        public List<string>  Roles        { get; set; } = [];
        [JsonPropertyName("expiresIn")]    public long          ExpiresIn    { get; set; }
        [JsonPropertyName("userId")]       public int?           UserId       { get; set; } = 0;
    }
}

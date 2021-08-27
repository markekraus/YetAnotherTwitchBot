using System;
using SpotifyAPI.Web;

namespace YetAnotherTwitchBot.Models
{
    public class SpotifyToken
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public string Scope { get; set; }
        public string RefreshToken { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddSeconds(-3600);

        public SpotifyToken(){}
        public SpotifyToken(AuthorizationCodeTokenResponse Response)
        {
            this.AccessToken = Response.AccessToken;
            this.CreatedAt = Response.CreatedAt;
            this.ExpiresIn = Response.ExpiresIn;
            this.RefreshToken = Response.RefreshToken;
            this.Scope = Response.Scope;
            this.TokenType = Response.TokenType;
        }
        public AuthorizationCodeTokenResponse GetAuthorizationCodeTokenResponse()
        {
            return new AuthorizationCodeTokenResponse(){
                AccessToken = this.AccessToken,
                CreatedAt = this.CreatedAt,
                ExpiresIn = this.ExpiresIn,
                RefreshToken = this.RefreshToken,
                Scope = this.Scope,
                TokenType = this.TokenType
            };
        }
    }
}
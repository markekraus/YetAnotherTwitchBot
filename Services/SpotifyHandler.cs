using System.IO;
using System.Threading.Tasks;
using System;
using SpotifyAPI.Web;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YetAnotherTwitchBot.Options;
using YetAnotherTwitchBot.Models;

namespace YetAnotherTwitchBot.Services
{
    public class SpotifyHandler
    {
        private static IList<SavedTrack> tracks;
        private static Random random = new Random();
        private static ILogger<SpotifyHandler> _logger;
        private static SpotifyClient _spotifyClient;
        private static object _lock = new Object();
        private static bool loaded = false;
        private IOptionsMonitor<SpotifyOptions> _spotifyOptions;
        private SettingsHelper _settingsHelper;

        public SpotifyHandler(
            ILogger<SpotifyHandler> logger,
            IOptionsMonitor<SpotifyOptions> SpotifyOptions,
            SettingsHelper SettingsHelper)
        {
            _logger = logger;
            _spotifyOptions = SpotifyOptions;
            _settingsHelper = SettingsHelper;
            _spotifyOptions.OnChange(OnSettingsUpdated);
            Init();
        }

        private void OnSettingsUpdated(SpotifyOptions Options)
        {
            Init();
        }
        private void Init()
        {
            if (string.IsNullOrEmpty(_spotifyOptions.CurrentValue.ClientId))
            {
                _logger.LogError("Please set Spotify 'ClientId' in appsettings.json before connecting.");
                return;
            }

            if (!string.IsNullOrEmpty(_spotifyOptions.CurrentValue?.Token?.RefreshToken))
            {
                Start();
            }
            else
            {
                _logger.LogInformation("Init skipped, no refresh token");
            }
        }

        public void Start()
        {
            if(!_spotifyOptions.CurrentValue.Enabled)
            {
                _logger.LogInformation("Disabled. Skipping start.");
                return;
            }
            var clientId = _spotifyOptions.CurrentValue.ClientId;
            var response = _spotifyOptions.CurrentValue.Token.GetAuthorizationCodeTokenResponse();
            var config = SpotifyClientConfig
                .CreateDefault()
                .WithAuthenticator(new AuthorizationCodeAuthenticator(_spotifyOptions.CurrentValue.ClientId, _spotifyOptions.CurrentValue.ClientSecret, response));

            _spotifyClient = new SpotifyClient(config);
            var nothing = UpdateTrackList();
        }

        public async Task UpdateTrackList()
        {
            loaded = false;
            _logger.LogInformation("Fetching 'Liked' Songs...");
            var freshTracks = await _spotifyClient.PaginateAll(await _spotifyClient.Library.GetTracks().ConfigureAwait(false));
            lock (_lock)
            {
                tracks = freshTracks;
            }
            _logger.LogInformation($"{tracks.Count} Tracks Found!");
            loaded = true;
        }

        public string GetRandomTrack()
        {
            if(!loaded)
            {
                return "Spotify list still loading... please wait.";
            }
            SavedTrack track;
            lock(tracks)
            {
                track = tracks[random.Next(0, tracks.Count - 1)];
            }
            return $"!sr {track.Track.Artists[0].Name} {track.Track.Name}";
        }

        public LoginRequest GetLoginRequest(Uri CallbackUri)
        {
            return new LoginRequest(CallbackUri, _spotifyOptions.CurrentValue.ClientId, LoginRequest.ResponseType.Code)
            {
                Scope = new List<string> { Scopes.UserReadEmail, Scopes.UserReadPrivate, Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative, Scopes.UserLibraryRead }
            };
        }

        public async Task AuthorizeClient(string Code, Uri CallbackUri)
        {
            var response = await new OAuthClient().RequestToken(
                new AuthorizationCodeTokenRequest(
                    _spotifyOptions.CurrentValue.ClientId, _spotifyOptions.CurrentValue.ClientSecret, Code, CallbackUri));
            
            var options = new SpotifyOptions()
            {
                ClientId = _spotifyOptions.CurrentValue.ClientId,
                ClientSecret = _spotifyOptions.CurrentValue.ClientSecret,
                Token = new SpotifyToken(response)
            };
            _settingsHelper.AddOrUpdateAppSetting<SpotifyOptions>(SpotifyOptions.Section, options);
        }

        public bool IsLoaded()
        {
            return loaded;
        }
        public bool IsAuthenticated()
        {
            return !string.IsNullOrWhiteSpace(_spotifyOptions.CurrentValue.Token?.RefreshToken);
        }
    }
}
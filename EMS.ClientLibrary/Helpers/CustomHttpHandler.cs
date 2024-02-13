

using EMS.BaseLibrary.DTOs;
using EMS.ClientLibrary.Services.Contracts;
using System.Net;

namespace EMS.ClientLibrary.Helpers
{
    //In C#, you can use classes like HttpClientHandler or create custom classes that inherit from DelegatingHandler.
    //This method is an implementation of the SendAsync method in a class that inherits from HttpClientHandler or a similar class
    //(e.g., DelegatingHandler). The purpose of this method is to intercept and modify the HTTP requests
    //and responses made by an HttpClient before they are sent or received.
    public class CustomHttpHandler(GetHttpClient getHttpClient, LocalStorageService localStorageService, IUserAccountService accountServie) :DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool loginUrl = request.RequestUri!.AbsoluteUri.Contains("login");
            bool registerUrl = request.RequestUri!.AbsoluteUri.Contains("register");
            bool refreshTokenUrl = request.RequestUri!.AbsoluteUri.Contains("register");
            if (loginUrl || registerUrl || refreshTokenUrl) return await base.SendAsync(request, cancellationToken);

            var result = await base.SendAsync(request, cancellationToken);
            if(result.StatusCode == HttpStatusCode.Unauthorized) 
            {
                // Get token from localstorage
                var stringToken = await localStorageService.GetToken();
                if (stringToken == null) return result;

                // check if the header contians token 
                string token = string.Empty;
                try
                {
                    token = request.Headers.Authorization!.Parameter!;
                }
                catch (Exception ex) {
                
                }
                var deserializedToken = Serializations.DeserializeJsonString<UserSession>(stringToken);
                if (deserializedToken is null) return result;
                if (string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", deserializedToken.Token);
                    return await base.SendAsync(request, cancellationToken);
                }
                var newJwtToken = await GetReshToken(deserializedToken.RefreshToken);
                if (newJwtToken is null) return result;
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newJwtToken);
                return await base.SendAsync(request, cancellationToken);
            }
            return result;
        }

        private async Task<string> GetReshToken(string? refreshToken)
        {
            var result = await accountServie.RefreshTokenAsync(new RefreshToken() { Token = refreshToken });
            string serializedToken = Serializations.SerializeObj(new UserSession() { Token = result.Token, RefreshToken = result.RefreshToken });
            await localStorageService.SetToken(serializedToken);
            return result.RefreshToken;
        }
    }
}

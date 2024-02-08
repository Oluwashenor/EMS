using EMS.BaseLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EMS.ClientLibrary.Helpers
{
	public class GetHttpClient(IHttpClientFactory httpClientFactory, LocalStorageService localStorageService)
	{
		private const string HeaderKey = "Authorization";

		public async Task<HttpClient> GetPrivateHttpClient()
		{
			var client = httpClientFactory.CreateClient("SystemApiClient");
			var stringToken = await localStorageService.GetToken();
			if (string.IsNullOrEmpty(stringToken)) return client;
			var deserializeToken = Serializations.DeserializeJsonString<UserSession>(stringToken);
			if(deserializeToken == null) return client;
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", deserializeToken.Token);
			return client;
		}

		public HttpClient GetPublicHttpClient()
		{
			var client = httpClientFactory.CreateClient("SystemApiClient");
			client.DefaultRequestHeaders.Remove(HeaderKey);
			return client;
		}
	}
}

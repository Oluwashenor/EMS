﻿using EMS.BaseLibrary.DTOs;
using EMS.BaseLibrary.Responses;
using EMS.ClientLibrary.Helpers;
using EMS.ClientLibrary.Services.Contracts;
using System.Net.Http.Json;

namespace EMS.ClientLibrary.Services.Implementation
{
	public class UserAccountService(GetHttpClient getHttpClient) : IUserAccountService
	{
		public const string AuthUrl = "api/authentication";
		public async Task<GeneralResponse> CreateAsync(Register user)
		{
			var httpClient = getHttpClient.GetPublicHttpClient();
			var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/register", user);
			if (!result.IsSuccessStatusCode) return new GeneralResponse(false, "Error Occurred");
			return await result.Content.ReadFromJsonAsync<GeneralResponse>();
		}

		public async Task<LoginResponse> SignInAsync(Login user)
		{
			var httpClient = getHttpClient.GetPublicHttpClient();
			var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/login", user);
			if (!result.IsSuccessStatusCode) return new LoginResponse(false, "Error Occurred");
			return await result.Content.ReadFromJsonAsync<LoginResponse>();
		}

		public async Task<WeatherForecast[]> GetWeatherForecast()
		{
			var httpClient = await getHttpClient.GetPrivateHttpClient();
			var result = await httpClient.GetFromJsonAsync<WeatherForecast[]>($"api/weatherforecast");	
			return result;
		}

		public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
		{
            var httpClient = getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/refresh-token", token);
            if (!result.IsSuccessStatusCode) return new LoginResponse(false, "Error Occurred");
            return await result.Content.ReadFromJsonAsync<LoginResponse>();
        }


	}
}

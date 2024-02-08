using EMS.BaseLibrary.DTOs;
using EMS.BaseLibrary.Entities;
using EMS.BaseLibrary.Responses;
using EMS.ServerLibrary.Data;
using EMS.ServerLibrary.Helpers;
using EMS.ServerLibrary.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Constants = EMS.ServerLibrary.Helpers.Constants;

namespace EMS.ServerLibrary.Repositories.Implementations
{
	public class UserAccountRepository(IOptions<JwtSection> jwtConfig, AppDbContext context) : IUserAccount
	{
		public async Task<GeneralResponse> CreateAsync(Register user)
		{
			if (user is null) return new GeneralResponse(false, "Model is Empty");
			var checkUser = await FindUserByEmail(user.Email);
			if(checkUser != null) return new GeneralResponse(false, "User registered Already");
			// Save User
			var appUser = await AddToDatabase(new ApplicationUser()
			{
				 Fullname = user.FullName,
				 Email = user.Email,
				 Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
			});
			// Check, Create and Assign Role
			var checkAdminRole = await context.SystemRoles.FirstOrDefaultAsync(_ => _.Name!.Equals(Constants.Admin));
			if(checkAdminRole is null)
			{
				var createAdminRole = await AddToDatabase(new SystemRole() { Name = Constants.Admin });
				await AddToDatabase(new UserRole() { RoleId = createAdminRole.Id, UserId = appUser.Id });
				return new GeneralResponse(true, "Account Created");
			}

			var checkUserRole = await context.SystemRoles.FirstOrDefaultAsync(_ => _.Name!.Equals(Constants.User));
			SystemRole response = new();
			if(checkUserRole is null)
			{
				response = await AddToDatabase(new SystemRole() { Name = Constants.User });
				await AddToDatabase(new UserRole() { RoleId = response.Id, UserId = appUser.Id });
			}
			else
			{
				await AddToDatabase(new UserRole() { RoleId = checkUserRole.Id, UserId = appUser.Id });
			}
			return new GeneralResponse(true, "Account Created");

			
		}

		public async Task<ApplicationUser> FindUserByEmail(string email) =>
			await context.ApplicationUsers.FirstOrDefaultAsync(_ => _.Email!.ToLower()!.Equals(email!.ToLower()));

		public async Task<LoginResponse> SignInAsync(Login user)
		{
			if (user == null) return new LoginResponse(false, "Model is Empty");
			var appUser = await FindUserByEmail(user.Email);
			if (appUser is null) return new LoginResponse(false, "User not found");

			//Verify Password
			if (!BCrypt.Net.BCrypt.Verify(user.Password, appUser.Password))
				return new LoginResponse(false, "Email/Password not valid");

			var getuserRole = await FindUserRole(appUser.Id);
			if (getuserRole is null) return new LoginResponse(false, "user role is not found");

			var getRoleName = await FindRoleName(getuserRole.RoleId);
			if (getRoleName is null) return new LoginResponse(false, "user role not found");

			string jwtToken = GenerateToken(appUser, getRoleName!.Name);
			string refreshToken = GenerateRefreshToken();

			//Save refresh token to the database
			var findUser = await context.RefreshTokenInfos.FirstOrDefaultAsync(_ => _.UserId == appUser.Id);
			if(findUser is not null)
			{
				findUser!.Token = refreshToken;
				await context.SaveChangesAsync();
			}
			else
			{
				await AddToDatabase(new RefreshTokenInfo() { Token = refreshToken, UserId = appUser.Id });
			}

			var data = new LoginResponse(true, "Login Successful", jwtToken, refreshToken);
			return data;
		}

		public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
		{
			if (token is null) return new LoginResponse(false, "Model is empty");
			var findToken = await context.RefreshTokenInfos.FirstOrDefaultAsync(_ => _.Token!.Equals(token.Token));
			if (findToken is null) return new LoginResponse(false, "Refresh Token is required");
			//get user details
			var user = await context.ApplicationUsers.FirstOrDefaultAsync(_ => _.Id == findToken.UserId);
			if (user is null) return new LoginResponse(false, "Refresh token could not be generated because user not found");

			var userRole = await FindUserRole(user.Id);
			var roleName = await FindRoleName(userRole.RoleId);
			string jwtToken = GenerateToken(user, roleName.Name!);
			string refreshToken = GenerateRefreshToken();

			var updateRefreshToken = await context.RefreshTokenInfos.FirstOrDefaultAsync(_ => _.UserId == user.Id);
			if (updateRefreshToken is null) return new LoginResponse(false, "Refresh token cold not be generated because user has not signed in");

			updateRefreshToken.Token = refreshToken;
			await context.SaveChangesAsync();
			return new LoginResponse(true, "Token refreshed successfully", jwtToken, refreshToken);

		}

		private string GenerateToken(ApplicationUser appUser, string? role)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Value.Key!));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
			var userClaims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
				new Claim(ClaimTypes.Name, appUser.Fullname!),
				new Claim(ClaimTypes.Email, appUser.Email),
				new Claim(ClaimTypes.Role, role),
			};
			var token = new JwtSecurityToken(
				issuer: jwtConfig.Value.Issuer,
				audience: jwtConfig.Value.Audience,
				claims: userClaims,
				expires:DateTime.Now.AddDays(1),
				signingCredentials:credentials
				);
			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private async Task<UserRole> FindUserRole(int userId) => await context.UserRoles.FirstOrDefaultAsync(_ => _.UserId == userId);
		private async Task<SystemRole> FindRoleName(int roleId) => await context.SystemRoles.FirstOrDefaultAsync(_ => _.Id == roleId);

		private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

		private async Task<T> AddToDatabase<T>(T model)
		{
			var result = context.Add(model!);
			await context.SaveChangesAsync();
			return (T)result.Entity;
		}
	}
}
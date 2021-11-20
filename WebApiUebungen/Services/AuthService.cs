using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiUebungen.Entities;
using WebApiUebungen.Models;
using WebApiUebungen.Settings;

namespace WebApiUebungen.Services
{
    public class AuthService : IAuthService
    {
        private static readonly JwtSecurityTokenHandler tokenHandler = new();

        private readonly WebApiDemoDbContext dbContext;
        private readonly JwtBearerSettings settings;

        public AuthService(WebApiDemoDbContext dbContext, JwtBearerSettings settings)
        {
            this.dbContext = dbContext;
            this.settings = settings;
        }

        public async Task<int> SignUp(Credentials credentials, CancellationToken cancellationToken)
        {
            credentials.ThrowIfNull(nameof(credentials));
            credentials.UserName.ThrowIfNullOrEmpty(nameof(credentials.UserName));
            credentials.UserName.ThrowIfNullOrEmpty(nameof(credentials.Password));

            var existing = await GetByUserName(credentials.UserName, cancellationToken);

            if (existing is not null)
                throw new Exception("User exists already.");

            var salt = GenerateSalt();
            var hash = GenerateHash(credentials.Password + salt);

            var user = new User
            {
                UserName = credentials.UserName,
                Password = hash,
                Salt = salt
            };

            await dbContext.AddAsync(user, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return user.Id;
        }

        public async Task<string> SignIn(Credentials credentials, CancellationToken cancellationToken)
        {
            var user = await GetByUserName(credentials.UserName, cancellationToken);

            if (user is null)
                throw new Exception("Invalid Username or Password");

            if (user.Password != GenerateHash(credentials.Password + user.Salt))
                throw new Exception("Invalid Username or Password");

            var claims = new Claim[]
            {
                new (ClaimTypes.NameIdentifier, user.UserName),
            };

            return CreateAccessToken(claims);
        }

        private Task<User> GetByUserName(string userName, CancellationToken cancellationToken)
        {
            return dbContext.User
                .Where(u => u.UserName == userName)
                .SingleOrDefaultAsync(cancellationToken);
        }

        private string CreateAccessToken(IEnumerable<Claim> claims)
        {
            if (claims is null || !claims.Any())
                throw new ArgumentException("Cannot create token without claims");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SigningKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claimsIdentity = new ClaimsIdentity(claims);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = settings.Issuer,
                Audience = settings.Audience,
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(settings.MaxTokenExpirationTimeInMinutes),
                SigningCredentials = credentials
            };

            var token = tokenHandler.CreateToken(securityTokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static string GenerateSalt()
        {
            // Empty salt array
            var salt = new byte[32];

            using (var random = new RNGCryptoServiceProvider())
                random.GetNonZeroBytes(salt);   // Build the random bytes

            // Return the string encoded salt
            return Convert.ToBase64String(salt);
        }

        private static string GenerateHash(string input)
        {
            StringBuilder sb = new();

            using HashAlgorithm provider = new SHA256CryptoServiceProvider();
            byte[] hashBytes = provider.ComputeHash(Encoding.UTF8.GetBytes(input));
            foreach (byte b in hashBytes)
                sb.Append(b.ToString("x2"));

            return sb.ToString().ToLower();
        }
    }
}

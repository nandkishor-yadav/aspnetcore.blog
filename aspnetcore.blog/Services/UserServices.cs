using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;

namespace aspnetcore.blog.Services
{
    public class UserServices : IUserServices
    {
        private readonly IConfiguration _config;

        public UserServices(IConfiguration config)
        {
            _config = config;
        }

        public bool ValidateUser(string username, string password) =>
            username == _config[Constants.Blog.User.UserName] && this.VerifyHashedPassword(password, _config);

        private bool VerifyHashedPassword(string password, IConfiguration config)
        {
            var saltBytes = Encoding.UTF8.GetBytes(config[Constants.Blog.User.Salt]);

            var hashBytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8);

            var hashText = BitConverter.ToString(hashBytes).Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
            return hashText == config[Constants.Blog.User.Password];
        }
    }
}

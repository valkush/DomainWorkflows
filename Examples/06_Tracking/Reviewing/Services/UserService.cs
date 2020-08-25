using System;
using System.Threading.Tasks;

using Reviewing.Domain;

namespace Reviewing.Services
{
    internal class UserService : IUserService
    {
        public async Task<UserInfo> GetUserInfo(int userId)
        {
            switch (userId)
            {
                case 12: return new UserInfo() { Id = 12, FirstName = "John", LastName="Doe" };
                case 14: return new UserInfo() { Id = 14, FirstName = "Alex", LastName = "Roe" };
                case 2: return new UserInfo() { Id = 2, FirstName = "Miranda", LastName = "Simmons" };
                default: throw new InvalidOperationException("User is not found");
            }
        }
    }
}

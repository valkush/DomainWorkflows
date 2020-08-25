using System.Threading.Tasks;

using Reviewing.Domain;

namespace Reviewing.Services
{
    public interface IUserService
    {
        Task<UserInfo> GetUserInfo(int userId);
    }
}

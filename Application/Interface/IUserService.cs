using SimsProject.Domain.Model;
using System.Collections.Generic;

namespace SimsProject.Application.Interface
{
    public interface IUserService : IService<User>
    {
        User GetByUsername(string username);
        public List<User> GetAll();
        User CreateUser(User user);
        User Update(User user);
        bool ChangePassword(User user, string currentPassword, string newPassword);
        void UpdateAllSuperUserStatuses();
        void Delete(User entity);
        bool IsAuthorValid(User user, int locationId);
    }
}

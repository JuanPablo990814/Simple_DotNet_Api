using SimpleDotNetAPI.Models;

namespace SimpleDotNetAPI.Services{

    public interface IUserService
    {
        List<User> GetAllUsers();
        User? GetUserById(int id);
        void AddUser(User user);
        bool UpdateUser(int id, User user);
        bool DeleteUser(int id);
    }

}

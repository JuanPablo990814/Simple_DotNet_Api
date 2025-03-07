using SimpleDotNetAPI.Models;

namespace SimpleDotNetAPI.Services
{
    public class UserService : IUserService
    {
        private readonly List<User> _users = new();
        private int _nextId = 1;


        public UserService()
        {
            // Crear dos usuarios por defecto
            _users.Add(new User(_nextId++, "123456789", "Juan Pérez", "juan.perez@example.com", DateTime.Now));
            _users.Add(new User(_nextId++, "987654321", "María García", "maria.garcia@example.com", DateTime.Now));
        }

        public List<User> GetAllUsers() => _users;

        public User? GetUserById(int id) => _users.FirstOrDefault(u => u.Id == id);

        public void AddUser(User user)
        {
            user.Id = _nextId++;
            _users.Add(user);
        }

        public bool UpdateUser(int id, User updatedUser)
        {
            var index = _users.FindIndex(u => u.Id == id);
            if (index == -1) return false;
            _users[index].Name = updatedUser.Name;
            _users[index].PhoneNumber = updatedUser.PhoneNumber;
            _users[index].Email = updatedUser.Email;
            _users[index].ModifyDate = DateTime.Now;
            return true;
        }

        public bool DeleteUser(int id)
        {
            var user = GetUserById(id);
            return user is not null && _users.Remove(user);
        }
    }
}

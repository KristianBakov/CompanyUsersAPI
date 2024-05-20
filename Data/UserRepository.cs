using CompanyUsersAPI.Models;

namespace CompanyUsersAPI.Data
{
    public class UserRepository : IUserRepository
    {
        DataContextEF _entityFramework;
        public UserRepository(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);
        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        public void AddEntity<T>(T entityToAdd) where T : class
        {
            if (entityToAdd != null) _entityFramework.Add(entityToAdd);
        }

        public void RemoveEntity<T>(T entityToAdd) where T : class
        {
            if (entityToAdd != null) _entityFramework.Remove(entityToAdd);
        }

        public IEnumerable<User> GetUsers()
        {
            IEnumerable<User> users = _entityFramework.Users.ToList();
            return users;
        }

        public User GetSingleUser(int userId)
        {
            User? user = _entityFramework.Users
            .Where(u => u.UserId == userId)
            .FirstOrDefault();
            if (user != null) return user;

            throw new Exception("Failed to get User " + userId);
        }
        public UserSalary GetSingleUserSalary(int userId)
        {
            UserSalary? user = _entityFramework.UserSalary
            .Where(u => u.UserId == userId)
            .FirstOrDefault();

            if (user != null) return user;

            throw new Exception("Failed to get User " + userId);
        }

        public UserJobInfo GetSingleUserJobInfo(int userId)
        {
            UserJobInfo? user = _entityFramework.UserJobInfo
            .Where(u => u.UserId == userId)
            .FirstOrDefault();
            if (user != null) return user;

            throw new Exception("Failed to get User " + userId);
        }
    }
}

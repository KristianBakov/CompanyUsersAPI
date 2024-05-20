using CompanyUsersAPI.Models;

namespace CompanyUsersAPI.Data
{
    public interface IUserRepository
    {
        bool SaveChanges();
        void AddEntity<T>(T entityToAdd) where T : class;
        void RemoveEntity<T>(T entityToAdd) where T : class;
        public IEnumerable<User> GetUsers();
        public User GetSingleUser(int userId);
        public UserJobInfo GetSingleUserJobInfo(int userId);
        public UserSalary GetSingleUserSalary(int userId);
    }
}
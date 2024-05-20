namespace CompanyUsersAPI.Data
{
    public interface IUserRepository
    {
        bool SaveChanges();
        void AddEntity<T>(T entityToAdd) where T : class;
        void RemoveEntity<T>(T entityToAdd) where T : class;
    }
}
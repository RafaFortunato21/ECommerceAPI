using ECommerceAPI.Models;

namespace ECommerceAPI.Repositories
{
    public interface IContribUserRepository
    {
        public List<User> GetUsers();
        public User GetById(int id);
        public void Insert(User user);
        public void Update(User user);
        public void Delete(int id);
    }
}

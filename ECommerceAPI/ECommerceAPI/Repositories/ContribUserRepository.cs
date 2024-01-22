using ECommerceAPI.Models;
using System.Data.SqlClient;
using System.Data;
using Dapper.Contrib.Extensions;

namespace ECommerceAPI.Repositories
{
    public class ContribUserRepository : IContribUserRepository
    {
        private IDbConnection _connection;
        public ContribUserRepository()
        {
            _connection = new SqlConnection(@"Data Source=IATR-193068;Initial Catalog=Ecommerce;User ID=sa;Password=@Sql2019;TrustServerCertificate=True");
        }

        public User GetById(int id)
        {
            return _connection.Get<User>(id);
        }

        public List<User> GetUsers()
        {
            return _connection.GetAll<User>().ToList();
        }

        public void Insert(User user)
        {
            var userId = _connection.Insert(user);
            user.Id = Convert.ToInt32(userId);
        }

        public void Update(User user)
        {
            _connection.Update(user);
        }

        public void Delete(int id)
        {
            var user = GetById(id);
            _connection.Delete(user);
        }
    }
}

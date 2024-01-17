using ECommerceAPI.Models;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Diagnostics.CodeAnalysis;

namespace ECommerceAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private IDbConnection _connection;
        public UserRepository()
        {
            _connection = new SqlConnection(@"Data Source=IATR-193068;Initial Catalog=Ecommerce;User ID=sa;Password=@Sql2019;TrustServerCertificate=True");
        }

        public List<User> GetUsers()
        {
            //_connection.Open();
            var users = _connection.Query<User>("SELECT * FROM Usuarios").ToList();
            //_connection.Close();

            return users;
        }

        public User GetById(int userId)
        {
            var query = " select * From Usuarios u " +
                            "    LEFT JOIN Contatos c " +
                            "        on u.Id = c.UsuarioId " +
                            "where u.id = @Id ";

            return _connection.Query<User, Contact, User>(query,
                        (usuario, contato) =>
                        {
                            usuario.Contato = contato;
                            return usuario;

                        }, new { Id = userId })
                        .SingleOrDefault();
        }

        public void Insert(User user)
        {
            _connection.Open();
            var transaction = _connection.BeginTransaction();
            try
            {
                var sql = "INSERT INTO Usuarios (Nome, Email, Sexo, Rg, Cpf, NomeMae, SituacaoCadastro, DataCadastro) VALUES " +
                          "                     (@Nome, @Email, @Sexo, @Rg, @Cpf, @NomeMae, @SituacaoCadastro, @DataCadastro);" +
                          " SELECT CAST(SCOPE_IDENTITY() AS INT);";
                user.Id = _connection.Query<int>(sql, user, transaction).Single();

                if (user.Contato != null)
                {
                    user.Contato.UsuarioId = user.Id;
                    string SqlContato = "NSERT INTO Contatos(UsuarioId, Telefone, Celular) VALUES " +
                         "                                   (@UsuarioId, @Telefone, @Celular);" +
                         " SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    user.Contato.Id = _connection.Query<int>(SqlContato, user.Contato, transaction).Single();
                }

                transaction.Commit();
            }
            catch(Exception ex) 
            {
                try { transaction.Rollback(); } 
                catch {
                    throw new Exception(ex.Message);
                }
            }
            finally
            {
                _connection.Close();
            }

        }

        public void Update(User user)
        {
            var sql = "UPDATE Usuarios set Nome = @Nome, Email = @Email, Sexo = @Sexo, Rg = @Rg, Cpf = @Cpf, NomeMae = @NomeMae, SituacaoCadastro = @SituacaoCadastro, " +
                      "                DataCadastro = @DataCadastro WHERE Id = @Id ";

            _connection.Execute(sql, user);
        }

        public void Delete(int id)
        {
            var sql = "DELETE FROM Usuarios WHERE Id = @Id ";

            _connection.Execute(sql, new { Id = id });
        }



    }
}

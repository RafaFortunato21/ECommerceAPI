using ECommerceAPI.Models;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Diagnostics.CodeAnalysis;
using System.Transactions;

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
            List<User> usuarios = new List<User>();

            var sql = @" SELECT U.*,C.* ,ee.*, d.* FROM Usuarios u
	                        INNER JOIN Contatos c 
		                        ON c.UsuarioId = u.Id
	                        LEFT JOIN EnderecosEntrega ee 
		                        ON ee.UsuarioId = u.Id
	                        LEFT JOIN UsuariosDepartamentos ud 
		                        ON ud.UsuarioId = u.Id
	                        LEFT JOIN Departamentos d
		                        ON d.Id = ud.DepartamentoId";

            _connection.Query<User, Contact, DeliveryAddress, Departament, User>(sql,
            (usuario, contato, endereco, departamento) =>
            {
                //Verificação do Usuário
                if (usuarios.SingleOrDefault(u => u.Id == usuario.Id) == null)
                {
                    usuario.Departamentos = new List<Departament>();
                    usuario.EnderecosEntrega = new List<DeliveryAddress>();
                    usuario.Contato = contato;
                    usuarios.Add(usuario);
                }
                else
                {
                    usuario = usuarios.SingleOrDefault(u => u.Id == usuario.Id);
                }

                //Verificação do Endereço de Entrega
                if (usuario.EnderecosEntrega.SingleOrDefault(e => e.Id == endereco.Id) == null)
                    usuario.EnderecosEntrega.Add(endereco);

                //if (usuario.Departamentos.SingleOrDefault(d => d.Id == departamento.Id) == null)
                if (usuario.Departamentos.FirstOrDefault(departamento) != null)
                    usuario.Departamentos.Add(departamento);

                return usuario;

            });

            return usuarios;

        }

        public User GetById(int userId)
        {
            List<User> usuarios = new List<User>();

            var sql = @" SELECT U.*,C.* ,ee.*, d.* FROM Usuarios u
	                        LEFT JOIN Contatos c 
		                        ON c.UsuarioId = u.Id
	                        LEFT JOIN EnderecosEntrega ee 
		                        ON ee.UsuarioId = u.Id
	                        LEFT JOIN UsuariosDepartamentos ud 
		                        ON ud.UsuarioId = u.Id
	                        LEFT JOIN Departamentos d
		                        ON d.Id = ud.DepartamentoId
                         WHERE u.Id = @Id";

            _connection.Query<User, Contact, DeliveryAddress, Departament, User>(sql,
            (usuario, contato, endereco, departamento) =>
            {
                //Verificação do Usuário
                if (usuarios.SingleOrDefault(u => u.Id == usuario.Id) == null)
                {
                    usuario.Departamentos = new List<Departament>();
                    usuario.EnderecosEntrega = new List<DeliveryAddress>();
                    usuario.Contato = contato;
                    usuarios.Add(usuario);
                }
                else
                {
                    usuario = usuarios.SingleOrDefault(u => u.Id == usuario.Id);
                }

                //Verificação do Endereço de Entrega
                if (usuario.EnderecosEntrega.SingleOrDefault(e => e.Id == endereco.Id) == null)
                    usuario.EnderecosEntrega.Add(endereco);
                
                if (departamento != null)
                {
                    if (usuario.Departamentos.SingleOrDefault(a => a.Id == departamento.Id) == null)
                        usuario.Departamentos.Add(departamento);
                }



                return usuario;

            }, new { Id = userId });

            return usuarios.SingleOrDefault();
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
                    string SqlContato = "INSERT INTO Contatos(UsuarioId, Telefone, Celular) VALUES " +
                         "                                   (@UsuarioId, @Telefone, @Celular);" +
                         " SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    user.Contato.Id = _connection.Query<int>(SqlContato, user.Contato, transaction).Single();
                }

                InserirEndereco(user, transaction);
                InserirDepartamentos(user, transaction);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                try { transaction.Rollback(); }
                catch
                {
                    throw new Exception(ex.Message);
                }
            }
            finally
            {
                _connection.Close();
            }

        }

        private void InserirDepartamentos(User user, IDbTransaction transaction)
        {
            var SqlDeleteDepartamentos = "DELETE FROM UsuariosDepartamentos where UsuarioId = @Id ";
            _connection.Execute(SqlDeleteDepartamentos, user, transaction);

            if (user.Departamentos != null && user.Departamentos.Count > 0)
            {
                foreach (var departamento in user.Departamentos)
                {
                    string SqlUsuariosDepartamentos = @"INSERT INTO UsuariosDepartamentos 
                               (UsuarioId,DepartamentoId) VALUES (@UsuarioId, @DepartamentoId)";
                    _connection.Execute(SqlUsuariosDepartamentos, new { UsuarioId = user.Id, DepartamentoId = departamento.Id }, transaction);
                }
            }
        }

        public void Update(User user)
        {
            _connection.Open();
            var transaction = _connection.BeginTransaction();
            try
            {
                var sql = "UPDATE Usuarios set Nome = @Nome, Email = @Email, Sexo = @Sexo, Rg = @Rg, Cpf = @Cpf, NomeMae = @NomeMae, SituacaoCadastro = @SituacaoCadastro, " +
                              "                DataCadastro = @DataCadastro WHERE Id = @Id ";
                _connection.Execute(sql, user, transaction);

                if (user.Contato != null)
                {
                    var sqlContatos = "UPDATE Contatos set UsuarioId = @UsuarioId, telefone = @telefone, celular = @celular " +
                    "where Id = @id";

                    _connection.Execute(sqlContatos, user.Contato, transaction);
                }

                InserirEndereco(user, transaction);
                InserirDepartamentos(user, transaction);

                transaction.Commit();


            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
                catch (Exception exc)
                {
                    throw new Exception(exc.Message);
                }
            }
            finally
            {
                _connection.Close();
            }

        }

        public void InserirEndereco(User user, IDbTransaction transaction)
        {

            var sqlDeletarEnderecos = "DELETE FROM EnderecosEntrega where UsuarioId = @Id ";
            _connection.Execute(sqlDeletarEnderecos, user, transaction);

            if (user.EnderecosEntrega != null && user.EnderecosEntrega.Count > 0)
            {
                foreach (var enderecoEntrega in user.EnderecosEntrega)
                {
                    enderecoEntrega.UsuarioId = user.Id;

                    string sqlEndereco = @"INSERT INTO EnderecosEntrega 
                               (UsuarioId,NomeEndereco, CEP,Estado, Cidade, Bairro, Endereco, Numero, Complemento ) VALUES 
                               (@UsuarioId,@NomeEndereco, @CEP,@Estado, @Cidade, @Bairro, @Endereco, @Numero, @Complemento );
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    enderecoEntrega.Id = _connection.Query<int>(sqlEndereco, enderecoEntrega, transaction).Single();
                }
            }
        }

        public void Delete(int id)
        {
            var sql = "DELETE FROM Usuarios WHERE Id = @Id ";

            _connection.Execute(sql, new { Id = id });
        }



    }
}

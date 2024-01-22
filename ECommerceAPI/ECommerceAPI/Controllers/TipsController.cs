using ECommerceAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using ECommerceAPI.Models;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipsController : ControllerBase
    {
        private IDbConnection _connection;
        public TipsController()
        {
            _connection = new SqlConnection(@"Data Source=IATR-193068;Initial Catalog=Ecommerce;User ID=sa;Password=@Sql2019;TrustServerCertificate=True");
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            string sql = @"SELECT * FROM Usuarios WHERE Id = @Id; 
                           SELECT * FROM Contatos WHERE UsuarioId = @Id; 
                           SELECT * FROM EnderecosEntrega WHERE UsuarioId = @Id;";

            using (var multipleResultSts = _connection.QueryMultiple(sql, new {Id = id}) )
            {
                var usuario = multipleResultSts.Read<User>().SingleOrDefault();
                var contato = multipleResultSts.Read<Contact>().SingleOrDefault();
                var enderecos = multipleResultSts.Read<DeliveryAddress>().ToList();
                
                if (usuario != null)
                {
                    usuario.Contato = contato;
                    usuario.EnderecosEntrega = enderecos;

                    return Ok(usuario);
                }
            }

            return NotFound();
        }

        [HttpGet("stored/usuarios")]
        public IActionResult StoredGet()
        {
            var usuarios = _connection.Query<User>("SelecionarUsuarios", commandType: CommandType.StoredProcedure);
            return Ok(usuarios);

        }


        [HttpGet("stored/usuarios/{id}")]
        public IActionResult StoredGet(int id)
        {
            var usuario = _connection.Query<User>("SelecionarUsuario", new {Id = id} , commandType: CommandType.StoredProcedure);
            return Ok(usuario);

        }

     

        [HttpPost("stored/usuario/")]
        public IActionResult PostStored(User user)
        {
            var usuario = _connection.Query<User>("CadastrarUsuario", user, commandType: CommandType.StoredProcedure);
            return Ok(usuario);

        }

    }
}

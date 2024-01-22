using Dapper.Contrib.Extensions;

namespace ECommerceAPI.Models
{

    [Table("Usuarios")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Sexo { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }
        public string NomeMae { get; set; }
        public string SituacaoCadastro { get; set; }
        public DateTimeOffset DataCadastro { get; set; }

        [Write(false)]
        public Contact Contato { get; set; }
        [Write(false)]
        public ICollection<DeliveryAddress> EnderecosEntrega { get; set; }
        [Write(false)]
        public ICollection<Departament> Departamentos { get; set; }

    }
}

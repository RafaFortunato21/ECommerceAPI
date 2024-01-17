namespace ECommerceAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Sexo { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }
        public string NomeMae { get; set; }
        public string SituacaoCadastro { get; set; }
        public DateTimeOffset DataCadastro { get; set; }
        
        public Contact Contato { get; set; }
        public ICollection<DeliveryAddress> EnderecosEntrega { get; set; }
        public ICollection<Departament> Departamentos { get; set; }

    }
}

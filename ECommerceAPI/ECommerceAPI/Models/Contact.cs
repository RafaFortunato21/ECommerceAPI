namespace ECommerceAPI.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public User Usuario { get; set; }
    }
}

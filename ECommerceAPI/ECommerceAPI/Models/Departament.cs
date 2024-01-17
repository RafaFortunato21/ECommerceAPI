namespace ECommerceAPI.Models
{
    public class Departament
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public ICollection<User> Usuarios { get; set; }
    }
}

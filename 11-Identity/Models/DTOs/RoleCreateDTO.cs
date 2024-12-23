namespace _11_Identity.Models.DTOs
{
    public class RoleCreateDTO
    {
        public string Name { get; set; }
        public DateTime OlusmaTarihi { get; set; } = DateTime.Now;
    }
}


namespace _11_Identity.Models.DTOs
{
    public class UserUpdateDTO
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string? Password { get; set; } // ? nullable property
        public string Email { get; set; }
    }
}

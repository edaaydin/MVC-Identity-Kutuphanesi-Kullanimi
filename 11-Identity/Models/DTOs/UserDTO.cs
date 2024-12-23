namespace _11_Identity.Models.DTOs
{
    public class UserDTO
    {
        // gerekli olacak property'leri yazmak icin dto sınıfını kullanırız. (katmanlar arasında gorebiliriz.)
        // kisinin baska tablodan da property'lerini almak istersek vm sınıfını kullanırız. (controller ile view arasında git gel yapabilir.)
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}

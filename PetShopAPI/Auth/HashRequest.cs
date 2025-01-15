namespace PetShopAPI.Auth
{
    public class HashRequest
    {
        public string Guid { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string HashPassword { get; set; } = string.Empty;
    }
}

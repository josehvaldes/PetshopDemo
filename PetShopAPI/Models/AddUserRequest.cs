﻿namespace PetShopAPI.Models
{
    public class AddUserRequest
    {
        public string Domain { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}

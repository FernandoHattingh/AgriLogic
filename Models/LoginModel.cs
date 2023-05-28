using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace AgriLogic.Models
{
    public class LoginModel
    { 
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }

        public LoginModel(){}    

        public LoginModel(string username, string password)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }
    }

}

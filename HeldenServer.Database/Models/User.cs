using System.ComponentModel.DataAnnotations;

namespace HeldenServer.Database.Models
{
    public class User : BaseEntity
    {

        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public string CharacterName { get; set; }

    }
}

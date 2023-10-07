using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAPI.Models
{
    public class User
    {
        public int UserId { get; set; }
        [StringLength(50)]
        public required string FirstName { get; set; }
        [StringLength(50)]
        public required string LastName { get; set; }
        [Column(TypeName = "Date")]
        public required DateTime BirthDate { get; set; }
        public required string PIN { get; set; }
        public required bool IsActive { get; set; }
    }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAPI.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
    }
}

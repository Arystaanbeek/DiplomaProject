using DiplomaProject.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class Transaction
{
    public int Id { get; set; }
    public string UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
}

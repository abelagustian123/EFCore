using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Models;

public class Player
{
    public int PlayerId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string NamePlayer { get; set; } = string.Empty;
    
    public string Position { get; set; } = string.Empty;
    public int JerseyNumber { get; set; }
    
    public int ClubId { get; set; }
    public virtual Club? Club { get; set; }
    
    public bool IsActive { get; set; } = true;
}
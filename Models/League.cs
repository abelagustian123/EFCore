using System.ComponentModel.DataAnnotations;

namespace EntityFramework.Models;

public class League
{
    public int LeagueId { get; set; }
    
    [Required]
    [MaxLength(40)]
    public string NameLeague { get; set; } = string.Empty;
    
    
    public virtual ICollection<Club> Clubs { get; set; } = new List<Club>();
}
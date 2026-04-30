namespace EntityFramework.Models;

public class Club
{
    public int ClubId { get; set; }
    
    public string NameClub { get; set; } = string.Empty;

    public int LeagueId { get; set; }
    public virtual League League { get; set; } = null!;
    
    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
    
    public bool IsActive { get; set; } = true;
}
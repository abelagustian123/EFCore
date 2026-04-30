using EntityFramework.Data;
using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Services;

public class ClubService
{
    private readonly SoccerDbContext _context;
    
    public ClubService(SoccerDbContext context)
    {
        _context = context;
    }
    
    //create
    public async Task<Club> CreateClubAsync(Club club)
    {
        if (string.IsNullOrWhiteSpace(club.NameClub))
        {
            throw new ArgumentException("Club name is required");
        }

        var existingClub = await _context.Clubs
            .FirstOrDefaultAsync(c => c.NameClub == club.NameClub);

        if (existingClub != null)
        {
            throw new InvalidOperationException($"Club with name {club.NameClub} already exists");
        }
        
        var leagueExists = await _context.Leagues.AnyAsync(l => l.LeagueId == club.LeagueId);
        if (!leagueExists)
        {
            throw new KeyNotFoundException($"League with ID {club.LeagueId} does not exist.");
        }
        
        _context.Clubs.Add(club);

        await _context.SaveChangesAsync();

        return club;
    }
    
    //read
    public async Task<List<Club>> GetAllClubsAsync()
    {
        return await _context.Clubs
            .Include(c => c.League)
            .ToListAsync();
    }
    
    //read
    public async Task<Club?> GetClubByIdAsync(int id)
    {
        return await _context.Clubs
            .Include(c => c.League)
            .Include(c => c.Players)
            .FirstOrDefaultAsync(c => c.ClubId == id);
    }
    
    //update
    public async Task<Club?> UpdateClubAsync(int id, Club updatedClub)
    {
        var existingClub = await _context.Clubs.FindAsync(id);

        if (existingClub == null)
        {
            return null;
        }

        existingClub.NameClub = updatedClub.NameClub;
        existingClub.IsActive = updatedClub.IsActive;

        await _context.SaveChangesAsync();

        return existingClub;
    }
    
    //update all properties
    public async Task<bool> UpdateClubCompleteAsync(Club club)
    {
        var existingClub = await _context.Clubs.FindAsync(club.ClubId);
        
        if (existingClub == null)
        {
            return false;
        }
        
        _context.Entry(existingClub).CurrentValues.SetValues(club);
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteClubAsync(int id)
    {
        var club = await _context.Clubs.FindAsync(id);

        if (club == null)
        {
            return false;
        }

        _context.Clubs.Remove(club);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateClubAsync(int id)
    {
        var club = await _context.Clubs.FindAsync(id);

        if (club == null)
        {
            return false;
        }
        
        club.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }
}
using EntityFramework.Data;
using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Services;

public class LeagueService
{
    private readonly SoccerDbContext _context;

    public LeagueService(SoccerDbContext context)
    {
        _context = context;
    }
    
    //create 
    public async Task<League> CreateLeagueAsync(League league)
    {
        var existingLeague = await _context.Leagues
            .FirstOrDefaultAsync(l => l.NameLeague.ToLower() == league.NameLeague.ToLower());

        if (existingLeague != null)
        {
            throw new InvalidOperationException($"League with name {league.NameLeague} already exists");
        }

        _context.Leagues.Add(league);
        await _context.SaveChangesAsync();
        return league;
    }
    
    //read
    public async Task<List<League>> GetAllLeaguesAsync()
    {
        return await _context.Leagues
            .Include(l => l.Clubs)
            .OrderBy(l => l.NameLeague)
            .ToListAsync();
    }
    
    //read get league by id
    public async Task<League?> GetLeagueByIdAsync(int id)
    {
        return await _context.Leagues
            .Include(l => l.Clubs)
            .OrderBy(l => l.NameLeague)
            .FirstOrDefaultAsync(l => l.LeagueId == id);
    }
    
    //update
    public async Task<League?> UpdateLeagueAsync(int id, League updateLeague)
    {
        var existingLeague = await _context.Leagues.FindAsync(id);

        if (existingLeague == null)
        {
            return null;
        }

        existingLeague.NameLeague = updateLeague.NameLeague;

        await _context.SaveChangesAsync();
        return existingLeague;
    }
    
    //delete
    public async Task<bool> DeleteLeagueAsync(int id)
    {
        var league = await _context.Leagues
            .Include(l => l.Clubs)
            .FirstOrDefaultAsync(l => l.LeagueId == id);

        if (league == null)
        {
            return false;
        }

        if (league.Clubs.Any(l => l.IsActive))
        {
            throw new InvalidOperationException("Cannot delete leagues with active club");
        }

        _context.Leagues.Remove(league);
        await _context.SaveChangesAsync();

        return true;
    }
}
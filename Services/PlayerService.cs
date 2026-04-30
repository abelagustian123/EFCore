using EntityFramework.Data;
using EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Services;

public class PlayerService
{
    private readonly SoccerDbContext _context;
    
    public PlayerService(SoccerDbContext context)
    {
        _context = context;
    }
    
    //create
    public async Task<Player> CreatePlayerAsync(Player player)
    {
        var clubExists = await _context.Clubs.AnyAsync(c => c.ClubId == player.ClubId);
        if (!clubExists)
        {
            throw new KeyNotFoundException($"Club with ID {player.ClubId} does not exist.");
        }

        var jerseyTaken = await _context.Players
            .AnyAsync(p => p.ClubId == player.ClubId 
                        && p.JerseyNumber == player.JerseyNumber 
                        && p.IsActive); 

        if (jerseyTaken)
        {
            throw new InvalidOperationException($"Jersey number {player.JerseyNumber} is already taken in this club.");
        }

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        return player;
    }

    //read
    public async Task<List<Player>> GetAllPlayersAsync()
    {
        return await _context.Players
            .Include(p => p.Club) 
            .ToListAsync();
    }

    //read by id
    public async Task<Player?> GetPlayerByIdAsync(int id)
    {
        return await _context.Players
            .Include(p => p.Club)
            .FirstOrDefaultAsync(p => p.PlayerId == id);
    }

    //update
    public async Task<Player?> UpdatePlayerAsync(int id, Player updatedPlayer)
    {
        var existingPlayer = await _context.Players.FindAsync(id);

        if (existingPlayer == null)
        {
            return null;
        }

        // cek jika ubah nomor punggung/pindah club, validasi lagi nomornya
        if (existingPlayer.JerseyNumber != updatedPlayer.JerseyNumber || existingPlayer.ClubId != updatedPlayer.ClubId)
        {
            var jerseyTaken = await _context.Players
                .AnyAsync(p => p.ClubId == updatedPlayer.ClubId 
                            && p.JerseyNumber == updatedPlayer.JerseyNumber 
                            && p.IsActive 
                            && p.PlayerId != id); 

            if (jerseyTaken)
            {
                throw new InvalidOperationException($"Jersey number {updatedPlayer.JerseyNumber} is already taken in this club.");
            }
        }

        existingPlayer.NamePlayer = updatedPlayer.NamePlayer;
        existingPlayer.Position = updatedPlayer.Position;
        existingPlayer.JerseyNumber = updatedPlayer.JerseyNumber;
        existingPlayer.ClubId = updatedPlayer.ClubId;
        existingPlayer.IsActive = updatedPlayer.IsActive;

        await _context.SaveChangesAsync();

        return existingPlayer;
    }

    //deactive
    public async Task<bool> DeactivatePlayerAsync(int id)
    {
        var player = await _context.Players.FindAsync(id);

        if (player == null)
        {
            return false;
        }
        
        player.IsActive = false;
        await _context.SaveChangesAsync();
        
        return true;
    }

    //delete
    public async Task<bool> DeletePlayerAsync(int id)
    {
        var player = await _context.Players.FindAsync(id);

        if (player == null)
        {
            return false;
        }

        _context.Players.Remove(player);
        await _context.SaveChangesAsync();

        return true;
    }
}
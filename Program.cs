using EntityFramework.Data;
using EntityFramework.Models;
using EntityFramework.Services;
using Microsoft.EntityFrameworkCore;

using var context = new SoccerDbContext();
            
// Apply migrations
await ApplyMigrationsAsync(context);

// Seed data
await SeedDatabaseAsync(context);

// Initialize services
var leagueService = new LeagueService(context);
var clubService = new ClubService(context);
var playerService = new PlayerService(context);

try
{
    await DemonstrateLeagueCrudAsync(leagueService);
    await DemonstrateClubCrudAsync(clubService);
    await DemonstratePlayerCrudAsync(playerService);
}
catch (Exception ex)
{
    Console.WriteLine($"\nError occurred: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}


static async Task ApplyMigrationsAsync(SoccerDbContext context)
{
    Console.WriteLine("Applying database migrations...");
            
    // Apply any pending migrations
    await context.Database.MigrateAsync();
            
    Console.WriteLine("Database migrations applied successfully!\n");
}

static async Task SeedDatabaseAsync(SoccerDbContext context)
{
    if (await context.Leagues.AnyAsync()) return;

    var leagues = new[]
    {
        new League { NameLeague = "Premier League (EPL)"},
        new League { NameLeague = "La Liga"},
        new League { NameLeague = "Serie A"}
    };
    
    context.Leagues.AddRange(leagues);
    await context.SaveChangesAsync();

    var clubs = new[]
    {
        new Club { NameClub = "Arsenal", LeagueId = 1 },
        new Club { NameClub = "Aston Villa", LeagueId = 1 },
        
        new Club { NameClub = "FC Barcelona", LeagueId = 2 },
        new Club { NameClub = "Real Madrid", LeagueId = 2 },
        
        new Club { NameClub = "AC Milan", LeagueId = 3 },
        new Club { NameClub = "Inter Milan", LeagueId = 3 },
    };
    
    context.Clubs.AddRange(clubs);
    await context.SaveChangesAsync();
    
    var players = new[]
    {
        new Player { NamePlayer = "Riccardo Calafiori", Position = "Defenders", JerseyNumber = 33,  ClubId = 1},
        new Player { NamePlayer = "Ross Barkley", Position = "Midfielders", JerseyNumber = 6,  ClubId = 2},
        
        new Player { NamePlayer = "Marc-André ter Stegen", Position = "Goalkeepers", JerseyNumber = 1,  ClubId = 3},
        new Player { NamePlayer = "Vinícius Júnior", Position = "Left wing", JerseyNumber = 7,  ClubId = 4},
        
        new Player { NamePlayer = "Niclas Füllkrug", Position = "Center Forward", JerseyNumber = 9,  ClubId = 5},
        new Player { NamePlayer = "Luis Henrique", Position = "Right Wing", JerseyNumber = 11,  ClubId = 6},
    };

    context.Players.AddRange(players);
    await context.SaveChangesAsync();
    
    Console.WriteLine("Database seeding completed successfully!\n");
}

// --- Demo CRUD operations untuk League ---
static async Task DemonstrateLeagueCrudAsync(LeagueService leagueService)
{

    // Create
    var newLeague = new League { NameLeague = "Bundesliga" };
    newLeague = await leagueService.CreateLeagueAsync(newLeague);
    Console.WriteLine($"Created League: {newLeague.NameLeague} (ID: {newLeague.LeagueId})");

    // Read All
    var leagues = await leagueService.GetAllLeaguesAsync();
    foreach (var l in leagues)
    {
        Console.WriteLine($"- {l.NameLeague} (Jumlah Klub: {l.Clubs?.Count ?? 0})");
    }

    // Update
    newLeague.NameLeague = "Bundesliga 1";
    var updatedLeague = await leagueService.UpdateLeagueAsync(newLeague.LeagueId, newLeague);
    Console.WriteLine($"Updated League Name to: {updatedLeague?.NameLeague}");

    // Delete
    var isDeleted = await leagueService.DeleteLeagueAsync(newLeague.LeagueId);
    Console.WriteLine($"Deleted League 'Bundesliga 1': {isDeleted}");
    
    Console.WriteLine("========================\n");
}

// --- Demo CRUD operations untuk Club ---
static async Task DemonstrateClubCrudAsync(ClubService clubService)
{

    // Create
    var newClub = new Club { NameClub = "Chelsea", LeagueId = 1 }; 
    newClub = await clubService.CreateClubAsync(newClub);
    Console.WriteLine($"Created Club: {newClub.NameClub} (ID: {newClub.ClubId})");

    // Read All
    var clubs = await clubService.GetAllClubsAsync();
    foreach (var c in clubs)
    {
        Console.WriteLine($"- {c.NameClub} (Liga: {c.League?.NameLeague}), acitve ? {c.IsActive}");
    }

    // Update
    newClub.NameClub = "Chelsea FC";
    newClub.IsActive = false;
    var updatedClub = await clubService.UpdateClubAsync(newClub.ClubId, newClub);
    Console.WriteLine($"Updated Club Name to: {updatedClub?.NameClub}");
    Console.WriteLine($"Updated Club Active to: {updatedClub?.IsActive}");

    // 4. Delete
    var isDeleted = await clubService.DeleteClubAsync(newClub.ClubId);
    Console.WriteLine($"Deleted Club 'Chelsea FC': {isDeleted}");
    
    Console.WriteLine("========================\n");
}

// --- Demo CRUD operations untuk Player ---
static async Task DemonstratePlayerCrudAsync(PlayerService playerService)
{
    // Create
    var newPlayer = new Player { NamePlayer = "Bukayo Saka", Position = "Right Wing", JerseyNumber = 7, ClubId = 1 }; 
    newPlayer = await playerService.CreatePlayerAsync(newPlayer);
    Console.WriteLine($"Created Player: {newPlayer.NamePlayer} (Nomor Punggung: {newPlayer.JerseyNumber})");

    // Read By Id
    var player = await playerService.GetPlayerByIdAsync(newPlayer.PlayerId);
    Console.WriteLine($"[Found] {player?.NamePlayer} bermain untuk klub: {player?.Club?.NameClub}");

    // Update
    newPlayer.JerseyNumber = 10;
    if (newPlayer.Club != null)
    {
        newPlayer.Club.ClubId = 3;
    }
    var updatedPlayer = await playerService.UpdatePlayerAsync(newPlayer.PlayerId, newPlayer);
    Console.WriteLine($"Updated {updatedPlayer?.NamePlayer} Jersey Number to: {updatedPlayer?.JerseyNumber}");

    // Deactivate
    var isDeactivated = await playerService.DeactivatePlayerAsync(newPlayer.PlayerId);
    Console.WriteLine($"Deactivated Player: {isDeactivated}");
    
    //delete
    var isDeleted = await playerService.DeletePlayerAsync(newPlayer.PlayerId);
    Console.WriteLine($"Deleted Player: {isDeleted}");
    
    Console.WriteLine("========================\n");
}
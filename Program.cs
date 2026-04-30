using EntityFramework.Data;
using EntityFramework.Models;
using EntityFramework.Services;
using Microsoft.EntityFrameworkCore;

using var context = new SoccerDbContext();
            


// Apply migrations
//await context.Database.EnsureCreatedAsync(); 
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

//untuk memperbarui struktur database secara otomatis
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
    
    //manambah data baru ke database
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

// Demo CRUD operations untuk League
static async Task DemonstrateLeagueCrudAsync(LeagueService leagueService)
{
    List<string> semuaLeague = new List<string>();
    
    Console.WriteLine("Masukkan nama league (pisahkan dengan koma):");
    string inputLeague = Console.ReadLine()?.ToLower().Trim();

    if (string.IsNullOrEmpty(inputLeague))
    {
        Console.Write("nama liga tidak boleh kosong");
    }

    string[] listLeague = inputLeague.Split(',', StringSplitOptions.RemoveEmptyEntries);
    foreach (var item in listLeague)
    {
        semuaLeague.Add(item.Trim());
    }

    // --- CREATE ---
    Console.WriteLine("\n--- Memproses Pembuatan League ---");
    foreach (var leagueName in semuaLeague)
    {
        var newLeague = new League { NameLeague = leagueName };
        await leagueService.CreateLeagueAsync(newLeague);
        Console.WriteLine($"Created League: {newLeague.NameLeague} (ID: {newLeague.LeagueId})");
    }

    // --- READ ALL ---
    var leagues = await leagueService.GetAllLeaguesAsync();
    Console.WriteLine("\n--- Daftar League di Database ---");
    foreach (var l in leagues)
    {
        // Gunakan ?.Count ?? 0 untuk menghindari null error pada collection
        Console.WriteLine($"- ID: {l.LeagueId} | Nama: {l.NameLeague} (Jumlah Klub: {l.Clubs?.Count ?? 0})");
    }

    // --- UPDATE ---
    Console.Write("\nIngin update liga (y/n)?: ");
    string answerUpdate = Console.ReadLine()?.ToLower().Trim();

    if (answerUpdate == "y")
    {
        Console.Write("Masukkan ID yang ingin diupdate: ");
        if (int.TryParse(Console.ReadLine(), out int targetIdUpdate))
        {
            var leagueToUpdate = leagues.FirstOrDefault(x => x.LeagueId == targetIdUpdate);
            if (leagueToUpdate != null)
            {
                Console.Write($"Masukkan nama baru untuk {leagueToUpdate.NameLeague}: ");
                string newName = Console.ReadLine()?.Trim();

                if (!string.IsNullOrEmpty(newName))
                {
                    leagueToUpdate.NameLeague = newName;
                    var result = await leagueService.UpdateLeagueAsync(targetIdUpdate, leagueToUpdate);

                    if (result != null)
                        Console.WriteLine($"Berhasil Update! Nama sekarang: {result.NameLeague}");
                    else
                        Console.WriteLine("Update gagal di database.");
                }
            }
            else Console.WriteLine("ID tidak ditemukan.");
        }
        else Console.WriteLine("Input ID harus angka.");
    }

    // --- DELETE ---
    Console.Write("\nIngin delete liga (y/n)?: ");
    string answerDel = Console.ReadLine()?.ToLower().Trim();

    if (answerDel == "y")
    {
        Console.Write("Masukkan ID yang ingin dihapus: ");
        if (int.TryParse(Console.ReadLine(), out int targetIdDelete))
        {
            // Ambil data terbaru dari list untuk memastikan ID ada
            var leagueToDelete = leagues.FirstOrDefault(x => x.LeagueId == targetIdDelete);
            if (leagueToDelete != null)
            {
                bool isDeleted = await leagueService.DeleteLeagueAsync(targetIdDelete);
                if (isDeleted)
                    Console.WriteLine($"Berhasil menghapus League ID: {targetIdDelete}");
                else
                    Console.WriteLine("Gagal menghapus data.");
            }
            else Console.WriteLine("ID tidak ditemukan.");
        }
        else Console.WriteLine("Input ID harus angka.");
    }
}


// Demo CRUD operations untuk Club 
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
        Console.WriteLine($"- {c.NameClub} (Liga: {c.League.NameLeague}), acitve ? {c.IsActive}");
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

// Demo CRUD operations untuk Player 
static async Task DemonstratePlayerCrudAsync(PlayerService playerService)
{
    // Create
    var newPlayer = new Player { NamePlayer = "Bukayo Saka", Position = "Right Wing", JerseyNumber = 70, ClubId = 1 }; 
    newPlayer = await playerService.CreatePlayerAsync(newPlayer);
    Console.WriteLine($"Created Player: {newPlayer.NamePlayer} (Nomor Punggung: {newPlayer.JerseyNumber})");

    // Read By Id
    var player = await playerService.GetPlayerByIdAsync(newPlayer.PlayerId);
    Console.WriteLine($"[Found] {player?.NamePlayer} bermain untuk klub: {player?.Club?.NameClub}");

    // Update
    newPlayer.JerseyNumber = 10;
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
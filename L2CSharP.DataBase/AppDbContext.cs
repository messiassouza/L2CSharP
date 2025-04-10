using L2CSharP.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace L2CSharP.DataBase
{
    public class AppDbContext : DbContext, IDisposable
    {
        private string _databasePath = string.Empty;
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public AppDbContext()
        {
            var str = Database.GetConnectionString();
            Database.Migrate();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=L2CSharP.DataBase;User ID=sa;Password=sql2022.;Trusted_Connection=False;Encrypt=False;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<CharacterShortcut>().HasKey((CharacterShortcut c) => new { c.Id, c.CharId, c.Slot, c.Page });

            modelBuilder.Entity<SpawnList>()
               .HasOne(s => s.Npc)
               .WithMany(n => n.Spawns)
               .HasForeignKey(s => s.NpcTemplateId)
               .OnDelete(DeleteBehavior.Cascade); // Define o comportamento de deleção


            base.OnModelCreating(modelBuilder);
        }

        public void Dispose()
        {
            // Fecha a conexão com o banco de dados
            Database.CloseConnection();
        }

        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<Character> Character { get; set; }
        public DbSet<CharacterItem> CharacterItem { get; set; }
        public DbSet<CharacterShortcut> CharacterShortcut { get; set; }
        public DbSet<CharacterSkill> CharacterSkill { get; set; }
        public DbSet<Clan> Clan { get; set; }
        public DbSet<GameServers> GameServers { get; set; }
        public DbSet<Npc> Npc { get; set; }
        public DbSet<SpawnList> SpawnList { get; set; }
        public DbSet<Weapon> Weapon { get; set; }


    }


}

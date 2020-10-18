using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace HGV.Warhorn.Api.Data
{
    public class PuzzleContext : DbContext
    {
        public DbSet<PuzzleEntity> Puzzles { get; set; }
        public DbSet<PuzzleVoteEntity> Votes { get; set; }

        public static PuzzleContext EnsureCreated()
        {
            var ctx = new PuzzleContext();
            ctx.Database.EnsureCreated();
            return ctx;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(optionsBuilder.IsConfigured == false)
            {
                var connectionString = Environment.GetEnvironmentVariable("CosmosConnectionString");
                var parts = connectionString
                    .Split(";", StringSplitOptions.RemoveEmptyEntries)
                    .Select(_ => new { key = _.Substring(0, _.IndexOf("=")), value = _.Substring(_.IndexOf("=") + 1) })
                    .ToDictionary(_ => _.key, _ => _.value);

                var endpoint = parts["AccountEndpoint"];
                if(string.IsNullOrWhiteSpace(endpoint))
                    throw new ArgumentNullException("CosmosConnectionString:AccountEndpoint");

                var key = parts["AccountKey"];
                if(string.IsNullOrWhiteSpace(key))
                    throw new ArgumentNullException("CosmosConnectionString:AccountKey");

                var databaseName = parts["DatabaseName"];
                if(string.IsNullOrWhiteSpace(databaseName))
                    throw new ArgumentNullException("CosmosConnectionString:DatabaseName");

                optionsBuilder.UseCosmos(endpoint, key, databaseName, null);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PuzzleEntity>().ToContainer("puzzles");
            modelBuilder.Entity<PuzzleVoteEntity>().ToContainer("votes");

            base.OnModelCreating(modelBuilder);
        }
    }
}

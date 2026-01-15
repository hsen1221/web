using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Line> Lines => Set<Line>();
        public DbSet<Bus> Buses => Set<Bus>();
        public DbSet<Stop> Stops => Set<Stop>();
        public DbSet<LinePassenger> LinePassengers => Set<LinePassenger>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Line>().HasKey(l => l.Id);
            modelBuilder.Entity<Bus>().HasKey(b => b.BusId);
            modelBuilder.Entity<Stop>().HasKey(s => s.StopId);
            modelBuilder.Entity<LinePassenger>().HasKey(lp => lp.Id);

            // Line (1) <-> Bus (1) : Bus has FK LineId
            
            // Line (1) -> Stops (many)
            
            // Line (1) -> LinePassengers (many)
            




            // 1. Line <-> Bus (The Rational Fix)
            // If Line is deleted, the Bus stays but LineId becomes NULL (Bus goes to garage).
            modelBuilder.Entity<Line>()
                .HasOne(l => l.Bus)
                .WithOne(b => b.Line)
                .HasForeignKey<Bus>(b => b.LineId)
                .OnDelete(DeleteBehavior.ClientSetNull); // <--- CHANGED: Was Cascade

            // 2. Line -> Stops
            // If Line is deleted, Stops are deleted (This is usually fine).
            modelBuilder.Entity<Stop>()
                .HasOne(s => s.Line)
                .WithMany(l => l.Stops)
                .HasForeignKey(s => s.LineId)
                .OnDelete(DeleteBehavior.Cascade); // <--- Kept as Cascade

            // 3. Line -> LinePassengers (The Safety Fix)
            // If Line has passengers, STOP the deletion. Don't let me delete it!
            modelBuilder.Entity<LinePassenger>()
                .HasOne(lp => lp.Line)
                .WithMany(l => l.LinePassengers)
                .HasForeignKey(lp => lp.LineId)
                .OnDelete(DeleteBehavior.Restrict); // <--- CHANGED: Was Cascade
        }
    }
}
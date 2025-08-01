using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WebApplication6.Models;


namespace WebApplication6.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Catagory> catagories { get; set; }
        public DbSet<Game> games { get; set; }
        public DbSet<Enter> enters { get; set; }
        public DbSet<GameCatagory> GameCatagories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Library> Libraries { get; set; }
        public DbSet<LibraryItem> LibraryItems { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<GameImage> GameImages { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<GameCatagory>()
                .HasKey(gc => new { gc.GameId, gc.CatagoryId });

            modelBuilder.Entity<GameCatagory>()
                .HasOne(gc => gc.Game)
                .WithMany(g => g.GameCatagories)
                .HasForeignKey(gc => gc.GameId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<GameCatagory>()
                .HasOne(gc => gc.Catagory)
                .WithMany(c => c.GameCatagories)
                .HasForeignKey(gc => gc.CatagoryId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<LibraryItem>()
                .HasOne(li => li.Library)
                .WithMany(l => l.Items)
                .HasForeignKey(li => li.LibraryId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Game)
                .WithMany()
                .HasForeignKey(oi => oi.GameId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<LibraryItem>()
                .HasOne(li => li.Game)
                .WithMany()
                .HasForeignKey(li => li.GameId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Game>()
                .Property(g => g.Price)
                .HasPrecision(18, 2);
            modelBuilder.Entity<LibraryItem>()
                .Property(li => li.Price)
                .HasPrecision(18, 2);
            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasPrecision(18, 2);

        }
    }
}

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TicketManager.Data;
using TicketManager.Models;

namespace TicketManager
{

    public class TicketManagerDbContext: DbContext
    {
        public TicketManagerDbContext(DbContextOptions<TicketManagerDbContext> options) : base(options)
        {

        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Client> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>(b =>
            {
                b.HasKey("Id");
                b.Property(x => x.ProductName).HasMaxLength(64);
                b.Property(x => x.ProblemDescription).HasMaxLength(64);
                b.HasOne<Client>(x => x.Client).WithMany(x => x.Tickets).HasForeignKey(x => x.ClientId);// this is for one-to-many relationship
                b.HasOne<Employee>(x => x.Employee).WithMany(x => x.Tickets).HasForeignKey(x => x.EmployeeId);
            });

            modelBuilder.Entity<Client>(b =>
            {
                b.HasKey("Id");
                b.Property(x => x.Name).HasMaxLength(64);
                b.Property(x => x.Email).HasMaxLength(64);
            });

            modelBuilder.Entity<Employee>(b =>
            {
                b.HasKey("Id");
                b.Property(x => x.FullName).HasMaxLength(64);
                b.Property(x => x.Email).HasMaxLength(64);
                b.Property(x => x.MobileNumber).HasMaxLength(64);
                b.Property(x => x.Address).HasMaxLength(64);
            });

            modelBuilder.Entity<Comment>(b =>
            {
                b.HasKey("Id");
                b.Property(x => x.Content).HasMaxLength(1024);
                b.Property(x => x.Author).HasConversion(
                    r => JsonConvert.SerializeObject(r, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
                    , r => JsonConvert.DeserializeObject<AuthorInfo>(r, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                b.HasOne<Ticket>(x => x.Ticket).WithMany(x => x.Comments).HasForeignKey(x => x.TicketId);
            });


        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json")
              .Build();
    
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}

   
    


 
using backend.Entity;
using Microsoft.EntityFrameworkCore;

namespace webapi.Data
{

    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        public DataContext() { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRefreshTokens> UserRefreshTokens { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Slug> Slugs { get; set; }
        public virtual DbSet<ForgotPasswordRequest> ForgotPasswordRequests { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<FeedBack> FeedBack { get; set; }
        public virtual DbSet<Hotel> Hotels { get; set; }
        public virtual DbSet<Itinerary> Itinerarie { get; set; }
        public virtual DbSet<Location1> Locations { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }
        public virtual DbSet<Resorts> Resorts { get; set; }
        public virtual DbSet<Restaurant> Restaurant { get; set; }
        public virtual DbSet<Tour> Tour { get; set; }
        public virtual DbSet<Transportation> Transportation { get; set; }
        public virtual DbSet<Service> Service { get; set; }

        public virtual DbSet<TourDetail> TourDetail { get; set; }

        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<Information> Information { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(ul => new { ul.Username }).IsUnique();

            modelBuilder.Entity<Order>().HasIndex(ul => new { ul.Tour_Detail_ID }).IsUnique();

            //    modelBuilder.Entity<Hotel>()
            //       .HasOne(h => h.location1)
            //       .WithMany(l => l.Hotels)
            //       .HasForeignKey(h => h.LocationId)
            //       .IsRequired()
            //       .OnDelete(DeleteBehavior.Cascade);
            //    modelBuilder.Entity<Resorts>()
            //        .HasOne(h => h.Location)
            //        .WithMany(l => l.Resorts)
            //        .HasForeignKey(h => h.LocationId)
            //        .IsRequired()
            //        .OnDelete(DeleteBehavior.Cascade);
            //    modelBuilder.Entity<Restaurant>()
            //        .HasOne(h => h.Location)
            //        .WithMany(l => l.Restaurant)
            //        .HasForeignKey(h => h.LocationId)
            //        .IsRequired()
            //        .OnDelete(DeleteBehavior.Cascade);
            //    modelBuilder.Entity<TourDetail>()
            //        .HasOne(h => h.tour)
            //        .WithMany(l => l.TourDetail)
            //        .HasForeignKey(h => h.TourId)
            //        .IsRequired()
            //        .OnDelete(DeleteBehavior.Cascade);
            //    modelBuilder.Entity<TourDetail>()
            //        .HasOne(h => h.Staff)
            //        .WithMany(b => b.TourDetails)
            //        .HasForeignKey(b => b.Staff_Id)
            //        .IsRequired()
            //        .OnDelete(DeleteBehavior.Cascade);


            //    modelBuilder.Entity<Itinerary>()
            //       .HasOne(h => h.tour)
            //       .WithMany(b => b.Itinerary)
            //       .HasForeignKey(b => b.TourID)
            //       .IsRequired()
            //       .OnDelete(DeleteBehavior.Cascade);

            //    modelBuilder.Entity<Order>()
            //       .HasOne(h => h.tourDetail)
            //       .WithMany(b => b.Orders)
            //       .HasForeignKey(b => b.Tour_Detail_ID)
            //       .IsRequired()
            //       .OnDelete(DeleteBehavior.Cascade);

            //    modelBuilder.Entity<OrderDetail>()
            //       .HasOne(h => h.order)
            //       .WithMany(b => b.OrderDetails)
            //       .HasForeignKey(b => b.OrderID)
            //       .IsRequired()
            //       .OnDelete(DeleteBehavior.Cascade);
            //    //modelBuilder.Entity<OrderDetail>()
            //    //   .HasOne(h => h.TourDetails)
            //    //   .WithMany(b => b.OrderDetails)
            //    //   .HasForeignKey(b => b.Tour_Detail_ID)
            //    //   .IsRequired()
            //    //   .OnDelete(DeleteBehavior.Cascade);

            //    modelBuilder.Entity<OrderDetail>()
            //       .HasOne(h => h.Users)
            //       .WithMany(b => b.OrderDetails)
            //       .HasForeignKey(b => b.UserID)
            //       .IsRequired()
            //       .OnDelete(DeleteBehavior.Cascade);
            //    modelBuilder.Entity<Service>()
            //       .HasOne(h => h.Tour)
            //       .WithMany(b => b.Services)
            //       .HasForeignKey(b => b.Tour_ID)
            //       .IsRequired()
            //       .OnDelete(DeleteBehavior.Cascade);
            //    modelBuilder.Entity<Tour>()
            //       .HasOne(h => h.category)
            //       .WithMany(b => b.Tours)
            //       .HasForeignKey(b => b.category_id)
            //       .IsRequired()
            //       .OnDelete(DeleteBehavior.Cascade);
            //    modelBuilder.Entity<Tour>()
            //       .HasOne(h => h.transportation)
            //       .WithMany(b => b.Tours)
            //       .HasForeignKey(b => b.Transportation_ID)
            //       .IsRequired()
            //       .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
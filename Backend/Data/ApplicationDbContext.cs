using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<AssetCategory> Categories { get; set; } = null!;
        public DbSet<Asset> Assets { get; set; } = null!;
        public DbSet<AssetAssignment> AssetAssignments { get; set; } = null!;
        public DbSet<AssetRequest> AssetRequests { get; set; } = null!;
        public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; } = null!;
        public DbSet<AssetHistory> AssetHistories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Email index
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Employee constraints
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.EmployeeCode)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            // Asset constraints
            modelBuilder.Entity<Asset>()
                .HasIndex(a => a.AssetTag)
                .IsUnique();

            modelBuilder.Entity<Asset>()
                .HasIndex(a => a.SerialNumber)
                .IsUnique();

            // Asset -> Category
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Assets)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // AssetAssignment -> Asset & Employee
            modelBuilder.Entity<AssetAssignment>()
                .HasOne(a => a.Asset)
                .WithMany(ast => ast.Assignments)
                .HasForeignKey(a => a.AssetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssetAssignment>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.Assignments)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // AssetRequest -> Employee & Category
            modelBuilder.Entity<AssetRequest>()
                .HasOne(r => r.Employee)
                .WithMany(e => e.AssetRequests)
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssetRequest>()
                .HasOne(r => r.Category)
                .WithMany()
                .HasForeignKey(r => r.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // MaintenanceRecord -> Asset
            modelBuilder.Entity<MaintenanceRecord>()
                .HasOne(m => m.Asset)
                .WithMany(a => a.MaintenanceRecords)
                .HasForeignKey(m => m.AssetId)
                .OnDelete(DeleteBehavior.Cascade);

            // AssetHistory -> Asset
            modelBuilder.Entity<AssetHistory>()
                .HasOne(h => h.Asset)
                .WithMany(a => a.Histories)
                .HasForeignKey(h => h.AssetId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using BookingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingApi.Data;

public class AppDbContext : DbContext
{
    
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();
    }
}
//eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwMTlmMTkwNC1jYzhhLTc4ZjMtYjdmZi0zYzBiNWNhMjQzM2IiLCJqdGkiOiJhMjEwNTk0NS0xYTMwLTRjYzItYmQ0MC0yN2IxZjQzMThjNjciLCJlbWFpbCI6ImJAYS5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsImV4cCI6MTc4Mjk0MDE5NywiaXNzIjoiQm9va2luZ0FwaSIsImF1ZCI6IkJvb2tpbmdBcGlVc2VycyJ9.cMtIPym5AWNldtUZY-ienUEplKwOwkDv43UTz7DoA1M
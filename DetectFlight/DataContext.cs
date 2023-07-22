// DataContext.cs
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DataContext : DbContext
{
    public DbSet<Route> Routes { get; set; }
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscription>().HasNoKey();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=.;Database=detectflight;Trusted_Connection = True; ");
    }
}

// Route.cs
public class Route
{
    [Key]
    public int route_id { get; set; }
    public int origin_city_id { get; set; }
    public int destination_city_id { get; set; }
}

// Flight.cs
public class Flight
{
    [Key]
    public int flight_id { get; set; }
    public int route_id { get; set; }
    public DateTime departure_time { get; set; }
    public DateTime arrival_time { get; set; }
    public int airline_id { get; set; }

    [ForeignKey("route_id")]
    public Route Route { get; set; }
}

// Subscription.cs
public class Subscription
{
    public int agency_id { get; set; }
    public int origin_city_id { get; set; }
    public int destination_city_id { get; set; }
}

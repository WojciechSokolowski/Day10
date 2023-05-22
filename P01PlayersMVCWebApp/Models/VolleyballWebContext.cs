using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace P01PlayersMVCWebApp.Models;

public partial class VolleyballWebContext : DbContext
{
    public VolleyballWebContext()
    {
    }

    public VolleyballWebContext(DbContextOptions<VolleyballWebContext> options)
        : base(options)
    {
    }

    public virtual DbSet<VolleyballPlayer> VolleyballPlayers { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseSqlServer("name=DefaultConnectionString");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using System;
using IdentityServerTest.Models;

using IdentityServer4.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace IdentityServerTest.Data;

public class IdentityServerTestDbContext : DbContext
{
    public IdentityServerTestDbContext(DbContextOptions<IdentityServerTestDbContext> options) : base (options)
    { }

    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>().Property(d => d.CreateDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Account>().Property(d => d.ModifyDate).HasDefaultValueSql("GETUTCDATE()");
    }
}

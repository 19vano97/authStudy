using System;
using IdentityServerMvc.Models.Account;
using Microsoft.EntityFrameworkCore;

namespace IdentityServerMvc.Data;

public class IdentityServerMvcDbContext : DbContext
{
    public IdentityServerMvcDbContext(DbContextOptions<IdentityServerMvcDbContext> options) : base(options)
    {
        
    }
    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>().Property(d => d.CreateDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Account>().Property(d => d.ModifyDate).HasDefaultValueSql("GETUTCDATE()");
    }
}

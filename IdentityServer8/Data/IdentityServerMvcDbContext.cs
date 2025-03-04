using System;
using IdentityServer8.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer8.Data;

public class IdentityServer8DbContext : IdentityDbContext<Account>
{
    public IdentityServer8DbContext(DbContextOptions<IdentityServer8DbContext> options) : base(options)
    {
        
    }

    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Account>().Property(d => d.CreateDate).HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Account>().Property(d => d.ModifyDate).HasDefaultValueSql("GETUTCDATE()");
    }
}

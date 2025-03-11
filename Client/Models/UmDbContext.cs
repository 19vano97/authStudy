using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Client.Models;

public partial class UmDbContext : DbContext
{
    public UmDbContext()
    {
    }

    public UmDbContext(DbContextOptions<UmDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ProjectItem> ProjectItems { get; set; }

    public virtual DbSet<TaskItem> TaskItems { get; set; }

    public virtual DbSet<TaskItemStatus> TaskItemStatuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=192.168.1.143,1433; Database=TaskManagerDb;User Id=dev;Password=P@ssw0rd;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectItem>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ModifyDate).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ModifyDate).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<TaskItemStatus>(entity =>
        {
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ModifyDate).HasDefaultValueSql("(getutcdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

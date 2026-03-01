using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Notatnik.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Filez> Filezs { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS01;Initial Catalog=notatnik;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Filez>(entity =>
        {
            entity.ToTable("filez");

            entity.HasIndex(e => e.Id, "IX_filez").IsUnique();

            entity.HasIndex(e => e.AuthorId, "IX_filez_1");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.File).HasColumnName("file");
            entity.Property(e => e.Name)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("name");
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.Parent).HasColumnName("parent");

            entity.HasOne(d => d.Author).WithMany(p => p.Filezs)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_filez_users");

            entity.HasOne(d => d.Organization).WithMany(p => p.Filezs)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_filez_organizations");

            entity.HasOne(d => d.ParentNavigation).WithMany(p => p.InverseParentNavigation)
                .HasForeignKey(d => d.Parent)
                .HasConstraintName("FK_filez_filez");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.ToTable("organizations");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.IsManager).HasColumnName("is_manager");
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.Username)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("username");

            entity.HasOne(d => d.Organization).WithMany(p => p.Users)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_users_organizations");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

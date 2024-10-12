using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Data_Transfer.EF;

public partial class ProcessContext : DbContext
{
    public ProcessContext()
    {
        Database.EnsureCreated();
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Divisione> Divisiones { get; set; }

    public virtual DbSet<ProcessName> ProcessNames { get; set; }

    public virtual DbSet<Process> Processes { get; set; }

    public virtual DbSet<ProcessDivision> ProcessesDivisions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string? stringConnection = new ConfigurationBuilder().AddJsonFile("appsettings.json").SetBasePath(Directory.GetCurrentDirectory()).Build().GetConnectionString("DefaultConnection");
        optionsBuilder.UseMySql(stringConnection, ServerVersion.Parse("8.0.33-mysql"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("categories");

            entity.HasIndex(e => e.Name, "Name_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Divisione>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("divisiones");

            entity.HasIndex(e => e.Name, "Name_UNIQUE1").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<ProcessName>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("process_names");

            entity.HasIndex(e => e.Name, "Name_UNIQUE2").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasMaxLength(500);
        });

        modelBuilder.Entity<Process>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("processes");

            entity.HasIndex(e => e.Code, "Code_UNIQUE").IsUnique();

            entity.HasIndex(e => e.CategoryId, "cat_idx");
            entity.HasIndex(e => e.ProcessNameId, "procname_idx");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Code).HasMaxLength(50);

            entity.HasOne(d => d.Category).WithMany(p => p.Processes)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cat");

            entity.HasOne(d => d.ProcessName).WithMany(p => p.Processes)
                .HasForeignKey(d => d.ProcessNameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("procname");
        });

        modelBuilder.Entity<ProcessDivision>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("processes_divisions");

            entity.HasIndex(e => e.DivisionId, "div_idx");

            entity.HasIndex(e => e.ProcessId, "proc_idx");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DivisionId).HasColumnName("DivisionID");
            entity.Property(e => e.ProcessId).HasColumnName("ProcessID");

            entity.HasOne(d => d.Division).WithMany(p => p.ProcessesDivisions)
                .HasForeignKey(d => d.DivisionId)
                .HasConstraintName("div");

            entity.HasOne(d => d.Process).WithMany(p => p.ProcessesDivisions)
                .HasForeignKey(d => d.ProcessId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("proc");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

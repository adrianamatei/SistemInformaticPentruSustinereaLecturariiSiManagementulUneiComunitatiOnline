using System;
using System.Collections.Generic;
using AplicatieLicenta.Models;
using Microsoft.EntityFrameworkCore;

namespace AplicatieLicenta.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Carti> Carti { get; set; }
    public DbSet<CategorieVarsta>CategoriiVarsta { get; set; }
    public DbSet<Gen> Genuri { get; set; }

    public virtual DbSet<CluburiLectura> CluburiLectura { get; set; }

    public virtual DbSet<MembriClub> MembriClub { get; set; }

    public virtual DbSet<Recenzii> Recenzii { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public DbSet<MesajClub> MesajClub { get; set; }


    public virtual DbSet<Vizitatori> Vizitatori { get; set; }
    public virtual DbSet<UsersActivity> UsersActivity { get; set; }
    public DbSet<MesajClub> MesajeClub { get; set; }
    public DbSet<Quiz> Quizuri { get; set; }
    public DbSet<IntrebareQuiz> IntrebariQuiz { get; set; }
    public DbSet<VariantaRaspuns> VarianteRaspuns { get; set; }
    public DbSet<RezultatQuiz> RezultateQuiz { get; set; }

   
  



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=Gestionare_app;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.IdAdmin).HasName("PK__Admins__89472E95FD08EAAE");

            entity.HasIndex(e => e.Email, "UQ__Admins__AB6E6164E28AE9E5").IsUnique();

            entity.Property(e => e.IdAdmin).HasColumnName("id_admin");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Parola)
                .HasMaxLength(255)
                .HasColumnName("parola");
        });

        modelBuilder.Entity<Carti>(entity =>
        {
            entity.HasKey(e => e.IdCarte).HasName("PK__Carti__D3C2E8FD46C36E6A");

            entity.ToTable("Carti");

            entity.Property(e => e.IdCarte).HasColumnName("id_carte");
            entity.Property(e => e.Autor)
                .HasMaxLength(100)
                .HasColumnName("autor");
            entity.Property(e => e.DataAdaugarii)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("data_adaugarii");
            entity.Property(e => e.DurataAscultare).HasColumnName("durata_ascultare");
            entity.Property(e => e.ImagineCoperta)
                .HasMaxLength(255)
                .HasColumnName("Imagine_Coperta");
            entity.Property(e => e.TipCarte)
                .HasMaxLength(10)
                .HasColumnName("tip_carte");
            entity.Property(e => e.Titlu)
                .HasMaxLength(200)
                .HasColumnName("titlu");
            entity.Property(e => e.UrlFisier)
                .HasMaxLength(255)
                .HasColumnName("url_fisier");
        });
        modelBuilder.Entity<MesajClub>()
        .HasOne(m => m.Club)
        .WithMany()
        .HasForeignKey(m => m.IdClub);

        modelBuilder.Entity<MesajClub>()
            .HasOne(m => m.Utilizator)
            .WithMany()
            .HasForeignKey(m => m.IdUtilizator);

        modelBuilder.Entity<Carti>()
     .HasMany(c => c.Genuri)
     .WithMany(g => g.Carti)
     .UsingEntity<Dictionary<string, object>>(
         "CarteGen",
         j => j.HasOne<Gen>().WithMany().HasForeignKey("GenuriId"),
         j => j.HasOne<Carti>().WithMany().HasForeignKey("CartiIdCarte")
     );

        modelBuilder.Entity<Carti>()
            .HasMany(c => c.CategoriiVarsta)
            .WithMany(cv => cv.Carti)
            .UsingEntity<Dictionary<string, object>>(
                "CarteCategorieVarsta",
                j => j.HasOne<CategorieVarsta>().WithMany().HasForeignKey("CategoriiVarstaId"),
                j => j.HasOne<Carti>().WithMany().HasForeignKey("CartiIdCarte")
            );



        modelBuilder.Entity<CluburiLectura>(entity =>
        {
            entity.HasKey(e => e.IdClub).HasName("PK__CluburiL__6FA6EEEF42C5FAB3");

            entity.ToTable("CluburiLectura");

            entity.Property(e => e.IdClub).HasColumnName("id_club");
            entity.Property(e => e.DataCrearii)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("data_crearii");
            entity.Property(e => e.Descriere).HasColumnName("descriere");
            entity.Property(e => e.IdCreator).HasColumnName("id_creator");
            entity.Property(e => e.Nume)
                .HasMaxLength(100)
                .HasColumnName("nume");

            entity.HasOne(d => d.IdCreatorNavigation).WithMany(p => p.CluburiLectura)
                .HasForeignKey(d => d.IdCreator)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CluburiLe__id_cr__4D94879B");
        });

        modelBuilder.Entity<MembriClub>(entity =>
        {
            entity.HasKey(e => e.IdMembru).HasName("PK__MembriCl__4CD20D51571C51C3");

            entity.ToTable("MembriClub");

            entity.Property(e => e.IdMembru).HasColumnName("id_membru");
            entity.Property(e => e.DataInscrierii)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("data_inscrierii");
            entity.Property(e => e.IdClub).HasColumnName("id_club");
            entity.Property(e => e.IdUtilizator).HasColumnName("id_utilizator");

            entity.HasOne(d => d.IdClubNavigation).WithMany(p => p.MembriClub)
                .HasForeignKey(d => d.IdClub)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MembriClu__id_cl__5165187F");

            entity.HasOne(d => d.IdUtilizatorNavigation).WithMany(p => p.MembriClub)
                .HasForeignKey(d => d.IdUtilizator)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MembriClu__id_ut__52593CB8");
        });

        modelBuilder.Entity<Recenzii>(entity =>
        {
            entity.HasKey(e => e.IdRecenzie).HasName("PK__Recenzii__8450F2741057739C");

            entity.ToTable("Recenzii");

            entity.Property(e => e.IdRecenzie).HasColumnName("id_recenzie");
            entity.Property(e => e.Comentariu).HasColumnName("comentariu");
            entity.Property(e => e.DataPublicarii)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("data_publicarii");
            entity.Property(e => e.EmailUtilizator)
                .HasMaxLength(100)
                .HasColumnName("email_utilizator");
            entity.Property(e => e.IdCarte).HasColumnName("id_carte");
            entity.Property(e => e.IdUtilizator).HasColumnName("id_utilizator");
            entity.Property(e => e.Rating).HasColumnName("rating");

            entity.HasOne(d => d.IdCarteNavigation).WithMany(p => p.Recenzii)
                .HasForeignKey(d => d.IdCarte)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Recenzii__id_car__48CFD27E");

            entity.HasOne(d => d.IdUtilizatorNavigation).WithMany(p => p.Recenzii)
                .HasForeignKey(d => d.IdUtilizator)
                .HasConstraintName("FK__Recenzii__id_uti__49C3F6B7");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUtilizator).HasName("PK__Users__55C459EA537962FB");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E616466EFE643").IsUnique();

            entity.Property(e => e.IdUtilizator).HasColumnName("id_utilizator");
            entity.Property(e => e.CategorieVarsta)
                .HasMaxLength(20)
                .HasColumnName("categorie_varsta");
            entity.Property(e => e.DataInregistrare)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("data_inregistrare");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Parola)
                .HasMaxLength(255)
                .HasColumnName("parola");
            entity.Property(e => e.TipUtilizator)
                .HasMaxLength(20)
                .HasColumnName("tip_utilizator");
        });

        modelBuilder.Entity<Vizitatori>(entity =>
        {
            entity.HasKey(e => e.IdVizita).HasName("PK__Vizitato__8E2EF95ABAC6D5B7");

            entity.ToTable("Vizitatori");

            entity.Property(e => e.IdVizita).HasColumnName("id_vizita");
            entity.Property(e => e.DataVizitei)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("data_vizitei");
            entity.Property(e => e.IdUtilizator).HasColumnName("id_utilizator");
            entity.Property(e => e.TipUtilizator)
                .HasMaxLength(20)
                .HasColumnName("tip_utilizator");

            entity.HasOne(d => d.IdUtilizatorNavigation).WithMany(p => p.Vizitatori)
                .HasForeignKey(d => d.IdUtilizator)
                .HasConstraintName("FK__Vizitator__id_ut__440B1D61");
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WebApiMyDocs.Models
{
    public partial class ApiDBContext : DbContext
    {
        public ApiDBContext()
        {
        }

        public ApiDBContext(DbContextOptions<ApiDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CreditCard> CreditCards { get; set; }
        public virtual DbSet<Inn> Inns { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Passport> Passports { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<Poli> Polis { get; set; }
        public virtual DbSet<Snil> Snils { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<TemplateDocument> TemplateDocuments { get; set; }
        public virtual DbSet<TemplateDocumentDatum> TemplateDocumentData { get; set; }
        public virtual DbSet<TemplateObject> TemplateObjects { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<CreditCard>(entity =>
            {
                entity.ToTable("CreditCard");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Cvv).HasColumnName("CVV");

                entity.Property(e => e.ExpiryDate).HasMaxLength(15);

                entity.Property(e => e.Fio)
                    .HasMaxLength(255)
                    .HasColumnName("FIO");

                entity.Property(e => e.Number).HasMaxLength(30);

                entity.Property(e => e.PhotoPage1).HasMaxLength(250);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.CreditCard)
                    .HasForeignKey<CreditCard>(d => d.Id)
                    .HasConstraintName("FK_CreditCard_Item");
            });

            modelBuilder.Entity<Inn>(entity =>
            {
                entity.ToTable("INN");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BirthDate).HasColumnType("datetime");

                entity.Property(e => e.BirthPlace).HasMaxLength(255);

                entity.Property(e => e.Fio)
                    .HasMaxLength(255)
                    .HasColumnName("FIO");

                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Number).HasMaxLength(25);

                entity.Property(e => e.PhotoPage1).HasMaxLength(250);

                entity.Property(e => e.RegistrationDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Inn)
                    .HasForeignKey<Inn>(d => d.Id)
                    .HasConstraintName("FK_INN_Item");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("Item");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateCreation).HasColumnType("datetime");

                entity.Property(e => e.Image).HasMaxLength(250);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Item_User");
            });

            modelBuilder.Entity<Passport>(entity =>
            {
                entity.ToTable("Passport");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BirthDate).HasColumnType("datetime");

                entity.Property(e => e.BirthPlace).HasMaxLength(255);

                entity.Property(e => e.ByWhom).HasMaxLength(255);

                entity.Property(e => e.DivisionCode).HasMaxLength(255);

                entity.Property(e => e.FacePhoto).HasMaxLength(250);

                entity.Property(e => e.Fio)
                    .HasMaxLength(255)
                    .HasColumnName("FIO");

                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.GiveDate).HasColumnType("datetime");

                entity.Property(e => e.PhotoPage1).HasMaxLength(250);

                entity.Property(e => e.PhotoPage2).HasMaxLength(250);

                entity.Property(e => e.ResidencePlace).HasMaxLength(255);

                entity.Property(e => e.SerialNumber).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Passport)
                    .HasForeignKey<Passport>(d => d.Id)
                    .HasConstraintName("FK_Passport_Item");
            });

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.ToTable("Photo");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CollectionId).HasColumnName("CollectionID");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.Photos)
                    .HasForeignKey(d => d.CollectionId)
                    .HasConstraintName("FK_Photo_Item");
            });

            modelBuilder.Entity<Poli>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BirthDate).HasColumnType("datetime");

                entity.Property(e => e.Fio)
                    .HasMaxLength(255)
                    .HasColumnName("FIO");

                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Number).HasMaxLength(50);

                entity.Property(e => e.PhotoPage1).HasMaxLength(250);

                entity.Property(e => e.PhotoPage2).HasMaxLength(250);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.ValidUntil).HasMaxLength(50);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Poli)
                    .HasForeignKey<Poli>(d => d.Id)
                    .HasConstraintName("FK_Polis_Item");
            });

            modelBuilder.Entity<Snil>(entity =>
            {
                entity.ToTable("SNILS");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BirthDate).HasColumnType("datetime");

                entity.Property(e => e.BirthPlace).HasMaxLength(255);

                entity.Property(e => e.Fio)
                    .HasMaxLength(255)
                    .HasColumnName("FIO");

                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.Number).HasMaxLength(50);

                entity.Property(e => e.PhotoPage1).HasMaxLength(250);

                entity.Property(e => e.RegistrationDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Snil)
                    .HasForeignKey<Snil>(d => d.Id)
                    .HasConstraintName("FK_SNILS_Item");
            });

            modelBuilder.Entity<Template>(entity =>
            {
                entity.ToTable("Template");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Templates)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Template_User");
            });

            modelBuilder.Entity<TemplateDocument>(entity =>
            {
                entity.ToTable("TemplateDocument");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.TemplateDocument)
                    .HasForeignKey<TemplateDocument>(d => d.Id)
                    .HasConstraintName("FK_TemplateDocument_Item");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.TemplateDocuments)
                    .HasForeignKey(d => d.TemplateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TemplateDocument_Template");
            });

            modelBuilder.Entity<TemplateDocumentDatum>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.Value).HasMaxLength(255);

                entity.HasOne(d => d.TemplateDocument)
                    .WithMany(p => p.TemplateDocumentData)
                    .HasForeignKey(d => d.TemplateDocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TemplateDocumentData_TemplateDocument");

                entity.HasOne(d => d.TemplateObject)
                    .WithMany(p => p.TemplateDocumentData)
                    .HasForeignKey(d => d.TemplateObjectId)
                    .HasConstraintName("FK_TemplateDocumentData_TemplateObject");
            });

            modelBuilder.Entity<TemplateObject>(entity =>
            {
                entity.ToTable("TemplateObject");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.TemplateObject)
                    .HasForeignKey<TemplateObject>(d => d.Id)
                    .HasConstraintName("FK_TemplateObject_Template");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.AccessCode).HasMaxLength(255);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Photo).HasColumnType("image");
                entity.Ignore(e => e.Photo64);


                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

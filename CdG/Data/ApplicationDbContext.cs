using CdG.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CdG.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Evento> Eventos { get; set; }

        public DbSet<VinculoEvento> VinculosEvento { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Evento>()
                .HasOne(evento => evento.Criador)
                .WithMany()
                .HasForeignKey(evento => evento.CriadorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<VinculoEvento>()
                .HasOne(vinculo => vinculo.Evento)
                .WithMany()
                .HasForeignKey(vinculo => vinculo.EventoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<VinculoEvento>()
                .HasOne(vinculo => vinculo.Usuario)
                .WithMany()
                .HasForeignKey(vinculo => vinculo.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<VinculoEvento>()
                .HasIndex(vinculo => new
                {
                    vinculo.EventoId,
                    vinculo.UsuarioId
                })
                .IsUnique();
        }
    }
}
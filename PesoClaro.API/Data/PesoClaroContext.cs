using Microsoft.EntityFrameworkCore;
using PesoClaro.API.Models;

namespace PesoClaro.API.Data;

public class PesoClaroContext : DbContext
{
    public PesoClaroContext(DbContextOptions<PesoClaroContext> options)
        : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Cuenta> Cuentas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Email único por usuario
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Un usuario tiene muchas cuentas
        modelBuilder.Entity<Cuenta>()
            .HasOne(c => c.Usuario)
            .WithMany(u => u.Cuentas)
            .HasForeignKey(c => c.UsuarioId);

        // Precisión de decimales para dinero
        modelBuilder.Entity<Cuenta>()
            .Property(c => c.Saldo)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Cuenta>()
            .Property(c => c.TasaAnual)
            .HasPrecision(5, 2);
    }
}
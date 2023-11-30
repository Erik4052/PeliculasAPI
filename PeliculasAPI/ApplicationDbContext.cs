using Microsoft.EntityFrameworkCore;
using PeliculasAPI.Entidades;

namespace PeliculasAPI
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Genero> Generos { get; set; } //Definimos el nombre de la tabla
        public DbSet<Actor> Actores { get; set; } //Definimos el nombre de la tabla
        public DbSet<Pelicula> Pelicuas { get; set; } //Definimos el nombre de tabla Peliculas
    }
}

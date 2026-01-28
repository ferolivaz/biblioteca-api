using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BibliotecaAPI.datos
{
    //Usuario, porque tengo que especificarle que ahora el tipo que representa a un usuario es la clase Usuario(para agregar columnas)
    public class AplicationDbContext : IdentityDbContext<Usuario>//DbContext
    {


        public AplicationDbContext(DbContextOptions options) : base(options)
        {
            //para poder crear la migracion, solo en caso de tener el  OnModelCreating
            //base.OnModelCreating(ModelBuilder);
        }



        public DbSet<Autor> Autores { get; set; }
        public DbSet<libro> Libros{ get; set; }

        public DbSet<Comentario> Comentarios { get; set; }
    }
}

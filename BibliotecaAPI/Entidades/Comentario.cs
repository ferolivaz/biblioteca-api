using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.Entidades
{
    public class Comentario
    {
        public Guid Id { get; set; }
        [Required]
        public required string Cuerpo { get; set; }
        public DateTime FechaPublicacion { get; set; }

        public int libroId { get; set; }
        public libro? Libro { get; set; }

        public required string UsuarioId { get; set; }

                //IdentityUser
        public Usuario? Usuario { get; set; }  

    }
}

using BibliotecaAPI.Entidades;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs
{
    public class libroDTO
    {
        public int id { get; set; }
        public required string titulo { get; set; }
        
    }
}

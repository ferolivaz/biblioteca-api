using BibliotecaAPI.validaciones;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs
{
    public class autorDTO
    {
        public int id { get; set; }
        public required string nombreCompleto { get; set; }         
    }
}

using BibliotecaAPI.validaciones;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs
{
    public class AutorPatchDTO
    {
        [Required(ErrorMessage = "El valor {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo{0} debe tener {1} caracteres o menos")]
        [primeraLetraAttribute]
        public required string nombres { get; set; }

        [Required(ErrorMessage = "El valor {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo{0} debe tener {1} caracteres o menos")]
        [primeraLetraAttribute]
        public required string apellidos { get; set; }

        [StringLength(20, ErrorMessage = "El campo{0} debe tener {1} caracteres o menos")]
        public string? identificacion { get; set; }
    }
}

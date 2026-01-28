using BibliotecaAPI.validaciones;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.Entidades
{
    public class Autor:IValidatableObject
    {
        public int id { get; set; }
        //-----------------------------------------------------------------------------------------------
        //ErrorMessage      para personalizar el error en el errorDetail
        [Required(ErrorMessage ="El valor {0} es requerido")]
        [StringLength(150,ErrorMessage ="El campo{0} debe tener {1} caracteres o menos")]  
        [primeraLetraAttribute]        
        public required string nombres { get; set; }
        //-----------------------------------------------------------------------------------------------
        [Required(ErrorMessage = "El valor {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo{0} debe tener {1} caracteres o menos")]
        [primeraLetraAttribute]
        public required string apellidos { get; set; }
        //-----------------------------------------------------------------------------------------------
        [StringLength(20, ErrorMessage = "El campo{0} debe tener {1} caracteres o menos")]
        public string? identificacion { get; set; }
        //-----------------------------------------------------------------------------------------------
        public List<libro> libros{ get; set; }=new List<libro>();





        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(nombres))
            {
                var primeraLetra = nombres[0].ToString();

                if (primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra debe ser mayuscula por modelo",new string[] {nameof(nombres)});
                }
            }
        }



    }
}

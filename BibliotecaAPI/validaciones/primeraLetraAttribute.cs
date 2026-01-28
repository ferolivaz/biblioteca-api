using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.validaciones
{
    public class primeraLetraAttribute:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value is null || string.IsNullOrEmpty(value.ToString()))
            {  
                return ValidationResult.Success;    
            }

            var primeraLetra = value.ToString()![0].ToString();

            if (primeraLetra != primeraLetra.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayuscula");
            }

            return ValidationResult.Success;    
        }
    }
}

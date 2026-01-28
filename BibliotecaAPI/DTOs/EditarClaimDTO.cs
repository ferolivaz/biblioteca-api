using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.DTOs
{
    public class EditarClaimDTO
    {
        [EmailAddress]
        [Required]  
        public required string EMail { get; set; } 
    }
}

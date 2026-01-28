using System.ComponentModel.DataAnnotations;

namespace BibliotecaAPI.Entidades
{
    public class libro
    {
        public int id { get; set; }
        [Required]
        public required string titulo { get; set; }
        public int autorId { get; set; }
        public Autor? Autor { get; set; }
        public List<Comentario> Comentarios { get; set; }=new List<Comentario>();   
    }
}

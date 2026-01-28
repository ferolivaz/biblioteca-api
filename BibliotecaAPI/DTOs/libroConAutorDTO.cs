using BibliotecaAPI.Entidades;

namespace BibliotecaAPI.DTOs
{
    public class libroConAutorDTO:libroDTO
    {

        public int autorId { get; set; }
        public required string AutorNombre { get; set; }

        //ya no se uso esto, porque trae todos los datos del autor
        //public Autor? Autor { get; set; }
    }
}

namespace BibliotecaAPI.DTOs
{
    public class autorConLibrosDTO:autorDTO
    {
        public List<libroDTO> libros { get; set; } = new List<libroDTO>();
    }
}

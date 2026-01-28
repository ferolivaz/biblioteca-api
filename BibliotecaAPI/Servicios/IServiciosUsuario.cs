using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Identity;

namespace BibliotecaAPI.Servicios
{
    public interface IServiciosUsuario
    {
        //IdentityUser
        Task<Usuario?> obtenerUsuario();
    }
}
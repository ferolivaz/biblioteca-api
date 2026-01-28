using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Identity;

namespace BibliotecaAPI.Servicios
{
    public class ServiciosUsuario : IServiciosUsuario
    {
                                    //IdentityUser
        private readonly UserManager<Usuario> userManager;
        private readonly IHttpContextAccessor contextAccessor;

        //IHttpContextAccessor permite acceder al contexto http, a travez del contexto http se tiene acceso a los claims(permisos, perfil)
        // El IdentityUser es el usuario que por default da Identity
                                            //IdentityUser
        public ServiciosUsuario(UserManager<Usuario> userManager, IHttpContextAccessor ContextAccessor)
        {
            this.userManager = userManager;
            contextAccessor = ContextAccessor;
        }

                            //IdentityUser
        public async Task<Usuario?> obtenerUsuario()
        {
            // obtiene el valor del email de la persona que se logueo
            var emailClaim = contextAccessor.HttpContext!.User.Claims.Where(x => x.Type == "email").FirstOrDefault();
            if (emailClaim is null)
            {
                return null;
            }
            var email = emailClaim.Value;

            return await userManager.FindByEmailAsync(email);
        }
    }
}

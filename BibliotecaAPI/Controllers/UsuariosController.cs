using AutoMapper;
using BibliotecaAPI.datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    //[Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly AplicationDbContext aplicationDbContext;

        //<IdentityUser>
        private readonly UserManager<Usuario> userManager;
        private readonly IConfiguration configuracion;
        private readonly IServiciosUsuario serviciosUsuario;
        private readonly Mapper mapper;


        //<IdentityUser>
        public SignInManager<Usuario> SignInManager { get; }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------

                                            //<IdentityUser>                                                  //<IdentityUser>
        public UsuariosController(  AplicationDbContext aplicationDbContext
                                    , UserManager<Usuario> userManager  //UserManager <IdentityUser> userManager
                                    , IConfiguration configuracion
                                    ,SignInManager<Usuario> signInManager
                                    ,IServiciosUsuario ServiciosUsuario
                                    ,Mapper mapper
                                )
        {
            this.aplicationDbContext = aplicationDbContext;
            this.userManager = userManager;
            this.configuracion = configuracion;
            SignInManager = signInManager;
            serviciosUsuario = ServiciosUsuario;
            this.mapper = mapper;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        [HttpPost("registro")]
        //[AllowAnonymous]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Registrar(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            //el identity user, representa un usuario, de este no se creo una entidad manualmente, este ya se configuro cuando se instal identity
            // pide usuario y email
            var usuario = new Usuario   //IdentityUser          //cambio por el tema de agregar un campo
            {
                UserName = credencialesUsuarioDTO.Email, //en este caso, se usa el mail como usuario
                Email = credencialesUsuarioDTO.Email
            };

            var resultado = await userManager.CreateAsync(usuario, credencialesUsuarioDTO.Password!);

            if (resultado.Succeeded)
            {
                var respuestaAutenticacion =await construirToken(credencialesUsuarioDTO);
                return respuestaAutenticacion;
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return ValidationProblem();
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        [HttpPut("actualizar")]
        [Authorize]
        //el dto en este caso, solo permite actualizar la fecha de nacimiento desde el postman
        public async Task<ActionResult> Put(ActualizarUsuarioDTO actualizarUsuarioDTO)
        {
            var usuario = await serviciosUsuario.obtenerUsuario();
            if (usuario is null)
            {
                return NotFound(); 
            }

            usuario.FechaNacimiento = actualizarUsuarioDTO.FechaNacimiento; 

            await userManager.UpdateAsync(usuario); 
            return NoContent(); 

        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> Get()
        {
            //Users ->AspNetUsers en identity
            var usuarios = await aplicationDbContext.Users.ToListAsync();   
            var usuariosDTO = mapper.Map<IEnumerable<UsuarioDTO>>(usuarios); 

            return Ok(usuariosDTO);
        }
            
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private async Task<RespuestaAutenticacionDTO> construirToken(CredencialesUsuarioDTO credencialesUsuarioDTO) 
        { 
            var claims = new List<Claim>
            {
                new Claim("email",credencialesUsuarioDTO.Email),
                new Claim("lo que yo quiera","cualquier valor"),
            };

            //se procede a buscar un usuario
            var usuario = await userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario!);
            claims.AddRange(claimsDB);  // para agregar los claims db

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuracion["llaveJwt"]!));

            //HmacSha256 es el algoritmo que permite firmar el jwt, para que nadie pueda editar su s valores
            var credenciales = new SigningCredentials(llave,SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            //issuer:null (emisor)
            var tockenDeSeguridad = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion,signingCredentials: credenciales);

            var token = new JwtSecurityTokenHandler().WriteToken(tockenDeSeguridad);

            return new RespuestaAutenticacionDTO
            {
                Token = token,
                Expiracion = expiracion,
            };
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        [HttpPost("login")]
        //[AllowAnonymous]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> login(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {

            var usuario = await userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);

            if (usuario is null) {
                RetornaLoginIncorrecto();
            }
          

            var resultado = await SignInManager.CheckPasswordSignInAsync(usuario!, credencialesUsuarioDTO.Password!,lockoutOnFailure:false);

            if (resultado.Succeeded)
            {
                var respuestaAutenticacion = await construirToken(credencialesUsuarioDTO);
                return respuestaAutenticacion;
            }
            else
            {
                return RetornaLoginIncorrecto();
            }

        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        private ActionResult RetornaLoginIncorrecto() {
             ModelState.AddModelError(string.Empty,"Login Incorrecto");
            return ValidationProblem(); 
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //RenovarToken, es el nombre para acceder en la liga /RenovarToken
        [HttpGet("RenovarToken")]
        [Authorize(Policy = "esAdmin")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> RenovarToken()
        {
            var usuario=await serviciosUsuario.obtenerUsuario();
            

            if (usuario is null) 
            {
                return NotFound();
            }

            var credencialesUsuarioDTO =  new CredencialesUsuarioDTO { Email=usuario.Email!};

            var respuestaAutenticacion = await construirToken(credencialesUsuarioDTO);

            return respuestaAutenticacion;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //api/usuarios/hacer-admin
        [HttpPost("hacer-admin")]
        [Authorize(Policy = "esAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarClaimDTO editarClaimDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarClaimDTO.EMail);

            if (usuario is null)
            {
                return NotFound();
            }

            //para agregar el claim se utiliza la siguiente sentencia
            await userManager.AddClaimAsync(usuario,new Claim("esAdmin","true"));
            return NoContent();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        [HttpPost("Remover-admin")]
        [Authorize(Policy = "esAdmin")]
        public async Task<ActionResult> RemoverAdmin(EditarClaimDTO editarClaimDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarClaimDTO.EMail);

            if (usuario is null)
            {
                return NotFound();
            }

            //para agregar el claim se utiliza la siguiente sentencia
            await userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "true"));
            return NoContent();
        }


    }
}

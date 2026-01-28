using AutoMapper;
using BibliotecaAPI.datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/libros/${id:int}/Comentarios")]
    [Authorize]
    public class ComentariosController : ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IServiciosUsuario serviciosUsuario;

        public ComentariosController(AplicationDbContext context, IMapper mapper,IServiciosUsuario ServiciosUsuario)
        {
            this.context = context;
            this.mapper = mapper;
            serviciosUsuario = ServiciosUsuario;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int Libroid)
        {
            var existeLibro = await context.Libros.AnyAsync(lib=>lib.id == Libroid);  

            if(!existeLibro)
            {
                //como se usa el notFound 404, tengo que usar un action result, y usar un tipo concreto "List"
                return NotFound();
            }

            var Comentarios = await context.Comentarios.Where(com=>com.libroId == Libroid)
                .Include(x=>x.Usuario)
                .OrderByDescending(x=>x.FechaPublicacion).ToListAsync();

            var ComentariosDTO = mapper.Map<List<ComentarioDTO>>(Comentarios);    
            return ComentariosDTO;  
        }

        [HttpGet("{idc}", Name = "obtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> Get(Guid Id)
        {
            var Comentario = await context.Comentarios.Include(x=>x.Usuario).SingleOrDefaultAsync(c=> c.Id== Id);
            if(Comentario is null)
            {
                return NotFound();
            }
            var ComentarioDTO = mapper.Map<ComentarioDTO>(Comentario);
            return ComentarioDTO;
        }

        [HttpPost]
        public async Task<ActionResult> Post(ComentarioCreacionDTO comentarioNuevoDTO, int Libroid)
        {
         

            var existeLibro = await context.Libros.AnyAsync(lib => lib.id == Libroid);

            if (!existeLibro)
            {
                //como se usa el notFound 404, tengo que usar un action result, y usar un tipo concreto "List"
                return NotFound();
            }

            var usuario = await serviciosUsuario.obtenerUsuario();

            if (usuario is null)
            {
                return NotFound();
            }


            var comentarioNuevo = mapper.Map<Comentario>(comentarioNuevoDTO);
            comentarioNuevo.libroId = Libroid;
            comentarioNuevo.FechaPublicacion = DateTime.UtcNow;
            comentarioNuevo.UsuarioId = usuario.Id;

            context.Comentarios.Add(comentarioNuevo);
            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentarioNuevo);

            return CreatedAtRoute("obtenerComentario", new { id = comentarioNuevo.Id,Libroid }, comentarioDTO);

        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        [HttpPatch("{idc}")]
        public async Task<ActionResult> Patch(Guid id,int libroId, JsonPatchDocument<ComentarioPatchDTO> patchDoc)
        {

          
            //AutorPatchDTO -> este dto, es para dellimitar los cambios que se pueden aplicar, es como el de creacion

            if (patchDoc is null)
            {
                return BadRequest();
            }

            var existeLibro = await context.Libros.AnyAsync(lib => lib.id == libroId);

            if (!existeLibro)
            {
                //como se usa el notFound 404, tengo que usar un action result, y usar un tipo concreto "List"
                return NotFound();
            }

            var usuario = await serviciosUsuario.obtenerUsuario();
            if (usuario is null)
            {
                return NotFound();
            }


            var comentarioDB = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);

            if (comentarioDB is null)
            {
                return NotFound();
            }


            if(comentarioDB.UsuarioId!= usuario.Id)
            {
                return Forbid();        //esta prohibido
            }


            var comentarioPatchDTO = mapper.Map<ComentarioPatchDTO>(comentarioDB);

            //patchDoc este es el documento que se recibe con la lista de campos que se aplicara el cambio
            patchDoc.ApplyTo(comentarioPatchDTO, ModelState);

            var esValido = TryValidateModel(comentarioPatchDTO);

            if (!esValido)
            {
                return ValidationProblem();
            }

            //esta sentencia, es para lla aplicar los cambios sobre la tabla autor
            mapper.Map(comentarioPatchDTO, comentarioDB);

            await context.SaveChangesAsync();

            return NoContent();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        [HttpDelete("{idc}")]
        public async Task<ActionResult> Delete(Guid id,int libroId)
        {

            var existeLibro = await context.Libros.AnyAsync(lib => lib.id == libroId);

            if (!existeLibro)
            {
                //como se usa el notFound 404, tengo que usar un action result, y usar un tipo concreto "List"
                return NotFound();
            }

            var usuario = await serviciosUsuario.obtenerUsuario();
            if (usuario is null)
            {
                return NotFound();
            }

            var comentarioDB = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);

            if (comentarioDB is null)
            {
                return NotFound();
            }

            if (comentarioDB.UsuarioId!= usuario.Id)
            {
                return Forbid();
            }

            context.Remove(comentarioDB); 
            await context.SaveChangesAsync();    

            //LA PRIMERA VEZ, SE USO ESTE METODO
            //var registrosBorrados = await context.Comentarios.Where(x => x.Id == id).ExecuteDeleteAsync();

            //if (registrosBorrados == 0)
            //{
            //    return NotFound();
            //}

            return NoContent();
        }
    }
}

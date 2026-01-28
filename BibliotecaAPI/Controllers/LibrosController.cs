using AutoMapper;
using BibliotecaAPI.datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaAPI.Controllers
{

    [ApiController]
    [Route("api/libros")]
    [Authorize(Policy = "esAdmin")]
    public class LibrosController:ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(AplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        //-----------------------------------------------------------------------------------------------
        [HttpGet]
        public async Task<IEnumerable<libroDTO>> Get()
        {
            var libros = await context.Libros.ToListAsync();
            //opcion 1 sin automaper
            //var librosDTO= libros.Select(libro => new libroDTO {id= libro.id,titulo=libro.titulo});

            //opcion 2 con automaper
            var librosDTO = mapper.Map<IEnumerable<libroDTO>>(libros);

            return librosDTO;

        }

        //-----------------------------------------------------------------------------------------------
        [HttpGet("{id:int}",Name ="obtenerLibro")]   //api/libros/1,2,3...etc
        //public async Task<ActionResult<libro>> Get(int id)
        public async Task<ActionResult<libroConAutorDTO>> Get(int id)
        {
            //Include es para que incluya los datos del la entidad autor con el dato de navegacion que se agrego
            var libro = await context.Libros.Include(x=>x.Autor).FirstOrDefaultAsync(x => x.id == id);

            
            if (libro is null)
            {
                return NotFound();
            }

            var libroConAutorDTO = mapper.Map<libroConAutorDTO>(libro);

            return libroConAutorDTO;
            //return libro;
        }
        //-----------------------------------------------------------------------------------------------
        [HttpPost]
        //public async Task<ActionResult> Post(libro libro)
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {

           
            var libro = mapper.Map<libro>(libroCreacionDTO);

            var existeAutor = await context.Autores.AnyAsync(x => x.id == libro.autorId);
            if (existeAutor == false)
            {
                //llave:autorId
                ModelState.AddModelError(nameof(libro.autorId), $"El autor: {libro.autorId},no es valido");
                return ValidationProblem();
                //return BadRequest($"El autor: {libro.id },no es valido");
            }


            context.Add(libro);
            await context.SaveChangesAsync();
            //return CreatedAtRoute("obtenerLibro", new { id = libro.id }, libro);
            return CreatedAtRoute("obtenerLibro", new { id = libro.id }, mapper.Map<libroDTO>(libro));
        }
        //-----------------------------------------------------------------------------------------------
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {


            var libro = mapper.Map<libro>(libroCreacionDTO);
            libro.id = id;

            //ya no tiene sentido esta validacion, puesto que ya le estoy asignando manualmente el id y siempre va a coincidir
            //if (id != libro.id)
            //{
            //    ModelState.AddModelError(nameof(libro.autorId), $"el autor id {libro.autorId} no existe");
            //    return ValidationProblem();
            //}


            var existeAutor = await context.Autores.AnyAsync(x => x.id == libro.autorId);
            if (existeAutor == false)
            {
                return BadRequest($"El autor: {libro.id},no es valido");
            }



            context.Update(libro);
            await context.SaveChangesAsync();

            return NoContent();
        }
        //-----------------------------------------------------------------------------------------------
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {


            var registrosBorrados = await context.Libros.Where(x => x.id == id).ExecuteDeleteAsync();

            if (registrosBorrados == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}

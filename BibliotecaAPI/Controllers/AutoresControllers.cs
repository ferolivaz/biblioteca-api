using AutoMapper;
using BibliotecaAPI.datos;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/autores")]
    [Authorize(Policy ="esAdmin")] //a nivel clase con esto ya se protegen las acciones, ya no se puede acceder, la politica se configura en program
    public class AutoresControllers:ControllerBase
    {
        private readonly AplicationDbContext context;
        private readonly IMapper mapper;

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public AutoresControllers(AplicationDbContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //[HttpGet("/ListadoAutores")]  para ignorar la regla de ruteo inicial, se pueden definir mas de una regla
        [HttpGet]
        //[Authorize] // proteccion a nivel accion
        [AllowAnonymous]    //permite el acceso a un usuario anonimo(cualquiera)para esta accion
        [EndpointSummary("obtiene autores")]
        [EndpointDescription("Descripcion")]
        public async Task<IEnumerable<autorDTO>> Get() 
        {
            var autores= await context.Autores.ToListAsync();

            //SIN UTILIZAR EL AUTOMAPER
            //var autoresDTO= autores.Select(autor => new autorDTO {id= autor.id,nombreCompleto= $"{autor.nombres}{autor.apellidos}" });

            var autoresDTO = mapper.Map<IEnumerable<autorDTO>>(autores);        //utilizando automapper

            return autoresDTO;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //ORIGINAL
        /*[HttpPost]
        public async Task<ActionResult> Post(Autor aurtor)
        {
            context.Add(aurtor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<autorDTO>(aurtor);
            return CreatedAtRoute("obtenerAutor", new { id = aurtor.id }, autorDTO);
        }*/


        [HttpPost]
        public async Task<ActionResult> Post(autorCreacionDTO autorCreacionDTO)     //autorCreacionDTO --> NO VEO QUE TUVIERA EL GRAN SENTIDO, QUIZA PARA NOO NECESITAR EL ID Y LA LISTA DE LIBROS
        {
            var aurtor = mapper.Map<Autor>(autorCreacionDTO);
            context.Add(aurtor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<autorDTO>(aurtor);
            return CreatedAtRoute("obtenerAutor", new {id= aurtor.id}, autorDTO);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        //EJEMPLO 1 

        //[HttpGet("{parm1}/{param2?}")]     //definir mas de un parametro
        //public IActionResult Get(string parm1, string? param2="VD") 
        //{
        //    return Ok(new{parm1,parm2});      retorna en el cuerpo de la respuesta
        //}

        //EJEMPLO 2
        //[HttpGet("{nombre:alpha}")] 
        //public async Task<ActionResult<IEnumerable<Autor>>> Get(string nombre)
        //{
        //    return await context.Autores.Where(a=>a.nombres.Contains(nombre)).ToListAsync();
        //}


        //NOTA 3
        //  IACTIONRESULT, SOLO TE PERMITE RETORNAR IACTIONRESULT, PARA RETORNAR VALORES SE PUEDE USAR COMO EL EJEMPLO SIGUIENTE
        //   return Ok(new{parm1,parm2});





        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        [HttpGet("{id:int}",Name ="obtenerAutor")]   //api/autores/1,2,3...etc
        public async Task<ActionResult<autorConLibrosDTO>> Get(int id)
        {
            var autor=await context.Autores.Include(x=>x.libros).FirstOrDefaultAsync(x=>x.id==id);    
            
            if(autor is null)
            {
                return NotFound();
            }

            var autorDTO = mapper.Map<autorConLibrosDTO>(autor);

            return autorDTO;        //utilizando automapper
//            return autor;         //antes
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, autorCreacionDTO autorCreacionDTO)
        {

            //if(id!= autor.id)
            //{
            //    return BadRequest("los ids deben de coincidir");
            //}

            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.id = id;  

            context.Update(autor);
            await context.SaveChangesAsync();

            return NoContent();
        }



        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var registrosBorrados= await context.Autores.Where(x=>x.id == id).ExecuteDeleteAsync();  
            
            if(registrosBorrados ==0)
            {
                return NotFound();  
            }

            return NoContent(); //eliminacion correcta
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //ejemplo       
        //[HttpGet("primero")]      //-->  plantilla para que pueda ser invocada la accion
        //public async Task<Autor> GetPrimerAutor()
        //{
        //    return await context.Autores.FirstAsync();
        //}
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<AutorPatchDTO> patchDoc)
        {

            //AutorPatchDTO -> este dto, es para dellimitar los cambios que se pueden aplicar, es como el de creacion

            if (patchDoc is null)
            {
                return BadRequest();
            }

            var autorDB = await context.Autores.FirstOrDefaultAsync(x => x.id == id);

            if (autorDB is null)
            {
                return NotFound();
            }

            var autorPatchDTO = mapper.Map<AutorPatchDTO>(autorDB);

            //patchDoc este es el documento que se recibe con la lista de campos que se aplicara el cambio
            patchDoc.ApplyTo(autorPatchDTO, ModelState);

            var esValido = TryValidateModel(autorPatchDTO);

            if (!esValido)
            {
                return ValidationProblem();
            }

            //esta sentencia, es para lla aplicar los cambios sobre la tabla autor
            mapper.Map(autorPatchDTO, autorDB);

            await context.SaveChangesAsync();

            return NoContent();
        }

    }
}

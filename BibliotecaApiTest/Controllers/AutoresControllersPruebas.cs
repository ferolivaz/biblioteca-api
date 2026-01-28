using BibliotecaApiTest.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibliotecaAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApiTest.Controllers
{
    [TestClass]
    public class AutoresControllersPruebas:basePruebas
    {
        [TestMethod]
        public async Task Get__retorna404CuandoAutorConIDNoExiste()
        {
            //preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutomapper();    
            var controller = new AutoresControllers(context, mapper); 

            //prueba
            var respuesta = await controller.Get(1);

            //verificacion
            var resultado = respuesta.Result as StatusCodeResult;
            Assert.AreEqual(expected: 404, actual: resultado!.StatusCode);
        }



        [TestMethod]

        public async Task Get__retornaAutorCuandoAutorConIDNoExiste()
        {
            //preparacion
            var nombreBD = Guid.NewGuid().ToString();            
            var mapper = ConfigurarAutomapper();            
            var autor =  new Autor { nombres = "Fer",apellidos ="Oli"};
            var autor2 = new Autor { nombres = "Fer", apellidos = "Oli" };

            var context = ConstruirContext(nombreBD);
            context.Autores.Add(autor);
            context.Autores.Add(autor2);
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new AutoresControllers(context2, mapper);

            //prueba
            var respuesta = await controller.Get(1);

            //verificacion
            var resultado = respuesta.Value;
            Assert.AreEqual(expected: 1, actual: resultado!.id);
        }


        [TestMethod]

        public async Task Post_CreaAutor()
        {
            //preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var mapper = ConfigurarAutomapper();
            var context = ConstruirContext(nombreBD);
            var AutorCreacionDTO = new autorCreacionDTO { nombres = "Nuevo", apellidos = "autor" };
            var controller = new AutoresControllers(context, mapper);

            //prueba
            var respuesta = await controller.Post(AutorCreacionDTO);

            //verificacion
            var resultado = respuesta as CreatedAtActionResult;
            Assert.IsNotNull(resultado);

            var context2 = ConstruirContext(nombreBD);
            var cantidad = await context2.Autores.CountAsync();
            Assert.AreEqual(expected:1,actual:cantidad);
        }


    }
}

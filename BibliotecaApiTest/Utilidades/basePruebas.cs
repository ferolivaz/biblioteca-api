using AutoMapper;
using BibliotecaAPI.datos;
using BibliotecaAPI.utilidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaApiTest.Utilidades
{
    public class basePruebas
    {
        protected AplicationDbContext ConstruirContext(string nombreBD)
        {
            var opciones = new DbContextOptionsBuilder<AplicationDbContext>()
                .UseInMemoryDatabase(databaseName: nombreBD)
                .Options;
            var contexto = new AplicationDbContext(opciones);
            return contexto;
        }

        protected IMapper ConfigurarAutomapper()
        {
            var configuracion = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new autoMapperProfiles());
            });
            return configuracion.CreateMapper();
        }
    }
}

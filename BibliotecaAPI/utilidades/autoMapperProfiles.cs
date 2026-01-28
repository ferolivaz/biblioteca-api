using AutoMapper;
using BibliotecaAPI.DTOs;
using BibliotecaAPI.Entidades;

namespace BibliotecaAPI.utilidades
{
    public class autoMapperProfiles:Profile
    {
        public autoMapperProfiles()
        {
            //                          PARA_EL_MIEBRO(ELMIEMBRO DTO, LA CONFIGURACION)
            CreateMap<Autor, autorDTO>().ForMember(dto=>dto.nombreCompleto,config => config.MapFrom(autor => mapearNombreYApellido(autor)));
            CreateMap<Autor, autorConLibrosDTO>().ForMember(dto => dto.nombreCompleto, config => config.MapFrom(autor => mapearNombreYApellido(autor)));
            CreateMap<autorCreacionDTO, Autor>();

            CreateMap<libro,libroConAutorDTO>().ForMember(dto => dto.AutorNombre, config => config.MapFrom(ent => mapearNombreYApellido(ent.Autor!)));
            CreateMap<LibroCreacionDTO, libro>();
            CreateMap<libro, libroDTO>();
            
            CreateMap<Autor, AutorPatchDTO>().ReverseMap();

            CreateMap<ComentarioCreacionDTO, Comentario>();   
            CreateMap<Comentario,ComentarioDTO>().ForMember(dto=> dto.UsuarioEmail, config=> config.MapFrom(ent=>ent.Usuario!.Email));
            CreateMap<ComentarioPatchDTO, Comentario>().ReverseMap();

            CreateMap<Usuario, UsuarioDTO>();
        }

        private string mapearNombreYApellido(Autor autor) => $"{autor.nombres}{autor.apellidos}";

    }
}

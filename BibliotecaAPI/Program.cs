using BibliotecaAPI.datos;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.middlewares;
using BibliotecaAPI.Servicios;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


//AREA DE SERVICIOS

//protecction  de encriptacion
builder.Services.AddDataProtection();

var origenesPermitidos = builder.Configuration.GetSection("origenesPermitidos").Get<string[]>()!;
//para permitir que se comuniquen desde otros dominios cruzado origin recurse source
builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(opcionesCors =>
    {
        //opcionesCors.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        opcionesCors.WithOrigins(origenesPermitidos).AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("mi-cabecera");
    });
}); 

builder.Services.AddAutoMapper(typeof(Program));
//IgnoreCycles  es para que no marque el error en la parte donde la entidad, manda a llamar a autor y autor manda a llamar a libro
builder.Services.AddControllers().AddJsonOptions(opciones=>opciones.JsonSerializerOptions.ReferenceHandler=ReferenceHandler.IgnoreCycles);
builder.Services.AddDbContext<AplicationDbContext>(opciones=> opciones.UseSqlServer("name=DefaultConection"));
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddTransient<IServiciosUsuario, ServiciosUsuario>();

                                //IdentityUser (cambio por lo de la columna)
builder.Services.AddIdentityCore<Usuario>().AddEntityFrameworkStores<AplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<UserManager<Usuario>>();
builder.Services.AddScoped<SignInManager<Usuario>>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication().AddJwtBearer(opciones => {
    opciones.MapInboundClaims = false;  // para que no cambie el claim por otro de manera automatica
    opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,      //emisor del tocken
        ValidateAudience = false,   //audiencia
        ValidateLifetime = true,       //expiracion
        ValidateIssuerSigningKey = true,    // la llave del token
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["llaveJwt"]!)),
        ClockSkew=    TimeSpan.Zero  //para no tener problemas de discrepacina de tiempo al validar la expiacion del token 
    };   //Microsoft.IdentityModel.Tokens.TokenValidationParameters();
});

//politica de autorizacion
builder.Services.AddAuthorization(opciones =>
{
    opciones.AddPolicy("esAdmin",politica=>politica.RequireClaim("esAdmin"));
    //opciones.AddPolicy("esvendedor", politica => politica.RequireClaim("esvendedor"));
});

//CONFIGURACION DE SWAGGER
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Biblioteca API",
        Description = "Este es un web api para trabajar con datos de autores y libros",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Email = "felipe@hotmail.com",
            Name = "Felipe Gavilán",
            Url = new Uri("https://gavilan.blog")
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/license/mit/")
        }
    });

    opciones.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v2",
        Title = "Biblioteca API",
        Description = "Este es un web api para trabajar con datos de autores y libros",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Email = "felipe@hotmail.com",
            Name = "Felipe Gavilán",
            Url = new Uri("https://gavilan.blog")
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/license/mit/")
        }
    });

    //para pasar un token en swagger y probar los endpoint que requieren autorizacion
    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    //opciones.OperationFilter<FiltroAutorizacion>();
    opciones.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
    //------------------------------------------------------------------------------------------

});

var app = builder.Build();


//AREA DE MIDDLEWARES



//app.Use(async (contexto, next) =>
//{
//    var logger = contexto.RequestServices.GetRequiredService <ILogger<Program>>();
//    logger.LogInformation($"Peticion:{contexto.Request.Method } {contexto.Request.Path}   ");

//    await next.Invoke();   

//    logger.LogInformation($"Respuesta:{contexto.Response.StatusCode}" );

//});



//app.Use(async (contexto, next) =>
//{
//    if (contexto.Request.Path == "/bloqueado")
//    {
//        contexto.Response.StatusCode = 403;
//        await contexto.Response.WriteAsync("Acceso denegado");
//    }
//    else { 
//        await next.Invoke();
//    }

//});


//app.UseLoguePeticion();
//app.UseDenegarPeticion();

////cabecera personalizada
//app.Use(async(contexto, next) =>
//{
//    contexto.Response.Headers.Append("mi-cabecera","valor");
//    await next();

//});

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.MapControllers();

app.Run();

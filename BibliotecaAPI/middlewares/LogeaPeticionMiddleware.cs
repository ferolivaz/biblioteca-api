namespace BibliotecaAPI.middlewares
{
    public class LogeaPeticionMiddleware
    {
        private readonly RequestDelegate next;


        //RequestDelegate -> para el next
        public LogeaPeticionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }


        public async Task InvokeAsync(HttpContext contexto)
        {
            //para requerir el servicio de ilogger en Program
            var logger = contexto.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation($"Peticion:{contexto.Request.Method} {contexto.Request.Path}   ");

            await next.Invoke(contexto);    //aqui se pasa el contexto "contexto",  continua con la ejecucion de la tuberia, cuando termine regresa

            logger.LogInformation($"Respuesta:{contexto.Response.StatusCode}");
        }
       
    }


    public static class LogeaPeticionMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoguePeticion(this IApplicationBuilder builder) 
        {
            return builder.UseMiddleware<LogeaPeticionMiddleware>();
        }
    }

}

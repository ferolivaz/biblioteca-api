namespace BibliotecaAPI.middlewares
{
    public class DenegarMiddleware
    {
        private readonly RequestDelegate next;

        public DenegarMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext contexto)
        {
            if (contexto.Request.Path == "/bloqueado")
            {
                contexto.Response.StatusCode = 403;
                await contexto.Response.WriteAsync("Acceso denegado");
            }
            else
            {
                await next.Invoke(contexto);
            }
        }
    }


    public static class DenegarMiddlewareExtensions
    {
        public static IApplicationBuilder UseDenegarPeticion(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DenegarMiddleware>();
        }
    }

}

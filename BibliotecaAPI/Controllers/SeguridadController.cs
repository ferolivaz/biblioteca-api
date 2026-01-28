using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaAPI.Controllers
{

    [ApiController]
    [Route("api/Seguridad")]
    public class SeguridadController:ControllerBase
    {
        private readonly IDataProtector protector;
        private readonly ITimeLimitedDataProtector protectorlimTiempo;

        public SeguridadController(IDataProtectionProvider dataProtectionProvider)
        {
            protector = dataProtectionProvider.CreateProtector("SeguridadController");  //string de propocito
            protectorlimTiempo = protector.ToTimeLimitedDataProtector();
        }



        [HttpGet("encriptar-tiepo")]
        public ActionResult EncriptarTiempo(string textoPlano)
        {
            string textoCifrado = protectorlimTiempo.Protect(textoPlano,lifetime:TimeSpan.FromSeconds(30));
            return Ok(new { textoCifrado });
        }

        [HttpGet("encriptar")]
        public ActionResult Encriptar(string textoPlano)
        {
            string textoCifrado = protector.Protect(textoPlano);    
            return Ok(new { textoCifrado });
        }


        [HttpGet("desencriptar")]
        public ActionResult DesEncriptar(string textoCifrado)
        {
            string textoPlano = protector.Unprotect(textoCifrado);
            return Ok(new { textoPlano });
        }

    }
}

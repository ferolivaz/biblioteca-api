using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BibliotecaAPI.validaciones;

namespace BibliotecaApiTest.PruebasUnitarias.Validaciones
{
    [TestClass]
    public class PrimeraLetraMayusculaAttributePruebas
    {
        [TestMethod]
        [DataRow("")]
        [DataRow(null)]
        [DataRow("Felipe")]
        public void IsValid_RetornaExitoso_SiValueNoTienePrimeraLetraMayusc(string value) 
        {
            // Preparacion
            var PrimeraLetraMayusculaAtributo = new primeraLetraAttribute();
            var validationContext = new ValidationContext(new object{ }); 
            //var value = string.Empty;
            // Prueba
            //GetValidationResult devuelve ValidationResult , en lugar de isvalid que es protected que devuelve bool
            var resultado = PrimeraLetraMayusculaAtributo.GetValidationResult(value, validationContext);
            // Verificacion
            Assert.AreEqual(expected:ValidationResult.Success,actual:resultado);

        }

        [TestMethod]
        [DataRow("felipe")]
        public void IsValid_RetornaError_SiTienePrimeraLetraMinc(string value)
        {
            // Preparacion
            var PrimeraLetraMayusculaAtributo = new primeraLetraAttribute();
            var validationContext = new ValidationContext(new object { });
            //var value = string.Empty;
            // Prueba
            //GetValidationResult devuelve ValidationResult , en lugar de isvalid que es protected que devuelve bool
            var resultado = PrimeraLetraMayusculaAtributo.GetValidationResult(value, validationContext);
            // Verificacion
            Assert.AreEqual(expected: "La primera letra debe ser mayuscula", actual: resultado!.ErrorMessage);

        }
    }
}

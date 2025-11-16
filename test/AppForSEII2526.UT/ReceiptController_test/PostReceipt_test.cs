using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ReceiptDTO;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.ReceiptController_test
{
    public class PostReceipt_test : AppForSEII2526SqliteUT
    {
        // Datos base para los tests
        private const string _userName = "Patrik.lopes@alu.uclm.es";
        private const string _customerName = "Patrik";
        private const string _customerSurname = "Lopes Bulhoes de Oliveira";
        private const string deliveryAddress = "Calle Yeste";
        // Constructor para inicializar datos en la base de datos en memoria
        public PostReceipt_test()
        {
            // Datos base: balanza y reparación
            var scale = new Scale { Id = 1, Name = "Balanza Aleatoria" };
           
            var repair = new Repair
            {
                Id = 1,
                Name = "Reparación pantalla",
                Description = "Cambio completo de pantalla",
                Cost = 49.99,
                Scale = scale,
                ScaleId = scale.Id
            };

            var user = new ApplicationUser
            {
                Id = "patrik1",
                Name = _customerName,
                Surname = _customerSurname,
                UserName = _userName,
                Email = _userName
            };
            // Inserción de las entidades en el contexto de pruebas
            _context.Scales.Add(scale);
            _context.Repairs.Add(repair);
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        // Casos de prueba para CreateReceipt_Error_test
        public static IEnumerable<object[]> TestCasesFor_CreateReceipt()
        {
            // Caso 1 : usuario no registrado
            var dtoUserNotRegistered = new ReceiptForCreateDTO()
            {
                UserName = "noexiste@alu.uclm.es",
                Name = "Nombre Falso",
                Surname = "Apellido Falso",
                DeliveryAddress = deliveryAddress,
                PaymentMethod = PaymentMethod.CreditCard,
                Repairs = new List<ReceiptItemDTO> { new ReceiptItemDTO("Reparación pantalla", "ModeloX") }
            };

            // Caso 2: reparación no existe
            var dtoRepairNotExisting = new ReceiptForCreateDTO()
            {
                UserName = _userName,
                Name = _customerName,
                Surname = _customerSurname,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = PaymentMethod.CreditCard,
                Repairs = new List<ReceiptItemDTO> { new ReceiptItemDTO("Reparación inexistente", "ModeloY") }
            };
            // Lista de todos los casos de prueba
            var allTests = new List<object[]>
            {
                new object[] { dtoUserNotRegistered, $"Usuario '{dtoUserNotRegistered.UserName}' no existe" },
                new object[] { dtoRepairNotExisting, $"Reparación '{dtoRepairNotExisting.Repairs.First().RepairName}' no existe" }
            };

            return allTests;
        }
        // TEST 1: Comprobar que una petición con datos inválidos devuelve BadRequest con el mensaje adecuado.
        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateReceipt))]
        public async Task CreateReceipt_Error_test(ReceiptForCreateDTO receiptDTO, string errorExpected)
        {
            // Arrange: Se crea un mock de ILogger para inyectarlo en el controlador si depender del sistema de logging real.
            var mockLogger = new Mock<ILogger<RecibosController>>();
            var controller = new RecibosController(_context, mockLogger.Object);

            // Act: Llamada al método CreateRepair con datos inválidos
            var result = await controller.CreateRepair(receiptDTO);

            // Assert: Se espera un BadRequestObjectResult y que el mensaje devuelto comience con el error esperado
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            // El controlador puede devolver un string o ValidationProblemDetails
            if (badRequestResult.Value is string s)
            {
                Assert.StartsWith(errorExpected, s);
            }
            else if (badRequestResult.Value is ValidationProblemDetails pd)
            {
                var errorActual = pd.Errors.First().Value[0];
                Assert.StartsWith(errorExpected, errorActual);
            }
            else 
            {
                Assert.True(false, "Tipo inesperado en BadRequestObjectResult.Value");
            }
        }
        // TEST 2: Comprobar que una petición válida crea el recibo correctamente y devuelve CreatedAtAction con los datos correctos.
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateReceipt_Success_test()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<RecibosController>>();
            var controller = new RecibosController(_context, mockLogger.Object);
            // Datos para la creación del recibo
            var repairs = new List<ReceiptItemDTO>
            {
                new ReceiptItemDTO("Reparación pantalla", "Modelo-Aleatorio-123")
            };
            // DTO para la creación del recibo
            var dto = new ReceiptForCreateDTO 
            {
                UserName = _userName,
                Name = _customerName,
                Surname = _customerSurname,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = PaymentMethod.CreditCard,
                Repairs = repairs
            };

            // Act: Llamada al método CreateRepair
            var result = await controller.CreateRepair(dto);

            // Assert: Se espera un CreatedAtActionResult con los datos correctos
            var createdResult = Assert.IsType<CreatedAtActionResult>(result); // Verifica que el resultado es CreatedAtActionResult
            var receiptDetail = Assert.IsType<ReceiptDetailDTO>(createdResult.Value); // Verifica que el valor es del tipo esperado
            // Verificaciones de los datos devueltos
            Assert.Equal(_customerName, receiptDetail.Name);
            Assert.Equal(_customerSurname, receiptDetail.Surname);
            Assert.Equal(deliveryAddress, receiptDetail.DeliveryAddress);
            Assert.Equal((float)49.99, receiptDetail.TotalPrice);
            Assert.NotNull(receiptDetail.Repairs);
            Assert.Single(receiptDetail.Repairs);
            // Verificación del item del recibo
            var item = receiptDetail.Repairs.First();
            Assert.Equal("Reparación pantalla", item.RepairName);
            Assert.Equal("Modelo-Aleatorio-123", item.ModelToRepair);

            // Fecha reciente 
            Assert.True((DateTime.Now - receiptDetail.OperationDate).TotalSeconds < 60, "La fecha de la operación debe ser reciente");
        }
    }
}

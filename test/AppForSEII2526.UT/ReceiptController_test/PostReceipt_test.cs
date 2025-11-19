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

            // Caso 3: DTO válido pero forzamos ModelState inválido
            var dtoValidButModelStateInvalid = new ReceiptForCreateDTO()
            {
                UserName = _userName,
                Name = _customerName,
                Surname = _customerSurname,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = PaymentMethod.CreditCard,
                Repairs = new List<ReceiptItemDTO> { new ReceiptItemDTO("Reparación pantalla", "Modelo-Test") }
            };

            // Lista de todos los casos de prueba
            var allTests = new List<object[]>
            {
                //  no existe usuario / reparación inexistente
                new object[] { dtoUserNotRegistered, $"Usuario '{dtoUserNotRegistered.UserName}' no existe", false },
                new object[] { dtoRepairNotExisting, $"Reparación '{dtoRepairNotExisting.Repairs.First().RepairName}' no existe", false },

                // caso ModelState inválido: no comprobamos mensaje concreto (se pasa null), y marcamos forceModelStateInvalid = true
                new object[] { dtoValidButModelStateInvalid, null, true }
            };

            return allTests;
        }
        // TEST parametrizado, incluye ModelState invalido.
        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateReceipt))]
        public async Task CreateReceipt_Error_test(ReceiptForCreateDTO receiptDTO, string? errorExpected, bool forceModelStateInvalid)
        {
            // Arrange: Se crea un mock de ILogger para inyectarlo en el controlador si depender del sistema de logging real.
            var mockLogger = new Mock<ILogger<RecibosController>>();
            var controller = new RecibosController(_context, mockLogger.Object);

            // Si la fila indica que debemos forzar ModelState inválido, lo añadimos antes del Act
            if (forceModelStateInvalid)
            {
                controller.ModelState.AddModelError("DeliveryAddress", "El campo DeliveryAddress es obligatorio.");
            }

            // Act: Llamada al método CreateRepair con datos inválidos o ModelState inválido forzado
            var result = await controller.CreateRepair(receiptDTO);

            // Assert: Se espera un BadRequestObjectResult
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            if (forceModelStateInvalid)
            {
                // En el caso del ModelState inválido comprobamos:
                // El BadRequest contiene algún valor (ModelState/ValidationProblemDetails/SerializableError)
                Assert.NotNull(badRequestResult.Value);
                // Se ha registrado un log de advertencia con el mensaje esperado
                var logInvocations = mockLogger.Invocations
                    .Where(inv =>
                        inv.Method.Name == "Log" &&
                        inv.Arguments != null &&
                        inv.Arguments.Count >= 3 &&
                        inv.Arguments[0] is LogLevel lvl &&
                        lvl == LogLevel.Warning &&
                        inv.Arguments[2] != null &&
                        inv.Arguments[2].ToString().Contains("Datos inválidos para crear el recibo"))
                    .ToList();

                Assert.Single(logInvocations);

                // No se ha creado ningún recibo en la BD de pruebas
                Assert.Empty(_context.Receipts);

                // Y el GET por id 1 no debería encontrar nada (NotFound)
                var getResult = await controller.GetRepair(1);
                Assert.IsType<NotFoundResult>(getResult);
            }
            else
            {
                // Para los demás casos comprobamos el mensaje esperado 
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

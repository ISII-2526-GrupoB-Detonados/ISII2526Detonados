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
    public class GetReceipts_test : AppForSEII2526SqliteUT
    {
        public GetReceipts_test()
        {
            // Datos de soporte: balanza y reparación, los valores son aleatorios 
            var scale = new Scale { Id = 1, Name = "Balanza Aleatoria" };
            // 
            var repair = new Repair
            {
                Id = 1,
                Name = "Reparación pantalla",
                Description = "Cambio completo de pantalla",
                Cost = 49.99, // Repair.Cost es double
                Scale = scale,
                ScaleId = scale.Id
            };

            var user = new ApplicationUser
            {
                Id = "patrik1",
                Name = "Patrik",
                Surname = "Lopes Bulhoes de Oliveira",
                UserName = "Patrik.lopes@alu.uclm.es",
                Email = "Patrik.lopes@alu.uclm.es"
            };

            // Item del recibo con campos aleatorios
            var receiptItem = new ReceiptItem
            {
                Repair = repair,
                Model = "Modelo-Aleatorio-123"
            };
            // Recibo completo que agrupa el usuario, dirección, fecha, precio total, método de pago y la lista de items.
            var receipt = new Receipt
            {
                Id = 1,
                ApplicationUser = user,
                DeliveryAddress = "Calle Yeste",
                ReceiptDate = DateTime.Now,
                TotalPrice = repair.Cost,
                PaymentMethodTypes = PaymentMethod.CreditCard,
                ReceiptItems = new List<ReceiptItem> { receiptItem }
            };
            // Inserción de las entidades en el contexto de pruebas.
            _context.Scales.Add(scale);
            _context.Repairs.Add(repair);
            _context.Users.Add(user);
            _context.Receipts.Add(receipt);
            // Persistir cambios en la base de datos en memoria.

            _context.SaveChanges();
        }
        // TEST 1: Comprobar que una petición con id inválido devuelve BadRequest.

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetReceipt_BadRequest_test()
        {
            // Arrange: Se crea un mock de ILogger para inyectarlo en el controlador si depender del sistema de logging real.
            var mockLogger = new Mock<ILogger<RecibosController>>();
            var controller = new RecibosController(_context, mockLogger.Object);

            // Act: Llamada con id inválido (<= 0) que debe ser validada por el controlador.
            var result = await controller.GetRepair(0); // id inválido (<=0)

            // Assert:Se espera un BadRequestObjectResult y que el mensaje devuelto
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid repair id", badRequest.Value);
        }
        // TEST 2: Comprobar que pedir un id inexistente devuelve NotFound.

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetReceipt_NotFound_test()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<RecibosController>>();
            var controller = new RecibosController(_context, mockLogger.Object);

            // Act
            var result = await controller.GetRepair(9999); // id que no existe

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        // TEST 3: Comprobar que pedir un id válido devuelve OK y el DTO esperado.

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetReceipt_Found_test()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<RecibosController>>();
            var controller = new RecibosController(_context, mockLogger.Object);

            // Crear el DTO esperado con los mismos datos que se insertaron en el constructor
            var expectedReceiptItems = new List<ReceiptItemDTO>
            {
                new ReceiptItemDTO("Reparación pantalla", "Balanza Aleatoria", "Modelo-Aleatorio-123", (float)49.99)
            };

            var expectedReceipt = new ReceiptDetailDTO(
                1,
                "Patrik",
                "Lopes Bulhoes de Oliveira",
                "Calle Yeste",
                DateTime.Now,
                (float)49.99,
                expectedReceiptItems
            );

            // Act
            var result = await controller.GetRepair(1); // id que existe

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var receiptDTOActual = Assert.IsType<ReceiptDetailDTO>(okResult.Value);

            // Comparación de objetos usando Equals
            Assert.Equal(expectedReceipt, receiptDTOActual);
        }
    }
}
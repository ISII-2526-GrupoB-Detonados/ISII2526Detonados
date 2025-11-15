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
        private const string _userName = "Patrik.lopes@alu.uclm.es";
        private const string _customerName = "Patrik";
        private const string _customerSurname = "Lopes Bulhoes de Oliveira";
        private const string deliveryAddress = "Calle Yeste";

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

            _context.Scales.Add(scale);
            _context.Repairs.Add(repair);
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CreateReceipt()
        {
            // Caso: usuario no registrado
            var dtoUserNotRegistered = new ReceiptForCreateDTO()
            {
                UserName = "noexiste@alu.uclm.es",
                Name = "Nombre Falso",
                Surname = "Apellido Falso",
                DeliveryAddress = deliveryAddress,
                PaymentMethod = PaymentMethod.CreditCard,
                Repairs = new List<ReceiptItemDTO> { new ReceiptItemDTO("Reparación pantalla", "ModeloX") }
            };

            // Caso: reparación no existe
            var dtoRepairNotExisting = new ReceiptForCreateDTO()
            {
                UserName = _userName,
                Name = _customerName,
                Surname = _customerSurname,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = PaymentMethod.CreditCard,
                Repairs = new List<ReceiptItemDTO> { new ReceiptItemDTO("Reparación inexistente", "ModeloY") }
            };

            var allTests = new List<object[]>
            {
                new object[] { dtoUserNotRegistered, $"Usuario '{dtoUserNotRegistered.UserName}' no existe" },
                new object[] { dtoRepairNotExisting, $"Reparación '{dtoRepairNotExisting.Repairs.First().RepairName}' no existe" }
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateReceipt))]
        public async Task CreateReceipt_Error_test(ReceiptForCreateDTO receiptDTO, string errorExpected)
        {
            // Arrange
            var mockLogger = new Mock<ILogger<RecibosController>>();
            var controller = new RecibosController(_context, mockLogger.Object);

            // Act
            var result = await controller.CreateRepair(receiptDTO);

            // Assert
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

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateReceipt_Success_test()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<RecibosController>>();
            var controller = new RecibosController(_context, mockLogger.Object);

            var repairs = new List<ReceiptItemDTO>
            {
                new ReceiptItemDTO("Reparación pantalla", "Modelo-Aleatorio-123")
            };

            var dto = new ReceiptForCreateDTO
            {
                UserName = _userName,
                Name = _customerName,
                Surname = _customerSurname,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = PaymentMethod.CreditCard,
                Repairs = repairs
            };

            // Act
            var result = await controller.CreateRepair(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var receiptDetail = Assert.IsType<ReceiptDetailDTO>(createdResult.Value);

            Assert.Equal(_customerName, receiptDetail.Name);
            Assert.Equal(_customerSurname, receiptDetail.Surname);
            Assert.Equal(deliveryAddress, receiptDetail.DeliveryAddress);
            Assert.Equal((float)49.99, receiptDetail.TotalPrice);

            Assert.NotNull(receiptDetail.Repairs);
            Assert.Single(receiptDetail.Repairs);

            var item = receiptDetail.Repairs.First();
            Assert.Equal("Reparación pantalla", item.RepairName);
            Assert.Equal("Modelo-Aleatorio-123", item.ModelToRepair);

            // Fecha reciente (delta razonable)
            Assert.True((DateTime.Now - receiptDetail.OperationDate).TotalSeconds < 60, "La fecha de la operación debe ser reciente");
        }
    }
}

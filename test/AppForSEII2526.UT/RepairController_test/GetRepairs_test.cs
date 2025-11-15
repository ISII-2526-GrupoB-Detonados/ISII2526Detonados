using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.DevicesDTOrepa;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.RepairController_test
{
    public class GetRepairs_test : AppForSEII2526SqliteUT
    {
        public GetRepairs_test()
        {
            // Escalas
            var scales = new List<Scale>
            {
                new Scale { Id = 1, Name = "Balanza A" },
                new Scale { Id = 2, Name = "Balanza B" },
                new Scale { Id = 3, Name = "Balanza C" }
            };

            var repairs = new List<Repair>
            {
                new Repair { Id = 1, Name = "Reparación pantalla", Description = "Cambio de pantalla completa", Cost = 49.99, Scale = scales[0], ScaleId = scales[0].Id },
                new Repair { Id = 2, Name = "Reparación batería", Description = "Sustitución batería", Cost = 29.99, Scale = scales[1], ScaleId = scales[1].Id },
                new Repair { Id = 3, Name = "Reparación placa", Description = "Reparación placa base", Cost = 79.50, Scale = scales[2], ScaleId = scales[2].Id },
                // Escala para comprobar filtros
                new Repair { Id = 4, Name = "Reparación cámara", Description = "Sustitución cámara trasera", Cost = 39.00, Scale = scales[0], ScaleId = scales[0].Id }
            };

            _context.Scales.AddRange(scales);
            _context.Repairs.AddRange(repairs);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_GetRepairs_OK()
        {
            var repairsDTOs = new List<repairDTOrepa>()
            {
                new repairDTOrepa(1, "Reparación pantalla", "Cambio de pantalla completa", "Balanza A", 49.99),
                new repairDTOrepa(2, "Reparación batería", "Sustitución batería", "Balanza B", 29.99),
                new repairDTOrepa(3, "Reparación placa", "Reparación placa base", "Balanza C", 79.50)
            };

            var allOrdered = new List<repairDTOrepa>
            {
                repairsDTOs[0],
                repairsDTOs[2],
                repairsDTOs[1]
            };

            var tcName_bateria = new List<repairDTOrepa>() { repairsDTOs[1] };
            var tcScale_BalanzaA = new List<repairDTOrepa>() { repairsDTOs[0] };

            var allTests = new List<object[]>
            {
                // nombre nulo, scale nulo 
                new object[] { null, null, allOrdered },
                new object[] { "batería", null, tcName_bateria },
                new object[] { null, "Balanza A", tcScale_BalanzaA },
                new object[] { "placa", "Balanza C", new List<repairDTOrepa>() { repairsDTOs[2] } }
            };

            return allTests;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetRepairs_OK))]
        [Trait("Database", "WithoutFixtures")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRepairs_OK(string? filterNombre, string? filterScaleNombre, List<repairDTOrepa> expectedRepairs)
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReparacionesController>>();
            ILogger<ReparacionesController> logger = mockLogger.Object;
            var controller = new ReparacionesController(_context, logger);

            // Act
            var result = await controller.GetRepairDTO(filterNombre, filterScaleNombre);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var repairsDTOsActual = Assert.IsType<List<repairDTOrepa>>(okResult.Value);

            Assert.Equal(expectedRepairs, repairsDTOsActual);
        }

        [Fact]
        [Trait("Database", "WithoutFixtures")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRepairs_badName_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReparacionesController>>();
            ILogger<ReparacionesController> logger = mock.Object;
            var controller = new ReparacionesController(_context, logger);

            // Act
            var result = await controller.GetRepairDTO("Nombre inexistente", null);

            // Assert
            // El controlador actual devuelve Ok aunque no haya coincidencias.
            // deja estas aserciones; si no, cámbialas para comprobar una lista vacía.
            var badNameResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badNameResult.Value);
            var problem = problemDetails.Errors.First().Value[0];

            Assert.Equal("No hay reparaciones con ese nombre", problem);
        }

        [Fact]
        [Trait("Database", "WithoutFixtures")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRepairs_badScale_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReparacionesController>>();
            ILogger<ReparacionesController> logger = mock.Object;
            var controller = new ReparacionesController(_context, logger);

            // Act
            var result = await controller.GetRepairDTO(null, "Balanza inexistente");

            // Assert
            // Igual que el test anterior: ajusta según el comportamiento real del controlador.
            var badScaleResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badScaleResult.Value);
            var problem = problemDetails.Errors.First().Value[0];

            Assert.Equal("No hay reparaciones con esa balanza", problem);
        }
    }
}
